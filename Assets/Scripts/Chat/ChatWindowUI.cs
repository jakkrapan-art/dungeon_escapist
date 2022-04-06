using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class ChatWindowUI : MonoBehaviour
{
    [SerializeField] private Text chatHistory;

    [SerializeField] private InputField messageInputField;
    [SerializeField] private Scrollbar scrollbar;

    [SerializeField] private Button sendMessageButton;

    private void Awake()
    {
        sendMessageButton.onClick.AddListener(sendMessage);
    }

    public void setChatMessageDisplay(string message)
    {
        chatHistory.text = message;
    }

    public void setScrollBarValue(float value)
    {
        scrollbar.value = value;
    }

    public void sendMessage()
    {
        var identity = NetworkClient.connection.identity;
        BoardPlayer player = null;

        if (identity.GetComponent<BoardPlayer>())
        {
            player = identity.GetComponent<BoardPlayer>();
        }
        else
        {
            if (identity.GetComponent<Indicator>())
            {
                player = NetworkClient.connection.identity.GetComponent<Indicator>().getPlayer();
            }
            else
            {
                Debug.Log($"Current identity: {NetworkClient.connection.identity}");
                return;
            }
        }

        try
        {
            player.CmdSendMessage(messageInputField.text.Trim());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Cannot find BoardPlayer component from network client identity.");
            return;
        }

        messageInputField.text = "";
    }
}
