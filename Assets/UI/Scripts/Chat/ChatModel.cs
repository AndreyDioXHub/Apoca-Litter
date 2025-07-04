using Mirror;
using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace cyraxchel.games.network.chat {
    public class ChatModel : NetworkBehaviour {

        public static ChatModel Instance { get; private set; }

        public class SyncListChatMessage : SyncList<ChatMessage> { }

        
        [SyncVar]
        public SyncListChatMessage Messages = new SyncListChatMessage();

        private ReactiveProperty<ChatMessage> _lastMessage = new ReactiveProperty<ChatMessage>();
        public ReadOnlyReactiveProperty<ChatMessage> LastMessage => _lastMessage;

        [SerializeField]
        private CensoredList _censoredList;

        [SerializeField]
        private int _maxMessages = 100;

        private void Awake() {
            Instance = this;
        }

        public override void OnStartClient() 
        {
            base.OnStartClient();
            Messages.Callback += OnMessagesChanged;
        }

        public override void OnStopClient() 
        {
            base.OnStopClient();
            Messages.Callback -= OnMessagesChanged;
        }

        [Command(requiresAuthority = false)]
        public void CmdSendMessage(string sender, string message) 
        {
            string checkedMessage = CheckForSpam(message);
            ChatMessage chatMessage = new ChatMessage(sender, checkedMessage);
            Messages.Add(chatMessage);

            if (Messages.Count > _maxMessages) 
            {
                Messages.RemoveAt(0);
            }

            _lastMessage.Value = chatMessage;
        }

        private void OnMessagesChanged(SyncList<ChatMessage>.Operation op, int itemIndex, ChatMessage oldItem, ChatMessage newItem) 
        {
            if (op == SyncList<ChatMessage>.Operation.OP_ADD)
            {
                _lastMessage.Value = newItem;
            }
        }

        private string CheckForSpam(string message) 
        {
            message = _censoredList.ReplaceText(message);
            return message;
        }
    }
}