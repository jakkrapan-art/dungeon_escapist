using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class QueueCard : NetworkBehaviour
{
    [SyncVar(hook = nameof(updateUI))]
    [SerializeField] private QueueCardInfo info;
    public QueueCardInfo Info
    {
        get { return info; }
    }

    [Header("Back side")]
    [SerializeField] private GameObject backSideObj = null;
    [SerializeField] private TMP_Text pickerNameBackUI;

    [Header("Front side")]
    [SerializeField] private GameObject frontSideObj = null;
    [SerializeField] private TMP_Text pickerNameFrontUI;
    [SerializeField] private TMP_Text cardNumber;

    [Space]
    [SyncVar]
    private bool isFrontSide = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            name = ($"[{info.queueNumber}] | {info.owner}");
        }
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
            flipCard();
        }

        if (isFrontSide)
        {
            frontSideObj.SetActive(true);
            backSideObj.SetActive(false);
        }
        else
        {
            frontSideObj.SetActive(false);
            backSideObj.SetActive(true);
        }

    }

    public void setCardInfo(QueueCardInfo info)
    {
        this.info = info;
    }
    public void pickCard(BoardPlayer player)
    {
        info.setOwner(player);
    }


    private void updateUI(QueueCardInfo oldValue, QueueCardInfo newValue)
    {
        if (newValue.owner)
        {
            pickerNameBackUI.text = newValue.owner.DisplayName;
            pickerNameFrontUI.text = newValue.owner.DisplayName;
        }
        else
        {
            pickerNameBackUI.text = string.Empty;
            pickerNameFrontUI.text = string.Empty;
        }

        cardNumber.text = newValue.queueNumber.ToString();
    }

    [Server]
    public void flipCard()
    {
        isFrontSide = !isFrontSide;
    }
}
