using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class RefreshServerListTimer : MonoBehaviour
{
    public string Value;
    public LocalizeStringEvent localizeString;

    [SerializeField]
    private GameObject _refreshText;
    [SerializeField]
    private GameObject _timerText;
    [SerializeField]
    private Button _refreshButton;


    public void UpdateValue(int time)
    {
        if (Value != null)
        {
            string timeValue = time < 10 ? $"0{time}" : $"{time}";
            Value = timeValue;
        }

        if (_refreshButton != null)
        {
            _refreshButton.interactable = time == 0;
        }

        if (_refreshText != null)
        {
            _refreshText.SetActive(time == 0);
        }

        if (_timerText != null)
        {
            _timerText.SetActive(time != 0);
        }

        if (localizeString != null)
        {
            localizeString.RefreshString();
        }
    }
}
