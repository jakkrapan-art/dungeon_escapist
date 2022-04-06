using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Collections;

public class EndGameUIController : MonoBehaviour
{
    public static EndGameUIController instance = null;
    public EscapistNetworkManager networkManager = null;

    [SerializeField]
    private Text mainTextEnding = null;
    [SerializeField]
    private Text subTextEnding = null;
    [SerializeField]
    private Image endingIcon;

    [Header("Sprites")]
    [SerializeField] private Sprite winIcon;
    [SerializeField] private Sprite loseIcon;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        try
        {
            networkManager = EscapistNetworkManager.singleton as EscapistNetworkManager;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    private void Start()
    {
        StartCoroutine(nameof(initialSetup));
    }

    private void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                NetworkManager.singleton.StopServer();
            }
        }*/

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            setDisplay(false);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            setDisplay(true);
        }
    }

    private void setEndingText(string mainText, string subText)
    {
        mainTextEnding.text = mainText;
        subTextEnding.text = subText;
    }

    private IEnumerator initialSetup()
    {
        BoardGameController boardGameController = null;
        yield return new WaitUntil(() => boardGameController = FindObjectOfType<BoardGameController>());
        setDisplay(boardGameController.getLocalPlayer().Equals(boardGameController.winner));
    }

    public void setDisplay(bool isWinner)
    {
        string mainText;
        string subText;

        if (isWinner)
        {
            mainText = $"<color=#FFBA00>CONGRATULATION</color>";
            subText = "You escaped from the dungeon.";
            endingIcon.sprite = winIcon;
        }
        else
        {
            mainText = $"<color=#FF0000>YOU LOSE</color>";
            subText = "The exit is closed, you struck in the dungeon.";
            endingIcon.sprite = loseIcon;
        }

        setEndingText(mainText, subText);
    }
}
