using game.configuration;
using NewBotSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using UnityEngine.AddressableAssets;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;



public class InventoryScreen : MonoBehaviour {

    [Header("Templates")]
    [SerializeField]
    private GameObject _botButtonPrefab;
    [SerializeField]
    private GameObject _categoryButtonPrefab;

    [Header("Containers")]
    [SerializeField]
    private RectTransform _categoryRoot;
    [SerializeField]
    private RectTransform _listRoot;
    [SerializeField]
    private ToggleGroup _categoryGroup;
    [SerializeField]
    private ToggleGroup _listGroup;

    private Dictionary<string, List<BotConfig>> _botsByTheme;

    private string _currectCategory;

    private bool _initComplete = false;

    // Start is called before the first frame update
    async void Awake() {
        
    }

    private void OnEnable() {
        if(!_initComplete) {
            InitComp();
        }
    }

    private async void InitComp() {
        //Read categories and bot in category
        await ParceBotCollection();
        FillHeader();
        _initComplete = true;
    }

    private async Task ParceBotCollection() {
        BotCollection botcoll = DataLoader.ReadJson<BotCollection>("bot.json");
        await UniTask.Yield();
        _botsByTheme = new Dictionary<string, List<BotConfig>>();

        // Добавляем стандартных ботов
        foreach (var bot in botcoll.bots) {
            if (!_botsByTheme.ContainsKey(bot.BotThemeName)) {
                _botsByTheme[bot.BotThemeName] = new List<BotConfig>();
            }
            _botsByTheme[bot.BotThemeName].Add(bot);
        }

        // Добавляем пользовательских ботов
        const string CUSTOM_CATEGORY = "Custom";
        _botsByTheme[CUSTOM_CATEGORY] = new List<BotConfig>();
        var customBots = UI.BotEditor.BotList.GetSavedBots();
        foreach (var customBot in customBots) {
            customBot.Config.BotThemeName = BotManager.CUSTOM_BOT_THEME;
            _botsByTheme[CUSTOM_CATEGORY].Add(customBot.Config);
        }
        
    }


    private void FillHeader() {
        SafelyDestroyChilds(_categoryRoot);
        SafelyDestroyChilds(_listRoot);
        //Fill header
        foreach (KeyValuePair<string, List<BotConfig>> item in _botsByTheme) {
            SpawnHeader(item.Key);
        }

    }

    private void SpawnHeader(string key) {
        GameObject header = Instantiate(_categoryButtonPrefab, _categoryRoot, false);
        Toggle toggle = header.GetComponentInChildren<Toggle>();
        toggle.SetIsOnWithoutNotify(false);
        toggle.group = _categoryGroup;
        TMP_Text hText = header.GetComponentInChildren<TMP_Text>();
        if (hText != null) {
            hText.text = key;
        }
        string category = key;
        toggle.onValueChanged.AddListener(isChecked => { if (isChecked) ReloadContent(key); });
    }

    private void ReloadContent(string category) {
        Debug.Log(category);
        if (_currectCategory != category) {
            //Fill
            _currectCategory = category;
            if (_listRoot.childCount > 0) {
                Toggle[] toggles = _listRoot.GetComponentsInChildren<Toggle>();
                foreach (Toggle toggle in toggles) {
                    _listGroup.UnregisterToggle(toggle);
                }
            }
            SafelyDestroyChilds(_listRoot);
            var catList = _botsByTheme[category];
            foreach (var item in catList) {
                SpawnListItem(item);
            }
        }
    }

    private async void SpawnListItem(BotConfig item) {
        var itemGO = Instantiate<GameObject>(_botButtonPrefab, _listRoot, false);
        var toggle = itemGO.GetComponent<Toggle>();
        bool selected = item.Name == GameConfig.CurrentConfiguration.BotConfig.Name;
        toggle.SetIsOnWithoutNotify(selected);
        toggle.group = _listGroup;
        toggle.onValueChanged.AddListener(isChecked => { if (isChecked) SelectBot(item); });

        // Добавляем обработчик для контекстного меню
        LongTapPanel longTapPanel = itemGO.GetComponentInChildren<LongTapPanel>();
        if (longTapPanel != null) {
            longTapPanel.OnStartOpenContext += () => {
                EditableContextPanel.ActiveConfig = item;
            };
        }

        //Name
        TMP_Text nameField = itemGO.GetComponentInChildren<TMP_Text>();
        if (nameField != null) {
            nameField.text = item.Name;
        }

        // Загружаем и устанавливаем иконку
        Image iconImage = itemGO.GetComponentsInChildren<Image>()
            .FirstOrDefault(img => img.gameObject.name == "load_icon");

        if (iconImage != null) {
            if (item.BotThemeName == BotManager.CUSTOM_BOT_THEME) {
                // Загружаем иконку для пользовательского бота
                string fullPath = Path.Combine(Application.persistentDataPath, UI.BotEditor.EditorScreen.folderForBotsImages, item.Iconlink);
                if (File.Exists(fullPath)) {
                    byte[] fileData = await File.ReadAllBytesAsync(fullPath);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(fileData);
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    iconImage.sprite = sprite;
                    iconImage.color = Color.white;
                }
            } else if (!string.IsNullOrEmpty(item.Iconlink)) {
                // Проверяем, существует ли Addressable ресурс с указанным ключом
                var locations = await Addressables.LoadResourceLocationsAsync(item.Iconlink).Task;
                if (locations != null && locations.Count > 0) {
                    // Загружаем иконку из addressable
                    var iconHandle = Addressables.LoadAssetAsync<Sprite>(item.Iconlink);
                    var sprite = await iconHandle.Task;
                    if (sprite != null) {
                        iconImage.sprite = sprite;
                        iconImage.color = Color.white;
                    }
                    //Addressables.Release(iconHandle);
                } else {
                    Debug.LogWarning($"Addressable resource with key {item.Iconlink} not found.");
                }
            }
        }
    }

    private void SelectBot(BotConfig item) {
        //GameConfig.CurrentConfiguration.Theme = item.BotTheme;
        //GameConfig.CurrentConfiguration.BotName = item.Name;
        GameConfig.CurrentConfiguration.BotConfig = item;
    }

    public static void SafelyDestroyChilds(Transform target) {
        if (target != null) {
            List<GameObject> childs = new List<GameObject>();
            for (int i = 0; i < target.childCount; i++) {
                childs.Add(target.GetChild(i).gameObject);
            }
            foreach (var child in childs) {
                Destroy(child);
            }
        }
    }
}
