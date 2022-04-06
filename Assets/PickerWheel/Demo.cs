using UnityEngine;
using EasyUI.PickerWheelUI;
using UnityEngine.UI;
using System.Collections;

public class Demo : MonoBehaviour
{
    [SerializeField] private Button uiSpinButton;
    [SerializeField] private Text uiSpinButtonText;

    [SerializeField] private PickerWheel pickerWheel;

    public GameObject countdownTimeBar = null;

    private void Start()
    {
        uiSpinButton.onClick.AddListener(() =>
        {

            uiSpinButton.interactable = false;
            uiSpinButtonText.text = "Spinning";

            pickerWheel.OnSpinEnd(wheelPiece =>
            {
                Debug.Log(
                   @" <b>Index:</b> " + wheelPiece.Index + "           <b>Point:</b> " + wheelPiece.point + $"[{wheelPiece.pointText}]"
                   + "\n<b>Chance:</b> " + wheelPiece.Chance + "%"
                );

                uiSpinButton.interactable = true;
                uiSpinButtonText.text = "Spin";
            });

            pickerWheel.Spin();

        });

    }

    private IEnumerator countdownForAutoSpin(float seconds)
    {
        yield break;
    }

    private void closePickerWheelUI()
    {
        StartCoroutine(countdownForSetActiveOff(2));
    }

    private void openPickerWheelUI()
    {
        pickerWheel.gameObject.SetActive(true);
    }

    private IEnumerator countdownForSetActiveOff(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        pickerWheel.gameObject.SetActive(false);
        yield return new WaitForSeconds(seconds);
        openPickerWheelUI();
    }
}
