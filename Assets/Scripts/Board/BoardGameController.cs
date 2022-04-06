using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class BoardGameController : NetworkBehaviour
{
    public static BoardGameController instance { get; private set; }

    [SerializeField] private BoardGameUIController ui = null;
    #region Accesor
    public BoardGameUIController UI
    {
        get
        {
            if (!ui)
            {
                ui = FindObjectOfType<BoardGameUIController>();
            }

            return ui;
        }
    }
    #endregion

    private CameraControl camController = null;
    #region Accessor
    public CameraControl CamController
    {
        get
        {
            if (!camController)
            {
                camController = CameraControl.instance;
            }

            return camController;
        }
    }
    #endregion
    private bool isCameraTransitionEnd = false;

    private EscapistNetworkManager networkManager;

    private Coroutine boardTurnControlRoutine = null;
    private Coroutine boardPlayRoutine = null;

    public List<BoardPlayer> players { get; private set; } = new List<BoardPlayer>();

    [SerializeField] private BoardPlayer localPlayer = null;
    #region getter/setter
    public BoardPlayer getLocalPlayer()
    {
        if (!localPlayer)
        {
            foreach (var player in players)
            {
                if (player.isLocalPlayer)
                {
                    localPlayer = player;
                }
            }
        }

        return localPlayer;
    }
    #endregion

    public Queue<BoardPlayer> playerQueue { get; private set; } = new Queue<BoardPlayer>();
    public SyncList<BoardPlayer> getQueueList()
    {
        var playerQueueList = new SyncList<BoardPlayer>(playerQueue.ToList());
        return playerQueueList;
    }

    [SyncVar]
    public BoardPlayer playerOnTurn = null;
    [ClientRpc]
    private void setPlayerOnTurn(BoardPlayer player)
    {
        playerOnTurn = player;
    }
    [SyncVar]
    public BoardPlayer winner = null;

    public List<BoardPlayer> rolledDicePlayers { get; private set; } = new List<BoardPlayer>();

    private bool hasPlayerEndedTurn = false;

    [SyncVar]
    [SerializeField] private BoardGameState gameState;

    [SyncVar]
    public int turnCount = 0;

    public BoardGameState GameState
    {
        get { return gameState; }
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        instance = this;

        networkManager = FindObjectOfType<EscapistNetworkManager>();
        players = networkManager.boardPlayers;
        gameState = BoardGameState.Prepare;
    }

    private void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            //swapDisplayBetweenMainUIandWheelSpinner();
        }

        if (SceneManager.GetActiveScene().path != networkManager.gameScenes[0])
        {
            StopAllCoroutines();
        }
    }

    [Server]
    public IEnumerator playTurnBoardGame()
    {
        turnCount++;

        if (playerQueue.Count < players.Count)
        {
            gameState = BoardGameState.SortPlayerQueue;
            yield return new WaitUntil(() => WheelSpinnerController.instance);
            StartCoroutine(WheelSpinnerController.instance.spin());
        }

        yield return new WaitUntil(() => isFinishedSortQueue);
        playerOnTurn = playerQueue.Peek();
        setPlayerOnTurn(playerOnTurn);

        switchCameraTarget(playerOnTurn);
        yield return new WaitUntil(() => /*CamController.isFinishedTransition()*/playerOnTurn.isPlayerReadyToPlay);
        Debug.Log($"Player ready to play: {playerOnTurn.isPlayerReadyToPlay}");
        yield return new WaitForSeconds(1f);
        playerOnTurn.setPlayerTurn(true);

        gameState = BoardGameState.Move;
        boardTurnControlRoutine = StartCoroutine(nameof(boardTurnControl));
    }

    private IEnumerator boardTurnControl()
    {
        Queue<BoardPlayer> tempPlayerQueue = new Queue<BoardPlayer>();
        while (playerQueue.Count > 0)
        {
            yield return new WaitUntil(() => hasPlayerEndedTurn);

            tempPlayerQueue.Enqueue(playerQueue.Dequeue());

            yield return new WaitForSeconds(1.15f);
            if (playerQueue.Count > 0)
            {
                var nextPlayer = playerQueue.Peek();
                giveTurnToPlayer(nextPlayer);
            }

            hasPlayerEndedTurn = false;
        }

        Debug.LogError($"End turn [{turnCount}].");
        playerQueue = new Queue<BoardPlayer>(tempPlayerQueue);

        //This part is for checking if it's pass every 3 turn will start minigame.
        if (turnCount % 3 == 0)
        //if (false)
        {
            Debug.Log("Start Minigame.");
            EscapistNetworkManager networkManager = NetworkManager.singleton as EscapistNetworkManager;
            networkManager.startWordSnatcher();
        }
        else if (isPlayerReachGoal())
        {
            //End game process
            BoardPlayer winner = findWinner();

            endGame(winner);
        }
        else
        {
            StartCoroutine(nameof(playTurnBoardGame));
        }
    }

    private BoardPlayer findWinner()
    {
        BoardPlayer winner = null;

        for (int i = 0; i < players.Count; i++)
        {
            for (int j = i; j < players.Count; j++)
            {
                var playerInventory1 = players[i].GetComponent<PlayerInventory>();
                var playerInventory2 = players[j].GetComponent<PlayerInventory>();

                if (playerInventory2.KeyCount > playerInventory1.KeyCount)
                {
                    winner = players[j];
                }
                else
                {
                    if (playerInventory2.KeyCount == playerInventory1.KeyCount && players[j].Scores > players[i].Scores)
                    {
                        winner = players[j];
                    }
                    else
                    {
                        winner = players[i];
                    }
                }
            }
        }

        return winner;
    }

    private bool isPlayerReachGoal()
    {
        foreach (var player in players)
        {
            if (player.GetComponent<PlayerInventory>().KeyCount >= 10)
            {
                return true;
            }
        }
        return false;
    }

    [ClientRpc]
    private void switchCameraTarget(BoardPlayer targetPlayer)
    {
        CamController.setCameraTarget(targetPlayer.getBoardCharacter().transform);
    }

    private bool isFinishedSortQueue = false;

    [Server]
    public void setPlayerQueueByWheelSpinner(Queue<BoardPlayer> queue)
    {
        playerQueue = queue;
        isFinishedSortQueue = true;
    }

    public void addRolledDicePlayer(BoardPlayer player)
    {
        if (rolledDicePlayers.Contains(player))
        {
            Debug.LogError($"{player.DisplayName} Already rolled the dice.");
            return;
        }

        Debug.LogWarning($"Player {player.DisplayName} rolled dice. (Pts {player.MovePoint})");

        ServerAddRolledDicePlayer(player);
    }

    [Server]
    private void ServerAddRolledDicePlayer(BoardPlayer player)
    {
        rolledDicePlayers.Add(player);
    }

    private bool havePlayerRolledDice(List<BoardPlayer> noPointPlayers)
    {
        if (rolledDicePlayers.Count <= 0)
        {
            return false;
        }
        else
        {
            return noPointPlayers.Contains(rolledDicePlayers[rolledDicePlayers.Count - 1]);
        }
    }

    [Server]
    public void moveTurnToNextPlayer()
    {
        hasPlayerEndedTurn = true;
    }

    private void giveTurnToPlayer(BoardPlayer player)
    {
        playerOnTurn = player;
        setPlayerOnTurn(playerOnTurn);
        switchCameraTarget(playerOnTurn);
        player.setPlayerTurn(true);
    }

    [Server]
    public void getAvailableTargetPosition(Tile tile, BoardPlayer player)
    {
        var targetPosition = tile.getTilePosition(player);
        player.setTargetMovePosition(targetPosition);
    }

    public void updateUIDisplay()
    {
        UI.updatePlayerDisplays(players);
    }

    /*[ClientRpc]
    public void swapDisplayBetweenMainUIandWheelSpinner()
    {
        bool currentActiveSelf = UI.gameObject.activeSelf;
        UI.gameObject.SetActive(!currentActiveSelf);

        if (currentActiveSelf)
        {
            if (WheelSpinnerController.instance)
                WheelSpinnerController.instance.showDisplay();
            else
            {
                WheelSpinnerController wheelSpinnerController = FindObjectOfType<WheelSpinnerController>();
                wheelSpinnerController.showDisplay();
            }
            WheelSpinnerController.instance.showDisplay();
        }
        else
        {
            WheelSpinnerController.instance.hideDisplay();
        }
    }*/

    public void endGame(BoardPlayer winner)
    {
        this.winner = winner;
        winner.RpcSetMovePointValue(0);
        StopCoroutine(boardTurnControlRoutine);
        networkManager.endGame();
    }
}

public enum BoardGameState { Move, SortPlayerQueue, Prepare }
