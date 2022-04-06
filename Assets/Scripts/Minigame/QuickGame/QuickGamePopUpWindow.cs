using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickGamePopUpWindow : MonoBehaviour
{
    [SerializeField] private Text gameName;
    [SerializeField] private Text gamePlayTime;
    [SerializeField] private Text gameDescriptionText;

    public void setGameNameText(string text)
    {
        gameName.text = text;
    }

    public void setGamePlayTimeText(string text)
    {
        gamePlayTime.text = text;
    }

    public void setGameDescriptionText(string text)
    {
        gameDescriptionText.text = text;
    }
}
