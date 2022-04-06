using Firebase.Database;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class DBQuestion : DBConnector
{
    public QuizMinigame quizMinigame;
    public double QuizCount;

    public void testGetQuizButton()
    {
        StartCoroutine(getQuizByCategory("grammar"));
    }

    public IEnumerator getQuiz()
    {
        var DBTask = DBreference.Child("Quiz").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //count all Quiz
            QuizCount = DBTask.Result.ChildrenCount;
            Debug.Log("QuizCount = " + QuizCount);
            var QuizTask = DBreference.Child("Quiz").StartAt(QuizCount).GetValueAsync();
        }
    }


    public IEnumerator getQuizByCategory(string _category)
    {
        var DBTask = FirebaseDatabase.DefaultInstance.GetReference("Quiz").Child(_category).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string Results = childSnapshot.GetRawJsonValue();
                if (JsonConvert.DeserializeObject<QuestionData>(Results) != null)
                {
                    Debug.Log(Results);
                    
                    //quizMinigame.PossibleQuestion.quizzes.Add(GetQuizJson(Results));
                }
            }
        }
    }
    public QuestionData GetQuizJson(string saveData)
    {
        return JsonConvert.DeserializeObject<QuestionData>(saveData);
    }
}
