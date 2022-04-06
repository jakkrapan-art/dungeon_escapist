using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseConnector : MonoBehaviour
{
    public static DatabaseConnector instance;

    [SerializeField]
    private QuestionDatabase QuestionDB;
    // Start is called before the first frame update

    public List<QuestionData> loadQuestions()
    {
        List<QuestionData> datas = new List<QuestionData>();
        //Load Questions from FireBase in future !!!!
        //datas = QuestionDB.quizzes;

        return datas;
    }
}
