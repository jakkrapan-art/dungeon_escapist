using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionChoice : MonoBehaviour
{
    public bool isChosen;
    public TMP_Text choiceText;
    private bool isCutChoice;
    private Button buttonComp;

    private void Awake()
    {
        isChosen = false;
        isCutChoice = false;
    }

    private void Start()
    {
        choiceText = this.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonComp = this.GetComponent<Button>();
    }

    private void Update()
    {
        if (isCutChoice)
        {
            isChosen = false;
            buttonComp.interactable = false;
            choiceText.text = "";
        }
    }

    public void onMouseClick()
    {
        isChosen = true;
    }

    public void cutChoice()
    {
        isCutChoice = true;
    }

    private void OnDisable()
    {
        isCutChoice = false;
        isChosen = false;
        buttonComp.interactable = true;
    }
}
