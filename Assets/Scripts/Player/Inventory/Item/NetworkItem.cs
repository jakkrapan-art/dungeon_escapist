using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkItem : NetworkBehaviour
{
    public Item item;

    [ClientRpc]
    public void transferItemToPlayer(BoardPlayer targetPlayer)
    {
        /*if (!targetPlayer || targetPlayer != NetworkClient.connection.identity.GetComponent<BoardPlayer>())
        {
            return;
        }*/
        targetPlayer.GetComponent<PlayerInventory>().receiveItem(item);
    }
}
