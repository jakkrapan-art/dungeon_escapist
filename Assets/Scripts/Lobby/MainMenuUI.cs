using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string ipAddress = "localhost";
    [SerializeField] private EscapistNetworkManager networkManager = null;


    public void joinServer()
    {
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
    }
    public void hostServer()
    {
        networkManager.StartHost();
    }
}
