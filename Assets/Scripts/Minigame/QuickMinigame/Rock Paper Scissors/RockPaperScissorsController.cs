using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockPaperScissorsController : MonoBehaviour
{
    private bool countDown = true;
    private int timer = 5;

    private Queue<int> solveColors = new Queue<int>();
    private RPSButton currentButtonChosen = null;//playerเลือก ณ ปัจจุบัน

    [SerializeField] private Transform MarkSelection;

    [Header("Hint GameObject")]
    [SerializeField] private List<GameObject> HintColorSprites;
    [SerializeField] private List<Transform> HintSpritePosition;
    [SerializeField] private GameObject TextHeader;
    [SerializeField] private Text alertText;
    [SerializeField] private List<Image> lightLifeImages;
    [SerializeField] private Slider countdownBar;
    [SerializeField] private List<RPSButton> ButtonRPS; //Rock Paper Sciessors

    void Start()
    {
        startGame();
    }
    public void startGame()
    {

        StartCoroutine("playingRPS");
    }
    private void Update()
    {
        countdownTimeOut();

    }

    public void countdownTimeOut()
    {
        if (countdownBar.maxValue != timer)
            countdownBar.maxValue = timer;

        Vector3 originalPos = countdownBar.transform.position;

        if (countDown)
            countdownBar.value += Time.deltaTime;

        if (countdownBar.value <= 0)
        {
            countDown = false;
        }
    }

    public IEnumerator playingRPS()//Rock Paper Sciessors
    {
        Debug.Log("start");

        yield return new WaitUntil(() => waitSelectNextColor(ButtonRPS));
        Debug.Log(currentButtonChosen);
        if (currentButtonChosen != null)
        {
            currentButtonChosen.transform.position = MarkSelection.position;
            //while (MoveToMark(currentButtonChosen.transform.position, MarkSelection.position)) { yield return null; }

        }

        Debug.Log("End");
    }


    private bool waitSelectNextColor(List<RPSButton> currentButtonHave)
    {
        foreach (var button in currentButtonHave)
        {
            //ถ้าเลือกแล้ว return true แล้วให้ current = ปุ่มที่เลือก
            if (button != null && button.ischose)
            {
                currentButtonChosen = button;
                return true;
            }
        }
        return false;
    }

    public void checkWinner(int player_action, int computer_action)
    {
        if (player_action == computer_action)//draw
        {

            //Debug.Log("Both players selected {user_action}. It's a tie!")
            if (player_action == 1)//Rock
            {
                if (computer_action == 2)//Sceioess
                    print("Rock smashes scissors! You win!");
                else
                    print("Paper covers rock! You lose.");
            }
            else if (player_action == 2)//Sceioess
            {
                if (computer_action == 3)//Paper
                    print("Rock smashes scissors! You win!");
                else
                    print("Paper covers rock! You lose.");
            }
            else if (player_action == 3)//paper
            {
                if (computer_action == 1)//Rock
                    print("Rock smashes scissors! You win!");
                else
                    print("Paper covers rock! You lose.");
            }
        }
    }

    bool MoveToMark(Vector3 buttonObj, Vector3 goal)
    {
        return goal != (buttonObj = Vector3.MoveTowards(buttonObj, goal, 300f * Time.deltaTime));
    }


}
