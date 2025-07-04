using NewBotSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableContextPanel : MonoBehaviour {
    public static BotConfig ActiveConfig { get; set; } = null;

    [SerializeField]
    private EditableRow _rowXP;
    [SerializeField]
    private EditableRow _rowSpeed;
    [SerializeField]
    private EditableRow _rowAggression;
    [SerializeField]
    private EditableRow _rowDamage;

    // Start is called before the first frame update
    void Start() {
        #region Test data
        ActiveConfig = new BotConfig() {

            Name = "Test",
            BotThemeName = BotManager.CUSTOM_BOT_THEME,
            Addrlink = "",
        };
        ActiveConfig.ConfigureInternal(10, 10, 10);
        #endregion
        _rowXP.OnValueChanged += OnXPChanged;
        _rowSpeed.OnValueChanged += OnSpeedChanged;
        _rowAggression.OnValueChanged += OnAggressionChanged;
        _rowDamage.OnValueChanged += OnDamageChanged;
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnEnable() {
        UpdateValues();
    }

    private void UpdateValues() {
        if (ActiveConfig != null) {
            _rowXP.SetValue((int)ActiveConfig.RawXP);
            _rowSpeed.SetValue((int)ActiveConfig.RawSpeed);
            _rowDamage.SetValue((int)ActiveConfig.RawAttack);
        }
    }

    private void OnXPChanged(int newValue) {
        if (ActiveConfig != null) {
            ActiveConfig.ConfigureInternal(newValue, ActiveConfig.RawAttack, ActiveConfig.RawSpeed);
        }
    }

    private void OnSpeedChanged(int newValue) {
        if (ActiveConfig != null) {
            ActiveConfig.ConfigureInternal(ActiveConfig.RawXP, ActiveConfig.RawAttack, newValue);
        }
    }

    private void OnAggressionChanged(int newValue) {
        if (ActiveConfig != null) {
        }
    }

    private void OnDamageChanged(int newValue) {
        if (ActiveConfig != null) {
            ActiveConfig.ConfigureInternal(ActiveConfig.RawXP, newValue, ActiveConfig.RawSpeed);
        }
    }
}
