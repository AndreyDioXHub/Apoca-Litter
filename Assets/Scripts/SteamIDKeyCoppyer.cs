using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class SteamIDKeyCoppyer : MonoBehaviour
{
    public string SteamID;
    public Animator ShowAnimator;
    public LocalizeStringEvent localizeString;
    public Button CoppyButton;

    void Start()
    {
        CoppyButton.onClick.AddListener(CoppySteamID);
        //SteamID = $"{SteamLobby.Instance.CurrentLobbyID}";
        localizeString.RefreshString();
    }

    void Update()
    {
        
    }

    public void CoppySteamID()
    {
        GUIUtility.systemCopyBuffer = SteamID;
        ShowAnimator.SetTrigger("Trigger");
    }
}
