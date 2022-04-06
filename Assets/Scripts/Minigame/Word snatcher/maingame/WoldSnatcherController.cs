using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
public class WoldSnatcherController : NetworkBehaviour
{
    public static WoldSnatcherController instance;
    public List<AreaStorage> storages;
    public QuestionSnatcher question;
    public EscapistNetworkManager networkManager;
    [SerializeField]
    private GameObject CubePrefab;//base obj
    [SerializeField]
    private List<char> listLetter;
    [SerializeField]
    private Transform spawnBoxPoint;
    [SerializeField]
    private Vector2 spawnSize;
    private Minigame minigame;

    private void Awake()
    {
        instance = this;
        
        minigame = FindObjectOfType<Minigame>();
    }


    private void Start()
    {
        storages = FindObjectsOfType<AreaStorage>().ToList();
        networkManager = EscapistNetworkManager.singleton as EscapistNetworkManager;
        StartCoroutine("waitTutorialReady");
        getQuestionDatabase();
        Debug.Log(question);
        

        

    }
    
    private IEnumerator waitTutorialReady()
    {
        SceneSnatcherScript.instance.tutorialScene.SetActive(true);
        yield return new WaitForSeconds(5);
        SceneSnatcherScript.instance.tutorialScene.SetActive(false);
        SceneSnatcherScript.instance.setSceneWord(question);
        StartCoroutine("waitAllPlayerToReady");
    }


    [Server]
    private IEnumerator waitAllPlayerToReady()
    {
        yield return new WaitUntil(() => networkManager.isPlayersReadied());
        RpcCountdownStart(5);
    }

    [ClientRpc]
    private void RpcCountdownStart(int timer)//รอรอบ2
    {
        StartCoroutine(countdownStart(timer));

    }


    public IEnumerator countdownStart(int timer)
    {
        int Time = timer;
        for (int i = 0; i < timer; i++)
        {
            SceneSnatcherScript.instance.ShowTimeout(Time);
            Time--;
            yield return new WaitForSeconds(1);
        }
        UpdateUIDisplay();
        SceneSnatcherScript.instance.setLocalOwner();
        SceneSnatcherScript.instance.GameStart();//เริ่มเกม
        InvokeRepeating("controllSpawnBoxes", 0f, 5f);
    }



    public AreaStorage getEmptyStorage()
    {
        foreach (AreaStorage h in storages)
        {
            if (h.owner == null)
            {
                return h;
            }
        }
        return null;
    }

    public void SetQuestionToStorage(AreaStorage h)
    {
        Debug.Log(h);
        Debug.Log(question);
        h.setQuestion(question);
        //h.letterCorrect = new string[question.getWord().Length];
    }
    void OnClientReady(NetworkConnection conn, ReadyMessage msg)
    {
        Debug.Log("Client is ready to start: " + conn);
        NetworkServer.SetClientReady(conn);
        InvokeRepeating("controllSpawnBoxes", 0f, 5f);
    }
    [Server]
    private void controllSpawnBoxes()
    {
        LetterCube[] BoxesOnBoand = GameObject.FindObjectsOfType<LetterCube>();
        if (BoxesOnBoand.Length <= 0 || BoxesOnBoand == null)
        {
            SpawnBoxActive(5);
        }
        else if (BoxesOnBoand.Length <= 14)
        {
            SpawnBoxActive(3);
        }
        else if (BoxesOnBoand.Length <= 20)
        {
            SpawnBoxActive(1);
        }
    }

    private void SpawnBoxActive(int index)
    {
        for (int i = 0; i < index; i++)
        {
            int RandomIndex = Random.Range(0, listLetter.Count);
            CmdSpawnBox();
        }
    }

    private bool checkInList(char inputLetter)
    {
        foreach (char letter in listLetter)
        {
            if (inputLetter == letter)
            {
                return true;//ซ้ำ
            }
        }
        return false;
    }
    [Server]
    private void CmdSpawnBox()
    {
        int RandomIndex = Random.Range(0, listLetter.Count);
        var letter = listLetter[RandomIndex];
        GameObject BoxLetter = CubePrefab;
        if (letter == ' ')
        {
            //genarate letter
            int num = Random.Range(0, 26); // Zero to 25
            letter = (char)('a' + num);
        }
        BoxLetter.GetComponent<LetterCube>().LetterWord = letter;

        Vector3 pos = spawnBoxPoint.position + new Vector3(Random.Range(-spawnSize.x / 2, spawnSize.x / 2), 0, Random.Range(-spawnSize.y / 2, spawnSize.y / 2));
        BoxLetter = Instantiate(BoxLetter, pos, Quaternion.identity);
        NetworkServer.Spawn(BoxLetter);
    }
    [Server]
    private void getQuestionDatabase()
    {
        var DBSnatcherQuestion = FindObjectOfType<DBConnector>().getSnatcherQuestion();
        int RandomNum = Random.Range(0, DBSnatcherQuestion.Count);

        Debug.Log("get Question form database");

        RpcGetQuestion(RandomNum);
    }
    [ClientRpc]
    private void RpcGetQuestion(int RandomNum)
    {
        var DBSnatcherQuestion = FindObjectOfType<DBConnector>().getSnatcherQuestion();
        question = DBSnatcherQuestion[RandomNum];

        Debug.Log(question);
        foreach (char h in question.getWord())
        {
            Debug.Log(h);
            if (!checkInList(h))
            {
                listLetter.Add(h);
                listLetter.Add(' ');
            }
        }
        Debug.Log(storages);
        foreach (AreaStorage h in storages)
        {
            if (h.owner != null)
            {
                Debug.Log(h);
                SetQuestionToStorage(h);
            }
        }
    }

    public void UpdateUIDisplay()
    {
        SceneSnatcherScript.instance.updateLetterDisplays(storages);//

    }

    [Server]
    public void SnatcherCheckWiner(AreaStorage storage)
    {
        Debug.Log(WoldSnatcherController.instance.checkFinished(storage));
        if (WoldSnatcherController.instance.checkFinished(storage))
        {
            Debug.Log(storage.owner + " is winer");
            RpcDeclareWiner(storage);
            getRewardWiner(storage.owner);
            StartCoroutine(DelayBackToBoardGame(5));
        }
    }
    [ClientRpc]
    public void RpcDeclareWiner(AreaStorage storage)
    {
        SceneSnatcherScript.instance.DeclareWiner(storage.owner);
    }
    public IEnumerator DelayBackToBoardGame(float timer)
    {
        yield return new WaitForSeconds(timer);
        if (networkManager != null)
        {
            networkManager.startGame();
        }
    }
    public void getRewardWiner(BoardPlayer winer)
    {
        Debug.Log(minigame);
        int randomIndex = Random.Range(0, minigame.rewards.Count);
        Debug.Log(winer);
        Debug.Log(minigame.rewards[randomIndex].getItem());
        Debug.Log(minigame.rewards[randomIndex].getAmount());
        minigame.giveReward(winer, 1);
    }

    public bool checkFinished(AreaStorage storage)
    {
        bool isArrayEqual = true;

        // Traverse both array and compare
        //each element
        for (int i = 0; i < question.getWord().Length; i++)
        {
            if (!storage.inventory.ContainsKey(i))
            {
                isArrayEqual = false;
                break;
            }
            // set true if each corresponding
            if (question.getWord()[i].ToString() != storage.inventory[i])
            {

                isArrayEqual = false;
                break;
            }
            //elements of arrays are equal
        }

        return isArrayEqual;
    }
}
