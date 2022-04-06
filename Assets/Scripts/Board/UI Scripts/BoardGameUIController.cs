using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoardGameUIController : MonoBehaviour
{
    public static BoardGameUIController instance = null;
    [SerializeField]
    private BoardPlayer localBoardPlayer = null;

    //public GameObject boardQueueSortingDiceParent = null;
    //public BoardQueueSortingDiceDisplay[] boardQueueSortingDiceDisplays = new BoardQueueSortingDiceDisplay[4];

    [SerializeField]
    private BoardGamePlayerDisplay[] playerDisplays;

    [Space]
    [SerializeField] private GameObject playerOnTurnStepCountUI;
    [SerializeField] private Text playerOnTurnName;
    [SerializeField] private Text playerStepLeft;

    public Button rollDiceButton = null;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        StartCoroutine(nameof(initializeSetup));
    }

    private void Update()
    {
        try
        {
            if (BoardGameController.instance.GameState != BoardGameState.SortPlayerQueue)
            {
                //boardQueueSortingDiceParent.SetActive(false);
            }
            else
            {
                //boardQueueSortingDiceParent.SetActive(true);
                updatePlayerDisplays(BoardGameController.instance.players);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public void setLocalBoardPlayer(BoardPlayer boardPlayer)
    {
        localBoardPlayer = boardPlayer;
    }

    private List<BoardPlayer> findLocalPlayer()
    {
        var players = FindObjectsOfType<BoardPlayer>();
        foreach (var player in players)
        {
            if (player.isLocalPlayer)
            {
                localBoardPlayer = player;
            }
        }

        return players.ToList();
    }

    public void rollDice()
    {
        if (localBoardPlayer == null)
        {
            return;
        }

        localBoardPlayer.rollDice();
    }

    public IEnumerator initializeSetup()
    {
        yield return new WaitUntil(() => findLocalPlayer().Count > 0);

        var players = findLocalPlayer();
        int totalPlayer = players.Count;

        for (int i = 0; i < totalPlayer; i++)
        {
            playerDisplays[i].gameObject.SetActive(true);
            //boardQueueSortingDiceDisplays[i].gameObject.SetActive(true);
        }

        updatePlayerDisplays(players);
    }

    public void updatePlayerDisplays(List<BoardPlayer> players)
    {
        int index = 1;
        foreach (var player in players)
        {
            if (!player.isLocalPlayer)
            {
                playerDisplays[index++].updateBoardGameUIDisplay(player);
            }
            else
            {
                playerDisplays[0].updateBoardGameUIDisplay(player);
            }
        }

        updatePlayerStepLeft(BoardGameController.instance.playerOnTurn);
    }

    private void updatePlayerStepLeft(BoardPlayer playerOnTurn)
    {

        if (playerOnTurn)
        {
            playerOnTurnStepCountUI.SetActive(true);
            playerOnTurnName.text = playerOnTurn.DisplayName;
            playerStepLeft.text = playerOnTurn.MovePoint.ToString();
        }
        else
        {
            playerOnTurnStepCountUI.SetActive(false);
            playerOnTurnName.text = "";
            playerStepLeft.text = "";
        }
    }

    private void updateDiceQueueUI(List<BoardPlayer> players)
    {
        string playerName = "";

        foreach (var player in players)
        {

            if (player.isLocalPlayer)
            {
                playerName = $"<color=#E9CA0D>You</color>";
            }
            else
            {
                playerName = player.DisplayName;
            }

            //boardQueueSortingDiceDisplays[index++].setDisplay(playerName, player.MovePoint, player.MovePoint == 0 ? 0 : player.getPlayerQueueOrder() + 1);
        }
    }
}
