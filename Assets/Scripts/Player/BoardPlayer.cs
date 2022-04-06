using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class BoardPlayer : NetworkBehaviour
{
    public event Action playerAction;
    public event Action<int, int> quickGameAction;

    private Coroutine moveCoroutine = null;
    public static event Action<BoardPlayer, string> OnMessage;

    [SerializeField]
    private EscapistNetworkManager networkManager;
    #region Accessor
    private EscapistNetworkManager NWManager
    {
        get
        {
            if (networkManager == null)
            {
                return networkManager = NetworkManager.singleton as EscapistNetworkManager;
            }
            return networkManager;
        }
    }
    #endregion

    #region player_attributes
    [SyncVar]
    private string displayName = "Loading...";
    public string DisplayName
    {
        get { return displayName; }
    }

    [SyncVar]
    [SerializeField] private Color playerColor = Color.white;
    #region Handeler & Getter/Setter
    public Color getPlayerColor() => playerColor;

    [Server]
    public void setPlayerColor(Color newColor) => playerColor = newColor;
    #endregion

    [SyncVar]
    private long scores = 0;
    public long Scores
    {
        get { return scores; }
    }

    [SyncVar]
    [SerializeField] private int movePoint = 0;
    public int MovePoint
    {
        get { return movePoint; }
        set { movePoint = value; }
    }

    #region tile
    [SyncVar]
    [SerializeField] private Tile currentTile = null;
    [SyncVar]
    [SerializeField] private Vector3 currentTilePosition;
    public Tile CurrentTile
    {
        get
        {
            if (!currentTile)
            {
                Tile tile = null;
                foreach (var i in Physics.OverlapBox(currentTilePosition, new Vector3(1f, 1f)))
                {
                    if (i.transform.parent.GetComponent<Tile>())
                    {
                        tile = i.transform.parent.GetComponent<Tile>();
                    }
                }
                RpcSetCurrentTile(tile);
                return tile;
            }

            return currentTile;
        }
        set
        {
            currentTile = value;
        }
    }
    [SerializeField]
    private Tile targetTile = null;

    [SerializeField]
    private Vector3 targetMovePosition = Vector3.zero;
    private bool hasGetTargetMovePositionFromServer = false;

    private Stack<Tile> pastTiles = new Stack<Tile>();

    public int targetTileIndex;
    private const int DEFAULT_TARGET_TILE_INDEX = 0;
    #endregion

    [SerializeField]
    private BoardTransactionArrow arrowPrefabs;

    public bool isOnPlayerTurn { get; private set; }
    [SyncVar] public int numberTurnCannotMove = 0;

    public bool isChoosingTransactionTile = false;
    private bool isFirstMove = true;

    [SerializeField] public bool isMoving = false;
    public bool isInBoardScene { get; private set; }

    [SyncVar]
    private bool isPlayerReadied;
    public bool IsPlayerReadied
    {
        get
        {
            return isPlayerReadied;
        }
    }
    [Command]
    private void CmdSetPlayerReadyStatus(bool isReadied)
    {
        RpcSetPlayerReadyStatus(isReadied);
    }
    [ClientRpc]
    private void RpcSetPlayerReadyStatus(bool isReadied)
    {
        isPlayerReadied = isReadied;
    }

    [SyncVar]
    public bool isPlayerReadyToPlay;
    [Command]
    private void CmdSetReadyToPlayStatus(bool isReady)
    {
        RpcSetReadyToPlayStatus(isReady);
    }
    [ClientRpc]
    private void RpcSetReadyToPlayStatus(bool isReady)
    {
        isPlayerReadyToPlay = isReady;
    }

    [Space]
    [SerializeField] private GameObject boardCharacterPrefab;
    [SerializeField] private GameObject snatcherPlayerPrefab;

    public SceneSnatcherScript snatcherUIScript = null;

    [SyncVar]
    [SerializeField] private BoardPlayerCharacter boardCharacter;
    #region Getter & Setter
    public BoardPlayerCharacter getBoardCharacter()
    {
        return boardCharacter;
    }

    [Server]
    private void setBoardCharacter(BoardPlayerCharacter character)
    {
        RpcSetBoardCharacter(character);
    }

    [ClientRpc]
    private void RpcSetBoardCharacter(BoardPlayerCharacter character)
    {
        boardCharacter = character;
    }
    #endregion

    [SyncVar(hook = "setSnatcherCharacter")]
    [SerializeField] private Snatcher SnatcherCharacter;
    [SyncVar]
    [SerializeField] private int testSyncVar;
    public Snatcher getSnatcherCharacter
    {
        get { return SnatcherCharacter; }
    }

    #endregion

    public StatusEffect appliedEffect;

    public override void OnStartClient()
    {
        NWManager.boardPlayers.Add(this);
        DontDestroyOnLoad(this);
    }

    public override void OnStopClient()
    {
        NWManager.boardPlayers.Remove(this);
        Destroy(boardCharacter);
    }

    private void Start()
    {
        targetTileIndex = -1;
        // arrowPrefabs = Resources.Load<BoardTransactionArrow>("arrowVariant");
    }

    private void Update()
    {
        if (!isLocalPlayer || !hasAuthority)
        {
            return;
        }

        //Check if player has finished load scene.
        if (connectionToServer.isReady && NetworkClient.ready)
        {
            CmdSetPlayerReadyStatus(SceneManager.GetActiveScene().isLoaded);

            if (CameraControl.instance)
            {
                CmdSetReadyToPlayStatus(CameraControl.instance.isFinishedTransition());
            }
            else
            {
                CmdSetReadyToPlayStatus(false);
            }
        }

        if (CameraControl.instance)
        {
            if (isMoving)
            {
                CameraControl.instance.setCameraMode(0);
                //CameraControl.instance.setActiveCameraViewButton(false);
            }
            else
            {
                //CameraControl.instance.setActiveCameraViewButton(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            networkManager.startWordSnatcher();
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            Debug.Log("Keypad5 pressed.");
            CmdStartQuickGame();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            Debug.Log("Keypad6 pressed.");
            Debug.Log(NetworkClient.connection.identity);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Debug.Log("Keypad7 pressed.");
            GetComponent<PlayerInventory>().CmdReceiveKey(10);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            Debug.Log("Keypad7 pressed.");
            CmdAddPlayerScore(500);
        }

        if (SceneManager.GetActiveScene().path == NWManager.gameScenes[0])
        {
            isInBoardScene = true;
        }
        else
        {
            isInBoardScene = false;

            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                //CmdSetMovePoint(0);
            }

            if (SceneManager.GetActiveScene().path == NWManager.gameScenes[1])
            {
                updateSnatcherDisplay();
                if (SnatcherCharacter != null)
                {
                    SnatcherCharacter.GetComponent<Snatcher>().SnatcherMovement(snatcherUIScript.getJoystickValue());
                    SelectionIndicator.instance.SelectionCube(SnatcherCharacter.GetComponent<Snatcher>().targetBoxObj);
                }
            }
        }
    }

    #region BoardGame
    [Server]
    public void setDisplayName(string name)
    {
        displayName = name;
    }

    [Command]
    public void CmdUpdateCurrentTile(Tile tile)
    {
        RpcSetCurrentTile(tile);
    }

    [ClientRpc]
    private void RpcSetCurrentTile(Tile tile)
    {
        currentTile = tile;
        currentTilePosition = tile.transform.position;
    }

    public void setTargetMovePosition(Vector3 position)
    {
        RpcSetTargetMovePosition(position);
    }

    [ClientRpc]
    private void RpcSetTargetMovePosition(Vector3 position)
    {
        targetMovePosition = position;
        hasGetTargetMovePositionFromServer = true;
    }

    [Command]
    private void CmdSetMovePoint(int value)
    {
        //movePoint = value;
        RpcSetMovePointValue(value);//แก้
    }

    [ClientRpc]
    public void RpcSetMovePointValue(int value)
    {
        movePoint = value;

        if (!isMoving && value > 0)
        {
            startMove();
        }

        BoardGameController.instance.updateUIDisplay();
        //CmdUpdateBoardUI();
    }

    [Command]
    private void CmdSetNumberTurnCannotMove(int turnCount)
    {
        RpcSetNumberTurnCannotMove(turnCount);
    }

    [ClientRpc]
    private void RpcSetNumberTurnCannotMove(int turnCount)
    {
        numberTurnCannotMove = turnCount;
        if (numberTurnCannotMove <= 0)
        {
            CmdApplyStatusEffectByName("Nothing");
        }
    }

    [ClientRpc]
    public void RpcAddPlayerScore(int scoreGain)
    {
        scores += scoreGain;
        BoardGameController.instance.updateUIDisplay();
        //CmdUpdateBoardUI();
    }
    [Command]
    private void CmdAddPlayerScore(int scoreGain)
    {
        RpcAddPlayerScore(scoreGain);
    }

    [ClientRpc]
    public void setPlayerTurn(bool value)
    {
        isOnPlayerTurn = value;
        if (isOnPlayerTurn && isLocalPlayer)
        {
            CameraControl.instance.setCameraMode(0);
            if (numberTurnCannotMove > 0)
            {
                endTurn();
                CmdSetNumberTurnCannotMove(--numberTurnCannotMove);
            }
            else
            {
                startWheelSpinnerCoroutine();
            }
        }
    }

    [Command]
    public void startWheelSpinnerCoroutine()
    {
        //BoardGameController.instance.UI.rollDiceButton.interactable = true;
        StartCoroutine(WheelSpinnerController.instance.spin());
    }

    [Command]
    private void CmdAddRolledDicePlayer()
    {
        BoardGameController.instance.addRolledDicePlayer(this);
    }

    [Client]
    public void requestToOpenDoor()
    {
        CmdRequestToOpenDoor();
    }

    [Command]
    public void CmdRequestToOpenDoor()
    {
        if (currentTile.GetComponent<Tile_Door>() != null)
        {
            var doorTileComponent = currentTile.GetComponent<Tile_Door>();
            doorTileComponent.unlockDoor(this);
        }
    }

    public void startMove()
    {
        StartCoroutine(boardMove(movePoint, true));
    }

    private IEnumerator boardMove(int currentMovePoint, bool isMoveForward)
    {
        if (!isLocalPlayer || !hasAuthority)
        {
            yield break;
        }

        isMoving = true;
        yield return new WaitUntil(() => !WheelSpinnerController.instance.wheelSpinner.gameObject.activeSelf);
        yield return new WaitForSeconds(0.055f);

        if (isMoveForward)
        {
            if (movePoint <= 0)
            {
                Debug.LogError($"Break coroutine boardMove.");
                endTurn();
                yield break;
            }

            StartCoroutine(findTargetTile(CurrentTile.AdjacentTiles));
            yield return new WaitUntil(() => !isChoosingTransactionTile);

            moveOutFromTile();
            CmdGetTargetMovePosition(targetTile);

            yield return new WaitUntil(() => hasGetTargetMovePositionFromServer);

            yield return new WaitUntil(() => moveToPosition(targetMovePosition)); //Wait character finished move to target position.

            Debug.Log("Move finished.");

            pastTiles.Push(currentTile);

            if (targetTile.GetComponent<Tile_Door>() != null)
            {
                accessDoor(targetTile.GetComponent<Tile_Door>());
            }

            currentTile = targetTile;
            CmdUpdateCurrentTile(currentTile);

            targetTile = null;
            hasGetTargetMovePositionFromServer = false;
            targetMovePosition = Vector3.zero;

            CmdSetMovePoint(--movePoint);

            currentTile.doEvent(this);

            playerAction?.Invoke();

            if (currentTile.GetComponent<Tile_Door>() != null)
            {
                yield return new WaitUntil(() => DoorUIController.instance.IsDone);
            }
            else if (currentTile.GetComponent<Tile_QuickGame>() != null)
            {
                yield return new WaitUntil(() => QuickGameController.instance.isGameEnded);
                yield return new WaitForEndOfFrame();
                yield return new WaitUntil(() => !NewItemDecitionUI.instance.Ui.activeSelf);
                yield return new WaitForSeconds(2.25f);
            }

            if (movePoint > 0)
            {
                moveCoroutine = StartCoroutine(boardMove(movePoint, true));
                yield break;
            }
            else
            {
                //var currentTileComponent = currentTile.GetComponent<Tile_Punish>();
                if (currentTile.GetComponent<Tile_Punish>() != null && currentTile.GetComponent<Tile_Punish>().type == punishType.moveBackward)
                {
                    yield break;
                }
            }
        }
        else
        {
            if (currentMovePoint <= 0)
            {
                Debug.LogError($"Break because current move point = {currentMovePoint}");
                yield break;
            }

            List<Tile> tiles = new List<Tile>();

            if (pastTiles.Count > 0)
            {
                tiles.Add(pastTiles.Pop());
            }
            else
            {
                Debug.LogWarning($"Stop move backward. because past tile count = {pastTiles.Count}");
                StopCoroutine(moveCoroutine);
                yield break;
            }
            StartCoroutine(findTargetTile(tiles));
            yield return new WaitUntil(() => targetTile != null);

            moveOutFromTile();
            CmdGetTargetMovePosition(targetTile); //Ask server for get position for stand on tile.

            yield return new WaitUntil(() => hasGetTargetMovePositionFromServer); //Wait server to give target position to move.
            yield return new WaitUntil(() => moveToPosition(targetMovePosition)); //Wait character finished move to target position.

            currentTile = targetTile;
            CmdUpdateCurrentTile(currentTile);

            targetTile = null;
            hasGetTargetMovePositionFromServer = false;
            targetMovePosition = Vector3.zero;

            int remainingMovePoint = currentMovePoint - 1;

            if (remainingMovePoint > 0)
            {
                moveCoroutine = StartCoroutine(boardMove(remainingMovePoint, false));
                yield break;
            }
        }

        isMoving = false;
        endTurn();
    }

    public bool moveToPosition(Vector3 targetPosition)
    {
        boardCharacter.CmdMoveToTargetPosition(targetPosition);

        if (!boardCharacter.isFinishedMove(targetPosition))
        {
            return false;
        }
        return true;
    }

    public IEnumerator punishMoveBackward(int stepBackwardCounts)
    {
        yield return new WaitForSeconds(0.35f);
        StartCoroutine(boardMove(stepBackwardCounts, false));
    }

    [Command]
    public void punishStopMove(int turnCount)
    {
        if (appliedEffect && appliedEffect.statusName.Equals("Stop Move Protector"))
        {
            GetComponent<PlayerInventory>().useItem();
            return;
        }

        numberTurnCannotMove = turnCount;
        CmdApplyStatusEffectByName("CannotMove");

        Debug.Log($"{name} was punished (Stop move for {numberTurnCannotMove} turns) || received {turnCount}");
    }

    public void rollDice()
    {
        if (movePoint > 0 || !isLocalPlayer || numberTurnCannotMove > 0)
        {
            return;
        }

        int randommovePoint = Random.Range(2, 13);
        CmdSetMovePoint(7);

        if (BoardGameController.instance.GameState.Equals(BoardGameState.SortPlayerQueue))
        {
            CmdAddRolledDicePlayer();
        }
        else if (isOnPlayerTurn)
        {
            StartCoroutine(boardMove(randommovePoint, true));
        }

        BoardGameController.instance.UI.rollDiceButton.interactable = false;
    }

    public void CmdUpdateBoardUI()
    {
        RpcUpdateBoardUI();
    }

    public void RpcUpdateBoardUI()
    {
        if (isInBoardScene)
        {
            BoardGameController.instance.updateUIDisplay();
        }
    }

    [Command]
    private void moveOutFromTile()
    {
        currentTile.removePlayerFromTile(this);
    }

    private IEnumerator findTargetTile(List<Tile> possibleTiles)
    {
        List<Tile> tiles = new List<Tile>();
        isChoosingTransactionTile = true;

        if (possibleTiles.Count > 1 || currentTile.GetComponent<Tile_Door>())
        {
            if (pastTiles.Count > 0)
            {
                tiles = findMoveRoute(possibleTiles);
                if (currentTile.GetComponent<Tile_Door>())
                {
                    var doorTileComponent = currentTile.GetComponent<Tile_Door>();
                    if (doorTileComponent.isHavePermissionToAccessDoor(this))
                    {
                        tiles.Add(doorTileComponent.getPermissionTile());
                    }
                }
            }
            else if (isFirstMove)
            {
                tiles.Add(possibleTiles[possibleTiles.Count - 1]);
                isFirstMove = false;
            }

            if (tiles.Count > 1)
            {
                selectTransactionTile(tiles);
                yield return new WaitUntil(() => targetTileIndex != -1);
                targetTile = tiles[targetTileIndex];
            }
            else
            {
                targetTile = tiles[DEFAULT_TARGET_TILE_INDEX];
            }

            targetTileIndex = -1;
        }
        else
        {
            targetTile = possibleTiles[0];
        }

        isChoosingTransactionTile = false;
    }

    [Command]
    private void accessDoor(Tile_Door doorTile)
    {
        doorTile.removePlayerAccessedDoor(this);
    }

    [Client]
    private List<Tile> findMoveRoute(List<Tile> tiles)
    {
        int loopLength = tiles.Count - 1;

        for (int i = 0; i <= loopLength; i++)
        {
            if (tiles[i].Equals(pastTiles.Peek()))
            {
                tiles.RemoveAt(i);
                break;
            }
        }

        return tiles;
    }

    [Command]
    public void CmdGetTargetMovePosition(Tile targetTile)
    {
        BoardGameController.instance.getAvailableTargetPosition(targetTile, this);
    }

    private void selectTransactionTile(List<Tile> tiles)
    {
        for (int index = 0; index < tiles.Count; index++)
        {
            //Vector3.Lerp ใช้หาระยะทางระหว่าง 2 points ช่องที่3 คือ 0 - 1 อัตราส่วนระยะทาง
            Vector3 instantiatePosition = Vector3.Lerp(boardCharacter.transform.position, tiles[index].transform.position, 0.4f) + new Vector3(0, 2, 0);
            var arrow = Instantiate(arrowPrefabs, instantiatePosition, Quaternion.identity);
            arrow.setArrowInfo(this, tiles[index].transform, index);
        }
    }

    [Server]
    public void spawnCharacter()
    {
        var tilePosition = CurrentTile.getTilePosition(this);

        var instantiatedBoardCharacter = Instantiate(boardCharacterPrefab, tilePosition, Quaternion.identity);
        instantiatedBoardCharacter.name = $"BoardCharacter[{connectionToClient.connectionId}]";

        var boardPlayerCharacterComponent = instantiatedBoardCharacter.GetComponent<BoardPlayerCharacter>();
        boardPlayerCharacterComponent.setOwner(this);

        boardPlayerCharacterComponent.setColor(playerColor);


        NetworkServer.Spawn(instantiatedBoardCharacter, connectionToClient);

        if (appliedEffect)
        {
            boardPlayerCharacterComponent.setStatusEffect(appliedEffect);
        }

        setBoardCharacter(instantiatedBoardCharacter.GetComponent<BoardPlayerCharacter>());
    }

    [Command]
    private void endTurn()
    {
        BoardGameController.instance.moveTurnToNextPlayer();
    }

    public void callForEndGame()
    {
        CmdCallForEndGame();
    }

    [Command]
    private void CmdCallForEndGame()
    {
        BoardGameController.instance.endGame(this);
    }

    [Client]
    public void endGame(BoardPlayer winner)
    {
        Debug.Log($"Game has end.");

        Debug.Log($"The WINNER is ....");

        Debug.Log($"{winner.displayName}");
    }
    #endregion

    [ClientRpc]
    public void useKey(int amount)
    {
        if (amount > GetComponent<PlayerInventory>().KeyCount)
        {
            return;
        }

        PlayerInventory inventory = GetComponent<PlayerInventory>();
        inventory.setKey(inventory.KeyCount - amount);
    }

    #region WordSnatcher
    [Server]
    public void spawnSnatcher(Transform transformStorage)
    {
        var instantiatedSnatcherCharacter = Instantiate(snatcherPlayerPrefab, transformStorage.position + Vector3.up, Quaternion.identity);
        instantiatedSnatcherCharacter.name = $"SnatcherCharacter[{connectionToClient.connectionId}]";
        instantiatedSnatcherCharacter.GetComponent<Snatcher>().setOwner(this);
        instantiatedSnatcherCharacter.GetComponent<Snatcher>().setColor(playerColor);
        NetworkServer.Spawn(instantiatedSnatcherCharacter.gameObject, connectionToClient);
        SetSnatcher(instantiatedSnatcherCharacter.GetComponent<Snatcher>());
        Debug.Log("form Board player = " + SnatcherCharacter);
    }

    public void setSnatcherCharacter(Snatcher old, Snatcher New)
    {
        SnatcherCharacter = New;
    }

    private void SetSnatcher(Snatcher snatcher)
    {
        SnatcherCharacter = snatcher;
    }



    public void updateSnatcherDisplay()
    {
        if (SceneManager.GetActiveScene().path == NWManager.gameScenes[1])
        {
            if (snatcherUIScript == null)
            {
                snatcherUIScript = FindObjectOfType<SceneSnatcherScript>();
            }

        }
    }

    #endregion

    [Command]
    public void CmdDoQuickGameAction(int buttonIndex, int buttonValue)
    {
        RpcDoQuickGameAction(buttonIndex, buttonValue);
    }

    [ClientRpc]
    public void RpcDoQuickGameAction(int buttonIndex, int buttonValue)
    {
        quickGameAction?.Invoke(buttonIndex, buttonValue);
    }

    [Command]
    public void CmdApplyStatusEffect()
    {
        RpcApplyStatusEffect();
    }
    [ClientRpc]
    private void RpcApplyStatusEffect()
    {
        StatusEffect effect;
        if (GetComponent<PlayerInventory>().Item)
        {
            effect = GetComponent<PlayerInventory>().Item.getItemStatus();
        }
        else
        {
            effect = null;
        }

        appliedEffect = effect;
        boardCharacter.setStatusEffect(effect);
    }

    [Command]
    public void CmdApplyStatusEffectByName(string effectName)
    {
        RpcAppplyStatusEffectByName(effectName);
    }
    [ClientRpc]
    private void RpcAppplyStatusEffectByName(string effectName)
    {
        StatusEffect effect = Resources.Load<StatusEffect>($"Items/StatusEffects/{effectName}");

        appliedEffect = effect;
        boardCharacter.setStatusEffect(effect);
    }
    #region Chat system
    [Command]
    public void CmdStartQuickGame()
    {
        QuickGameController.instance.playQuickGame(this);
    }

    [Command]
    public void CmdSendMessage(string message)
    {
        if (message.Trim() == "")
            return;

        RpcReceiveMessage(message);
    }

    [ClientRpc]
    private void RpcReceiveMessage(string message)
    {
        OnMessage?.Invoke(this, message);
    }
    #endregion
}
