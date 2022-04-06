using UnityEngine;

[System.Serializable]
public class Reward
{
    [SerializeField] private Item item = null;
    [SerializeField] private int amount = 0;

    public Reward(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public Item getItem()
    {
        return item;
    }

    public int getAmount()
    {
        return amount;
    }
}
