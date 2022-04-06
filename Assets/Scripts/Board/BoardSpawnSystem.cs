using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class BoardSpawnSystem : NetworkBehaviour
{
    private Dictionary<NetworkConnection, Tile> playerPositions = new Dictionary<NetworkConnection, Tile>();

    [SerializeField]
    private EscapistNetworkManager networkManager;

    [SerializeField]
    private Tile startTile;

    private bool isBoardScene;
    private bool isAlreadySetPlayerPosition;

    void Start()
    {
        DontDestroyOnLoad(this);

        networkManager = NetworkManager.singleton as EscapistNetworkManager;

        if (playerPositions.Count <= 0)
        {
            foreach (var player in networkManager.boardPlayers)
            {
                savePlayerPosition(player, startTile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "BoardScene" && !isAlreadySetPlayerPosition)
        {
            foreach (var player in networkManager.boardPlayers)
            {
                var conn = player.connectionToClient;
                setPlayerPosition(conn);
            }
        }
        else
        {
            foreach (var player in networkManager.boardPlayers)
            {
                //savePlayerPosition(player , player.currentTile);
            }
        }
    }

    private void setPlayerPosition(NetworkConnection conn)
    {
        conn.identity.gameObject.transform.position = playerPositions[conn].transform.position;
    }

    private void savePlayerPosition(BoardPlayer player, Tile currentPos)
    {
        var conn = player.connectionToClient;
        //player.currentTile = currentPos;

        playerPositions.Add(conn, currentPos);
    }
}
