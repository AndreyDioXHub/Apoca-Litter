using NewBotSystem;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BotConfig {
    private static readonly float BotMaxSpeed = 2.5f;//6.8f;

    [JsonIgnore]
    public string BotTheme => BotThemeName;
    [JsonIgnore]
    public int SkinIndex = 0;

    [JsonIgnore]
    public float XP => _xp * 10;
    [JsonIgnore]
    public float Speed => _speed * 0.1f * BotMaxSpeed;
    [JsonIgnore]
    public float Attack => _attack * 0.1f * 20;

    // Публичные свойства для получения сырых значений
    [JsonIgnore]
    public float RawXP => _xp;
    [JsonIgnore]
    public float RawSpeed => _speed;
    [JsonIgnore]
    public float RawAttack => _attack;

    #region JSON
    [JsonProperty("name")]
    public string Name;
    [JsonProperty("theme")]
    public string BotThemeName;
    [JsonProperty("link")]
    public string Addrlink;
    [JsonIgnore]
    public string Iconlink {
        get {
            if(BotTheme != BotManager.CUSTOM_BOT_THEME) {
                return $"{Addrlink}Icon";
            } else {
                return _icon_custom_bot;
            }
        }
        set {
            if(BotTheme == BotManager.CUSTOM_BOT_THEME) {
                _icon_custom_bot = value;
            } else {
                Debug.LogWarning("Set value for non custom bot not allowed");
            }
        }
    }
    [JsonProperty]
    private string _icon_custom_bot;

    //PRIVATE in JSON
    [SerializeField, JsonProperty("xp")]
    private float _xp { get; set; }
    [SerializeField, JsonProperty("speed")]
    private float _speed;
    [SerializeField, JsonProperty("attack")]
    private float _attack;

    #endregion

    public void ConfigureInternal(float xp, float attack, float speed) {
        _xp = xp;
        _attack = attack;
        _speed = speed;
    }

    public BotConfig Copy() {
        return new BotConfig {
            Name = this.Name,
            BotThemeName = this.BotThemeName,
            _xp = this._xp,
            _speed = this._speed,
            _attack = this._attack,
            SkinIndex = this.SkinIndex,
            Addrlink = this.Addrlink,
        };
    }
}
