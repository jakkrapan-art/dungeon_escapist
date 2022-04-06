using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class testDB : MonoBehaviour
{
    public DependencyStatus dependencyStatus;
    public DatabaseReference DBreference;
    private void Awake()
    {
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
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void TestGetData()
    {
        StartCoroutine("GetUsernameDatabase");
    }

    private IEnumerator GetUsernameDatabase()
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("user").Child("02").Child("ID").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log(DBTask.Result);
        }
    }
}
