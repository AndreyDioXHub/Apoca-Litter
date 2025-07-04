using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using NewBotSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BotFromPerdanskAI : MonoBehaviour
{
    public BotResolver Resolver => _resolver;

    [SerializeField]
    private BotResolver _resolver;
    [SerializeField]
    private BotAnimationEventHandled _eventHandled;

    [SerializeField]
    private LayerMask _playerAttackMask;

    [SerializeField]
    private BotMovingStatus _movingStatus;
    [SerializeField]
    private BotStatus _status;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _lookTarget;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private bool _needWait;
    [SerializeField]
    private Transform _myPlayerForAttack;
    [SerializeField]
    private PlayerNetworkResolver _resolverPlayer;
    [SerializeField]
    private List<Transform> _patrolPoints = new List<Transform>();

    private string _curentAnimation = "Idle";

    private string _animationIdleName = "Idle";
    private string _animationWalkName = "Walk";
    private string _animationAtackName = "Atack";
    private string _animationFallName = "Fall";

    private float _meleeHitDistance = 1;
    private float _offcet = 0.5f;

    private Vector3 _prevPosition = Vector3.zero;

    void Start()
    {
        _eventHandled.OnActackImpact.AddListener(OnActackImpact);
        _eventHandled.OnActackEnd.AddListener(OnAttackEnd);

        if (_patrolPoints.Count > 0)
        {
            for (int i = 0; i < _patrolPoints.Count; i++)
            {
                _patrolPoints[i].parent = null;
            }

            ChangeTarget(_patrolPoints[0]);
        }
        else
        {
            ChangeTarget(BotManagerNetwork.Instance.GetRandomPatrolPointCloseToMe(transform.position));            
        }

        _lookTarget.parent = null;

        //CheckMyPosition();
        LateStart();
    }
    /*
    public async void CheckMyPosition()
    {
        while (true)
        {
            _prevPosition = transform.position;
            await UniTask.WaitForSeconds(15);
            float distance = Vector3.Distance(_prevPosition, transform.position);
            float distanceCheck = 1;

            if (distance < distanceCheck)
            {
                Debug.Log($"Bot is Stuck in position {_prevPosition}");
                _resolver.PermamentDie();
            }
        }
    }*/

    public async void LateStart()
    {
        await UniTask.WaitForSeconds(0.5f);

        BotManagerNetwork.Instance.RegisterBot(this);
    }

    public void DestroyPatrolPoints()
    {
        if (_patrolPoints.Count > 0)
        {
            for (int i = 0; i < _patrolPoints.Count; i++)
            {
                Destroy(_patrolPoints[i].gameObject);
            }
        }
        else
        {
        }
    }

    void Update()
    {
        if (_resolver.IsDead)
        {
            if (_navMeshAgent.enabled)
            {
                _navMeshAgent.isStopped = true;
            }
            _movingStatus = BotMovingStatus.idle;
        }
        else
        {
            if (gameObject.activeSelf)
            {
                if (_navMeshAgent.isOnNavMesh)
                {
                    _navMeshAgent.isStopped = false;
                    BotBehavior();
                }
            }
        }
    }

    public void BotBehavior()
    {
        if (_needWait)
        {
            _navMeshAgent.isStopped = true;
            _movingStatus = BotMovingStatus.idle;
            
            /*if (_resolver.IsDead)
            {
                ChangeTarget(transform);
            }*/
        }
        else
        {
            Vector2 flatBotPosition = Vector2.zero;
            Vector2 flatTargetPosition = Vector2.zero;

            try
            {
                try
                {
                    flatBotPosition = new Vector2(transform.position.x, transform.position.z);
                    flatTargetPosition = new Vector2(_target.position.x, _target.position.z);
                }
                catch (MissingReferenceException e)
                {

                }
            }
            catch (NullReferenceException e)
            {

            }

            float distanceToTarget = Vector2.Distance(flatBotPosition, flatTargetPosition);

            _movingStatus = BotMovingStatus.move;

            switch (_status)
            {
                case BotStatus.quiet:
                    _navMeshAgent.speed = 2.5f;
                    _animator.SetFloat("WalkMultiplier", 1);

                    if (distanceToTarget <= 0.5f)
                    {
                        _needWait = true;
                        ChangeTarget();
                    }

                    break;
                case BotStatus.triggered:

                    if(distanceToTarget > BotManagerNetwork.Instance.AgrDistance)
                    {
                        ReturnToken();
                        _status = BotStatus.quiet;
                        ChangeTarget();
                    }

                    _navMeshAgent.speed = 4.5f;
                    _animator.SetFloat("WalkMultiplier", 2);

                    float distanceToPlayer = 2;

                    if (distanceToTarget <= distanceToPlayer)
                    {
                        _movingStatus = BotMovingStatus.attack;
                        _navMeshAgent.isStopped = true;
                    }
                    else
                    {
                        if (_myPlayerForAttack == null)
                        {
                            _status = BotStatus.quiet;
                        }
                        else
                        {
                            ChangeTarget(_myPlayerForAttack);
                        }
                    }

                    break;
            }
        }

        switch (_movingStatus)
        {
            case BotMovingStatus.attack:
                _curentAnimation = _animationAtackName;
                break;
            case BotMovingStatus.idle:
                _curentAnimation = _animationIdleName;
                break;
            case BotMovingStatus.move:
                _curentAnimation = _animationWalkName;
                break;
            case BotMovingStatus.stuck:
                _curentAnimation = _animationAtackName;
                break;
            case BotMovingStatus.crashed:
                _curentAnimation = _animationIdleName;
                break;
        }

        _resolver.Status = _status;
        _resolver.Clip = _curentAnimation;
        //_resolver.PlayClip(_curentAnimation);
    }

    public void ReturnToken()
    {
        BotManagerNetwork.Instance.ReturnToken(this, _resolverPlayer);
        _resolverPlayer = null;
        _resolver.PlayerNetID = 0;
    }

    public async void ChangeTarget()
    {
        if (_navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = true;
        }

        await UniTask.WaitForSeconds(1);

        if (_patrolPoints.Count > 0)
        {
            int nextTargetIndex = _patrolPoints.FindIndex(p => p == _target);
            nextTargetIndex++;
            nextTargetIndex = nextTargetIndex > _patrolPoints.Count - 1 ? 0 : nextTargetIndex;
            ChangeTarget(_patrolPoints[nextTargetIndex]);
        }
        else
        {
            ChangeTarget(BotManagerNetwork.Instance.GetRandomPatrolPointCloseToMe(transform.position));
        }

        await UniTask.Yield();
        _movingStatus = BotMovingStatus.move;
        _needWait = false;
    }

    public void OnActackImpact()
    {

        RaycastHit[] hits = Physics.BoxCastAll(transform.position + transform.forward * _offcet,
            Vector3.one/2, transform.forward, transform.rotation,
            _meleeHitDistance, _playerAttackMask, QueryTriggerInteraction.Collide);

        float damage = 50;

        //Debug.Log($"OnActackImpact {hits.Length}");

        if (hits.Length > 0)
        {
            /*
            if (hits[0].transform.gameObject.TryGetComponent(out Damageble damageble))
            {
                damageble.ReceiveDamage(damage, 0, false);
            }*/

            /*
            if (hits[0].transform.gameObject.TryGetComponent(out BotDamageble botDamageble))
            {
                botDamageble.ReceiveDamage(damage, 0, false);
            }*/

            if (hits[0].transform.gameObject.TryGetComponent(out PlayerZombieImpackt zombieImpackt))
            {
                //Debug.Log($"OnActackImpact have zombieImpackt");
                zombieImpackt.ReceiveDamage(damage, 0, 0);
            }
        }

        foreach (RaycastHit hit in hits)
        {
            //Debug.Log("Вот тут нас будут ебать читеры! \nЧитеры привет! Это ОЧКО осталось непрекрытым специально для вас... Уебы... \nКЧАУ!");

        }
    }
    public void OnAttackEnd()
    {
        _needWait = true;
        RestBeforNewAttack();
    }

    public async void RestBeforNewAttack()
    {
        await UniTask.WaitForSeconds(0.3f);

        if (_myPlayerForAttack != null)
        {
            if (_myPlayerForAttack.GetComponent<PlayerNetworkResolver>().IsDead)
            {
                _status = BotStatus.quiet;
                ChangeTarget();
                _myPlayerForAttack = null;
            }
        }

        _needWait = false;
    }

    public void ResetAfterDeth()
    {
        //Debug.Log("Bot Reseted After Deth");
        _status = BotStatus.quiet;
        ChangeTarget();
        _myPlayerForAttack = null;
        _needWait = false;
    }

    public void ChangeTarget(Transform target)
    {
        if (_navMeshAgent.enabled)
        {
            _target = target;
            _navMeshAgent.SetDestination(_target.position);

            Vector3 flatBotPosition = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 flatTargetPosition = new Vector3(_target.position.x, 0, _target.position.z);
            Vector3 normilizedDirection = (flatTargetPosition - flatBotPosition).normalized;
            normilizedDirection = normilizedDirection.magnitude == 0 ? target.forward : normilizedDirection;
            _lookTarget.position = _target.position + normilizedDirection + Vector3.up * 0.5f;
            ChangeTargetResolverPart(_lookTarget.position);
        }
    }

    public async void ChangeTargetResolverPart(Vector3 position)
    {
        while (_resolver == null) 
        {
            await UniTask.Yield();
        }
        _resolver.AimPointPosition = position;
    }

    internal void PlayerDetected(GameObject visiblePlayer)
    {
        //Debug.Log($"PlayerDetected {visiblePlayer.name}");

        if (visiblePlayer.GetComponent<PlayerNetworkResolver>().IsDead)
        {

        }
        else
        {
            _status = BotStatus.triggered;

            if (_navMeshAgent.enabled)
            {
                _navMeshAgent.isStopped = true;
            }

            _myPlayerForAttack = visiblePlayer.transform;
            _resolverPlayer = visiblePlayer.GetComponent<PlayerNetworkResolver>();
            _resolver.PlayerNetID = _resolverPlayer.netId;
            ChangeTarget(_myPlayerForAttack);
        }
    }
}

public enum BotStatus
{
    quiet,
    triggered
}

public enum BotMovingStatus
{
    idle = 0,
    move = 1,
    stuck = 2,
    crashed = 3,
    stopped = 4,
    attack = 5
}
