using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Minigame : MonoBehaviour
{
    [Header("Rewards")]
    [SerializeField] public List<Reward> rewards = new List<Reward>();
    [Space]
    protected bool isDone;

    [Header("Scene")]
    public Scene gameScene;

    //minigameText is for some minigame that need some condition to play
    public virtual void play(string minigameText)
    {
        gameObject.SetActive(true);
    }

    public virtual void StartMinigame()
    {
        //BoardPlayer.minigameAction += playerAction;
        StartCoroutine(gameProcess());
    }

    protected virtual IEnumerator gameProcess()
    {
        yield return null;
    }

    protected virtual void OnMinigameEnd()
    {
        //BoardPlayer.minigameAction -= playerAction;
    }

    protected virtual void resetGame()
    {

    }

    protected virtual void playerAction()
    {

    }

    public virtual void giveReward(BoardPlayer targetPlayer, int keyAmount)
    {
        PlayerInventory playerInv = targetPlayer.GetComponent<PlayerInventory>();
        if (playerInv == null)
        {
            Debug.LogError("Null Player Inventory.");
            return;
        }
        playerInv.CmdReceiveKey(keyAmount);
    }

    public List<Reward> getRewards()
    {
        return rewards;
    }
}
