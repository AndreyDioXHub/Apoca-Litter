using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class AtantionTextView : MonoBehaviour
{
    public string WaveIndex = "1";
    public string WaveTime = "10";

    public LocalizeStringEvent localizeString;

    void Start()
    {
        gameObject.SetActive(false);
        //WaitingScreenModel.Instance.OnGameReady.AddListener(() => gameObject.SetActive(true));        
    }

    public void UpdateTimeText(string text)
    {
        WaveTime = text;
        localizeString.RefreshString();
    } 

    void Update()
    {
        /*
        WaveIndex = (BotManagerNetwork.Instance.WaveIndex + 1).ToString();
        WaveTime = BotManagerNetwork.Instance.TimeString;*/

    }
}
