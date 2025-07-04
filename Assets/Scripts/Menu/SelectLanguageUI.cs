using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SelectLanguageUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _screen;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OpenScreen()
    {
        _screen.SetActive(true);
    }

    public void CloseScreen()
    {
        _screen.SetActive(false);
    }

    public void SetLanguage(string languageCode)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;

        for (int i = 0; i < locales.Count; i++)
        {
            if (locales[i].Identifier.Code == languageCode)
            {
                LocalizationSettings.SelectedLocale = locales[i];
                return;
            }
        }

        Debug.LogError($"язык с кодом '{languageCode}' не найден!");
    }

}
