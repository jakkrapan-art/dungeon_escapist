using UnityEngine;
using UnityEngine.UI;

public class NewItemDecitionUI : MonoBehaviour
{
    public static NewItemDecitionUI instance;

    [Header("Texts")]
    public Text newItemNameText;

    public bool isPlayerAcceptedItem;
    public bool isFinishedDecition;
    [Space]

    public GameObject Ui;

    private void Awake()
    {
        instance = this;
    }

    private void setupUI(string newItemName)
    {
        newItemNameText.text = newItemName;
    }

    public void acceptNewItem()
    {
        isPlayerAcceptedItem = true;
        isFinishedDecition = true;
    }

    public void rejectNewItem()
    {
        isPlayerAcceptedItem = false;
        isFinishedDecition = true;
    }

    public void openWindow(BoardPlayer player, string newItemName)
    {
        if (player == Mirror.NetworkClient.connection.identity.GetComponent<BoardPlayer>())
        {
            Ui.SetActive(true);
            setupUI(newItemName);
        }
    }

    public void closeWindow()
    {
        isPlayerAcceptedItem = false;
        isFinishedDecition = false;

        Ui.SetActive(false);
    }
}
