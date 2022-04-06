using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_End : Tile
{
    public override void doEvent(BoardPlayer player)
    {
        playerCurrentTurn = player;
        player.playerAction += tileEvent;
    }

    protected override void tileEvent()
    {
        base.tileEvent();
        Debug.Log($"Player {playerCurrentTurn.DisplayName} has reach the finish line.");
        playerCurrentTurn.callForEndGame();
    }
}
