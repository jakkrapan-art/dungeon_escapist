using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;

public class Indicator : NetworkBehaviour
{
    [SerializeField] private WheelSpinner spinnerWheel;

    [SerializeField] private BoardPlayer player;
    #region getter / setter
    public BoardPlayer getPlayer() => player;
    [ClientRpc]
    public void setPlayer(BoardPlayer value) => player = value;
    #endregion

    [SyncVar(hook = nameof(HandleColorChanged))]
    [SerializeField] private Color indicatorColor;

    #region Handlerer, Getter / Setter
    private void HandleColorChanged(Color oldColor, Color newColor)
    {
        try
        {
            var img = transform.GetChild(0).GetComponent<Image>();
            /*var img = transform.GetChild(0).GetComponent<SpriteRenderer>();*/
            img.color = newColor;
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void setColor(Color c) => indicatorColor = c;

    public Color getColor() => indicatorColor;

    #endregion

    [Space]
    [SerializeField] private int currentIndex = 0;

    [ClientRpc]
    public void setIndex(int value)
    {
        currentIndex = value;

        rotateToSelection(spinnerWheel.pieceAngles[value], value);
        //calcurateRotateAngle(spinnerWheel.pieceAngles[value]);
    }

    public bool locked { get; private set; }
    public bool isLocked;

    [SerializeField] private int startIndex = 0;

    private void Awake()
    {
        if (!spinnerWheel)
        {
            spinnerWheel = FindObjectOfType<WheelSpinner>();
        }
    }

    public Vector2 normalizePosition;
    public float angle;

    void Update()
    {
        if (!hasAuthority || !isLocalPlayer) { return; }

        if ((Input.GetMouseButton(0) || Input.touchCount > 0) && !locked)
        {
            if (isClickedOnWheelCircle)
            {
                mouseSelectionIndex();
            }
        }
    }

    [ClientRpc]
    public void setRotationEulers(Vector3 eulers)
    {
        transform.eulerAngles = eulers;
    }


    public void mouseSelectionIndex()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;
            normalizePosition = new Vector2(touchPosition.x - Screen.width / 2, touchPosition.y - Screen.height / 2);
        }
        else
        {
            normalizePosition = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);
        }

        angle = Mathf.Atan2(normalizePosition.y, normalizePosition.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;
        calcurateRotateAngle(angle);

    }

    [Command]
    private void calcurateRotateAngle(float angle)
    {
        int targetIndex = (int)angle / (360 / spinnerWheel.pieceCount);

        if (!isNoIndicatorOnTargetWheelPiece(targetIndex))
        {
            return;
        }

        float targetAngle = spinnerWheel.pieceAngles[targetIndex];

        rotateToSelection(targetAngle, targetIndex);
    }

    public void rotateToSelection(float targetAngle, int newIndex)
    {
        int oldIndex = currentIndex;
        currentIndex = newIndex;
        transform.eulerAngles = new Vector3(0, 0, targetAngle);
        /*transform.eulerAngles = new Vector3(0, targetAngle, 0);*/

        wheelPieceSelection(oldIndex, currentIndex);
    }

    private void wheelPieceSelection(int oldTarget, int newTarget)
    {
        spinnerWheel.deselectPiece(oldTarget);
        spinnerWheel.selectPiece(newTarget, this);
    }

    private bool isNoIndicatorOnTargetWheelPiece(int index)
    {
        return !spinnerWheel.getIndicatorOnPiece(index);
    }

    [Command]
    public void CmdSetLockIndicatorValue(bool isLocked)
    {
        setLockIndicatorValue(isLocked);
        giveConnectionIdentityToOtherObject(player.gameObject);
    }

    [ClientRpc]
    public void setLockIndicatorValue(bool isLocked)
    {
        locked = isLocked;
    }

    [Server]
    public void setControlFromBoardPlayer(BoardPlayer b)
    {
        NetworkConnection conn = b.connectionToClient;
        NetworkServer.ReplacePlayerForConnection(conn, gameObject, true);
    }

    public int getPointOnWheelPiece()
    {
        return spinnerWheel.calcurateCurrentPieceValue(this);
    }

    [Command]
    public void pressedSpinButton()
    {
        WheelSpinner.instance.isPressedSpinButton = true;
        giveConnectionIdentityToOtherObject(player.gameObject);
    }

    private bool isClickedOnWheelCircle
    {
        get
        {
            foreach (var touch in Input.touches)
            {
                int id = touch.fingerId;
                if (EventSystem.current.IsPointerOverGameObject(id))
                {
                    PointerEventData pointerEventData = new PointerEventData(EventSystem.current);

                    pointerEventData.position = touch.position;

                    List<RaycastResult> raycastResults = new List<RaycastResult>();

                    EventSystem.current.RaycastAll(pointerEventData, raycastResults);
                    return raycastResults[0].gameObject == spinnerWheel.wheelCircle.gameObject;
                }
            }

            return false;
        }
    }

    [Server]
    private void giveConnectionIdentityToOtherObject(GameObject otherObject)
    {
        NetworkServer.ReplacePlayerForConnection(connectionToClient, otherObject);
    }
}
