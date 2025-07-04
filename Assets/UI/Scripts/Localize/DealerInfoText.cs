using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class DealerInfoText : MonoBehaviour
{
    public string Coins;
    public LocalizeStringEvent localizeString;

    void Start()
    {
        Coins = CoinsManager.Instance.Coins.ToString();
        localizeString.RefreshString();
        CoinsManager.Instance.OnCoinsCountChanged.AddListener(UpdateCoins);
    }

    void Update()
    {
        
    }

    public void UpdateCoins()
    {
        Coins = CoinsManager.Instance.Coins.ToString();
        localizeString.RefreshString();
    }
}
