using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ListLetter : NetworkBehaviour
{
    public string[] letterCorrect;

    public ListLetter(string[] newList)
    {
        letterCorrect = newList;
    }
}
