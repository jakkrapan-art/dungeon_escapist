using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoardPlayerCharacter : NetworkBehaviour
{
    public BoardPlayer owner;

    [SerializeField]
    private StatusEffectShowcase effectShowcase;

    private float moveSpeed = 50f;
    [SyncVar(hook = nameof(HandleCharacterColorChanged))]
    private Color characterColor = Color.white;
    #region Handlerer
    private void HandleCharacterColorChanged(Color oldColor, Color newColor)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Renderer>())
            {
                transform.GetChild(i).GetComponent<Renderer>().material.color = newColor;
            }
        }
    }
    #endregion

    [Server]
    public void setOwner(BoardPlayer player)
    {
        owner = player;
    }

    [Command]
    public void CmdMoveToTargetPosition(Vector3 targetPosition)
    {
        RpcMoveToTargetPosition(targetPosition);
    }

    [ClientRpc]
    private void RpcMoveToTargetPosition(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void setColor(Color color)
    {
        characterColor = color;
    }

    public bool isFinishedMove(Vector3 targetPosition)
    {
        //CmdMoveToTargetPosition(targetPosition);

        return transform.position == targetPosition;
    }

    public void setStatusEffect(StatusEffect effect)
    {
        effectShowcase.setStatusEffect(effect);
    }
}
