using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizUI_QuizWindow : MonoBehaviour
{
    public Text questionUI;
    public Text quiCategoryUI;
    public List<QuestionChoice> choicesUI;

    public Button useItemBtn;

    private string chosenAnswer;
    private int chosenAnswerIndex;

    public string ChosenAnswer
    {
        get { return chosenAnswer; }
    }

    public int ChosenAnswerIndex
    {
        get { return chosenAnswerIndex; }
    }

    private void OnDisable()
    {
        chosenAnswer = null;
    }

    private void Update()
    {
        /*var playerItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().Item;

        if (playerItem == null)
        {
            disableUseItemBtn("item is null");
        }
        else
        {
            if (playerItem.isNull())
            {
                disableUseItemBtn("item is null");
            }
            else
            {
                if (playerItem.ItemName.Equals(""))
                {
                    disableUseItemBtn("name is \"\"");
                }
                else
                {
                    enableUseItemBtn();
                }
            }
        }*/
    }

    public void setQuizWindowUI(Question q)
    {
        if (q != null)
        {
            questionUI.text = q.questionText;
            quiCategoryUI.text = q.category.ToString();
            int index = 0;
            foreach (var choice in choicesUI)
            {
                choice.choiceText.text = q.choices[index];
                index++;
            }
        }
        else
        {
            questionUI.text = "";
            quiCategoryUI.text = "";

            int index = 0;
            foreach (var choice in choicesUI)
            {
                choice.choiceText.text = "";
                index++;
            }
        }
    }

    public bool isChooseOnlyOneChoice()
    {
        List<QuestionChoice> chosenChoices = new List<QuestionChoice>();
        int chosenChoiceIndex = 0;
        int index = 0;

        foreach (var choice in choicesUI)
        {
            QuestionChoice _choiceScript = choice.GetComponent<QuestionChoice>();
            if (_choiceScript != null && _choiceScript.isChosen)
            {
                chosenChoices.Add(_choiceScript);
                chosenChoiceIndex = index;
            }
            index++;
        }

        if (chosenChoices.Count == 1)
        {
            chosenAnswer = chosenChoices[0].choiceText.text;
            chosenAnswerIndex = chosenChoiceIndex;
            return true;
        }
        else if (chosenChoices.Count > 1)
        {
            resetChoiceButton();
            Debug.LogError("User chosen more than 1 choice.");
        }

        return false;
    }

    public void openWindow()
    {
        this.gameObject.SetActive(true);
    }
    public void closeWindow()
    {
        this.gameObject.SetActive(false);
    }

    private void resetChoiceButton()
    {
        foreach (var choice in choicesUI)
        {
            QuestionChoice _choiceScript = choice.GetComponent<QuestionChoice>();
            _choiceScript.isChosen = false;
        }
    }

    private void enableUseItemBtn()
    {
        Debug.Log("Enable item button.");
        useItemBtn.gameObject.SetActive(true);
    }

    private void disableUseItemBtn(string msg)
    {
        Debug.Log("Disble use item button because " + msg);
        useItemBtn.gameObject.SetActive(false);
    }
}
