using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class QuickGame : NetworkBehaviour
{
    [Header("Game Details")]
    public string gameName;
    public string gameDescription;

    [Space]

    [SyncVar]
    public BoardPlayer player;
    public QuickGameUI gameUI;
    public QuickGamePopUpWindow popUpWindow;
    public float playTime;
    [SyncVar]
    protected float remainingPlayTime;
    protected int lastPressedValue;
    public List<int> buttonPressedValues = new List<int>();

    protected Coroutine gameProcessRoutine;
    protected Coroutine countdownRoutine;

    [SyncVar]
    public bool setUpFinished;
    [SyncVar]
    public bool isSuccess;

    [ClientRpc]
    public void setPlayer(BoardPlayer player)
    {

        this.player = player;
        //Debug.Log(player);
    }

    [ClientRpc]
    public void setRemainingTime(float remainingTime)
    {
        remainingPlayTime = remainingTime;
        gameUI.setRemainTimeUI(remainingPlayTime, remainingPlayTime / playTime);
    }

    [ClientRpc]
    public void setSuccessStatus(bool isSuccess)
    {
        this.isSuccess = isSuccess;
    }

    [ClientRpc]
    public void setSetupStatus(bool isFinishedSetup)
    {
        setUpFinished = isFinishedSetup;
    }
    public virtual void StartGame()
    {
        gameProcessRoutine = StartCoroutine(gameProcess());
    }

    public virtual IEnumerator gameProcess()
    {
        showGamePopupWindow();
        yield return new WaitForSeconds(3.5f);
        hideGamePopupWindow();
        openGameUI();
        StartCoroutine(SetupGame());
        yield return new WaitUntil(() => setUpFinished);
        countdownRoutine = StartCoroutine(CountdownTime());
        yield return new WaitUntil(() => buttonPressedValues.Count > 1);
        setSuccessStatus(true);
        StartCoroutine(EndGame());
    }

    public virtual IEnumerator EndGame()
    {
        removeQuickGameActionFromPlayer();
        setPlayer(null);
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
        }

        yield return new WaitForSeconds(2f);

        setSetupStatus(false);
        resetGame();
        closeGameUI();
        QuickGameController.instance.setGameEndStatus(true);
    }

    public virtual IEnumerator SetupGame()
    {
        yield return new WaitUntil(() => player);
        setRemainingTime(playTime);
        addQuickGameActionToPlayer();

        setSetupStatus(true);
    }

    [ClientRpc]
    public virtual void resetGame()
    {

        buttonPressedValues.Clear();
        lastPressedValue = 0;
        StopAllCoroutines();
    }

    public IEnumerator CountdownTime()
    {
        while (remainingPlayTime > 0)
        {
            if (isSuccess)
            {
                yield break;
            }
            setRemainingTime(remainingPlayTime - Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        StopCoroutine(gameProcessRoutine);
        StartCoroutine(EndGame());
    }

    [ClientRpc]
    public void addQuickGameActionToPlayer()
    {
        player.quickGameAction += actionInGame;
    }
    [ClientRpc]
    public void removeQuickGameActionFromPlayer()
    {
        player.quickGameAction -= actionInGame;
    }

    public virtual void actionInGame(int buttonIndex, int buttonValue)
    {
        BoardPlayer playerDoneAction = NetworkClient.connection.identity.GetComponent<BoardPlayer>();
        if (buttonPressedValues.Contains(buttonValue))
        {
            Debug.LogWarning("Button already pressed.");
            return;
        }

        gameUI.setButtonInteractable(buttonIndex, false);
        lastPressedValue = buttonValue;
        Debug.Log("buttonPressedValues.Add(lastPressedValue) = " + lastPressedValue);
        buttonPressedValues.Add(lastPressedValue);
    }

    [ClientRpc]
    public void openGameUI()
    {
        gameUI.showGameUI();
        gameUI.setUpUI();
    }
    [ClientRpc]
    public void closeGameUI()
    {
        gameUI.hideGameUI();
    }

    [ClientRpc]
    public virtual void showGamePopupWindow()
    {
        popUpWindow.gameObject.SetActive(true);
        setPopUpWindowUI();
    }

    [ClientRpc]
    public virtual void hideGamePopupWindow()
    {
        popUpWindow.gameObject.SetActive(false);
    }

    protected virtual void setPopUpWindowUI()
    {
        popUpWindow.setGameNameText(gameName);
        popUpWindow.setGameDescriptionText(gameDescription);
        popUpWindow.setGamePlayTimeText(playTime.ToString());
    }
}
