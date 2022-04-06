using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class SceneSnatcherScript : MonoBehaviour
{
    public static SceneSnatcherScript instance;
    [Header("controllerUI")]
    public Text canvasStatusText;
    public VariableJoystick joystick;
    public GameObject LetterObj;//ใช้Generate
    [Header("StataUI")]
    public Transform wordLine;//ใส่ตำแหน่งUI letter
    private GameObject[] lettersUI;//Obj ที่สร้างแล้ว
    public List<GameObject> ObjOtherPlayer;
    public List<Text> TextOtherPlayer;
    [Header("QuestionUI")]
    [SerializeField] private Text QuestionText;
    [SerializeField] private Text timeText;
    [Header("HintUI")]
    [SerializeField] private Text HintText;
    [Header("ObjectScenes")]
    public GameObject tutorialScene;
    public GameObject controllerScene;
    public GameObject QuestionScene;
    public GameObject WinScene;
    public GameObject LoseScene;
    [Header("other")]
    [SerializeField] private BoardPlayer owner;
    [SerializeField] private Text alertText;
    [SerializeField] private CameraSnatch cameraSnatch;


    //[SyncVar(hook = nameof(OnStatusTextChanged))]
    //public string statusCountdown;

    private void Awake()
    {
        instance = this;
    }

    public void setOwner(BoardPlayer player)
    {
        owner = player;
        Debug.Log(player.getSnatcherCharacter);
        if (player.getSnatcherCharacter != null)
        {
            Debug.Log(player.getSnatcherCharacter);
            cameraSnatch.setTarget(player.getSnatcherCharacter.transform);
        }
    }
    public void GameStart()
    {
        QuestionScene.SetActive(false);
        controllerScene.SetActive(true);
    }

    public void ShowTimeout(int i)
    {
        timeText.text = i.ToString();
    }

    private BoardPlayer findLocalPlayer()
    {
        Debug.Log(NetworkClient.connection.identity);
        var player = NetworkClient.connection.identity.GetComponent<BoardPlayer>();
        owner = player;
        return player;
    }

    public void setLocalOwner()
    {
        var players = findLocalPlayer();
        setOwner(findLocalPlayer());
    }

    public void setSceneWord(QuestionSnatcher question)//เริ่มจัดหน้าsceneตัวอักษร
    {
        int countLetter = question.getWord().Length;
        Vector3 fristLetter = wordLine.transform.position;
        lettersUI = new GameObject[countLetter];
        int i = 0;
        foreach (char letter in question.getWord())
        {
            GameObject l = Instantiate(LetterObj, fristLetter + new Vector3(i * 130, 0f), LetterObj.transform.rotation, wordLine);
            lettersUI[i] = l;
            i += 1;
        }
        QuestionScene.SetActive(true);
        controllerScene.SetActive(false);
        QuestionText.text = question.getHint();
        HintText.text = question.getHint();
    }

    public void showLetter(int index, string letter)//show letter ที่รับมาในช่องที่ถูกต้อง
    {
        Text t = lettersUI[index].transform.Find("Text").GetComponent<Text>();
        t.text = letter.ToUpper();
    }


    public void pickCubeButton()
    {
        if (owner != null)
        {
            Debug.Log(owner.getSnatcherCharacter.carriedBox);
            Debug.Log(owner.getSnatcherCharacter.isCarrying());
            if (owner.getSnatcherCharacter.targetBoxObj != null && !owner.getSnatcherCharacter.isCarrying())

            {
                owner.getSnatcherCharacter.CmdPlayerSnatch(owner.getSnatcherCharacter.targetBoxObj);

            }
            else if (owner.getSnatcherCharacter.carriedBox != null && owner.getSnatcherCharacter.isCarrying())
            {
                owner.getSnatcherCharacter.CmdPlayerDiscardBox();
            }
        }
    }

    public Vector2 getJoystickValue()
    {

        return new Vector2(joystick.Horizontal, joystick.Vertical);
    }
    public void DeclareWiner(BoardPlayer winer)
    {
        if (winer == owner)
        {
            WinScene.SetActive(true);
        }
        else
        {
            LoseScene.SetActive(false);
        }
        controllerScene.SetActive(false);
        timeText.gameObject.SetActive(false);
    }
    public void updateLetterDisplays(List<AreaStorage> players)
    {
        int no = 0;
        for (int num = 0; num < players.Count; num++) //ดึงข้อมูลแต่ละplayer
        {
            var player = players[num];
            if (player.owner != null)
            {
                if (player.owner.isLocalPlayer)
                {
                    for (int index = 0; index < WoldSnatcherController.instance.question.getWord().Length; index++)
                    {
                        if (player.inventory.ContainsKey(index))
                        {
                            showLetter(index, player.inventory[index]);
                        }
                    }
                }
                else//เครื่องอื่น
                {
                    ObjOtherPlayer[no].SetActive(true);
                    TextOtherPlayer[no].text = player.owner.DisplayName + " : " + player.inventory.Count + " / " + WoldSnatcherController.instance.question.getWord().Length;
                    no++;
                }
            }
            else
            {
                ObjOtherPlayer[no].SetActive(false);
                TextOtherPlayer[no].text = string.Empty;
                no++;
            }

        }
    }

    public void setAlertText(bool setAction, string letter, bool isCorrect)
    {
        if (owner == findLocalPlayer())
        {
            alertText.gameObject.SetActive(true);
            if (isCorrect)
            {
                alertText.text = $"{letter} is Correct";
            }
            else
            {
                alertText.text = $"{letter} is Wrong";
            }
        }
    }


}
