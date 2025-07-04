using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Utils;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using Cysharp.Threading.Tasks;
using game.configuration;
using NewBotSystem;

public class MainMenuUi : MonoBehaviour {
    
    [SerializeField]
    private Toggle _sandboxToggle;
    [SerializeField]
    private Toggle _missionsToggle;

    private ScenesConfiguration _scenesConfig;

    [SerializeField]
    private RectTransform _listContainer;
    [SerializeField]
    private GameObject _sceneCardPrefab;

    [Header("Editor")]
    [SerializeField]
    private Button _editorButton;
    [SerializeField]
    private string _editorStringName = "Editor";
    [Header("Editor")]
    [SerializeField]
    private Button _settingsButton;
    [SerializeField]
    private string _settingsStringName = "Settings";

    [SerializeField]
    private GameObject _canvasCrossLoader;

    private const string _lockText = "Open after level {0}";

    [SerializeField]
    private string hashSceneValue;

    private const string _scenesConfigFile = "scenes.json";

    private void Awake() {
        AddressableSceneManager.CanvasCrossLoader = _canvasCrossLoader;
    }

    async void Start() {
        //preload bot.json
        DataLoader.LoadJsonFile<BotCollection>("bot.json").Forget();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ConfigureButtons();

        if (_scenesConfig == null) {
            // Загружаем конфигурацию сцен
            _scenesConfig = await DataLoader.LoadJsonFile<ScenesConfiguration>(_scenesConfigFile);

            string missions = JsonConvert.SerializeObject(_scenesConfig.MissionScenes);
            hashSceneValue = DataLoader.GetStringHash(missions);
            if (!string.IsNullOrEmpty(hashSceneValue)) {
                GameConfig.SessionKeyMissions = hashSceneValue;
            }

            GameConfig.CurrentConfiguration.AllScenes = _scenesConfig;
        }
        var addrInst = AddressableSceneManager.Instance;
        var config = GameConfig.CurrentConfiguration;

        #region Toggles
        _sandboxToggle.isOn = config.GameMode == game.configuration.GameConfig.GameModes.Sandbox;
        _missionsToggle.isOn = !_sandboxToggle.isOn;

        _sandboxToggle.onValueChanged.AddListener(isOn => {
            if (isOn) {
                config.GameMode = game.configuration.GameConfig.GameModes.Sandbox;
                UpdateScenesList(_scenesConfig.SandboxScenes);
            }
        });

        _missionsToggle.onValueChanged.AddListener(isOn => {
            if (isOn) {
                config.GameMode = game.configuration.GameConfig.GameModes.Missions;
                UpdateScenesList(_scenesConfig.MissionScenes);
            }
        });

        // Инициализируем список сцен в зависимости от текущего режима
        if (config.GameMode == GameConfig.GameModes.Sandbox) {
            _sandboxToggle.SetIsOnWithoutNotify(true);
            UpdateScenesList(_scenesConfig.SandboxScenes);
        } else {
            _missionsToggle.SetIsOnWithoutNotify(true);
            UpdateScenesList(_scenesConfig.MissionScenes);
        }
        #endregion
    }

    private void ConfigureButtons() {
        _editorButton.onClick.AddListener(() => {
            MainMenuPanelManager.Instance.SwitchPanel(_editorStringName);
        });
        _settingsButton.onClick.AddListener(() => {
            MainMenuPanelManager.Instance.SwitchPanel(_settingsStringName);
        });
    }

    private void UpdateScenesList(List<SceneInfo> scenes) {
        InventoryScreen.SafelyDestroyChilds(_listContainer);
        if (scenes == null || scenes.Count == 0) {
            Debug.LogWarning("No scenes found");
            return;
        }
        // Создаем карточки для каждой сцены
        int i = 0;
        foreach (var scene in scenes) {
            scene.LevelIndex = i++;
            CreateSceneCard(scene);
        }
    }

    private void CreateSceneCard(SceneInfo sceneInfo) {

        GameObject card = Instantiate(_sceneCardPrefab, _listContainer, false);

        // Настраиваем текст
        TMP_Text titleLabel = card.GetComponentsInChildren<TMP_Text>()
            .FirstOrDefault(txt => txt.gameObject.name == "SceneName");
        if (titleLabel != null) {
            titleLabel.text = sceneInfo.Title;
        }

        if (GameConfig.CurrentConfiguration.GameMode == GameConfig.GameModes.Missions) {
            if (GameConfig.CurrentConfiguration.Level < sceneInfo.LevelIndex) {
                //Set current disable state
                GameObject lockObject = card.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(tr => tr.gameObject.name == "BlurOverlay").gameObject;
                if (lockObject != null) {
                    lockObject.SetActive(true);
                    TMP_Text lockText = lockObject.GetComponentInChildren<TMP_Text>();
                    if (lockText != null) {
                        lockText.text = string.Format(_lockText, sceneInfo.LevelIndex);
                    }
                }
            }
        }

        // Находим компонент Image для иконки
        Image sceneIcon = card.GetComponentsInChildren<Image>()
            .FirstOrDefault(img => img.gameObject.name == "SceneIcon");

        if (sceneIcon != null) {
            // Загружаем иконку из Addressables
            LoadSceneIcon(sceneIcon, sceneInfo.PreviewIcon).Forget();
        }

        // Настраиваем кнопку
        Button button = card.GetComponentInChildren<Button>();
        if (button != null) {
            button.onClick.AddListener(() => LoadScene(sceneInfo).Forget());
        }

    }

    private async UniTaskVoid LoadSceneIcon(Image iconImage, string iconPath) {
        try {
            var handle = Addressables.LoadAssetAsync<Sprite>(iconPath);
            await UniTask.WaitUntil(() => handle.IsDone);

            if (this == null || iconImage == null) return; // проверка на уничтожение объекта

            var sprite = handle.Result;
            if (sprite != null) {
                iconImage.sprite = sprite;
            }
        }
        catch (Exception e) {
            Debug.LogError($"Ошибка загрузки иконки сцены {iconPath}: {e.Message}");
        }
    }

    private async UniTaskVoid LoadScene(SceneInfo sceneInfo) {
        GameConfig.CurrentConfiguration.CurrentScene = sceneInfo;
        var success = await AddressableSceneManager.Instance.LoadScene(sceneInfo.SceneName);
        if (!success) {
            GameConfig.CurrentConfiguration.CurrentScene = null;
        }
    }

    void Update() {

    }
}

[Serializable]
public class SceneInfo {
    [JsonProperty("scene_name")]
    public string SceneName { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("preview_icon")]
    public string PreviewIcon { get; set; }

    [JsonIgnore]
    public bool IsLocked { get; set; } = false;

    [JsonProperty("is_locked")]
    public bool IsLockedPurchase { get; set; } = false;

    [JsonProperty("kills_required")]
    public int KillsRequired { get; set; } = 30;
    
    [JsonIgnore]
    public int LevelIndex { get; set; } = 0;

    [JsonProperty("bot_attack_distance")]
    public float BotAttackDistance { get; set; } = 5;

    [JsonProperty("jetpack_fuel_time")]
    public float JetPackFuelTick { get; set; } = 2;

    [JsonProperty("kill_bots_on_start")]
    public bool KillAllBotsOnStart { get; set; } = false;


}

[Serializable]
public class ScenesConfiguration {
    [JsonProperty("sandbox_scenes")]
    public List<SceneInfo> SandboxScenes { get; set; }

    [JsonProperty("mission_scenes")]
    public List<SceneInfo> MissionScenes { get; set; }
}
