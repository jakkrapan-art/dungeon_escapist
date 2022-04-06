using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinigameController : NetworkBehaviour
{
    public List<Minigame> minigames = new List<Minigame>();

    public BoardPlayer winner = null;
    private EscapistNetworkManager networkManager
    {
        get
        {
            return EscapistNetworkManager.singleton as EscapistNetworkManager;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public void playRandomMinigame()
    {
        int randomIndex = Random.Range(0, minigames.Count);
        playMinigame(randomIndex);
    }

    [ClientRpc]
    private void playMinigame(int index)
    {
        StartCoroutine(startMinigame(minigames[index]));
    }

    protected IEnumerator startMinigame(Minigame game)
    {
        networkManager.ServerChangeScene(game.gameScene.path);
        yield return new WaitUntil(() => networkManager.isPlayersReadied());
        game.StartMinigame();
    }
}
