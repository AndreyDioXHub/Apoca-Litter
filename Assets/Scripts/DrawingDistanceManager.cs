using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawingDistanceManager : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private TextMeshProUGUI _sliderValueText;

    [SerializeField]
    private float _sliderValue;

    void Start()
    {
        _sliderValue = PlayerPrefs.GetFloat(PlayerPrefsConsts.drawingdistance, 0.7f);
        _slider.value = _sliderValue;
        _slider.onValueChanged.AddListener(UpdateValue);
        EventsBus.Instance?.OnDrawingDistanceChanged?.Invoke(_sliderValue);
    }

    public void UpdateValue(float value)
    {
        _sliderValue = value;
        PlayerPrefs.SetFloat(PlayerPrefsConsts.drawingdistance, _sliderValue);
        EventsBus.Instance?.OnDrawingDistanceChanged?.Invoke(_sliderValue);
        //_slider.value = _sliderValue;
    }
}
