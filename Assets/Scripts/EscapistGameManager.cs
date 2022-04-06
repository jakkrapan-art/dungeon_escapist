using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EscapistGameManager : NetworkBehaviour
{
    [SyncVar]
    public int numPlayer = 0;

    [Header("Scenes")]
    [Scene] public string LobbyScene = null;

    [Scene] public string BoardGameScene = null;
    [Scene] public string WordSnatcherScene = null;

    [Space]
    public static EscapistGameManager instance = null;

    public List<LobbyPlayer> lobbyPlayers { get; } = new List<LobbyPlayer>();
    public List<BoardPlayer> boardPlayers { get; } = new List<BoardPlayer>();

    [Header("Board scene components")]
    [SerializeField] private GameObject boardGameController = null;
    [SerializeField] private GameObject minigameCenterPrefab = null;

    [Header("Player prefabs")]
    [SerializeField] private LobbyPlayer lobbyPlayerPrefab = null;
    [SerializeField] private BoardPlayer boardPlayerPrefab = null;

    private void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (isServer)
        {
            updateCurrentPlayerCount();
        }
    }

    private void updateCurrentPlayerCount()
    {
        numPlayer = NetworkServer.connections.Count;
    }

    [Server]
    public IEnumerator spawnBoardPlayer()
    {
        yield return new WaitUntil(() => isBoardPlayersReadied());

        foreach (var player in boardPlayers)
        {
            player.spawnCharacter();
        }
    }

    [Server]
    public bool isBoardPlayersReadied()
    {
        foreach (var player in boardPlayers)
        {
            if (!player.connectionToClient.isReady)
            {
                return false;
            }
        }

        return true;
    }

    [Server]
    public void transformLobbyPlayerToBoardPlayer()
    {
        GameObject boardControllerInstance = Instantiate(boardGameController);
        NetworkServer.Spawn(boardControllerInstance);
        var boardControllerComponent = boardControllerInstance.GetComponent<BoardGameController>();

        for (int i = lobbyPlayers.Count - 1; i >= 0; i--)
        {
            NetworkConnection conn = lobbyPlayers[i].connectionToClient;

            var boardPlayer = Instantiate(boardPlayerPrefab);
            boardPlayer.name = $"BoardPlayer[{conn.connectionId}]";
            boardPlayer.setDisplayName(lobbyPlayers[i].displayName);

            NetworkServer.Destroy(conn.identity.gameObject);
            NetworkServer.ReplacePlayerForConnection(conn, boardPlayer.gameObject);
        }
    }

    [Server]
    public void ServerChangeScene(string sceneName)
    {
        NetworkManager.singleton.ServerChangeScene(sceneName);
    }
}
