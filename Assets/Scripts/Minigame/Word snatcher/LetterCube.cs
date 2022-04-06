using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
public class LetterCube : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnLetterWordChanged))]
    [SerializeField] private char letterWord;//private
    [SyncVar]
    [SerializeField] private Transform parent;
    [SyncVar]
    [SerializeField] private float timeout = 5f;
    [SerializeField] private TMP_Text[] LetterOnBox;

    [SyncVar]
    private bool isCarried = false;

    public char LetterWord
    {
        get { return letterWord; }
        set { letterWord = value; }
    }
    public bool IsCarried
    {
        get { return isCarried; }
    }
    public Transform Parent
    {
        get { return parent; }
        set { parent = value; }
    }


    void OnLetterWordChanged(char _Old, char _New)
    {
        letterWord = _New;
    }


    public void Carried()
    {
        isCarried = true;

    }
    public void Dorpped()
    {
        isCarried = false;
        parent = null;
    }

    private void Start()
    {
        LetterOnBox = transform.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text letterText in LetterOnBox)
        {
            letterText.text = letterWord.ToString().ToUpper();
        }
        StartCoroutine("Countdown");
    }
    [Server]
    private IEnumerator Countdown()
    {
        float loopPerTick = 0.1f;
        do
        {
            if (!isCarried)
            {
                timeout -= loopPerTick;
            }
            yield return new WaitForSeconds(loopPerTick);
        }
        while (0 < timeout);
        RpcDestroy();
    }
    [ClientRpc]
    public void RpcDestroy()
    {
        Destroy(this.gameObject);
        NetworkServer.UnSpawn(this.gameObject);
    }

    private void Update()
    {
        if (parent != null)
        {
            transform.position = parent.transform.position;
        }
    }

}
