using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_QuickGame : Tile
{
    public override void doEvent(BoardPlayer player)
    {
        playerCurrentTurn = player;
        player.playerAction += tileEvent;
    }

    protected override void tileEvent()
    {
        base.tileEvent();
        playerCurrentTurn.CmdStartQuickGame();
    }
}
