using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyInfo
{
    public System.Uri uri { get; private set; }
    public string lobbyName { get; private set; }
    public string mapName { get; private set; }
    public int totalPlayer { get; private set; }
    public int maxPlayer { get; private set; }

    public LobbyInfo(System.Uri _uri, string _lobbyName, string _mapName, int _totalPlayer, int _maxPlayer)
    {
        uri = _uri;
        lobbyName = _lobbyName;
        mapName = _mapName;
        totalPlayer = _totalPlayer;
        maxPlayer = _maxPlayer;
    }
}
