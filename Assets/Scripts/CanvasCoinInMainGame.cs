using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasCoinInMainGame : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _coinsText;

    void Start()
    {
        _coinsText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {

        _coinsText.text = $"{CoinsManager.Instance.Coins.ToString()}";
    }
}
