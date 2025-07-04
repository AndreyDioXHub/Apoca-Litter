using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using NewBotSystem;

namespace UI.BotEditor {
    public class EditorScreen : MonoBehaviour {

        internal static readonly string folderForBotsImages = "userbots";
        [Header("Buttons")]
        [SerializeField]
        private Button _uploadImageButton;
        [SerializeField]
        private Button _saveImageButton;
        [SerializeField]
        private Button _removeImageButton;

        [SerializeField]
        private CanvasGroup _mainPanel;

        [SerializeField]
        private GameObject _saveDialogue;

        [SerializeField]
        private Button _closeButton;

        private BotList _list;
        private BotPreview _preview;

        private CustomBot _preparedBot = null;
        private CustomBot _selectedBot = null;

        private void Awake() {
            _list = GetComponent<BotList>();
            _list.OnBotContext.AddListener(SetNewContext);
            _list.OnBotSelected.AddListener(OnBotSelected);
            _preview = GetComponent<BotPreview>();
            _uploadImageButton.onClick.AddListener(UploadNewImage);
            _saveImageButton.interactable = false;
            _saveImageButton.onClick.AddListener(async () => await SaveBot());
            _removeImageButton.onClick.AddListener(RemoveSelectedBot);
            _removeImageButton.interactable = false;
            _closeButton.onClick.AddListener(() => MainMenuPanelManager.Instance.Back());
        }

        private void SetNewContext(CustomBot botContext) {
            EditableContextPanel.ActiveConfig = botContext.Config;
        }

        private async void OnBotSelected(CustomBot selectedBot) {
            _selectedBot = selectedBot;
            _removeImageButton.interactable = selectedBot != null;
            if (selectedBot != null) {
                var texture = await selectedBot.GetBotTexture();
                _preview.SetImage(texture);
            }
        }

        // Start is called before the first frame update
        void Start() {
        }

        private void UploadNewImage() {
            // Открываем галерею для выбора изображения

            var permission = NativeGallery.GetImageFromGallery((path) => {
                if (path != null) {
                    // Получаем расширение файла
                    string extension = Path.GetExtension(path);
                    // Создаем имя файла на основе времени и расширения
                    string fileName = $"bot_{DateTime.Now.Ticks}{extension}";
                    string destinationPath = Path.Combine(Application.persistentDataPath, folderForBotsImages, fileName);

                    // Создаем директорию, если её нет
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                    // Копируем файл
                    File.Copy(path, destinationPath, true);

                    // Создаем новый CustomBot
                    var newBot = new CustomBot {
                        Name = $"Бот {DateTime.Now.Ticks}",
                        IconPath = fileName
                    };

                    _preparedBot = newBot;
                    _saveImageButton.interactable = true;
                    OnBotSelected(newBot);

                }
            }, "Выберите изображение для бота", "image/*"); /**/

        }

        private async Task SaveBot() 
        {
            if (_preparedBot == null) 
            {
                return;
            }
            _mainPanel.alpha = 0;
            bool doSave = await _saveDialogue.GetComponent<EditorSaveDialogue>().Show();
            string saveName = _saveDialogue.GetComponent<EditorSaveDialogue>().SaveName;
            if (doSave && _preparedBot != null) {
                if (string.IsNullOrEmpty(saveName)) {
                    saveName = $"Бот {DateTime.Now.Ticks}";
                }
                _preparedBot.Name = saveName;
                //TODO Render texture from 
                _preview.SaveTexture(Path.Combine(Application.persistentDataPath, folderForBotsImages, _preparedBot.IconPath));
                _list.AddBot(_preparedBot);
                _preparedBot = null;
                _saveImageButton.interactable = false;
            }
            _mainPanel.alpha = 1;
        }

        private void RemoveSelectedBot() {
            if (_selectedBot != null) {
                _list.RemoveBot(_selectedBot);
                _selectedBot = null;
                _preview.SetImage(null);
            }
        }

        public class CustomBot {
            [JsonProperty("name")]
            public string Name { get => Config.Name; set => Config.Name = value; }

            [JsonProperty("iconPath")]
            public string IconPath { get => Config.Iconlink; set => Config.Iconlink = value; }
            [JsonProperty("config")]
            private BotConfig _config;
            [JsonIgnore]
            public BotConfig Config { get {
                    if (_config == null) {
                        _config = new BotConfig() {
                            BotThemeName = BotManager.CUSTOM_BOT_THEME,
                        };
                        _config.ConfigureInternal(10, 10, 10);
                    }
                    return _config;

                } set {
                    _config = value;
                    Name = _config.Name;
                } }

            private Texture2D _iconTexture;
            public async UniTask<Texture2D> GetBotTexture() {
                if (_iconTexture == null) {
                    string fullPath = Path.Combine(Application.persistentDataPath, folderForBotsImages, IconPath);
                    if (File.Exists(fullPath)) {
                        byte[] fileData = await File.ReadAllBytesAsync(fullPath);
                        _iconTexture = new Texture2D(2, 2);
                        _iconTexture.LoadImage(fileData);
                        if (_listImage != null) {
                            Sprite iconSprite = Sprite.Create(_iconTexture, new Rect(0, 0, _iconTexture.width, _iconTexture.height), Vector2.zero);
                            _listImage.sprite = iconSprite;
                            _listImage.color = Color.white;
                            _listImage = null;
                        }
                    } else {
                        Debug.LogError($"Texture file not found at path: {fullPath}");
                        return null;
                    }
                }
                return _iconTexture;
            }

            private Image _listImage;
            public void SetImageContainer(Image container) {
                _listImage = container;
                if (_iconTexture != null) {
                    Sprite iconSprite = Sprite.Create(_iconTexture, new Rect(0, 0, _iconTexture.width, _iconTexture.height), Vector2.zero);
                    _listImage.sprite = iconSprite;
                    _listImage.color = Color.white;
                }
            }
        }
    }
}