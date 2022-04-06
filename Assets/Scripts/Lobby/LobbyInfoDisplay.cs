using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public class LobbyInfoDisplayEvent : UnityEvent<LobbyInfoDisplay> { };

public class LobbyInfoDisplay : MonoBehaviour
{

    public bool isSelected { get; private set; }

    public LobbyInfo lobbyInfo { get; private set; }

    private Button buttonComponent;

    [Header("UI")]
    [SerializeField] private Text UI_lobbyName;
    [SerializeField] private Text UI_mapName;
    [SerializeField] private Text UI_playerCount;

    [Space]
    [SerializeField] private LobbyListUIController lobbyListUIController;

    private void Start()
    {
        lobbyListUIController = GameObject.FindObjectOfType<LobbyListUIController>();
        buttonComponent = GetComponent<Button>();
        buttonComponent.onClick.AddListener(onPressed);
    }

    /*    private void Update()
        {
            if (lobbyInfo != null)
            {
                setDisplay(lobbyInfo.lobbyName, lobbyInfo.mapName, lobbyInfo.totalPlayer, lobbyInfo.maxPlayer);
            }
        }
    */
    public void onPressed()
    {
        buttonComponent.interactable = false;
        Debug.Log("Pressed.");
        lobbyListUIController.setCurrentSelectingLobby(this);
    }

    public void choose()
    {
        //lobbyListUIController.OnLobbyDisplayButtonPress(this);
    }

    public void select()
    {
        isSelected = true;
    }

    public void deselect()
    {
        isSelected = false;
    }
    public void setLobbyInfo(LobbyInfo info)
    {
        lobbyInfo = info;
        setDisplay(info.lobbyName, info.lobbyName, info.totalPlayer, info.maxPlayer);
    }

    private void setDisplay(string lobbyName, string mapName, int playerCount, int maxPlayer)
    {
        UI_lobbyName.text = lobbyName;
        UI_mapName.text = mapName;
        UI_playerCount.text = (playerCount >= maxPlayer ? "<color=red>" : "") + $"{playerCount}/{maxPlayer}";
        if (playerCount >= maxPlayer)
            setButtonToUnInteracable();
    }


    private void setButtonToUnInteracable()
    {
        buttonComponent.interactable = false;
    }
}
