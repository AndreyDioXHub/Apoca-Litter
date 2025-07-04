using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cyraxchel.games.network.chat {
    [Serializable]
    public class ChatMessage {
        public string Sender;
        public string Message;
        public DateTime Timestamp;

        public ChatMessage(string sender, string message) {
            Sender = sender;
            Message = message;
            Timestamp = DateTime.Now;
        }

        public ChatMessage() {
        }
    }
}