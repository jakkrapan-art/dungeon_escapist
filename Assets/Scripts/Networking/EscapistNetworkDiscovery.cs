using System;
using System.Net;
using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.Events;

/*
    Documentation: https://mirror-networking.gitbook.io/docs/components/network-discovery
    API Reference: https://mirror-networking.com/docs/api/Mirror.Discovery.NetworkDiscovery.html
*/

public class DiscoveryRequest : NetworkMessage
{
    // Add properties for whatever information you want sent by clients
    // in their broadcast messages that servers will consume.
}

public struct DiscoveryResponse : NetworkMessage
{
    public IPEndPoint EndPoint { get; set; }

    public string lobbyName;
    public string mapName;

    public int totalPlayer;
    public Uri uri;
    public long serverId;

    public int maxPlayer;

    // Add properties for whatever information you want the server to return to
    // clients for them to display or consume for establishing a connection.
}

[Serializable]
public class ServerFoundUnityEvent : UnityEvent<DiscoveryResponse> { };

public class EscapistNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    #region Server

    public long ServerId { get; private set; }

    public Transport transport;

    public ServerFoundUnityEvent OnServerFound;

    public override void Start()
    {
        ServerId = RandomLong();

        // active transport gets initialized in awake
        // so make sure we set it here in Start()  (after awakes)
        // Or just let the user assign it in the inspector
        if (transport == null)
            transport = Transport.activeTransport;

        base.Start();
    }

    /// <summary>
    /// Reply to the client to inform it of this server
    /// </summary>
    /// <remarks>
    /// Override if you wish to ignore server requests based on
    /// custom criteria such as language, full server game mode or difficulty
    /// </remarks>
    /// <param name="request">Request coming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>

    protected override void ProcessClientRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        base.ProcessClientRequest(request, endpoint);
    }

    /// <summary>
    /// Process the request from a client
    /// </summary>
    /// <remarks>
    /// Override if you wish to provide more information to the clients
    /// such as the name of the host player
    /// </remarks>
    /// <param name="request">Request coming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    /// <returns>A message containing information about this server</returns>
    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        var networkManager = EscapistNetworkManager.singleton.gameObject.GetComponent<EscapistNetworkManager>();
        return new DiscoveryResponse
        {
            totalPlayer = NetworkServer.connections.Count,
            maxPlayer = networkManager.maxConnections,
            lobbyName = networkManager.lobbyName,
            mapName = networkManager.mapName,
            uri = transport.ServerUri(),
            serverId = ServerId
        };
    }

    #endregion

    #region Client

    /// <summary>
    /// Create a message that will be broadcasted on the network to discover servers
    /// </summary>
    /// <remarks>
    /// Override if you wish to include additional data in the discovery message
    /// such as desired game mode, language, difficulty, etc... </remarks>
    /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
    protected override DiscoveryRequest GetRequest()
    {
        return new DiscoveryRequest();
    }

    /// <summary>
    /// Process the answer from a server
    /// </summary>
    /// <remarks>
    /// A client receives a reply from a server, this method processes the
    /// reply and raises an event
    /// </remarks>
    /// <param name="response">Response that came from the server</param>
    /// <param name="endpoint">Address of the server that replied</param>
    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        response.EndPoint = endpoint;

        UriBuilder realUri = new UriBuilder(response.uri)
        {
            Host = response.EndPoint.Address.ToString()
        };

        response.uri = realUri.Uri;

        OnServerFound.Invoke(response);
    }

    #endregion
}
