using System;
using System.Collections.Generic;
using Gameplay;
using Map;
using Mirror;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private InputField _inputAddress;
    [SerializeField] private InputField _inputUsername;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _connectionMenu;
    [SerializeField] private TextMeshProUGUI _connectionMenuTitle;

    private int _playerCount;
    /*private String _playerUsername;
    private GameObject _grid;
    private GridScript _gridScript;
    private Tilemap _bombReferenceTilemap;*/
    
    public Dictionary<int, PlayerControl> players = new Dictionary<int, PlayerControl>();

    public void StartHostOnClick()
    {
        StartHost();
        //_playerUsername = _inputUsername.text == "" ? "Host" : _inputUsername.text;
        _mainMenu.SetActive(false);
    }

    public void ConnectToIP()
    {
        networkAddress = _inputAddress.text == "" ? "localhost" : _inputAddress.text;
        //_playerUsername = _inputUsername.text == "" ? "Bomberman" : _inputUsername.text;
        StartClient();
        _mainMenu.SetActive(false);
        _connectionMenu.SetActive(true);
        _connectionMenuTitle.text = "Connecting to: " + networkAddress;
    }

    public void CancelConnection()
    {
        StopClient();
        _mainMenu.SetActive(true);
        _connectionMenu.SetActive(false); 
    }
    
    public void UpdateStatus(TextMeshProUGUI textObj)
    {
        // host mode
        // display separately because this always confused people:
        //   Server: ...
        //   Client: ...
        if (NetworkServer.active && NetworkClient.active)
        {
            textObj.text = ($"<b>Host</b>: running via {Transport.activeTransport}");
        }
        // server only
        else if (NetworkServer.active)
        {
            textObj.text = ($"<b>Server</b>: running via {Transport.activeTransport}");
        }
        // client only
        else if (NetworkClient.isConnected)
        {
            textObj.text = ($"<b>Client</b>: connected to {networkAddress} via {Transport.activeTransport}");
        }
    }
    
    
    public void HandleStopButtons()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            StopServer();
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //var startPosition = GetStartPosition();
        //var player = Instantiate(_whitePlayerPrefab, startPosition);
        //player.name = $"{_playerUsername} [connId={conn.connectionId}]" ;
        var startPos = GetStartPosition();
        var player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
        _playerCount++;
        players.Add(_playerCount, player.GetComponent<PlayerControl>());
        //print("SERVER: player grid instance" + playerScript.grid);
    }


    public void OnCreateFireCommand(GameObject fireInstance)
    {
        NetworkServer.Spawn(fireInstance);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        _playerCount++;
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        _playerCount--;
    }
}
