using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameCenter : MonoBehaviour
{
    [Header("Singleton")]
    public static MinigameCenter instance;

    [SerializeField]
    private Minigame quiz;

    [Header("Current playing")]
    [SerializeField] private Minigame currentGame;
    [SerializeField] private bool isDone;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        updateCurrentGameStatus();
    }

    public void play(string gameName)
    {
        isDone = false;
        switch (gameName)
        {
            case "grammarQuiz":
                quiz.play("grammar");
                currentGame = quiz;
                break;
            case "tenseQuiz":
                quiz.play("tense");
                currentGame = quiz;
                break;
        }
    }

    private void updateCurrentGameStatus()
    {
        if (currentGame != null)
        {
            if (!currentGame.gameObject.activeSelf)
            {
                currentGame = null;
                isDone = true;
            }
        }
        else
        {
            isDone = false;
        }
    }

    public bool IsDone
    {
        get { return isDone; }
    }
}
