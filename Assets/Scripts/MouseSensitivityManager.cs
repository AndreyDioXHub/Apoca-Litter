using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MouseSensitivityManager : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private TextMeshProUGUI _sliderValueText;

    [SerializeField]
    private float _sliderValue;

    void Start()
    {
        //Debug.Log("PlayerPrefs Init MouseSensitivityManager");
        _sliderValue = PlayerPrefs.GetFloat(PlayerPrefsConsts.sensitivity, 1);
        _slider.value = _sliderValue;
        _slider.onValueChanged.AddListener(UpdateValue);
        if (_sliderValueText != null)
        {
            _sliderValueText.text = $"{(int)(_sliderValue * 600)}";
        }
        EventsBus.Instance?.OnMouseSensitivityChanged?.Invoke(_sliderValue);
    }

    public void Init()
    {

    }

    public void UpdateValue(float value)
    {
        _sliderValue = value;
        PlayerPrefs.SetFloat(PlayerPrefsConsts.sensitivity, _sliderValue); 
        if (_sliderValueText != null)
        {
            _sliderValueText.text = $"{(int)(_sliderValue * 600)}";
        }
        EventsBus.Instance?.OnMouseSensitivityChanged?.Invoke(_sliderValue);
        //_slider.value = _sliderValue;
    }

}
