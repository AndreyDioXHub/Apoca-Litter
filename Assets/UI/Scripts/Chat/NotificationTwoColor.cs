using DevionGames.UIWidgets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cyraxchel.games.network.chat {
    [Serializable]
    public class NotificationTwoColor : NotificationOptions {
        public Color32 TitleColor = Color.blue;

        public NotificationTwoColor(NotificationOptions other) : base(other) {
            if (other is NotificationTwoColor) {
                this.TitleColor = (other as NotificationTwoColor).TitleColor;
            }
        }
    }
}
