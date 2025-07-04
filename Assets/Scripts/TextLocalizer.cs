using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextLocalizer : MonoBehaviour
{
    [SerializeField]
    private langkey _key;

    private TextMeshProUGUI _text;

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        UpdateText();
        LocalizationStrings.OnLanguageChanged += UpdateText;

    }

    private void OnEnable()
    {
        UpdateText();
    }

    void Update()
    {
        
    }

    public void UpdateText()
    {
        if (_text != null)
        {
            _text.text = LocalizationStrings.Strings[_key];
        }
    }
}
