using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyListUIController : MonoBehaviour
{
    [SerializeField] private EscapistNetworkDiscovery networkDiscovery;

    [SerializeField] private LobbyInfoDisplay currentSelectedLobby = null;

    private Dictionary<System.Uri, GameObject> discoveredLobby = new Dictionary<System.Uri, GameObject>();

    [Header("Lobby display colors")]
    [SerializeField] private Color selectedButtonColor;
    [SerializeField] private Color notSelectedButtonColor;

    [Space]
    [SerializeField] private GameObject lobbyInfoDisplayPrefab;
    [SerializeField] private Transform lobbyInfoDisplayContainer;

    private void OnEnable()
    {
        findServer();
    }

    public void hostServer()
    {
        EscapistNetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    private void findServer()
    {
        resetDiscoveredServer();
        networkDiscovery.StartDiscovery();
    }

    private void resetDiscoveredServer()
    {
        for (int i = 0; i < lobbyInfoDisplayContainer.childCount; i++)
        {
            Destroy(lobbyInfoDisplayContainer.GetChild(0).gameObject);
        }

        currentSelectedLobby = null;
        discoveredLobby.Clear();
    }

    private void setLobbyDisplayColor(LobbyInfoDisplay display)
    {
        Button displayButtonComponent = display.gameObject.GetComponent<Button>();

        ColorBlock colors = displayButtonComponent.colors;

        if (display.isSelected)
        {
            colors.normalColor = selectedButtonColor;
        }
        else
        {
            colors.normalColor = notSelectedButtonColor;
        }

        displayButtonComponent.colors = colors;
    }

    public void OnLobbyDisplayButtonPress(LobbyInfoDisplay chosenLobby)
    {
        LobbyInfoDisplay newLobby = chosenLobby;

        if (newLobby.Equals(currentSelectedLobby))
        {
            return;
        }

        newLobby.select();
        setLobbyDisplayColor(newLobby);

        if (currentSelectedLobby != null)
        {
            currentSelectedLobby.deselect();
            setLobbyDisplayColor(currentSelectedLobby);
        }

        currentSelectedLobby = newLobby;
    }

    public void setCurrentSelectingLobby(LobbyInfoDisplay lobbyInfo)
    {
        if (currentSelectedLobby)
        {
            currentSelectedLobby.GetComponent<Button>().interactable = false;
        }

        currentSelectedLobby = lobbyInfo;
    }

    public void OnServerDiscovered(DiscoveryResponse info)
    {
        LobbyInfo lobbyInfo = new LobbyInfo(info.uri, info.lobbyName, info.mapName, info.totalPlayer, info.maxPlayer);

        if (!discoveredLobby.ContainsKey(info.uri))
        {
            var lobby = spawnNewDiscoveredLobby(lobbyInfo);
            discoveredLobby.Add(info.uri, lobby);
            return;
        }

        discoveredLobby[info.uri].GetComponent<LobbyInfoDisplay>().setLobbyInfo(lobbyInfo);
    }

    private GameObject spawnNewDiscoveredLobby(LobbyInfo lobbyInfo)
    {
        GameObject spawnedDisplay = Instantiate(lobbyInfoDisplayPrefab, lobbyInfoDisplayContainer);

        LobbyInfoDisplay displayComponent = spawnedDisplay.GetComponent<LobbyInfoDisplay>();
        //Set info to display on UI
        displayComponent.setLobbyInfo(lobbyInfo);

        return spawnedDisplay;
    }

    public void connect()
    {
        if (currentSelectedLobby != null)
            NetworkManager.singleton.StartClient(currentSelectedLobby.GetComponent<LobbyInfoDisplay>().lobbyInfo.uri);
    }
}
