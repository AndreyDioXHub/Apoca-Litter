using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyView : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    public bool AvatarReceived;

    public TextMeshProUGUI PlayerNameText;
    public RawImage PlayerIcon;



    void Start()
    {
    }


    private void GetPlayerIcon()
    {
    }

    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
        if(AvatarReceived)
        {
            GetPlayerIcon();
        }
    }

    void Update()
    {
        
    }


}
