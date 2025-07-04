using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewBotSystem
{
    public class BotSoundResolver : MonoBehaviour
    {
        [SerializeField]
        private BotResolver _resolver;
        [SerializeField]
        private BotAnimationEventHandled _eventHandled;
        [SerializeField]
        private AudioSource _idle;
        [SerializeField]
        private AudioSource _walkBase;
        [SerializeField]
        private AudioSource _walkVoice;
        [SerializeField]
        private AudioSource _fall;
        [SerializeField]
        private AudioSource _atackStart;
        [SerializeField]
        private List<AudioClip> _atackStartClips = new List<AudioClip>();
        [SerializeField]
        private AudioSource _atackImpact;
        [SerializeField]
        private List<AudioClip> _atackImpactClips = new List<AudioClip>();
        [SerializeField]
        private AudioSource _damage;
        [SerializeField]
        private List<AudioClip> _damageClips = new List<AudioClip>();


        void Start()
        {
            _resolver.OnBotState.AddListener(BotStateChanged);
            _resolver.OnDamage.AddListener(Damage);
            _eventHandled.OnActackStart.AddListener(ActackStart);
            _eventHandled.OnActackImpact.AddListener(ActackImpact);
        }

        void Update()
        {

        }

        public void BotStateChanged(string state)
        {
            switch (state)
            {
                case "Idle":
                    _idle.Play();
                    break;
                case "Walk":
                    _idle.Stop();
                    _walkBase.Play();
                    _walkVoice.Play();
                    break;
                case "Atack":
                    _idle.Stop();
                    _walkBase.Stop();
                    _walkVoice.Stop();
                    break;
                case "Fall":
                    _idle.Stop();
                    _walkBase.Stop();
                    _walkVoice.Stop();
                    _fall.Play();
                    break;
            }
        }

        public void Damage()
        {
            _idle.Stop();
            _damage.Stop();
            AudioClip clip = _damageClips[Random.Range(0, _damageClips.Count)];
            _damage.clip = clip;
            _damage.Play();
        }

        public void ActackStart()
        {
            _idle.Stop();
            _atackStart.Stop();
            AudioClip clip = _atackStartClips[Random.Range(0, _atackStartClips.Count)];
            _atackStart.clip = clip;
            _atackStart.Play();
        }

        public void ActackImpact()
        {
            _idle.Stop();
            _atackImpact.Stop();
            AudioClip clip = _atackImpactClips[Random.Range(0, _atackImpactClips.Count)];
            _atackImpact.clip = clip;
            _atackImpact.Play();
        }
    }
}
