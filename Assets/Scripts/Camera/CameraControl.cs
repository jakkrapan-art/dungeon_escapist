using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

//public class CameraController : MonoBehaviour
public class CameraControl : MonoBehaviour
{
    public static CameraControl instance = null;

    [Header("Cameras")]
    [SerializeField] private GameObject camera_followCam = null; //2.5d view camera
    [SerializeField] private GameObject camera_freeCam = null; //2.5d view camera
    [SerializeField] private float cameraMaxSpeed;

    [SerializeField] private GameObject localPlayerIndicator;

    //[SerializeField] private Slider cameraSpeed_slider;
    [SerializeField] private Button changeCameraViewButton;

    [SerializeField] private cameraMode mode;

    private Vector2 startTouchPosition;

    private enum cameraMode { follow, free }

    [Space]
    [SerializeField] public Transform target = null;
    #region Getter & Setter
    public Transform getCameraTarget()
    {
        return target;
    }

    public void setCameraTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    #endregion

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        //changeCameraViewButton.gameObject.SetActive(false);

        target = GameObject.Find("Camera_Initial_Target").transform;
    }

    void Update()
    {
        if (mode == cameraMode.follow)
        {
            moveCameraFollowTarget();
        }
        else
        {
            cameraPerspectiveModeMove();
            //free cam mode;
        }

        //show camera change button condition
        /*if ((NetworkClient.connection.identity && BoardGameController.instance) && BoardGameController.instance.GameState == BoardGameState.SortPlayerQueue)
        {
            setActiveCameraViewButton(false);
        }*/

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            changeCameraMode();
        }

    }

    public void setActiveCameraViewButton(bool isActive)
    {
        changeCameraViewButton.gameObject.SetActive(isActive);
    }

    private void moveCameraFollowTarget()
    {
        if ((target.transform.position - transform.position).magnitude > 15f)
        {
            transform.position = target.position;
        }

        try
        {
            Vector3 targetPosition;
            if (target)
            {
                targetPosition = target.position/* + mainCameraOffset*/;
            }
            else
            {
                targetPosition = Vector3.zero;
            }

            if ((transform.position - targetPosition).magnitude <= 1)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public bool isFinishedTransition()
    {
        BoardPlayer localPlayer = NetworkClient.connection.identity.GetComponent<BoardPlayer>();

        if (!localPlayer || !localPlayer.getBoardCharacter())
        {
            return false;
        }

        Vector3 targetPosition = localPlayer.getBoardCharacter().transform.position/*+ mainCameraOffset*/;
        return (transform.position.magnitude - targetPosition.magnitude) <= 2.5f;
    }

    public void setCameraMode(int modeIndex)
    {
        int index = 0;
        if (modeIndex < 0)
            index = Mathf.Abs(modeIndex);
        else
            index = modeIndex;

        bool isFollowCamIndex = (index % 2) == 0;
        mode = (cameraMode)(index % 2);
        camera_followCam.gameObject.SetActive(isFollowCamIndex);
        camera_freeCam.gameObject.SetActive(!isFollowCamIndex);
    }

    public void changeCameraMode()
    {
        switch (mode)
        {
            case (cameraMode)0:
                setCameraMode(1);
                WheelSpinnerController.instance.hideDisplay();
                WheelSpinnerController.instance.hideIndicators();
                break;
            case (cameraMode)1:
                setCameraMode(0);
                WheelSpinnerController.instance.showDisplay();
                WheelSpinnerController.instance.showIndicators();
                break;
        }
    }

    private void cameraPerspectiveModeMove()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                startTouchPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                float cameraSpeed = cameraMaxSpeed;
                Vector2 cameraDirection = (startTouchPosition - mousePos) * Time.deltaTime * cameraSpeed;
                transform.Translate(new Vector3(cameraDirection.y, 0, -(cameraDirection.x)));
                startTouchPosition = mousePos;
            }
        }
    }
}
