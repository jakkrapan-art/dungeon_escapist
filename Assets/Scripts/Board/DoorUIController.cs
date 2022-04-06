using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorUIController : MonoBehaviour
{
    public static DoorUIController instance = null;

    public GameObject doorUI = null;
    public Text descriptionText = null;
    public Button doorOpenButton = null;

    [Space]

    [SerializeField] private Tile_Door doorTile = null;

    private BoardPlayer interactedPlayer = null;
    private bool isDone = false;

    public bool IsDone
    {
        get { return isDone; }
        set { isDone = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void setDescriptionText(string text)
    {
        descriptionText.text = text;
    }

    public void openUI(Tile_Door doorTile, BoardPlayer player, long scoreRequire)
    {
        doorUI.SetActive(true);
        isDone = false;

        this.doorTile = doorTile;
        interactedPlayer = player;

        long different = scoreRequire - player.Scores;

        //string description;
        if (different > 0) //Player key is not enough
        {
            doorOpenButton.interactable = false;
        }
        else
        {
            doorOpenButton.interactable = true;
        }

        //setDescriptionText(description);
    }

    public void closeUI()
    {
        isDone = true;
        interactedPlayer = null;
        doorUI.SetActive(false);
    }

    public void openDoor()
    {
        interactedPlayer.requestToOpenDoor();
    }
}
