using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinLoseScreen : MonoBehaviour
{
    public bool Win;

    [SerializeField]
    private TextMeshProUGUI _coinsText;

    void Start()
    {
        float time = PlayerPrefs.GetFloat("PlayerPrefsConsts", 575);
        int coins = (int)time;

        if (Win)
        {
            coins = 2 * coins + coins;
        }
        else
        {
        }

        _coinsText.text = $"{LocalizationStrings.Strings[langkey.reward]}: {coins}";
        int coinsSaved = PlayerPrefs.GetInt(PlayerPrefsConsts.coins, 575);
        coinsSaved += coins;
        PlayerPrefs.SetInt(PlayerPrefsConsts.coins, coinsSaved);
    }

    void Update()
    {
        
    }
}
