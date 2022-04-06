using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class DBLoginTest : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();


    [Header("Login")]
    public InputField usernameLoginField;
    public InputField passwordLoginField;
    public Text warningLoginText;

    private void Awake()
    {
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
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    public void ClearLoginFeilds()
    {
        usernameLoginField.text = "";
        passwordLoginField.text = "";
    }

    //Function for the login button
    public void LoginButton()
    {
        Debug.Log("Logined");
        StartCoroutine(usernameLogin(usernameLoginField.text));

    }
    //Function for the register button

    private IEnumerator usernameLogin(string _username)
    {
        var LoginTask = FirebaseDatabase.DefaultInstance.GetReference("Users").Child(_username).Key.ToLower();
        Debug.Log(LoginTask);
        yield return null;
        

    }
    private IEnumerator Login(string _email, string _password)
    {
        Debug.LogError(_email + _password);
        //Call the Firebase auth signin function passing the email and password

        var LoginTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            Debug.Log("Logged In"); //confirmLoginText.text = "Logged In";
            //StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(2);

            //usernameField.text = User.DisplayName;
            // UIManager.instance.UserDataScreen(); // Change to user data UI
            //confirmLoginText.text = "";
            ClearLoginFeilds();
        }
    }



    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }
    public void TestLogined()
    {
        string name = User.DisplayName;
        Debug.Log(name);
    }

    public void Logout()
    {
        auth = FirebaseAuth.DefaultInstance;
        if (auth == null)
        {
            Debug.Log("auth == null");
        }
        else if (DBreference == null)
        {
            Debug.Log("DBreference == null");
        }
        else if (User == null)
        {
            Debug.Log("User == null");
        }
        Debug.Log("SignOut");
        Debug.Log(User.DisplayName);
        //auth.SignOut();
        FirebaseAuth.DefaultInstance.SignOut();
        Debug.Log(User.DisplayName);
    }
    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        Firebase.Auth.FirebaseUser user = null;
        if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        if (senderAuth == auth && senderAuth.CurrentUser != user)
        {
            bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                Debug.Log("AuthStateChanged Signed in " + user.UserId);
                Debug.Log(user.DisplayName ?? "");

            }
        }
    }

}

