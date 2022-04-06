using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    private LobbyUIController lobbyUIScript = null;

    public static LobbyManager instance = null;
    public EscapistNetworkManager networkManager;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        networkManager = EscapistNetworkManager.singleton as EscapistNetworkManager;
    }

    public void updatePlayerDisplay()
    {
        try
        {
            if (SceneManager.GetActiveScene().path == networkManager.lobbyScene)
            {
                if (lobbyUIScript == null)
                {
                    lobbyUIScript = FindObjectOfType<LobbyUIController>();
                }

                lobbyUIScript.updatePlayerDisplay(networkManager.lobbyPlayers);
            }
        }
        catch (System.Exception e)
        {

        }
    }

    public bool isReadyToStartGame()
    {
        foreach (var player in networkManager.lobbyPlayers)
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
        networkManager.ServerChangeScene(networkManager.gameScenes[0]);
    }
}
