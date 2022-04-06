using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Random = UnityEngine.Random;

public class WheelSpinner : MonoBehaviour
{
    public Image wheelCircle = null;
    public GameObject WheelSpinnerPrefab;

    public static WheelSpinner instance = null;

    [Space]
    [SerializeField] private float rotSpeed = 1000f;

    [SerializeField] private bool isSpinning = false;
    public bool isFinishSpin = false;
    public bool isPressedSpinButton = false;

    [SerializeField] private Button wheelSpinnerButton;

    [Header("WheelPieces")]
    [SerializeField] public int pieceCount;
    [SerializeField] public float[] pieceAngles;

    [Header("Indicators")]
    [SerializeField] private Indicator[] indicatorsOnWheelPiece;
    public Indicator localIndicator { get; private set; } = null;

    private void Start()
    {
        wheelSpinnerButton.onClick.AddListener(wheelSpinnerButtonEvent);
        instance = this;
    }

    private void OnEnable()
    {
        wheelSpinnerButton.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        isFinishSpin = false;
        isPressedSpinButton = false;
    }

    public IEnumerator Spin(float seconds, float speed, float initialWheelAngle)
    {
        if (isSpinning)
        {
            yield break;
        }

        isSpinning = true;

        wheelCircle.transform.eulerAngles = new Vector3(0, 0, initialWheelAngle);
        //Debug.Log(seconds / 185f
        WaitForSeconds waitTime = new WaitForSeconds(seconds * (1f / 187));
        while (Mathf.Round(speed) > 0)
        {
            wheelCircle.transform.Rotate(0, 0, speed);
            speed *= 0.96f;
            yield return waitTime;
        }

        isFinishSpin = true;
        isSpinning = false;
    }

    public int calcurateCurrentPieceValue(Indicator indicator)
    {
        float wheelRotationZ = wheelCircle.transform.localRotation.eulerAngles.z;
        float startAngleZ = indicator.transform.localRotation.eulerAngles.z;
        float pieceWidth = 360 / pieceCount;
        int point = 0;

        if (wheelRotationZ > startAngleZ % 360 && wheelRotationZ <= ((startAngleZ + (pieceWidth * 1)) % 360))
        {
            point = 1;
        }
        else if (wheelRotationZ > (startAngleZ + (pieceWidth * 1)) % 360 && wheelRotationZ <= (startAngleZ + (pieceWidth * 2)) % 360)
        {
            point = 2;
        }
        else if (wheelRotationZ > (startAngleZ + (pieceWidth * 2)) % 360 && wheelRotationZ <= (startAngleZ + (pieceWidth * 3)) % 360)
        {
            point = 3;
        }
        else if (wheelRotationZ > (startAngleZ + (pieceWidth * 3)) % 360 && wheelRotationZ <= (startAngleZ + (pieceWidth * 4)) % 360)
        {
            point = 4;
        }
        else if (wheelRotationZ > (startAngleZ + (pieceWidth * 4)) % 360 && wheelRotationZ <= (startAngleZ + (pieceWidth * 5)) % 360)
        {
            point = 5;
        }
        else if (wheelRotationZ > (startAngleZ + (pieceWidth * 5)) % 360 && wheelRotationZ <= (startAngleZ + (pieceWidth * 6)) % 360)
        {
            point = 6;
        }
        else if (wheelRotationZ > (startAngleZ + (pieceWidth * 6)) % 360 && wheelRotationZ <= (startAngleZ + (pieceWidth * 7)) % 360)
        {
            point = 7;
        }
        else if (wheelRotationZ > (startAngleZ + (pieceWidth * 7)) % 360 && wheelRotationZ <= (startAngleZ + (pieceWidth * 8)) % 360)
        {
            point = 8;
        }
        else if (wheelRotationZ > (startAngleZ + (pieceWidth * 8)) % 360 && wheelRotationZ <= (startAngleZ + (pieceWidth * 9)) % 360)
        {
            point = 9;
        }
        else if (wheelRotationZ > (startAngleZ + (pieceWidth * 9)) % 360 && wheelRotationZ <= (startAngleZ + (pieceWidth * 10)) % 360)
        {
            point = 10;
        }
        else if (wheelRotationZ > (startAngleZ + (pieceWidth * 10)) % 360 && wheelRotationZ <= (startAngleZ + (pieceWidth * 11)) % 360)
        {
            point = 11;
        }
        else
        {
            point = 12;
        }

        return point;
    }

    public void updateSpinnerButton()
    {
        localIndicator = NetworkClient.connection.identity.GetComponent<Indicator>();
        if (localIndicator && !isSpinning)
        {
            wheelSpinnerButton.gameObject.SetActive(true);

            BoardGameState gameState = BoardGameController.instance.GameState;
            switch (gameState)
            {
                case BoardGameState.SortPlayerQueue:
                    wheelSpinnerButton.transform.GetChild(0).GetComponent<Text>().text = "Lock";
                    break;
                case BoardGameState.Move:
                    wheelSpinnerButton.transform.GetChild(0).GetComponent<Text>().text = "Spin";
                    break;
            }
        }
        else
        {
            wheelSpinnerButton.gameObject.SetActive(false);
        }
    }

    public void selectPiece(int pieceIndex, Indicator indicator)
    {
        if (indicatorsOnWheelPiece[pieceIndex])
        {
            return;
        }

        indicatorsOnWheelPiece[pieceIndex] = indicator;
    }

    public void deselectPiece(int pieceIndex)
    {
        if (!indicatorsOnWheelPiece[pieceIndex])
        {
            return;
        }

        indicatorsOnWheelPiece[pieceIndex] = null;
    }

    public Indicator getIndicatorOnPiece(int index)
    {
        return indicatorsOnWheelPiece[index];
    }

    private void generatePieceAnglesArray(int size)
    {
        if (size != pieceAngles.Length - 1)
        {
            pieceAngles = new float[size];
            int startIndex = 3;
            float currentAngle = 0f;
            for (int i = 0; i < size; i++)
            {
                int targetIndex = (i + startIndex) % size;
                pieceAngles[targetIndex] = currentAngle + ((360 / size) / 2);
                currentAngle += (360 / size);
            }
        }
    }

    private void wheelSpinnerButtonEvent()
    {
        wheelSpinnerButton.gameObject.SetActive(false);
        if (BoardGameController.instance)
        {
            BoardGameState gameState = BoardGameController.instance.GameState;
            switch (gameState)
            {
                case BoardGameState.SortPlayerQueue:
                    localIndicator.CmdSetLockIndicatorValue(true);
                    break;
                case BoardGameState.Move:
                    localIndicator.pressedSpinButton();
                    break;
            }
        }
    }

    private void OnValidate()
    {
        generatePieceAnglesArray(pieceCount);
    }
}
