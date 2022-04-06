using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Linq;
using System.Net;
using System.IO;
using System;

public class EscapistNetworkManager : NetworkManager
{
    private LobbyUIController lobbyUIScript;

    [Header("Scenes")]
    [Scene] public string menuScene = string.Empty;
    [Scene] public string lobbyScene = string.Empty;
    [Scene] public List<string> gameScenes = new List<string>();
    [Scene] public string endScene = string.Empty;

    /* 
    ============  Scene Indexs ==============
          ๐ [0] Board game scene
          ๐ [1] Word Snatcher scene
    */
    [Header("Player prefab")]
    [SerializeField] private LobbyPlayer lobbyPlayerPrefab = null;
    [SerializeField] private BoardPlayer boardPlayerPrefab = null;

    [Space]
    [SerializeField] private int minPlayer;

    public List<LobbyPlayer> lobbyPlayers { get; } = new List<LobbyPlayer>();
    public List<BoardPlayer> boardPlayers { get; } = new List<BoardPlayer>();

    public string lobbyName = string.Empty;
    public string mapName = string.Empty;

    [Space]
    [SerializeField] private GameObject boardGameController;
    [SerializeField] private GameObject WoldSnatcherController;

    [Space]
    [SerializeField] private GameObject ChatSystemObject;
    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnableObjects").ToList();
    }

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnableObjects");

        foreach (var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= NetworkServer.maxConnections || SceneManager.GetActiveScene().path != lobbyScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Debug.Log("OnServerAddPlayer() called.");
        if (SceneManager.GetActiveScene().path == lobbyScene)
        {
            bool isHost = lobbyPlayers.Count == 0;

            LobbyPlayer lobbyPlayerInstane = Instantiate(lobbyPlayerPrefab);

            lobbyPlayerInstane.isHost = isHost;

            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstane.gameObject);

            lobbyPlayerInstane.changePlayerColor(NetworkServer.connections.Count - 1);

            updatePlayerDisplay();
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        updatePlayerDisplay();
    }

    public void updatePlayerDisplay()
    {
        if (SceneManager.GetActiveScene().path == lobbyScene)
        {
            try
            {
                if (lobbyUIScript == null)
                {
                    lobbyUIScript = FindObjectOfType<LobbyUIController>();
                }
                lobbyUIScript.updatePlayerDisplay(lobbyPlayers);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public bool isReadyToStartGame()
    {
        foreach (var player in lobbyPlayers)
        {
            if (!player.isReady)
            {
                if (!player.isHost)
                    return false;
            }
        }

        return true;
    }

    public void startGame()
    {
        ServerChangeScene(gameScenes[0]);
    }

    public void endGame()
    {
        ServerChangeScene(endScene);
    }

    public void startWordSnatcher()
    {
        ServerChangeScene(gameScenes[1]);
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().path == lobbyScene && newSceneName.Equals(gameScenes[0]))
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
                boardPlayer.setPlayerColor(lobbyPlayers[i].getPlayerColor());

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, boardPlayer.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.Equals(gameScenes[0]))
        {
            StartCoroutine(nameof(boardSceneSetup));
        }
        else if (sceneName.Equals(gameScenes[1]))//หกดหสวดาฟหก
        {
            StartCoroutine(spawnSnatcherCharacter());
        }
    }

    private IEnumerator boardSceneSetup()
    {
        yield return new WaitUntil(() => isPlayersReadied());

        foreach (var player in boardPlayers)
        {
            player.spawnCharacter();
        }

        StartCoroutine(BoardGameController.instance.playTurnBoardGame());
    }

    private IEnumerator spawnSnatcherCharacter()
    {
        yield return new WaitUntil(() => isPlayersReadied());

        GameObject wordSnatcherController = Instantiate(WoldSnatcherController);
        WoldSnatcherController snatcherController = wordSnatcherController.GetComponent<WoldSnatcherController>();
        NetworkServer.Spawn(wordSnatcherController);
        yield return new WaitUntil(() => snatcherController.storages.Count > 0);
        foreach (var player in boardPlayers)
        {
            AreaStorage h = snatcherController.getEmptyStorage();
            h.setOwner(player);//Set Owner in Stroage
            h.setColor(player.getPlayerColor());
            player.spawnSnatcher(h.transform);
        }
    }

    #region network_connection
    public static bool isConnectingToNetwork()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");

        if (HtmlText == "")
        {
            //no connection
            return false;
        }
        else
        {
            //success
            return true;
        }
    }

    private static string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }
    #endregion
    public bool isPlayersReadied()
    {
        foreach (var player in boardPlayers)
        {
            if (!player.connectionToClient.isReady || !player.IsPlayerReadied)
            {
                Debug.Log(player.DisplayName + " is not ready.");
                return false;
            }
        }

        return true;
    }

    [Server]
    public void giveConnectionIdentityToObject(GameObject currentObject, GameObject targetObject)
    {
        Debug.Log($"Replace client identity to .... [{targetObject}]");
        NetworkServer.ReplacePlayerForConnection(currentObject.GetComponent<NetworkIdentity>().connectionToClient, targetObject);
    }
}
