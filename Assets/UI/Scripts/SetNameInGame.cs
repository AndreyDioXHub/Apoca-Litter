using Cysharp.Threading.Tasks;
using game.configuration;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetNameInGame : MonoBehaviour
{/*
    [SerializeField]
    private TextMeshProUGUI _nameText;*/
    [SerializeField]
    private TMP_InputField _nameInputField;

    void Start()
    {
        WaitSteamManager();
    }

    public async void WaitSteamManager()
    {
        await UniTask.Yield();
        /*
        while (!SteamManager.Initialized) 
        {
            await UniTask.Yield();
        }*/
        /*
        string playerame = $"";
        _nameInputField.text = playerame;
        GameConfig.CurrentConfiguration.PlayerName = playerame;*/

        string playerame = GameConfig.CurrentConfiguration.PlayerName;
        _nameInputField.text = playerame;


    }

    void Update()
    {
        
    }

    public void SetName(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            name = "Player";
            _nameInputField.text = name;
        }

        GameConfig.CurrentConfiguration.PlayerName = name;

        if(EventsBus.Instance!= null)
        {
            EventsBus.Instance.OnNameUpdated?.Invoke(name);
        }
    }
}
