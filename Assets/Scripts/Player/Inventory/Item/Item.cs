using UnityEngine;
using UnityEngine.UI;
using Mirror;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    [Header("Item Description")]
    [SerializeField]
    protected string itemName = null;
    [SerializeField]
    protected string itemDescription = null;
    [SerializeField]
    private Sprite itemIcon = null;

    [SerializeField]
    private StatusEffect itemStatusEffect;

    public StatusEffect getItemStatus() => itemStatusEffect;

    public string ItemName
    {
        get { return itemName; }
    }

    public string ItemDescription
    {
        get { return itemDescription; }
    }

    public Sprite getItemIcon() => itemIcon;

    /*public bool isNull()
    {
        if (itemName == string.Empty || itemName.Equals(string.Empty) || itemName == null || itemName.Trim().Length <= 0)
            return true;

        return false;
    }

    public virtual void use()
    {

    }

    public virtual void itemAction()
    {

    }*/
}
