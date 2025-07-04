using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectController : NetworkBehaviour
{
    [SyncVar]
    public int ConnectionID;
    [SyncVar]
    public int PlayerIDNumber;
    [SyncVar]
    public ulong PlayerSteamID;
    [SyncVar(hook =nameof(PlayerNameUpdate))]
    public string PlayerName;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    /*
    public void Init(ControlSteamNetworkManager manager, int connectionID, int playerIDNumber, ulong playerSteamID)
    {
        _manager = manager;
        ConnectionID = connectionID;
        PlayerIDNumber = playerIDNumber;
        PlayerSteamID = playerSteamID;
    }*/

    public void PlayerNameUpdate(string oldValue, string newValue)
    {

    }
}
