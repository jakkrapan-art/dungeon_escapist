using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;

public class DBConnector : MonoBehaviour
{
    public static DBConnector instance;//
    protected DependencyStatus dependencyStatus;
    public DatabaseReference DBreference;
    [SerializeField]
    private UserInfoUIController userInfoUIController;

    [Header("Loaded Databases")]
    public List<QuestionSnatcher> snatcherQuestions = new List<QuestionSnatcher>();

    private void Awake()
    {
        instance = this;
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Start()
    {
        StartCoroutine(loadSnatcherQuestionFromDatabase());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            getSnatcherQuestion();
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public IEnumerator loadQuizDatabase(System.Action<List<QuestionData>> action)
    {
        List<QuestionData> loadedQuestions = new List<QuestionData>();

        yield return new WaitUntil(() => DBreference != null);

        //load Grammar
        using (var DBTask = DBreference.Child("Question").GetValueAsync())
        {
            yield return new WaitUntil(() => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                DataSnapshot snapshot = DBTask.Result;
                foreach (var item in snapshot.Children)
                {
                    QuestionData question = JsonConvert.DeserializeObject<QuestionData>(item.GetRawJsonValue());
                    loadedQuestions.Add(question);
                }
            }
        }

        action(loadedQuestions);
    }

    private IEnumerator loadSnatcherQuestionFromDatabase()
    {
        yield return new WaitUntil(() => DBreference != null);

        var DBsnatcherQuestion = DBreference.Child("WordSnatcherQuestion");

        using (var DBTask = DBreference.Child("WordSnatcherQuestion").GetValueAsync())
        {
            yield return new WaitUntil(() => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                DataSnapshot snapshot = DBTask.Result;
                foreach (var item in snapshot.Children)
                {
                    QuestionSnatcher question = JsonConvert.DeserializeObject<QuestionSnatcher>(item.GetRawJsonValue());
                    snatcherQuestions.Add(question);
                }
            }
        }
        yield break;
    }

    public List<QuestionSnatcher> getSnatcherQuestion()
    {
        foreach (var item in snatcherQuestions)
        {
            Debug.Log($"Snatcher Question({item.getWordText()}, {item.getHint()})");
        }
        return snatcherQuestions;
    }
}
