using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;
    public GameObject MenuPlayUIObj;
    public GameObject AccountUIObj;

    public void changeUI2(string name)
    {
        back2Main();
        switch (name)
        {
            case "MenuPlayUI":
                MenuPlayUIObj.SetActive(true);
                break;
            case "AccountUI":
                AccountUIObj.SetActive(true);
                break;
        }

    }

    public void closeUI(string name)
    {
        switch (name)
        {
            case "MenuPlayUI":
                MenuPlayUIObj.SetActive(false);
                break;
            case "AccountUI":
                AccountUIObj.SetActive(false);
                break;
        }
    }

    public void back2Main()
    {
        MenuPlayUIObj.SetActive(false);
        AccountUIObj.SetActive(false);

    }
}
