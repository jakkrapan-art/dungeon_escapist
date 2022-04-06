using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Newtonsoft.Json;
using TMPro;

public class LobbyPlayer : NetworkBehaviour
{

    [SerializeField]
    private EscapistNetworkManager networkManager;
    private EscapistNetworkManager NWManager
    {
        get
        {
            if (networkManager == null)
            {
                return networkManager = NetworkManager.singleton as EscapistNetworkManager;
            }

            return networkManager;
        }
    }

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string displayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool isReady = false;
    [SyncVar(hook = nameof(HandleLobbyHostStatusChanged))]
    public bool isHost = false;

    [SyncVar]
    public Color playerColor = Color.white;
    #region Handlerer & Getter/Setter
    private void HandlePlayerColorChanged(Color oldColor, Color newColor)
    {
        updateLobbyDisplay();
    }
    public Color getPlayerColor() => playerColor;
    private void setPlayerColor(Color newColor) => playerColor = newColor;
    #endregion

    [SerializeField] private GameObject boardPlayerPrefab;

    public GameObject BoardPlayerPrefab
    {
        get { return boardPlayerPrefab; }
    }

    private void HandleDisplayNameChanged(string oldValue, string newValue)
    {
        updateLobbyDisplay();
    }

    private void HandleReadyStatusChanged(bool oldValue, bool newValue)
    {
        updateLobbyDisplay();
    }

    private void HandleLobbyHostStatusChanged(bool oldValue, bool newValue)
    {
        updateLobbyDisplay();
    }

    private void updateLobbyDisplay()
    {
        LobbyManager.instance.updatePlayerDisplay();
    }

    public override void OnStartAuthority()
    {
        User user = JsonConvert.DeserializeObject<User>(PlayerPrefs.GetString("user"));
        CmdSetDisplayName(user.getDisplayName(12));
    }

    public override void OnStartClient()
    {
        NWManager.lobbyPlayers.Add(this);

        LobbyManager.instance.updatePlayerDisplay();
    }

    public override void OnStopClient()
    {
        NWManager.lobbyPlayers.Remove(this);
        LobbyManager.instance.updatePlayerDisplay();
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    [Client]
    public void readyUp()
    {
        if (!this.hasAuthority)
        {
            Debug.Log("Didn't authority yet");
            return;
        }

        cmdReadyUp();
    }

    [Command]
    private void cmdReadyUp()
    {
        isReady = !isReady;
    }

    [Server]
    public void changePlayerColor(int colorIndex)
    {
        Color color = Color.white;
        switch (colorIndex)
        {
            case 0:
                ColorUtility.TryParseHtmlString("#AF0000", out color);
                break;
            case 1:
                ColorUtility.TryParseHtmlString("#0000AF", out color);
                break;
            case 2:
                ColorUtility.TryParseHtmlString("#00FB00", out color);
                break;
            case 3:
                ColorUtility.TryParseHtmlString("#EC9913", out color);
                break;
        }

        setPlayerColor(color);
    }
}
