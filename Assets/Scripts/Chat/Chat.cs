using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class Chat : NetworkBehaviour
{
    public static Chat instance;

    [SerializeField] private ChatWindowUI chatWindow;

    [Header("Buttons")]
    [SerializeField] private Button openChatWindowButton;
    [SerializeField] private Button closeChatWindowButton;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        chatWindow.gameObject.SetActive(false);
        openChatWindowButton.gameObject.SetActive(true);

        openChatWindowButton.onClick.AddListener(openChatWindow);
        closeChatWindowButton.onClick.AddListener(closeChatWindow);
    }

    public void openChatWindow()
    {
        chatWindow.gameObject.SetActive(true);
        openChatWindowButton.gameObject.SetActive(false);
    }

    public void closeChatWindow()
    {
        chatWindow.gameObject.SetActive(false);
        openChatWindowButton.gameObject.SetActive(true);
    }
}
