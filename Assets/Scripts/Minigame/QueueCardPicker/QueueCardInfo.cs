using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class QueueCardInfo
{
    public int queueNumber { get; private set; }
    public BoardPlayer owner { get; private set; }

    public void setQueueNumber(int value)
    {
        queueNumber = value;
    }

    public void setOwner(BoardPlayer player)
    {
        owner = player;
    }
}
