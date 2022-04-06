using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitInternetUI : MonoBehaviour
{
    public GameObject waitInternetObj;
    public static WaitInternetUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void Waiting()
    {
        waitInternetObj.SetActive(true);
    }

    public void WaitSuccess()
    {
        waitInternetObj.SetActive(false);
    }
}
