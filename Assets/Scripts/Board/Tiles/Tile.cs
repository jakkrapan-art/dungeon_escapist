using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class Tile : NetworkBehaviour
{
    [SerializeField] protected BoardPlayer playerCurrentTurn;

    [SerializeField]
    protected Transform[] standPointTransforms = new Transform[4];

    protected Dictionary<Transform, BoardPlayer> standPoints = new Dictionary<Transform, BoardPlayer>();

    [SerializeField] private List<Tile> adjacentTiles = new List<Tile>(); //Last index is a forward tile.
    private void Start()
    {
        var cubeRenderer = GetComponent<Renderer>();
        initialSetupStandPointsDict();
    }

    private void initialSetupStandPointsDict()
    {
        foreach (var transform in standPointTransforms)
        {
            standPoints.Add(transform, null);
        }
    }

    public List<Tile> AdjacentTiles
    {
        get { return adjacentTiles; }
    }

    public Vector3 getTilePosition(BoardPlayer player)
    {
        foreach (var standPoint in standPoints)
        {
            if (standPoint.Value == null)
            {
                standPoints[standPoint.Key] = player;
                return standPoint.Key.position;
            }
        }
        return transform.position;
    }

    [Server]
    public void removePlayerFromTile(BoardPlayer player)
    {
        Transform targetKey = null;
        if (player == null)
        {
            return;
        }

        int index = 0;
        foreach (var standPoint in standPoints)
        {
            if (standPoint.Value != null && standPoint.Value.Equals(player))
            {
                targetKey = standPoint.Key;
            }

            index++;
        }

        //Debug.Log(targetKey);
        standPoints[targetKey] = null;
    }

    public virtual void doEvent(BoardPlayer player)
    {
        playerCurrentTurn = player;

        if (player.MovePoint <= 0)
        {
            player.playerAction += tileEvent;
        }
    }

    protected virtual void tileEvent()
    {
        if (playerCurrentTurn != null)
        {
            playerCurrentTurn.playerAction -= tileEvent;
        }
    }
}
