using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestroyRockController : QuickGame
{
    private int liftPoint = 2;
    private Coroutine coMoving = null;
    public override IEnumerator gameProcess()
    {
        showGamePopupWindow();
        yield return new WaitForSeconds(3.5f);
        hideGamePopupWindow();

        int checkerButton = -1;
        while (buttonPressedValues.Count < liftPoint)
        {
            StartCoroutine(SetupGame());
            openGameUI();
            
            yield return new WaitUntil(() => setUpFinished);
            StartMovingPickaxeArrow();
            countdownRoutine = StartCoroutine(CountdownTime());

            lastPressedValue = -1;
            yield return new WaitUntil(() => checkerButton != lastPressedValue);
            RPCStopMovingPickaxeArrow();
            if (!gameUI.GetComponent<DestroyRockUI>().CheakBreakPoint())
            {
                if (buttonPressedValues.Count < liftPoint)
                {
                    Debug.Log("ผิด");
                    StopCoroutine(countdownRoutine);
                    RPCStopMovingPickaxeArrow();
                    removeQuickGameActionFromPlayer();
                    setAlertUI(true, Color.red, "Try again");
                    yield return new WaitForSeconds(3);
                    setAlertUI(false, Color.red, "Try again");
                }
                else
                {
                    Debug.Log("แพ้");
                    setAlertUI(true, Color.red, "You Fail");
                    StartCoroutine(EndGame());
                }
            }
            else
            {
                Debug.Log("ถูก");
                setAlertUI(true, Color.green, "Complete");
                setSuccessStatus(true);
                StartCoroutine(EndGame());
                yield break;
            }
        }

    }

    public override IEnumerator EndGame()
    {
        RPCStopMovingPickaxeArrow();
        removeQuickGameActionFromPlayer();
        setPlayer(null);
        StopCoroutine(countdownRoutine);

        yield return new WaitForSeconds(2f);

        setSetupStatus(false);
        setAlertUI(false, Color.black, "Null");
        resetGame();
        closeGameUI();
        QuickGameController.instance.setGameEndStatus(true);
    }

    public override void actionInGame(int buttonIndex, int buttonValue)
    {
        lastPressedValue = buttonValue;
        buttonPressedValues.Add(lastPressedValue);
    }


    public override IEnumerator SetupGame()
    {
        yield return new WaitUntil(() => player);
        setRemainingTime(playTime);
        addQuickGameActionToPlayer();


        ServerSetBreakPoint();
        setSetupStatus(true);
        setAlertUI(false, Color.green, "Complete");

    }

    [ClientRpc]
    public void setAlertUI(bool active, Color textColor, string alertText)
    {
        gameUI.setAlertText(active, textColor, alertText);
    }

    [Server]
    public void ServerSetBreakPoint()
    {
        float posY = gameUI.GetComponent<DestroyRockUI>().RandomPosYBreakPoint();
        RpcUpdateBreakPoint(posY);
    }
    [ClientRpc]
    public void RpcUpdateBreakPoint(float posY)
    {
        gameUI.GetComponent<DestroyRockUI>().UpdatePosYBreakPoint(posY);
    }

    private IEnumerator moving()
    {
        var UI = gameUI.GetComponent<DestroyRockUI>();
        if (UI.isMovingUp)
        {
            yield return new WaitUntil(() => UI.MoveUp());
            UI.isMovingUp = false;
            coMoving = StartCoroutine(moving());
        }
        else
        {
            yield return new WaitUntil(() => UI.MoveDown());
            UI.isMovingUp = true;
            coMoving = StartCoroutine(moving());
        }
    }

    public void StartMovingPickaxeArrow()
    {
        var UI = gameUI.GetComponent<DestroyRockUI>();
        UI.pickaxeArrow.localPosition = UI.lowPoint.localPosition;
        coMoving = StartCoroutine(moving());

    }
    [ClientRpc]
    public void RPCUpdateMovingPickaxeArrow(Vector3 y)
    {
        var UI = gameUI.GetComponent<DestroyRockUI>();
        UI.UpdatePosPickaxeArrow(y);
    }

    
    public void RPCStopMovingPickaxeArrow()
    {
        StopCoroutine(coMoving);
    }



}
