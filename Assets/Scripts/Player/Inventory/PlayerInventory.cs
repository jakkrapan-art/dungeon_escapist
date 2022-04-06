using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] private BoardPlayer owner = null;

    [Header("Items In Backpack")]
    [SerializeField] private Item item = null;

    [SerializeField] private GameObject inventoryParent;

    [SyncVar(hook = nameof(HandleKeyCountChanged))]
    [SerializeField] private int keyCount = 0;
    #region Handlerer
    private void HandleKeyCountChanged(int oldValue, int newValue)
    {
        owner.CmdUpdateBoardUI();
    }
    #endregion

    private void Start()
    {
        owner = transform.GetComponent<BoardPlayer>();
    }

    public Item Item
    {
        get { return item; }
    }

    public int KeyCount
    {
        get { return keyCount; }
    }

    [Command]
    public void CmdReceiveKey(int amount)
    {
        RpcReceiveKey(amount);
    }
    [ClientRpc]
    public void RpcReceiveKey(int amount)
    {
        keyCount += amount;
    }

    public void receiveItem(Item item)
    {
        if (this.item)
        {
            StartCoroutine(getNewItemDecition(item));
        }
        else
        {
            getNewItem(item);
        }
    }

    private IEnumerator getNewItemDecition(Item item)
    {
        NewItemDecitionUI newItemUI = NewItemDecitionUI.instance;
        newItemUI.openWindow(owner, item.ItemName);

        yield return new WaitUntil(() => newItemUI.isFinishedDecition);

        if (newItemUI.isPlayerAcceptedItem)
        {
            getNewItem(item);
        }

        newItemUI.closeWindow();
    }

    private void getNewItem(Item item)
    {
        this.item = item;
        owner.CmdApplyStatusEffect();
    }

    [Command]
    public void useItem()
    {
        if (item == null)
        {
            return;
        }

        item = null;
        owner.CmdApplyStatusEffect();
    }

    [Command]
    public void setKey(int amount)
    {
        keyCount = amount;
    }
}
