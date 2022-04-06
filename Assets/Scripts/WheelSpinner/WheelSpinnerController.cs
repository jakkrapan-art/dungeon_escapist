using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WheelSpinnerController : NetworkBehaviour
{
    public static WheelSpinnerController instance;

    [SerializeField] private GameObject wheelSpinnerUI;
    [SerializeField] public WheelSpinner wheelSpinner { get; private set; }

    [SerializeField] private List<Indicator> indicators;

    public Transform indicatorParent;

    [SerializeField] private GameObject indicatorPrefab;
    private bool isFinishedSetup = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        wheelSpinner = FindObjectOfType<WheelSpinner>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //wheelSpinner.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log($"Keypad Enter pressed.");
            StartCoroutine(nameof(spin));
        }
    }

    public IEnumerator spawnIndicators()
    {
        yield return new WaitUntil(() => BoardGameController.instance);
        var players = BoardGameController.instance.players;
        bool origincalParentActiveSelf = indicatorParent.gameObject.activeSelf;

        if (origincalParentActiveSelf == false)
        {
            //RpcShowDisplay();
            RpcShowDisplay();
        }

        //indicatorParent = GameObject.Find("IndicatorParent").transform;

        for (int i = 0; i < players.Count; i++)
        {
            int wheelPieceIndex = (14 - i) % 12;
            try
            {
                spawnIndicator(indicatorPrefab, indicatorParent, players[i], false, wheelPieceIndex);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        //indicatorParent.gameObject.SetActive(origincalParentActiveSelf);
        if (origincalParentActiveSelf == false)
        {
            hideDisplay();
        }

        isFinishedSetup = true;
    }

    [Server]
    public IEnumerator spin()
    {
        //BoardGameController.instance.swapDisplayBetweenMainUIandWheelSpinner();
        //showDisplay();
        RpcShowDisplay();
        bool isMovingState = BoardGameController.instance.GameState == BoardGameState.Move;
        BoardPlayer playerOnTurn = BoardGameController.instance.playerOnTurn;

        if (!isMovingState)
        {
            StartCoroutine(nameof(spawnIndicators));
            yield return new WaitUntil(() => isFinishedSetup);
            yield return new WaitUntil(() => isAllPlayerLockIndicator());
        }
        else
        {
            //yield return new WaitForSeconds(1.5f);
            yield return new WaitUntil(() => wheelSpinner.gameObject.activeSelf);
            var indicator = spawnIndicator(indicatorPrefab, indicatorParent, playerOnTurn, true, 0);
            indicator.GetComponent<Indicator>().setRotationEulers(new Vector3(0, 0, 0));
            yield return new WaitUntil(() => wheelSpinner.isPressedSpinButton);
        }

        spinWheel(Random.Range(5, 8), Random.Range(1150, 1500f), Random.Range(0, 360f));

        yield return new WaitUntil(() => wheelSpinner.isFinishSpin);
        yield return new WaitForSeconds(1.75f);

        if (!isMovingState)
        {
            BoardGameController.instance.setPlayerQueueByWheelSpinner(sortPlayerQueue());
        }
        else
        {
            Indicator indicator = null;
            for (int i = 0; i < 12; i++)
            {
                if (wheelSpinner.getIndicatorOnPiece(i))
                {
                    indicator = wheelSpinner.getIndicatorOnPiece(i);
                    break;
                }
            }

            playerOnTurn.RpcSetMovePointValue(indicator.getPointOnWheelPiece());
            //playerOnTurn.RpcSetMovePointValue(500);
        }

        do
        {
            var indicator = indicators[0];
            var player = indicator.getPlayer();

            indicators.Remove(indicator);
            NetworkServer.Destroy(indicator.gameObject);
        }
        while (indicators.Count > 0);

        RpcHideDisplay();
        //BoardGameController.instance.swapDisplayBetweenMainUIandWheelSpinner();
        //hideDisplay();
    }

    private Queue<BoardPlayer> sortPlayerQueue()
    {
        Queue<BoardPlayer> playerQueue = new Queue<BoardPlayer>();
        List<Indicator> indicators = new List<Indicator>(this.indicators);

        indicators.Sort((x, y) => y.getPointOnWheelPiece() - x.getPointOnWheelPiece());

        foreach (var indicator in indicators)
        {
            playerQueue.Enqueue(indicator.getPlayer());
        }
        return playerQueue;
    }

    private bool isAllPlayerLockIndicator()
    {
        if (indicators.Count > 0)
        {
            foreach (var i in indicators)
            {
                if (!i.locked)
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    [ClientRpc]
    private void spinWheel(float seconds, float rotateSpeed, float initialRotateAngle)
    {
        StartCoroutine(wheelSpinner.Spin(seconds, rotateSpeed, initialRotateAngle));
    }

    [Server]
    private GameObject spawnIndicator(GameObject prefab, Transform parent, BoardPlayer player, bool lockIndicator, int index)
    {
        var instantiatedIndicator = Instantiate(indicatorPrefab, indicatorParent);
        instantiatedIndicator.transform.parent = indicatorParent;
        NetworkServer.Spawn(instantiatedIndicator);
        syncIndicatorObject(instantiatedIndicator, instantiatedIndicator.transform.localScale, instantiatedIndicator.transform.localPosition, instantiatedIndicator.transform.localEulerAngles, instantiatedIndicator.transform.parent);

        var indicatorComponent = instantiatedIndicator.GetComponent<Indicator>();
        indicators.Add(indicatorComponent);

        indicatorComponent.setPlayer(player);
        indicatorComponent.setIndex(index);
        indicatorComponent.setColor(player.getPlayerColor());
        indicatorComponent.setControlFromBoardPlayer(player);

        indicatorComponent.setLockIndicatorValue(lockIndicator);

        updateSpinnerButtonDisplay();

        return instantiatedIndicator;
    }

    [ClientRpc]
    private void syncIndicatorObject(GameObject indicator, Vector3 scale, Vector3 position, Vector3 rotationAngles, Transform parent)
    {
        indicator.transform.parent = parent;
        indicator.transform.localScale = scale;
        indicator.transform.localPosition = position;
        indicator.transform.localEulerAngles = rotationAngles;
    }

    public void showDisplay()
    {
        wheelSpinner.gameObject.SetActive(true);
        indicatorParent.gameObject.SetActive(true);

        wheelSpinner.updateSpinnerButton();
    }

    [ClientRpc]
    private void RpcShowDisplay()
    {
        /*NetworkIdentity localIdentity = NetworkClient.connection.identity;
        BoardPlayer localPlayer = localIdentity.GetComponent<BoardPlayer>() ?
            localIdentity.GetComponent<BoardPlayer>() : localIdentity.GetComponent<Indicator>().getPlayer();

        if (localPlayer == BoardGameController.instance.playerOnTurn)
        {
            showDisplay();
        }*/
        showDisplay();
    }

    public void hideDisplay()
    {
        wheelSpinner.gameObject.SetActive(false);
        indicatorParent.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void RpcHideDisplay()
    {
        hideDisplay();
    }

    public void showIndicators()
    {
        indicatorParent.gameObject.SetActive(true);
    }

    public void hideIndicators()
    {
        indicatorParent.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void updateSpinnerButtonDisplay()
    {
        wheelSpinner.updateSpinnerButton();
    }
}
