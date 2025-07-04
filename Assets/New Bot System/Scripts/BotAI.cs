using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NewBotSystem
{
    public class BotAI : MonoBehaviour
    {
        [SerializeField]
        private BotResolver _resolver;
        [SerializeField]
        private LayerMask _playerMask;
        [SerializeField]
        private float _meleeHitDistance = 1;
        [SerializeField]
        private float _offcet = 0.5f;
        [SerializeField]
        private BotMovingStatus _status;

        private BotAnimationEventHandled _eventHandled;

        private float _attachDistance = 1.0f;

        private string _curentAnimation = "Idle";

        private string _animationIdleName = "Idle";
        private string _animationWalkName = "Walk";
        private string _animationAtackName = "Atack";
        private string _animationFallName = "Fall";

        private bool _isAtack;
        private bool _isStuck;
        private Vector3 _aimTargetPosition;
        private Vector3 _targetDirection;

        void Start()
        {
            _eventHandled = GetComponentInChildren<BotAnimationEventHandled>();
            _eventHandled.OnActackImpact.AddListener(ActackImpact);
            _eventHandled.OnActackEnd.AddListener(AttacIsEnd);

            //_animator = GetComponent<BotAnimatorManager>();
        }

        void Update()
        {
            if (gameObject.activeSelf)
            {
                if (_resolver == null)
                {
                    return;
                }

                BotBehavior();
            }
        }

        public void ActackImpact()
        {

            RaycastHit[] hits = Physics.BoxCastAll(transform.position + transform.forward * _offcet,
                Vector3.one, transform.forward, transform.rotation,
                _meleeHitDistance, _playerMask, QueryTriggerInteraction.Collide);

            float damage = 50;

            if(hits.Length > 0)
            {
                if (hits[0].transform.gameObject.TryGetComponent(out Damageble damageble))
                {
                    damageble.ReceiveDamage(damage, 0, false);
                }

                /*
                if (hits[0].transform.gameObject.TryGetComponent(out BotDamageble botDamageble))
                {
                    botDamageble.ReceiveDamage(damage, 0, false);
                }*/

                if (hits[0].transform.gameObject.TryGetComponent(out Character character))
                {
                    character.Resolver.OnReceiveDamageOnServer(damage, 0, 0);
                }
            }

            foreach (RaycastHit hit in hits)
            {
                //Debug.Log("Вот тут нас будут ебать читеры! \nЧитеры привет! Это ОЧКО осталось непрекрытым специально для вас... Уебы... \nКЧАУ!");

            }
        }

        public void AttacIsEnd()
        {
            if(_isAtack)
            {
                _isAtack = false;
                //_resolver.PlayClip(_animationIdleName);
            }

            if(_isStuck)
            {
                _isStuck = false;
                //_resolver.PlayClip(_animationIdleName);
            }
        }

        public void Respawn()
        {
            _isAtack = false;
            //_resolver.PlayClip(_animationWalkName);
        }

        public void BotBehavior()
        {
            float distanceToTarget = 0;

            _curentAnimation = _animationIdleName;
            /*
            if(_direction.Target != null)
            {
                distanceToTarget = Vector3.Distance(transform.position, _direction.Target.position);

                if (distanceToTarget < _attachDistance)
                {
                    //_direction.Stop(Time.fixedDeltaTime);
                    //_curentAnimation = _animationAtackName;
                }

                switch (_status)
                {
                    case BotMovingStatus.stopped:
                        _isAtack = true;
                        _curentAnimation = _animationAtackName;
                        break;
                    case BotMovingStatus.idle:
                        _curentAnimation = _animationIdleName;
                        break;
                    case BotMovingStatus.move:
                        _curentAnimation = _animationWalkName;
                        break;
                    case BotMovingStatus.stuck:
                        _isStuck = true;
                        _curentAnimation = _animationAtackName;
                        break;
                    case BotMovingStatus.crashed:
                        _curentAnimation = _animationIdleName;
                        _resolver.OnCrashed(0.05f);
                        //_direction.Idled(Time.fixedDeltaTime);
                        break;
                }
            }
            */
            /*
            if (!_controller.isGrounded)
            {
                _curentAnimation = _animationFallName;
            }*/

            //_resolver.PlayClip(_curentAnimation);
        }
    }
}