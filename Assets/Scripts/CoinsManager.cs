using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CoinsManager : MonoBehaviour
{

    public static CoinsManager Instance;

    public UnityEvent OnBuyFailure = new UnityEvent();
    public UnityEvent OnBuySuccess = new UnityEvent();
    public UnityEvent OnCoinsCountChanged = new UnityEvent();
    public int Coins => _coins;

    [SerializeField]
    private int _coins;

    /*
    [SerializeField]
    private TextMeshProUGUI _coinsText;*/

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _coins = PlayerPrefs.GetInt(PlayerPrefsConsts.coins, 0);

        /*
        if(_coinsText != null)
        {
            _coinsText.text = $"{_coins}";
            _coinsText.gameObject.SetActive(true);
        }*/
    }

    void Update()
    {
        /*
        if (_coinsText != null)
        {
            _coinsText.text = $"{_coins}";
        }*/
    }

    /*
    public void SetText(TextMeshProUGUI coinsText)
    {
        _coinsText = coinsText;
    }*/

    [ContextMenu("Add 500 Coins")]
    public void AddCoins()
    {
        AddCoins(500);
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        _coins = 0;
        PlayerPrefs.SetInt(PlayerPrefsConsts.coins, _coins);
        OnCoinsCountChanged?.Invoke();
        //PlayerPrefs.DeleteAll();
    }

    public void AddCoins(int coins)
    {
        _coins += coins;
        PlayerPrefs.SetInt(PlayerPrefsConsts.coins, _coins);
        OnCoinsCountChanged?.Invoke();
    }

    public bool TryBuy(int coins)
    {
        bool result = false;

        if (coins > _coins)
        {
            OnBuyFailure?.Invoke();
            result = false;
        }
        else
        {
            result = true;
            _coins = _coins - coins;
            PlayerPrefs.SetInt(PlayerPrefsConsts.coins, _coins);
            OnBuySuccess?.Invoke();
            OnCoinsCountChanged?.Invoke();
        }

        return result;
    }
}
