using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Punish : Tile
{
    [SerializeField] private int punishCount = 0; //count as step for move backward type. And count as turn for stop move type.
    public punishType type;

    public override void doEvent(BoardPlayer player)
    {
        base.doEvent(player);
    }

    protected override void tileEvent()
    {
        base.tileEvent();

        if (type == punishType.moveBackward)
        {
            StartCoroutine(playerCurrentTurn.punishMoveBackward(punishCount));
        }
        else
        {
            playerCurrentTurn.punishStopMove(punishCount);
        }
    }
}

public enum punishType { moveBackward, stop }
