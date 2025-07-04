using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.Events;
using Mirror.Examples.Chat;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

public class ControlNetworkManager : NetworkManager
{
    public static new ControlNetworkManager singleton => (ControlNetworkManager)NetworkManager.singleton;

    public int CurrentPlayersCount { get; private set; }

    public UnityEvent OnClientDisconected = new UnityEvent();
    public UnityEvent<uint> OnRemovePlayer = new UnityEvent<uint>();
    public UnityEvent<int> ServerPlayerCountChanged = new UnityEvent<int>();

    private Dictionary<uint, NetworkConnectionToClient> connectedPlayers = new Dictionary<uint, NetworkConnectionToClient>();

    public override void ConfigureHeadlessFrameRate()
    {
        base.ConfigureHeadlessFrameRate();
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }
    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }
    public override void OnServerChangeScene(string newSceneName) { }
    public override void OnServerSceneChanged(string sceneName) { }
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) { }
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        LogConnection(conn.address);
    }

    private void LogConnection(string address)
    {
        //TODO Save in log
        Debug.Log($"New Connection: {address}");
    }
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        // Получаем идентификатор пользователя и его соединение
        uint netId = conn.identity.netId;
        NetworkConnectionToClient connection = conn;
        Debug.Log($"Server Add player {conn.identity.netId}");
        // Сохраняем подключенного пользователя в словаре
        SaveConnectedPlayer(netId, connection);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        // Удаляем подключенного игрока из словаря при отключении
        if (conn != null && conn.identity != null)
        {
            uint netId = conn.identity.netId;
            Debug.Log($"Server Disconnect {netId}");

            if (connectedPlayers.ContainsKey(netId))
            {
                connectedPlayers.Remove(netId);
                OnRemovePlayer?.Invoke(netId);
            }
        }
        else
        {
            Debug.Log($"Server Disconnect but have not identity");

            uint netIdForRemove = 0;
            bool mayRemove = false;

            foreach (var conectionPair in connectedPlayers)
            {
                if (conectionPair.Value.identity == null)
                {
                    Debug.Log($"{conectionPair.Key} have null conection");
                    netIdForRemove = conectionPair.Key;
                    mayRemove = true;
                }                
            }

            if(mayRemove)
            {
                connectedPlayers.Remove(netIdForRemove);
                OnRemovePlayer?.Invoke(netIdForRemove);
            }
        }

        CurrentPlayersCount = connectedPlayers.Count;
        ServerPlayerCountChanged?.Invoke(CurrentPlayersCount);
    }

    public override void OnServerError(NetworkConnectionToClient conn, TransportError transportError, string message) 
    {
        Debug.Log($"OnServerError {message}");
    }

    public override void OnClientError(TransportError transportError, string message)
    {
        Debug.Log($"OnClientError {message}");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }
    public override void OnClientDisconnect() 
    {
        Debug.Log("OnClientDisconnect");
        OnClientDisconected?.Invoke();
    }

    public override void OnClientNotReady() { }


    public override void OnStartHost() { }
    public override void OnStartServer() { }
    public override void OnStartClient() { }
    public override void OnStopHost() { }
    public override void OnStopServer() { }
    public override void OnStopClient() { }
    /*
    public PlayerTeam GetPlayerTeam(uint key)
    {
        NetworkConnectionToClient connection = GetConnectionByNetId(key);
        if (connection != null)
        {
            return connection.identity.GetComponent<PlayerNetworkResolver2>().Team;
        }
        return PlayerTeam.empty;
    }*/


    // Метод для сохранения подключенных игроков в словарь
    private void SaveConnectedPlayer(uint netId, NetworkConnectionToClient connection)
    {
        connectedPlayers[netId] = connection;
        CurrentPlayersCount = connectedPlayers.Count;
        ServerPlayerCountChanged?.Invoke(CurrentPlayersCount);
    }

    // Публичный метод для получения объекта NetworkConnectionToClient по ключу netId
    public NetworkConnectionToClient GetConnectionByNetId(uint netId)
    {
        if (connectedPlayers.ContainsKey(netId))
        {
            return connectedPlayers[netId];
        }
        else
        {
            return null; // или выбросить исключение, в зависимости от логики вашего приложения
        }
    }
}

