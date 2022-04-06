using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;

public class SortNumber : QuickGame
{
    public Queue<int> UITextValues = new Queue<int>();

    public override IEnumerator gameProcess()
    {
        showGamePopupWindow();
        yield return new WaitForSeconds(2.5f);
        hideGamePopupWindow();
        StartCoroutine(SetupGame());
        yield return new WaitUntil(() => setUpFinished);
        openGameUI();
        countdownRoutine = StartCoroutine(CountdownTime());

        int buttonPressed = 0;
        while (buttonPressedValues.Count < gameUI.buttons.Count)
        {
            int correctAns = lastPressedValue + 1;
            yield return new WaitUntil(() => buttonPressed != lastPressedValue);
            if (lastPressedValue == correctAns)//ตอบถูก
            {
                buttonPressed++;
            }
            else
            {
                setAlertUI(true, Color.red, "Fail");
                StartCoroutine(EndGame());
                yield break;
            }
        }

        setAlertUI(true, Color.green, "Complete");

        setSuccessStatus(true);
        StartCoroutine(EndGame());
    }

    public override IEnumerator SetupGame()
    {
        setAlertUI(false, Color.green, "Complete");
        generateUIButtonValues();
        yield return new WaitUntil(() => UITextValues.Count == gameUI.buttons.Count);
        StartCoroutine(base.SetupGame());
    }

    public override void resetGame()
    {
        base.resetGame();
        UITextValues.Clear();
        setAlertUI(false, Color.black, "");
    }

    public override void actionInGame(int buttonIndex, int buttonValue)
    {
        base.actionInGame(buttonIndex, buttonValue);
    }

    public void generateUIButtonValues()
    {
        List<int> addedNums = new List<int>();
        while (addedNums.Count < gameUI.buttons.Count)
        {
            int randomNumber = Random.Range(1, gameUI.buttons.Count + 1);
            if (!addedNums.Contains(randomNumber))
            {
                addedNums.Add(randomNumber);
                addUINumberToQueue(randomNumber);
            }
        }
    }

    [ClientRpc]
    public void addUINumberToQueue(int number)
    {
        UITextValues.Enqueue(number);
    }

    [ClientRpc]
    public void setAlertUI(bool active, Color textColor, string alertText)
    {
        gameUI.setAlertText(active, textColor, alertText);
    }
}