using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;
using Firebase.Database;

public class QuizMenager : DBConnector
{
    [Header("Quiz")]
    [SerializeField] private InputField questionField;
    [SerializeField] private Dropdown categoryDropDown;
    [SerializeField] private InputField correctField;
    [SerializeField] private InputField wrongAns1Field;
    [SerializeField] private InputField wrongAns2Field;
    [SerializeField] private InputField wrongAns3Field;
    [SerializeField] private InputField wrongAns4Field;
    [SerializeField] private InputField wrongAns5Field;
    [SerializeField] private InputField wrongAns6Field;
    [SerializeField] private InputField ansExplainField;

    public void CrateNewQuizButton()
    {
        Debug.Log(categoryDropDown.options[categoryDropDown.value].text);
        StartCoroutine(CrateNewQuizDatabase(categoryDropDown.options[categoryDropDown.value].text));
    }

    private IEnumerator CrateNewQuizDatabase(string _category)
    {
        int idQuiz = 1;
        var cnt = DBreference.Child("Quiz").Child(_category).GetValueAsync();
        yield return new WaitUntil(predicate: () => cnt.IsCompleted);
        if (cnt.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {cnt.Exception}");
        }
        else
        {
            DataSnapshot snapshot = cnt.Result;
            idQuiz = Convert.ToInt32(snapshot.ChildrenCount) + 1;
        }
        QuestionData Quiz = new QuestionData(idQuiz, questionField.text,_category, correctField.text, wrongAns1Field.text, wrongAns2Field.text, wrongAns3Field.text, wrongAns4Field.text, wrongAns5Field.text, wrongAns6Field.text);
        string json = JsonConvert.SerializeObject(Quiz);
        Debug.Log(json);
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("Quiz").Child(_category).Child(idQuiz.ToString()).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("CrateNewUser is done");
        }


    }
}
