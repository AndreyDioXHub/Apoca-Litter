using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DevionGames.UIWidgets;
using game.configuration;
using UnityEngine.Events;


namespace cyraxchel.games.network.chat {
    public class ChatUI : MonoBehaviour {
        public UnityEvent OnEmptySend;
        private ChatModel _chatModel => ChatModel.Instance;
        [SerializeField]
        private TMP_InputField _inputField;
        [SerializeField]
        private Button _sendButton;
        
        private TMP_Text _chatText;

        [SerializeField]
        private NotificationTwoColor _myStyleOpt;
        [SerializeField]
        private NotificationTwoColor _otherStyleOpt;
        [SerializeField]
        private Notification _notificationView;

        private void Start() {
            _sendButton.onClick.AddListener(OnSendButtonClicked);
            _chatModel.LastMessage.Subscribe(OnNewMessageReceived);
            _inputField.onSubmit.AddListener((string text) => OnSendButtonClicked());
        }

        private void OnSendButtonClicked() {
            if (!string.IsNullOrEmpty(_inputField.text)) {
                Debug.Log($"try send message: {_inputField.text}");
                _chatModel.CmdSendMessage(GameConfig.CurrentConfiguration.PlayerName, _inputField.text);
                _inputField.text = string.Empty;
                FocusInput();
            } else {
                //Do close
                OnEmptySend.Invoke();
            }
        }

        private void OnNewMessageReceived(ChatMessage message) {
            if (_chatText != null) {
                _chatText.text += $"{message.Timestamp.ToShortTimeString()} {message.Sender}: {message.Message}\n";
            }

            var _defaultOption = message.Sender == GameConfig.CurrentConfiguration.PlayerName ? _myStyleOpt : _otherStyleOpt;
            NotificationTwoColor opt = new NotificationTwoColor(_defaultOption);
            opt.text = StylizeText(message, opt);
            //opt.title = message.Sender;
            _notificationView.AddItem(opt);
        }

        public void FocusInput() {
            _inputField.ActivateInputField();
        }

        private string StylizeText(ChatMessage message, NotificationTwoColor options) {
            //return $"{message.Timestamp.ToShortTimeString()} {message.Sender}: {message.Message}\n"; ;
            
            
            string ucolor = ColorUtility.ToHtmlStringRGB(options.TitleColor);
            string mcolor = ColorUtility.ToHtmlStringRGB(options.color);

            string prettyString = $"<color=#{ucolor}>[{message.Sender}]:</color> <color=#{mcolor}>{message.Message}</color>";

            return prettyString; /**/

        }
    }
}