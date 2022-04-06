using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string questionText { get; private set; }
    public QuestionCategory category { get; private set; }
    public string correctAnswer { get; private set; }
    public int correctAnswerIndex { get; private set; }
    public string[] choices { get; private set; } = new string[4];

    public Question(string question, QuestionCategory category, string correctAns, List<string> wrongAns)
    {
        questionText = question.Trim();
        this.category = category;
        correctAnswer = correctAns.Trim();
        choices = randomChoise(correctAns, wrongAns);
    }

    private string[] randomChoise(string correctAns, List<string> wrongAns)
    {
        string[] choices = new string[4];
        correctAnswerIndex = Random.Range(0, 4);
        List<string> wrongAnswers = wrongAns;

        choices[correctAnswerIndex] = correctAnswer;

        for (int i = 0; i < 4; i++)
        {

            int wrongAnsIndex = Random.Range(0, wrongAnswers.Count);
            if (i != correctAnswerIndex)
            {
                string chosenWrongAnswer = wrongAnswers[wrongAnsIndex];
                wrongAnswers.RemoveAt(wrongAnsIndex);
                choices[i] = chosenWrongAnswer.Trim();
            }

            if (wrongAnswers.Count < 1)
                break;
        }

        return choices;
    }

    public override string ToString()
    {
        return $"Question : {questionText}\n    (1){choices[0]}    (2){choices[1]}    (3){choices[2]}    (4){choices[3]}     [Correct Answer : {correctAnswer}]";
    }
}
