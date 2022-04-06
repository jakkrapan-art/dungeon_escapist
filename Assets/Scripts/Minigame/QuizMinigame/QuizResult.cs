using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizResult : MonoBehaviour
{
    public static QuizResult instance;
    private Dictionary<Question, string> quizResults; //string is type of user answered 
    // Start is called before the first frame update

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        quizResults = new Dictionary<Question, string>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (KeyValuePair<Question,string> result in quizResults)
            {
                if (result.Value.Equals(result.Key.correctAnswer))
                {
                    Debug.Log(result.Key.questionText + "|" +result.Value);
                }
                else
                {
                    Debug.LogError(result.Key.questionText + "|" +result.Value);
                }
            }
        }
    }

    public bool addResult(Question q, string userAnswer)
    {
        if (q == null || userAnswer == null || userAnswer.Equals(""))
        {
            return false;
        }

        quizResults.Add(q, userAnswer);
        return true;
    }
}
