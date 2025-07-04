using Cysharp.Threading.Tasks;
using game.configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ServerInfoText : MonoBehaviour
{
    public string server;
    public string online;

    public LocalizeStringEvent localizeString;

    void Start()
    {
        localizeString.RefreshString();
        LateStart();
    }

    public async void LateStart()
    {
        await UniTask.WaitForSeconds(5);

        string server = $"{GameConfig.CurrentConfiguration.ServerName} : " +
            $"{GameConfig.CurrentConfiguration.Server} : " +
            $"{GameConfig.CurrentConfiguration.Port}";
        this.server = server;

        int online = WaitingScreenModel.Instance == null ? 0 : WaitingScreenModel.Instance.Players.Count;

        this.online = online.ToString();

        localizeString.RefreshString();
    }

    public void UpdateValue(int online)
    {
        //Debug.Log($"Update {online}");

        string server = $"{GameConfig.CurrentConfiguration.ServerName} : " +
            $"{GameConfig.CurrentConfiguration.Server} : " +
            $"{GameConfig.CurrentConfiguration.Port}";

        this.server = server;
        this.online = online.ToString();

        localizeString.RefreshString();
    }
}
