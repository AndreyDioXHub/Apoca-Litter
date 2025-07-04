using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditableRow : MonoBehaviour {
    public enum ValueType { String, Int, Delimiter }

    [SerializeField]
    private TMP_Text _text;
    [SerializeField]
    private TMP_Text _textValue;
    [SerializeField]
    private Button _buttonMin;
    [SerializeField]
    private Button _buttonPlus;

    [SerializeField]
    private ValueType _valueType;
    [SerializeField]
    public int MaxValue = 10;
    [SerializeField]
    private string[] _values;

    private int _currentValue = 1;

    public event Action<int> OnValueChanged;

    // Start is called before the first frame update
    void Start() {
        _buttonMin.onClick.AddListener(OnMinButtonClicked);
        _buttonPlus.onClick.AddListener(OnPlusButtonClicked);
        UpdateTextValue();
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnMinButtonClicked() {
        if (_valueType == ValueType.Int || _valueType == ValueType.Delimiter) {
            if (_currentValue > 1) {
                _currentValue--;
                UpdateTextValue();
                OnValueChanged?.Invoke(_currentValue);
            }
        } else if (_valueType == ValueType.String) {
            if (_currentValue > 0) {
                _currentValue--;
            } else {
                _currentValue = _values.Length - 1; // Круговой переход к последнему элементу
            }
            UpdateTextValue();
            OnValueChanged?.Invoke(_currentValue);
        }
    }

    private void OnPlusButtonClicked() {
        if (_valueType == ValueType.Int || _valueType == ValueType.Delimiter) {
            if (_currentValue < MaxValue) {
                _currentValue++;
                UpdateTextValue();
                OnValueChanged?.Invoke(_currentValue);
            }
        } else if (_valueType == ValueType.String) {
            if (_currentValue < _values.Length - 1) {
                _currentValue++;
            } else {
                _currentValue = 0; // Круговой переход к первому элементу
            }
            UpdateTextValue();
            OnValueChanged?.Invoke(_currentValue);
        }
    }

    private void UpdateTextValue() {
        if (_valueType == ValueType.Int) {
            _textValue.text = _currentValue.ToString();
        } else if (_valueType == ValueType.String) {
            _textValue.text = _values[_currentValue];
        } else if (_valueType == ValueType.Delimiter) {
            _textValue.text = $"{_currentValue}/{MaxValue}";
        }
    }

    public void SetValue(int value) {
        _currentValue = value;
        UpdateTextValue();
    }
}
