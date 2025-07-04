using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UI.BotEditor.EditorScreen;
using System.Linq;
using System.IO;

namespace UI.BotEditor {

    [RequireComponent(typeof(ToggleGroup))]
    public class BotList : MonoBehaviour {

        internal static readonly string KEY_CUSTOM_BOT_SAVES = "custombots";

        public UnityEvent<CustomBot> OnBotSelected;
        public UnityEvent<CustomBot> OnBotContext;

        [SerializeField]
        private RectTransform _listContainer;
        [SerializeField]
        private GameObject _galleryItemPrefab;

        private List<CustomBot> _bots;

        private ToggleGroup _toggleGroup;
        // Start is called before the first frame update
        void Awake() {
            _toggleGroup = GetComponent<ToggleGroup>();
            _toggleGroup.allowSwitchOff = true;
            InventoryScreen.SafelyDestroyChilds(_listContainer);
        }

        private void OnEnable() {
            if (_toggleGroup != null) {
                _toggleGroup.allowSwitchOff = true;
                LoadBots();
                if (_bots.Count > 0) {
                    BuildBotList();
                }
            }
        }

        private void BuildBotList() {
            InventoryScreen.SafelyDestroyChilds(_listContainer);
            //Spawn bot icons
            foreach (var item in _bots) {
                CreateItemView(item);
            }
            LoadIcons();

        }

        private void CreateItemView(CustomBot item) {
            GameObject icon = Instantiate(_galleryItemPrefab, _listContainer, false);
            TMP_Text labelName = icon.GetComponentInChildren<TMP_Text>();
            labelName.text = item.Name;

            // Получаем все компоненты Image и находим нужный по имени объекта
            Image botIcon = icon.GetComponentsInChildren<Image>()
                .FirstOrDefault(img => img.gameObject.name == "BotIcon");
            if (botIcon == null) {
                Debug.LogWarning($"Не удалось найти Image с именем 'BotIcon' в префабе {_galleryItemPrefab.name}");
            }
            item.SetImageContainer(botIcon);
            Toggle toggle = icon.GetComponentInChildren<Toggle>();
            if (toggle != null) {
                toggle.SetIsOnWithoutNotify(false);
                toggle.group = _toggleGroup;
                toggle.onValueChanged.AddListener(isChecked => { if (isChecked) SelectBot(item); });
            }

            // Подписываемся на событие OnStartOpenContext
            LongTapPanel longTapPanel = icon.GetComponentInChildren<LongTapPanel>();
            if (longTapPanel != null) {
                longTapPanel.OnStartOpenContext += () => OnBotContext?.Invoke(item);
            }
        }

        private void SelectBot(CustomBot item) {
            _toggleGroup.allowSwitchOff = false;
            OnBotSelected?.Invoke(item);
        }

        private async void LoadIcons() {
            foreach (var item in _bots) {
                Texture2D tex = await item.GetBotTexture();

            }
        }

        private async void LoadIcon(CustomBot item) {
            await item.GetBotTexture();
        }

        private void OnDisable() {
            SaveBots();
        }

        #region Bots Load/Save methods

        public void LoadBots() {
            string json = PlayerPrefs.GetString(KEY_CUSTOM_BOT_SAVES, "[]");
            _bots = JsonConvert.DeserializeObject<List<CustomBot>>(json);
        }

        public void SaveBots() {
            string json = JsonConvert.SerializeObject(_bots);
            PlayerPrefs.SetString(KEY_CUSTOM_BOT_SAVES, json);
            PlayerPrefs.Save();

#if UNITY_EDITOR
            File.WriteAllText(Path.Combine(Application.dataPath, "custom_bots.json"), json);
#endif
        }

        public void AddBot(CustomBot bot) {
            _bots.Add(bot);
            CreateItemView(bot);
            LoadIcon(bot);
            SaveBots();
        }

        internal void RemoveBot(CustomBot selectedBot) {
            if (selectedBot == null) return;

            // Получаем индекс бота в списке
            int index = _bots.IndexOf(selectedBot);

            // Удаляем бота из списка
            _bots.RemoveAt(index);

            // Удаляем соответствующий GameObject по индексу
            if (index >= 0 && index < _listContainer.childCount) {
                Destroy(_listContainer.GetChild(index).gameObject);
            }

            // Удаляем текстуру
            string fullPath = Path.Combine(Application.persistentDataPath, folderForBotsImages, selectedBot.IconPath);
            if (File.Exists(fullPath)) {
                File.Delete(fullPath);
            }

            // Сохраняем изменения
            SaveBots();
        }

        public static List<CustomBot> GetSavedBots() {
            string json = PlayerPrefs.GetString(KEY_CUSTOM_BOT_SAVES, "[]");
            return JsonConvert.DeserializeObject<List<CustomBot>>(json);
        }

        #endregion

    }
}