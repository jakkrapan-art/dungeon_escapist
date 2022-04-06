using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{

    [SerializeField] private ChatWindowUI chatWindow;

    [Space]
    public string chatHistoryText;

    [Header("Buttons")]
    [SerializeField] private Button openChatWindowButton;
    [SerializeField] private Button closeChatWindowButton;


    private void Start()
    {
        chatWindow.gameObject.SetActive(false);
        openChatWindowButton.gameObject.SetActive(true);

        EscapistNetworkManager networkManager = EscapistNetworkManager.singleton as EscapistNetworkManager;

        BoardPlayer.OnMessage += OnPlayerSendMessage;

        openChatWindowButton.onClick.AddListener(openChatWindow);
        closeChatWindowButton.onClick.AddListener(closeChatWindow);
    }

    public void openChatWindow()
    {
        Debug.Log("Clicked");
        chatWindow.gameObject.SetActive(true);
        openChatWindowButton.gameObject.SetActive(false);
    }

    public void closeChatWindow()
    {
        chatWindow.gameObject.SetActive(false);
        openChatWindowButton.gameObject.SetActive(true);
    }

    private void OnPlayerSendMessage(BoardPlayer player, string message)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(player.getPlayerColor());
        string prettyMessage = player.isLocalPlayer ?
            $"<color=#{hexColor}>You:</color> {message}" :
            $"<color=#{hexColor}>{player.DisplayName}:</color> {message}";
        appendMessage(prettyMessage);
    }

    private void appendMessage(string message)
    {
        StartCoroutine(appendMessageAndScrollDown(message));
    }

    private IEnumerator appendMessageAndScrollDown(string message)
    {
        chatHistoryText += message + "\n";
        chatWindow.setChatMessageDisplay(chatHistoryText);

        yield return new WaitForEndOfFrame();

        chatWindow.setScrollBarValue(0);
    }

    private void OnDestroy()
    {
        BoardPlayer.OnMessage -= OnPlayerSendMessage;
    }
}
