using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizUI_ResultWindow : MonoBehaviour
{
    public TMP_Text resultTextUI;
    public TMP_Text questionCorrectAnswerUI;

    [SerializeField] private string correctText;
    [SerializeField] private string incorrectText;

    public void setQuizResultUI(string correctAnswer,bool isCorrect)
    {
        if (!correctAnswer.Equals(string.Empty))
        {
            if (isCorrect)
            {
                resultTextUI.text = correctText;
                resultTextUI.color = Color.green;
            }
            else
            {
                resultTextUI.text = incorrectText;
                resultTextUI.color = Color.red;
            }

            questionCorrectAnswerUI.text = correctAnswer;
        }
        else
        {
            resultTextUI.text = "";
            questionCorrectAnswerUI.text = "";
        }
    }

    public void openWindow()
    {
        this.gameObject.SetActive(true);
    }
    public void closeWindow()
    {
        this.gameObject.SetActive(false);
    }
}
