using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class AreaStorage : NetworkBehaviour
{
    [SyncVar]
    public BoardPlayer owner;
    [SerializeField]
    private QuestionSnatcher question;
    public readonly SyncDictionary<int, string> inventory = new SyncDictionary<int, string>();
    public List<string> testDic = new List<string>();
    // public string[] letterCorrect;//letter ที่ถูก ไว้checkการซ้ำ //ทำprivateด้วย

    [SyncVar(hook = nameof(OnColorChanged))]
    public Color characterColor = Color.white;


    [Server]
    public void setOwner(BoardPlayer player)
    {
        owner = player;
        RpcSetOwner(player);
    }
    [ClientRpc]
    public void RpcSetOwner(BoardPlayer player)
    {
        owner = player;
    }
    public void setColor(Color color)
    {
        characterColor = color;
    }
    public void setQuestion(QuestionSnatcher question)
    {
        this.question = question;
    }
    void OnColorChanged(Color _Old, Color _New)
    {
        GetComponent<Renderer>().material.color = _New;
    }

    public override void OnStartClient()
    {
        // Equipment is already populated with anything the server set up
        // but we can subscribe to the callback in case it is updated later on
        inventory.Callback += OnInventoryChange;
    }

    void OnInventoryChange(SyncDictionary<int, string>.Operation op, int key, string letter)
    {
        try
        {
            WoldSnatcherController.instance.UpdateUIDisplay();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

    }

    public QuestionSnatcher getQuestion()
    {
        return question;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner != null && owner.getSnatcherCharacter != null)
            if (other.gameObject.tag == "Snatcher" && owner.getSnatcherCharacter.Equals(other.GetComponent<Snatcher>()) && owner.getSnatcherCharacter.GetComponent<Snatcher>().isCarrying())
            {
                getLetterCube(other.GetComponent<Snatcher>().carriedBox.GetComponent<LetterCube>());
                other.GetComponent<Snatcher>().sendLetterCube();
            }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(owner.DisplayName);
            foreach (var item in inventory)
            {
                Debug.Log($"{item.Key} + {item.Value}");
            }

        }
    }

    private void getLetterCube(LetterCube box)//get & check
    {
        int i = 0;//นับตำแหน่ง
        int wrongPoint = 0;//นับletterที่ไปใช่ในword
        if (box == null)
        {
            Debug.Log("box is null");
            return;
        }
        Debug.Log(question.getWord());
        foreach (char l in question.getWord())//บัคนะไอสัส
        {
            if (l == box.LetterWord && !inventory.ContainsKey(i))// Correct
            {
                markCorrect(i, l);//show letterเมื่อใส่ตัวอักษรถูก
                break;
            }
            else//worng
            {
                wrongPoint += 1;
            }
            i += 1;
        }
        if (wrongPoint == question.getWord().Length)
        {
            punish(box.LetterWord);
        }
    }

    private void markCorrect(int index, char letter)//show letterเมื่อใส่ตัวอักษรถูก
    {
        //letterCorrect[index] = letter.ToString();
        inventory.Add(index, letter.ToString());//error
        WoldSnatcherController.instance.SnatcherCheckWiner(this);
        showAlertText(letter.ToString(), true);
    }

    private void punish(char letter)//ลงโทษเมื่อใส่ตัวอักษรผิด
    {
        Debug.Log("punish");
        showAlertText(letter.ToString(), false);
    }


    private void ClientUpdateUI()
    {
        WoldSnatcherController.instance.UpdateUIDisplay();
    }


    [ClientRpc]
    public void showAlertText(string letter, bool isCorrect)
    {
        SceneSnatcherScript.instance.setAlertText(true, letter, isCorrect);
        
    }
}
