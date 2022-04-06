using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;

public class EscapistNetworkDiscoveryHUD : MonoBehaviour
{
    readonly Dictionary<long, DiscoveryResponse> discoveredServers = new Dictionary<long, DiscoveryResponse>();
    Vector2 scrollViewPos = Vector2.zero;

    public EscapistNetworkDiscovery networkDiscovery;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (networkDiscovery == null)
        {
            networkDiscovery = GetComponent<EscapistNetworkDiscovery>();
            UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
        }
    }
#endif

    /*void OnGUI()
    {
        if (NetworkManager.singleton == null)
            return;

        if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
            DrawGUI();

        if (NetworkServer.active || NetworkClient.active)
            StopButtons();
    }

    void DrawGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 500));
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Find Servers"))
        {
            discoveredServers.Clear();
            networkDiscovery.StartDiscovery();
        }

        // LAN Host
        if (GUILayout.Button("Start Host"))
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartHost();
            networkDiscovery.AdvertiseServer();
        }

        // Dedicated server
        if (GUILayout.Button("Start Server"))
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartServer();

            networkDiscovery.AdvertiseServer();
        }

        GUILayout.EndHorizontal();

        // show list of found server

        GUILayout.Label($"Discovered Servers [{discoveredServers.Count}]:");

        // servers
        scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);

        foreach (DiscoveryResponse info in discoveredServers.Values)
        {
            if (GUILayout.Button(info.EndPoint.Address.ToString()))
                Connect(info);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }*/

    void StopButtons()
    {
        GUILayout.BeginArea(new Rect(10, 40, 100, 25));

        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Host"))
            {
                NetworkManager.singleton.StopHost();
            }
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Client"))
            {
                NetworkManager.singleton.StopClient();
            }
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            if (GUILayout.Button("Stop Server"))
            {
                NetworkManager.singleton.StopServer();
            }
        }

        GUILayout.EndArea();
    }

    void Connect(DiscoveryResponse info)
    {
        NetworkManager.singleton.StartClient(info.uri);
    }

    public void OnDiscoveredServer(DiscoveryResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;
    }
}
