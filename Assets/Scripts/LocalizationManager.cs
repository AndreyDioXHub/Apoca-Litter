using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    [SerializeField]
    private string _local = "ru";
    /*
    [SerializeField]
    private TextLocalizer[] _texts;*/

    void Start()
    {
        //_texts = Resources.FindObjectsOfTypeAll<TextLocalizer>();

        if (LocalizationStrings.first_start)// && _sdk != null)
        {
            //_sdk._RequestingEnvironmentData();
            InitLanguage();
        }
        else
        {
            _local = PlayerPrefs.GetString(PlayerPrefsConsts.local, "ru");
            LocalizationStrings.SetLanguage(_local);
        }
    }

    void Update()
    {

    }

    public void InitLanguage()
    {
        string lang = "ru";// YandexGame.EnvironmentData.language;

        if (lang.Equals("ru") || lang.Equals("be") || lang.Equals("kk") || lang.Equals("uk") || lang.Equals("uz"))
        {
            _local = "ru";
        }
        else
        {
            _local = "en";
        }

        switch (_local)
        {
            case "ru":
                break;
            case "en":
                break;
            default:
                break;
        }

        PlayerPrefs.SetString(PlayerPrefsConsts.local, _local);
        LocalizationStrings.SetLanguage(_local);
        /*
        foreach (var t in _texts)
        {
            t.UpdateText();
        }*/

        LocalizationStrings.first_start = false;
    }

    public void ChangeLanguage()
    {
        switch (_local)
        {
            case "ru":
                _local = "en";
                break;
            case "en":
                _local = "ru";
                break;
            default:
                _local = "ru";
                break;
        }

        PlayerPrefs.SetString(PlayerPrefsConsts.local, _local);

        LocalizationStrings.SetLanguage(_local);
        /*
        foreach (var t in _texts)
        {
            t.UpdateText();
        }*/
    }
}
