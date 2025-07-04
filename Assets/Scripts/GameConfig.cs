using NewBotSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.configuration 
{
    public class GameConfig 
    {
        public static string Globalkey { get; set; } = "profile1";  // ключ для сохранения глобальных настроек
        public static string SessionKeyMissions { get; set; } = "dev";
        static GameConfig _current = null;
        public static GameConfig CurrentConfiguration 
        { 
            get 
            { 
                if(_current == null) 
                {
                    _current = new GameConfig();
                    _current.BotConfig = new BotConfig() {

                        Name = "Test",
                        BotThemeName = BotManager.CUSTOM_BOT_THEME,
                    };
                    _current.GameMode = GameModes.Sandbox;
                }

                return _current;
            } 
        }

        public string ServerName = "localhost";
        public string Server = "localhost";
        public string Port = "7777";

        public bool IsWrongVersion = false;

        public GameModes GameMode = GameModes.Sandbox;
        public bool IsSteam = true;
        private int _mapIndex = -1;
        public int MapIndex  
        {
            get 
            { 
                if(_mapIndex < 0) 
                {
                    _mapIndex = PlayerPrefs.GetInt(nameof(MapIndex), 0);
                }
                return _mapIndex;
            }
            set
            {
                _mapIndex = value;
                PlayerPrefs.SetInt(nameof(MapIndex), _mapIndex);
            }
        }

        public string MapName => CurrentScene == null ? "" : CurrentScene.Title;

        #region Player

        private string _playerName = string.Empty;
        public string PlayerName 
        {
            get 
            {
                if(string.IsNullOrEmpty(_playerName)) 
                {
                    _playerName = PlayerPrefs.GetString(nameof(PlayerName), "Player");
                }
                return _playerName;
            }
            set 
            {
                _playerName = value;
                PlayerPrefs.SetString(nameof(PlayerName), _playerName);
            }
        }         
        #endregion

        #region Scenes
        public SceneInfo CurrentScene { get; set; } = null;
        public ScenesConfiguration AllScenes { get; set; } = null;

        #endregion

        #region Missions
        private int _level = -1;
        public int Level {
            get {
                if (_level == -1) {
                    _level = PlayerPrefs.GetInt(SessionKeyMissions + nameof(Level), 0);
                }
                return _level;
            }
            set {
                _level = value;
                PlayerPrefs.SetInt(SessionKeyMissions+nameof(Level) , _level);
            }

        }
        public int KillsInLevel => CurrentScene == null ? 30 : CurrentScene.KillsRequired;
        #endregion

        #region Sandbox
        public BotConfig BotConfig { get; set; } = null;
        #endregion


        public enum GameModes 
        {
            Sandbox,
            Missions
        }

    }
}