using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class TabColorController : QuickGame
{
    public Queue<int> solveColors = new Queue<int>();

    public List<GameObject> HintCreated = new List<GameObject>();

    public override IEnumerator gameProcess()
    {
        showGamePopupWindow();
        yield return new WaitForSeconds(3.5f);
        hideGamePopupWindow();

        openGameUI();
        StartCoroutine(SetupGame());
        yield return new WaitUntil(() => setUpFinished);

        showHintPopUp();
        setUpHintUI();
        yield return new WaitForSeconds(2f);
        hideHintPopUp();

        countdownRoutine = StartCoroutine(CountdownTime());

        Queue<int> answer = new Queue<int>();
        //เป็นตัวเริ่มนับ
        int countSolve = solveColors.Count;
        int checkerButton = -1;//lastPressedValue
        int buttonPressed = 0;
        while (buttonPressedValues.Count < countSolve)
        {
            lastPressedValue = -1;
            yield return new WaitUntil(() => checkerButton != lastPressedValue);
            Debug.Log("lastPressedValue =" + lastPressedValue);
            if (lastPressedValue == solveColors.Dequeue())//ถูก
            {
                setLifeLight(buttonPressed, Color.green);
                answer.Enqueue(lastPressedValue);
                buttonPressed++;
            }
            else
            {
                while (buttonPressed < countSolve)
                {
                    setLifeLight(buttonPressed, Color.red);
                    buttonPressed++;
                }
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
        randomSolveColor();
        yield return new WaitUntil(() => solveColors.Count == gameUI.buttons.Count - 1);

        StartCoroutine(base.SetupGame());
        //StartCoroutine("ShowHintColor");
    }

    [ClientRpc]
    public override void resetGame()
    {
        base.resetGame();
        solveColors.Clear();
        gameUI.GetComponent<TabColorUI>().ResetLamp();
        setAlertUI(false, Color.black, "Null");
    }

    [ClientRpc]
    private void setUpHintUI()
    {
        TabColorUI ui = gameUI as TabColorUI;
        Queue<int> symbolIndexes = new Queue<int>(solveColors);
        ui.setUpHintSymbol(symbolIndexes);
    }
    [ClientRpc]
    private void showHintPopUp()
    {
        TabColorUI ui = gameUI as TabColorUI;
        ui.showHintPopup();
    }
    [ClientRpc]
    private void hideHintPopUp()
    {
        TabColorUI ui = gameUI as TabColorUI;
        ui.hideHintPopup();
    }


    private void randomSolveColor()
    {
        for (int i = 0; i < gameUI.buttons.Count - 1; i++)
        {
            int randNum;
            do
            {
                randNum = Random.Range(0, 4);
            } while (solveColors.Contains(randNum));
            addUINumberToQueue(randNum);
        }
    }

    public override void actionInGame(int buttonIndex, int buttonValue)
    {
        lastPressedValue = buttonValue;
        buttonPressedValues.Add(lastPressedValue);
    }

    [ClientRpc]
    public void addUINumberToQueue(int number)
    {
        solveColors.Enqueue(number);
    }

    [ClientRpc]
    public void setAlertUI(bool active, Color textColor, string alertText)
    {
        gameUI.setAlertText(active, textColor, alertText);
    }


    [ClientRpc]
    public void setLifeLight(int index, Color lightColor)
    {
        gameUI.GetComponent<TabColorUI>().setLight(index, lightColor);
    }


}

