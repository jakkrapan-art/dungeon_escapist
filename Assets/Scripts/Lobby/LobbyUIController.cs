using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField] private Text readyBottonText;

    [Header("Player Displays")]
    [SerializeField] private List<PlayerDisplay> playerDisplays;

    [Header("Map")]
    [SerializeField] private Text mapName;
    [SerializeField] private Image mapImage;

    private LobbyPlayer lobbyPlayerScript;
    private EscapistNetworkManager networkManager;

    private void Start()
    {
        if (networkManager == null)
        {
            networkManager = NetworkManager.singleton as EscapistNetworkManager;
        }
    }

    public void updatePlayerDisplay(List<LobbyPlayer> players)
    {
        if (players.Count > 0)
        {
            for (int i = 0; i < playerDisplays.Count; i++)
            {
                if (i < players.Count)
                {
                    var player = players[i];
                    Color textColor;

                    ColorUtility.TryParseHtmlString(player.isLocalPlayer ? "#E9CA0D" : "#000000", out textColor);
                    playerDisplays[i].displayName.text = $"{player.displayName}";
                    playerDisplays[i].displayName.color = textColor;

                    ColorUtility.TryParseHtmlString(player.isHost ? "#1C1EBC" : player.isReady ? "#00FF50" : "#BC0000", out textColor);
                    playerDisplays[i].displayStatus.text = player.isHost ? "Host" : player.isReady ? "/" : "X";
                    playerDisplays[i].displayStatus.color = textColor;

                    playerDisplays[i].displayColor.color = player.getPlayerColor();
                }
                else
                {
                    playerDisplays[i].displayName.text = $"";
                    playerDisplays[i].displayStatus.text = $"";
                    playerDisplays[i].displayColor.color = Color.gray;
                }
            }
        }

    }

    public void buttonStartOrReady()
    {
        if (lobbyPlayerScript.isHost)
        {
            startGame();
        }
        else
        {
            readyUp();
        }
    }

    private void startGame()
    {
        if (LobbyManager.instance.isReadyToStartGame())
            LobbyManager.instance.startGame();
        else
            Debug.Log($"Some player not ready or player less than 2.");
    }

    private void readyUp()
    {
        lobbyPlayerScript.readyUp();
    }

    private void Update()
    {
        if (lobbyPlayerScript == null)
        {
            var lobbyPlayerObjects = networkManager.lobbyPlayers;
            foreach (var player in lobbyPlayerObjects)
            {
                if (player.isLocalPlayer)
                {
                    lobbyPlayerScript = player;
                    readyBottonText.text = player.isHost ? "START" : "READY";
                }
            }
        }
    }

    public void disconnect()
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
    }
}
