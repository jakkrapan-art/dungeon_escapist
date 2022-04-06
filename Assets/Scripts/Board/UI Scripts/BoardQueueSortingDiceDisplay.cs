using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardQueueSortingDiceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName = null;
    [SerializeField] private TMP_Text dicePoint = null;
    [SerializeField] private TMP_Text orderNumber = null;

    private RectTransform rectTransform = null;
    private RectTransform targetMoveTransform = null;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void setDisplay(string playerName, int dicePoint, int orderNumber)
    {
        this.playerName.text = playerName;
        this.dicePoint.text = dicePoint.ToString();

        string orderNumberText = "";
        switch (orderNumber)
        {
            case 1:
                orderNumberText = $"{orderNumber}st";
                break;
            case 2:
                orderNumberText = $"{orderNumber}nd";
                break;
            case 3:
                orderNumberText = $"{orderNumber}rd";
                break;
            case 4:
                orderNumberText = $"{orderNumber}th";
                break;
        }

        this.orderNumber.text = orderNumberText;
    }

    public IEnumerator swap(RectTransform target)
    {
        targetMoveTransform = target;
        yield return new WaitUntil(() => isFinishedMove());
        targetMoveTransform = null;
    }

    private bool isFinishedMove()
    {
        if (rectTransform.anchoredPosition != targetMoveTransform.anchoredPosition)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetMoveTransform.anchoredPosition, 0.25f);
            return false;
        }

        return true;
    }
}
