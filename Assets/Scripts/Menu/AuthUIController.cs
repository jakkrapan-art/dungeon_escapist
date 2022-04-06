using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Firebase;
using Firebase.Auth;

public class AuthUIController : MonoBehaviour
{
    public static AuthUIController instance;
    public GameObject StartUIObj;
    public GameObject loginUIObj;
    public GameObject registerUIObj;

    private void Awake()
    {
        instance = this;
    }

    public void BeginButton()
    {
        User user = JsonConvert.DeserializeObject<User>(PlayerPrefs.GetString("user"));
        Debug.Log("Connect to server as " + user);
        if (user != null)
        {
            LoadingSceneController.instance.loadingSceneTo("MainMenu");
        }
        else
        {
            changeUI2("LoginUserUI");
            StartUIObj.SetActive(false);
        }
    }

    public void changeUI2(string name)
    {
        back2Main();
        switch (name)
        {
            case "LoginUserUI":
                loginUIObj.SetActive(true);
                break;
            case "RegisterUI":
                registerUIObj.SetActive(true);
                break;
        }

    }

    public void closeUI(string name)
    {
        switch (name)
        {
            case "LoginUserUI":
                loginUIObj.SetActive(false);
                break;
            case "RegisterUI":
                registerUIObj.SetActive(false);
                break;
        }
    }

    public void back2Main()
    {
        loginUIObj.SetActive(false);
        registerUIObj.SetActive(false);
        StartUIObj.SetActive(true);
    }
}
