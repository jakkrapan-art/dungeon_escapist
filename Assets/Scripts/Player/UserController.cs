using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UserController : MonoBehaviour
{
    public static UserController instance;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void login(User user)
    {
        PlayerPrefs.SetString("user", JsonConvert.SerializeObject(user));
    }

    public void logout()
    {
        PlayerPrefs.DeleteKey("user");
        LoadingSceneController.instance.loadingSceneTo("AuthScene");
    }

    private void OnApplicationQuit()
    {
        //logout
    }
}
