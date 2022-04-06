using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class QuickGameController : NetworkBehaviour
{
    public static QuickGameController instance;
    public List<QuickGame> quickGames = new List<QuickGame>();
    [Header("Rewards")]
    [SerializeField] private Transform rewardsParent;
    [SerializeField] private List<NetworkItem> rewardPool = new List<NetworkItem>();
    [Space]
    [SyncVar]
    public bool isGameEnded;

    [ClientRpc]
    public void setGameEndStatus(bool isEnded)
    {
        isGameEnded = isEnded;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < rewardsParent.childCount; i++)
        {
            rewardPool.Add(rewardsParent.GetChild(i).GetComponent<NetworkItem>());
        }
    }

    [Server]
    public void playQuickGame(BoardPlayer player)
    {
        //StartCoroutine(StartQuickGame(quickGames[0], player));
        StartCoroutine(StartQuickGame(quickGames[Random.Range(0, quickGames.Count)], player));
    }

    public IEnumerator StartQuickGame(QuickGame game, BoardPlayer player)
    {
        game.setPlayer(player);
        game.StartGame();

        yield return new WaitUntil(() => isGameEnded);

        if (game.isSuccess)
        {
            if (rewardPool.Count > 0)
            {
                NetworkItem rewardItem = null;
                rewardItem = rewardPool[Random.Range(0, rewardPool.Count)];

                player.RpcAddPlayerScore(100);
                givePlayerReward(player, rewardItem);
            }
        }

        game.setSuccessStatus(false);
        setGameEndStatus(false);
    }

    private void givePlayerReward(BoardPlayer player, NetworkItem item)
    {
        PlayerInventory inventory = player.gameObject.GetComponent<PlayerInventory>();
        item.transferItemToPlayer(player);
    }
}
