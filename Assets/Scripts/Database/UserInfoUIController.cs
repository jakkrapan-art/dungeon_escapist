using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class UserInfoUIController : MonoBehaviour
{
    public static UserInfoUIController instance;

    private User user;

    [SerializeField] private Text displayNameText;
    [SerializeField] private Text winText;
    [SerializeField] private Text matchText;
    [SerializeField] private Text mostScoreText;

    [SerializeField] private GameObject changeDisplayNamePopupWindow;
    [SerializeField] private InputField newDisplayNameInput;

    public void onPressedInfoButton()
    {
        if (user == null)
        {
            if (JsonConvert.DeserializeObject<User>(PlayerPrefs.GetString("user")) == null)
            {
                LoadingSceneController.instance.loadingSceneTo("AuthScene");
                return;
            }

            user = JsonConvert.DeserializeObject<User>(PlayerPrefs.GetString("user"));
        }

        setUserInfo(user);
    }
    public void LogoutButton()
    {
        /*AuthController.instance.Logout();*/
        user = null;
        UserController.instance.logout();
    }
    public void setUserInfo(User user)
    {
        this.user = user;
        displayNameText.text = user.getDisplayName(20);
        winText.text = "Wins : " + user.getWinCount();
        matchText.text = "Match : " + user.getMatchCount();
        mostScoreText.text = "Most Score : " + user.getMostScore();
    }

    public void openChangeDisplayNamePopupWindow()
    {
        for (int i = 0; i < newDisplayNameInput.transform.childCount; i++)
        {
            if (newDisplayNameInput.transform.GetChild(i).name == "Placeholder")
            {
                newDisplayNameInput.transform.GetChild(i).GetComponent<Text>().text = user.getDisplayName();
            }
        }
    }

    public void onConfirmChangeDisplayName()
    {
        if (newDisplayNameInput.text.Equals(user.getUsername()) || newDisplayNameInput.text == string.Empty)
        {
            changeDisplayNamePopupWindow.SetActive(false);
            return;
        }

        string newDisplayName = newDisplayNameInput.text;

        AuthController.instance.updateDisplayName(user.getUsername(), newDisplayName);

        User newUser = new User(user.getUsername(), newDisplayName, user.getEmail(), user.getMatchCount(), user.getMostScore(), user.getWinCount());

        PlayerPrefs.SetString("user", JsonConvert.SerializeObject(newUser));
        user = newUser;

        setUserInfo(user);
    }

    public void setUser(User user)
    {
        this.user = user;
    }
}
