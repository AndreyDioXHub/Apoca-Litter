using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewBotSystem
{    public class BotDamageble : MonoBehaviour
    {
        public float DamageMultiplier => _damageMultiplier;

        //public uint PlayerID => _resolver.PlayerID;

        [SerializeField]
        private BotResolver _resolver;
        [SerializeField]
        private float _damageMultiplier = 1;

        public virtual void Start()
        {
            if(_resolver == null)
            {
                _resolver = GetComponentInParent<BotResolver>();
            }
        }

        public virtual void Update()
        {

        }

        public void Init(BotResolver resolver)
        {
            _resolver = resolver;
        }

        public virtual void ReceiveDamage(float damage, uint sender, bool multyplierEffect = true)
        {
            float damageMultiplier = multyplierEffect ? _damageMultiplier : 1;
            _resolver.OnReceiveDamageOnServer(damage * damageMultiplier, sender, 0);
        }
    }
}
