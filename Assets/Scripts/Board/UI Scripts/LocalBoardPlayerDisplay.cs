using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalBoardPlayerDisplay : BoardGamePlayerDisplay
{
    [SerializeField] private Image itemDisplay = null;

    public void updateHoldingItemDisplay(Item item)
    {
        if (item == null)
        {
            return;
        }

        itemDisplay.sprite = item.getItemIcon();
    }

    public override void updateBoardGameUIDisplay(BoardPlayer player)
    {
        base.updateBoardGameUIDisplay(player);
        updateHoldingItemDisplay(player.GetComponent<PlayerInventory>().Item);
    }
}
