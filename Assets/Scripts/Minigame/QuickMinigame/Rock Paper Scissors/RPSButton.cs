using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPSButton : MonoBehaviour
{
    public int index;
    public bool ischose = false;

    public void dochose()
    {
        ischose = true;
    }
}
