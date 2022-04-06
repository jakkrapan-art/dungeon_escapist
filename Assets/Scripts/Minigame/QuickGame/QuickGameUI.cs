using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class QuickGameUI : MonoBehaviour
{
    public QuickGame quickGame;
    public List<Button> buttons;

    public Image playTimeRemainBar;
    public Text playTimeRemainText;

    public Text alertText;

    protected AudioSource audioSource;
    protected AudioClip ButtonClickSE;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ButtonClickSE = Resources.Load<AudioClip>("Minigame/SortNumber/button-click");
    }
    public virtual void setUpUI()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            setUpUIButton(i, i);
        }
    }

    protected virtual void setUpUIButton(int buttonIndex, int buttonMethodValue)
    {
        setButtonInteractable(buttonIndex, true);
        buttons[buttonIndex].onClick.AddListener(delegate { onButtonPressed(buttonIndex, buttonMethodValue); });
        if (!quickGame.player.Equals(NetworkClient.connection.identity.GetComponent<BoardPlayer>()))
        {
            buttons[buttonIndex].enabled = false;
        }
        else
        {
            buttons[buttonIndex].enabled = true;
        }
    }

    private void resetUIButton(int buttonIndex)
    {
        buttons[buttonIndex].onClick.RemoveAllListeners();
    }

    public void setButtonInteractable(int buttonIndex, bool isInteractable)
    {
        buttons[buttonIndex].interactable = isInteractable;
    }

    public virtual void setRemainTimeUI(float remainingTime, float remainingTimeAsPercent)
    {
        if (Mathf.Round(remainingTime) > 0)
        {
            playTimeRemainText.text = Mathf.Round(remainingTime).ToString();
        }
        else if (remainingTimeAsPercent < 0)
        {
            playTimeRemainText.text = 0.ToString();
        }
        else
        {
            playTimeRemainText.text = remainingTime.ToString("0.##");
        }
        playTimeRemainBar.fillAmount = remainingTimeAsPercent;
    }

    public virtual void showGameUI()
    {
        gameObject.SetActive(true);
    }

    public virtual void hideGameUI()
    {
        resetUI();
        gameObject.SetActive(false);
    }

    protected virtual void resetUI()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            resetUIButton(i);
        }
    }

    public void doGameAction(int buttonIndex, int buttonValue)
    {
        if (!quickGame.player.Equals(NetworkClient.connection.identity.GetComponent<BoardPlayer>()) || quickGame.isSuccess)
        {
            return;
        }

        quickGame.player.CmdDoQuickGameAction(buttonIndex, buttonValue);
    }

    public void onButtonPressed(int buttonIndex, int buttonValue)
    {
        audioSource.PlayOneShot(ButtonClickSE, 0.7F);
        doGameAction(buttonIndex, buttonValue);
    }

    public void setAlertText(bool setActive, Color color, string text)
    {
        alertText.gameObject.SetActive(setActive);
        alertText.color = color;
        alertText.text = text;
    }

}
