using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NewBotSystem
{
    public class BotHP : MonoBehaviour
    {
        public UnityEvent<GameObject, GameObject> OnDead = new UnityEvent<GameObject, GameObject>();

        [SerializeField]
        private float _hitPointsMax = 100;
        [SerializeField]
        private float _hitPoints;

        /*
        [SerializeField]
        private LayerMask _layerMask;*/

        void Start()
        {
            ResetHP();
        }

        private void OnEnable()
        {
            
        }

        /// <summary>
        /// метод ресив дамаг висит в инспекторе на коллайдере со скриптом Damageble
        /// метод прокидывается на событие OnDamage на скрипте Damageble
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="position"></param>
        /// <param name="normal"></param>
        public void ReceiveDamage(float damage, Vector3 position, Vector3 normal, GameObject sender)
        {

            if (sender != null)
            {
                Debug.Log($"{gameObject.name} получил урон от {sender.name} : {damage}");
            }

            _hitPoints -= damage;
            _hitPoints = _hitPoints < 0 ? 0 : _hitPoints;

            if (_hitPoints <= 0)
            {
                OnDead?.Invoke(gameObject, sender);
                EventsBus.Instance.OnBotDieFromKiller?.Invoke(gameObject, sender);
            }
        }

        public void ResetHP()
        {
            _hitPoints = _hitPointsMax;
        }

        internal void SetMaxHitPoint(float maxHP) {
            _hitPointsMax = maxHP;
        }
    }
}
