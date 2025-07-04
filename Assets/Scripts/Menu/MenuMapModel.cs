using game.configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;

public class MenuMapModel : MonoBehaviour
{
    [SerializeField]
    private List<MenuSelectMap> _maps = new List<MenuSelectMap>();

    [SerializeField]
    private TMP_InputField _keyInputField;

    void Start()
    {
    }

    private void OnEnable()
    {
        var maps = GetComponentsInChildren<MenuSelectMap>();

        for (int i = 0; i < maps.Length; i++)
        {
            maps[i].Init(i, this);
            _maps.Add(maps[i]);
        }

        SelectIndex(GameConfig.CurrentConfiguration.MapIndex);
    }

    void Update()
    {

    }

    public void SelectIndex(int index)
    {
        foreach (var map in _maps)
        {
            map.SetActiveStatus(false);
        }

        _maps[index].SetActiveStatus(true);

        GameConfig.CurrentConfiguration.MapIndex = index;
    }

    public void CreateServer()
    {
        if (GameConfig.CurrentConfiguration.IsSteam)
        {
            //SteamLobby.Instance.HostLobby();
        }
        else
        {
            ControlNetworkManager.singleton.StartHost();
        }
    }

    public void CLientServer()
    {
        if (GameConfig.CurrentConfiguration.IsSteam)
        {
            ulong steamID = ulong.Parse(_keyInputField.text);
            //SteamLobby.Instance.JoinLobby(steamID);
            //ControlSteamNetworkManager.singleton.StartClient();
        }
        else
        {
            ControlNetworkManager.singleton.StartClient();
        }
    }
}
