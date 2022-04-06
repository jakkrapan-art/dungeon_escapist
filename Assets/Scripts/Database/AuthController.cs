using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using System;

public class AuthController : MonoBehaviour
{
    public static AuthController instance;
    [Header("Firebase")]
    protected DependencyStatus dependencyStatus;
    protected DatabaseReference DBreference;
    public Text warningInternet;

    [Header("Login")]
    [SerializeField] private InputField usernameLoginField;//Field
    [SerializeField] private InputField passwordLoginField;//Field
    public Text warningLoginText;

    [Header("Register")]
    [SerializeField] private InputField usernameRegisterField;
    [SerializeField] private InputField emailRegisterField;
    [SerializeField] private InputField passwordRegisterField;
    [SerializeField] private InputField passwordRegisterVerifyField;
    public Text warningRegisterText;

    private void Awake()
    {
        instance = this;
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void ClearLoginFeilds()
    {
        usernameLoginField.text = "";
        passwordLoginField.text = "";
    }

    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    //Function for the login button
    public void LoginButton()
    {
        Debug.Log("Logined");
        WaitInternetUI.instance.Waiting();
        StartCoroutine(Login(usernameLoginField.text, passwordLoginField.text));

    }
    //Function for the register button
    public void RegisterButton()
    {
        warningRegisterText.text = "";
        WaitInternetUI.instance.Waiting();

        string usernameInput = usernameRegisterField.text.Trim();
        string passwordInput = passwordRegisterField.text.Trim();
        string confPasswordInput = passwordRegisterVerifyField.text.Trim();
        string emailInput = emailRegisterField.text.Trim();

        if (usernameInput == "" || passwordInput == "" || emailInput == "" || confPasswordInput == "")
        {
            warningRegisterText.text = "Input is not fulfill.";
            return;
        }
        else if (passwordInput != confPasswordInput)
        {
            warningRegisterText.text = "Password is not match.";
            WaitInternetUI.instance.WaitSuccess();
            return;
        }
        else if (usernameInput.Length > 20)
        {
            warningRegisterText.text = "Username can use only 20 characters or lower.";
            return;
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(usernameInput, "^[a-zA-Z0-9._-]+$"))
        {
            warningRegisterText.text = "Username can use only a-z,A-Z,0-9";
            return;
        }
        else if (!isEmail(emailInput))
        {
            warningRegisterText.text = "Please input email in email field.";
            return;
        }

        StartCoroutine(Register(usernameInput, passwordInput, emailInput));
    }

    //Login ด้วยUsername 
    private IEnumerator Login(string _username, string _password)
    {
        if (_username.Trim() == string.Empty || _password.Trim() == string.Empty || _username.Trim() == null || _password.Trim() == null)
        {
            warningLoginText.text = "Input is not fulfill.";
            WaitInternetUI.instance.WaitSuccess();
            yield return null;
        }
        else
        {
            var LoginTask = FirebaseDatabase.DefaultInstance.GetReference("Users").Child(_username.ToLower()).GetValueAsync();
            yield return new WaitUntil(() => LoginTask.IsCompleted);
            var snapshot = LoginTask.Result;

            if (snapshot.Value == null)
            {
                warningLoginText.text = "User doesn't exist.";
                yield break;
            }
            else if (!snapshot.Child("password").Value.Equals(Hash.hash(_password)))
            {
                warningLoginText.text = "Incorrect password.";
                yield break;
            }
            else
            {
                User user = JsonConvert.DeserializeObject<User>(snapshot.GetRawJsonValue());
                UserController.instance.login(user);

                warningRegisterText.text = "";
                warningLoginText.text = "";
                AuthUIController.instance.back2Main();
                ClearLoginFeilds();//Field
                ClearRegisterFeilds();
            }
        }
    }


    private IEnumerator Register(string _username, string _password, string _email)
    {
        var LoginTask = FirebaseDatabase.DefaultInstance.GetReference("Users").GetValueAsync();
        yield return new WaitUntil(() => LoginTask.IsCompleted);
        var snapshot = LoginTask.Result;
        if (snapshot.Child(_username.ToLower()).Value != null)
        {
            warningRegisterText.text = "Username is already use.";
            WaitInternetUI.instance.WaitSuccess();//จบloading
        }
        else
        {
            User user = new User(_username, _username, _email, 0, 0, 0);
            string hashedPassword = Hash.hash(_password);
            insertUserToDatabase(user, hashedPassword);
            AuthUIController.instance.back2Main();

            warningRegisterText.text = "";
            warningLoginText.text = "";

            Debug.Log("Registed " + user.getDisplayName());
            ClearRegisterFeilds();
            ClearLoginFeilds();
        }
    }

    private void insertUserToDatabase(User user, string password)
    {
        var json = JsonConvert.SerializeObject(user);
        DBreference.Child("Users").Child(user.getUsername().ToLower()).SetRawJsonValueAsync(json);
        DBreference.Child("Users").Child(user.getUsername().ToLower()).Child("password").SetValueAsync(password);
    }

    private bool isEmail(string textForCheck)
    {
        var charArray = textForCheck.ToCharArray();
        bool isFoundAtSign = false;
        bool isFoundDotSign = false;

        for (int i = 0; i < charArray.Length; i++)
        {
            if (!isFoundAtSign && charArray[i].ToString() == "@")
            {
                isFoundAtSign = true;
            }
            else
            {
                if (charArray[i].ToString() == "@")
                {
                    return false;
                }
                else if (charArray[i].ToString() == ".")
                {
                    if (isFoundDotSign)
                    {
                        return false;
                    }

                    isFoundDotSign = true;
                }
            }
        }

        return isFoundAtSign && isFoundDotSign;
    }

    public void updateDisplayName(string username, string newDisplayName)
    {
        var DBTask = DBreference.Child("Users").Child(username.ToLower()).Child("displayName").SetValueAsync(newDisplayName);
    }
}
    
