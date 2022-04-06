using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardGamePlayerDisplay : MonoBehaviour
{
    [SerializeField] protected Text playerName = null;
    [SerializeField] protected Text keyOwn = null;
    [SerializeField] protected Text scoreText = null;

    [SerializeField] protected GameObject diceDisplay = null;
    [SerializeField] protected Text diceValue = null;

    [SerializeField] protected bool isInQueueSorting = false;

    [SerializeField] protected Image background = null;

    private void Update()
    {
        try
        {
            isInQueueSorting = BoardGameController.instance.GameState == BoardGameState.SortPlayerQueue;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public virtual void updateKeyDisplay(int keyCount)
    {
        keyOwn.text = $"{keyCount}";
    }

    public void updatePlayerNameDisplay(string playerName)
    {
        this.playerName.text = playerName;
    }

    public void updateBackgroundColor(Color color)
    {
        background.color = color;
    }

    public void updatePlayerScore(long scores)
    {
        scoreText.text = $"{scores}";

    }

    public void updatePlayerDiceValue(int value)
    {
        diceValue.text = $"{value}";
        if (isInQueueSorting)
        {
            diceDisplay.SetActive(false);
        }
        else
        {
            toggleDiceDisplayer(value > 0);
        }
    }

    private void toggleDiceDisplayer(bool show)
    {
        if (show)
        {
            diceDisplay.SetActive(true);
        }
        else
        {
            diceDisplay.SetActive(false);
        }
    }

    public virtual void updateBoardGameUIDisplay(BoardPlayer player)
    {
        updatePlayerNameDisplay(player.DisplayName);
        updateKeyDisplay(player.GetComponent<PlayerInventory>().KeyCount);
        updateBackgroundColor(player.getPlayerColor());
        updatePlayerScore(player.Scores);
        //updatePlayerDiceValue(player.MovePoint);
    }
}
