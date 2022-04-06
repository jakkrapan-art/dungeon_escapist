using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Tile_Door : Tile
{
    [SerializeField] private DoorUIController doorUI = null;
    [SerializeField] private Tile permissionTile = null;
    [SerializeField] private int keyRequire = 0;
    [SerializeField] private int scoreRequire = 1000;
    [SerializeField] private List<BoardPlayer> doorPermissionPlayers = new List<BoardPlayer>();

    public Tile getPermissionTile() => permissionTile;

    public bool isHavePermissionToAccessDoor(BoardPlayer player) => doorPermissionPlayers.Contains(player);

    [Server]
    private void addPlayerPermission(BoardPlayer player) => RpcAddPlayerPermission(player);

    [ClientRpc]
    private void RpcAddPlayerPermission(BoardPlayer player) => doorPermissionPlayers.Add(player);

    [ClientRpc]
    public void removePlayerAccessedDoor(BoardPlayer player) => doorPermissionPlayers.Remove(player);

    public void unlockDoor(BoardPlayer player)
    {
        //var playerInventory = player.GetComponent<PlayerInventory>();
        if (player.Scores >= scoreRequire)
        {
            Debug.Log($"{player} unlocks the door {name}.");
            //player.useKey(keyRequire);
            player.RpcAddPlayerScore(-keyRequire);
            addPlayerPermission(player);
        }
        else
        {
            Debug.LogError($"Score is not enough for open the door (Require: {scoreRequire}).");
        }
    }

    public override void doEvent(BoardPlayer player)
    {
        playerCurrentTurn = player;
        player.playerAction += tileEvent;
    }

    protected override void tileEvent()
    {
        base.tileEvent();

        if (doorUI == null)
        {
            Debug.LogWarning("DoorUI is null.");
            doorUI = DoorUIController.instance;
        }

        if (doorPermissionPlayers.Contains(playerCurrentTurn))
        {
            return;
        }

        //open doorUI
        doorUI.openUI(this, playerCurrentTurn, scoreRequire);
    }
}
