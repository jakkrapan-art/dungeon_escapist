using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class QuizMinigame : Minigame
{

    [SerializeField]
    private QuizUI ui;

    public List<QuestionData> possibleGrammarQuestions { get; private set; } = new List<QuestionData>();//Mockup Question database.
    public List<QuestionData> possibleTenseQuestions { get; private set; } = new List<QuestionData>();//Mockup Question database.

    private bool isReady;

    private bool isActivateSecondChance;
    private bool alreadyCutChoice;

    [SerializeField]
    private Question currentQuestion;

    private bool isSuccessLoadQuestion;

    private void Start()
    {
        StartCoroutine(nameof(loadQuiz));
    }

    private void OnDisable()
    {
        resetGame();
    }

    public override void play(string minigameText)
    {
        base.play(minigameText);

        if (minigameText.Equals("grammar"))
        {
            StartCoroutine(playQuiz(QuestionCategory.Grammar));
        }
        else
        {
            StartCoroutine(playQuiz(QuestionCategory.Tense));
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(currentQuestion);
        }
    }

    private IEnumerator playQuiz(QuestionCategory category)
    {
        if ((possibleGrammarQuestions.Count == 0 && possibleTenseQuestions.Count == 0) || !isReady)
        {
            Debug.LogError("Not ready yet.");
            yield break;
        }

        currentQuestion = pickQuestion(category);

        var quizWindow = ui.quizWindow;
        var resultWindow = ui.resultWindow;

        quizWindow.setQuizWindowUI(currentQuestion);
        quizWindow.openWindow();

        yield return new WaitUntil(() => quizWindow.isChooseOnlyOneChoice());

        yield return new WaitForFixedUpdate();

        string userAnswer = quizWindow.ChosenAnswer;
        int userAnswerIndex = quizWindow.ChosenAnswerIndex;

        if (isActivateSecondChance && !isCorrect(userAnswer))
        {
            cutChoice(userAnswerIndex);
            yield return new WaitUntil(() => quizWindow.isChooseOnlyOneChoice());
        }

        userAnswer = quizWindow.ChosenAnswer;
        saveResult(currentQuestion, userAnswer);

        if (isCorrect(userAnswer))
        {
            //if user choose the correct answer then get random reward from rewards list.

            int randomIndex = Random.Range(0, rewards.Count);
            //giveReward(BoardGameController.instance.getLocalPlayer(), rewards[randomIndex].getItem());
        }

        quizWindow.closeWindow();

        resultWindow.setQuizResultUI(currentQuestion.correctAnswer, isCorrect(userAnswer));
        resultWindow.openWindow();

        yield return new WaitUntil(() => resultWindow.gameObject.activeSelf == false);

        gameObject.SetActive(false);
    }

    private void saveResult(Question q, string userAnswer)
    {
        QuizResult result = GetComponent<QuizResult>();
        if (result != null)
        {
            result.addResult(q, userAnswer);
        }
        else
        {
            Debug.LogError("QuizResult is null.");
        }
    }

    protected override void resetGame()
    {
        alreadyCutChoice = false;
        isActivateSecondChance = false;
        isDone = false;
    }

    private Question pickQuestion(QuestionCategory category)
    {
        List<QuestionData> questionList = category == QuestionCategory.Grammar ? possibleGrammarQuestions : possibleTenseQuestions;

        int length = questionList.Count;

        int pickIndex = Random.Range(0, length);

        QuestionData chosenData = questionList[pickIndex];
        Question q = new Question(chosenData.question, chosenData.category, chosenData.correctAns, groupingWrongAnswer(chosenData));

        //possibleQuestion.quizzes.RemoveAt(pickIndex);

        return q;
    }

    private List<string> groupingWrongAnswer(QuestionData data)
    {
        List<string> choices = new List<string>();

        for (int i = 0; i < 6; i++)
        {
            string wrongAnswer = null;
            switch (i)
            {
                case 0:
                    wrongAnswer = data.wrongAns1;
                    break;
                case 1:
                    wrongAnswer = data.wrongAns2;
                    break;
                case 2:
                    wrongAnswer = data.wrongAns3;
                    break;
                case 3:
                    wrongAnswer = data.wrongAns4;
                    break;
                case 4:
                    wrongAnswer = data.wrongAns5;
                    break;
                case 5:
                    wrongAnswer = data.wrongAns6;
                    break;
            }

            if (wrongAnswer != null && (!wrongAnswer.Equals("")))
            {
                choices.Add(wrongAnswer);
            }
        }

        return choices;
    }

    private void cutChoice()
    {
        int randomNum = Random.Range(0, 4);

        int loopCount = 0;

        while (randomNum == currentQuestion.correctAnswerIndex)
        {
            randomNum = Random.Range(0, 4);
            loopCount++;
        }
        cutChoice(randomNum);
    }

    private void cutChoice(int index)
    {
        if (!alreadyCutChoice)
        {
            Debug.LogError("Cuting one wrong choice.");
            ui.quizWindow.choicesUI[index].cutChoice();
            alreadyCutChoice = true;
        }
        else
        {
            Debug.LogError("Choice had been cut.");
        }
    }

    private void secondChance()
    {
        Debug.Log("Activate second chance");
        isActivateSecondChance = true;
    }

    public void useItem()
    {
        PlayerInventory playerInv = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        switch (playerInv.Item.ItemName)
        {
            case "Second Chance":
                secondChance();
                break;
            case "Cut Choice":
                cutChoice();
                break;
            default:
                break;
        }

        playerInv.useItem();
    }

    private bool isCorrect(string chosenAnswer)
    {
        return chosenAnswer.Equals(currentQuestion.correctAnswer);
    }

    private IEnumerator loadQuiz()
    {
        isSuccessLoadQuestion = false;
        yield return new WaitUntil(() => loadLocalQuestion());

        if (EscapistNetworkManager.isConnectingToNetwork())
        {
            StartCoroutine(DBConnector.instance.loadQuizDatabase(getLoadedQuestions));
        }

        yield return new WaitUntil(() => isSuccessLoadQuestion);

        gameObject.SetActive(false);
    }

    private void getLoadedQuestions(List<QuestionData> loadedQuestionDataFromFirebase)
    {
        getQuestionFromDB(loadedQuestionDataFromFirebase);
    }

    private void getQuestionFromDB(List<QuestionData> loadedQuestionDataFromFirebase)
    {
        foreach (var question in loadedQuestionDataFromFirebase)
        {
            if (question.category.Equals(QuestionCategory.Grammar))
            {
                possibleGrammarQuestions.Add(question);
            }
            else
            {
                possibleTenseQuestions.Add(question);
            }
        }

        isSuccessLoadQuestion = true;
        isReady = true;
    }

    private bool loadLocalQuestion()
    {
        getLocalQuestion("grammarQuestion", possibleGrammarQuestions);
        getLocalQuestion("tenseQuestion", possibleTenseQuestions);

        return true;
    }

    private void getLocalQuestion(string key, List<QuestionData> questionList)
    {
        if (PlayerPrefs.GetString(key) != string.Empty)
        {
            string json = PlayerPrefs.GetString(key);

            List<QuestionData> questions = JsonConvert.DeserializeObject<List<QuestionData>>(json);
            foreach (var question in questions)
            {
                questionList.Add(question);
            }
        }
    }
}