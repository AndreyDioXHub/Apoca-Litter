using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using DevionGames.UIWidgets;
using TMPro;

namespace cyraxchel.ui {
    public class NotificationSlotTMPro : UISlot<NotificationOptions> {
        /// <summary>
        /// Notification text to display
        /// </summary>
        [SerializeField]
        protected TMP_Text m_Text;
        /// <summary>
        /// Notification text to display
        /// </summary>
        [SerializeField]
        protected TMP_Text m_Time;
        /// <summary>
        /// Notification icon to show.
        /// </summary>
        [SerializeField]
        protected Image m_Icon;

        [SerializeField]
        protected bool recolorText = true;

        public override void Repaint() {
            if (ObservedItem != null) {
                Notification container = Container as Notification;

                if (this.m_Text != null) {
                    if (recolorText) {
                        this.m_Text.text = WidgetUtility.ColorString(ObservedItem.text, ObservedItem.color);
                    } else {
                        this.m_Text.text = ObservedItem.text;
                    }
                    DelayCrossFade(this.m_Text, ObservedItem);
                }

                if (this.m_Time != null) {
                    this.m_Time.text = (string.IsNullOrEmpty(container.timeFormat) ? "" : "[" + DateTime.Now.ToString(container.timeFormat) + "] ");
                    DelayCrossFade(this.m_Time, ObservedItem);
                }

                if (this.m_Icon != null) {
                    this.m_Icon.gameObject.SetActive(ObservedItem.icon != null);
                    if (ObservedItem.icon != null) {
                        this.m_Icon.overrideSprite = ObservedItem.icon;
                        DelayCrossFade(this.m_Icon, ObservedItem);
                    }
                }
            }
        }

        private void DelayCrossFade(Graphic graphic, NotificationOptions options) {
            if ((Container as Notification).fade)
                StartCoroutine(DelayCrossFade(graphic, options.delay, options.duration, options.ignoreTimeScale));
        }

        private IEnumerator DelayCrossFade(Graphic graphic, float delay, float duration, bool ignoreTimeScale) {
            yield return new WaitForSeconds(delay);
            graphic.CrossFadeAlpha(0f, duration, ignoreTimeScale);
        }
    }
}