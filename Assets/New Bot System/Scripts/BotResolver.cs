using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NewBotSystem
{
    public class BotResolver : NetworkBehaviour
    {
        public UnityEvent<string> OnBotState = new UnityEvent<string>();
        public UnityEvent OnDamage = new UnityEvent();

        public bool IsServer => isServer;

        public readonly SyncList<Vector3> BotBonePositions = new SyncList<Vector3>();
        public readonly SyncList<Quaternion> BotBoneQuaternion = new SyncList<Quaternion>();


        [SerializeField]
        private Transform[] _botBones;
        [SerializeField]
        private Transform[] _ragdollBones;
        [SerializeField]
        private Transform _botBase;

        [SerializeField]
        private GameObject _aimAssystSphere;
        [SerializeField]
        private float _hitPointsMax = 100;



        [SerializeField]
        private GameObject _botStatusMarker;


        [SerializeField, SyncVar]
        private uint _playerNetID;
        public uint PlayerNetID
        {
            get
            {
                return _playerNetID;
            }
            set
            {
                _playerNetID = value;
            }
        }

        [SerializeField, SyncVar]
        private BotStatus _status;
        public BotStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        [SerializeField, SyncVar(hook = nameof(HitPointsChanged))]
        private float _hitPoints;

        public float HitPoints
        {
            get
            {
                return _hitPoints;
            }
            set
            {
                _hitPoints = value;
            }
        }

        [SerializeField, SyncVar]
        private bool _isDead;

        public bool IsDead
        {
            get
            {
                return _isDead;
            }
            set
            {
                _isDead = value;
            }
        }

        [SerializeField, SyncVar]//(hook = nameof(ClipChanged))]
        private string _clip;

        public string Clip
        {
            get
            {
                return _clip;
            }
            set
            {
                _clip = value;
            }
        }

        [SerializeField]
        private string _clipValuePrev;

        [SerializeField, SyncVar]
        private Vector3 _aimPointPosition;

        public Vector3 AimPointPosition
        {
            get
            {
                return _aimPointPosition;
            }
            set
            {
                _aimPointPosition = value;
            }
        }

        [SerializeField]
        private GameObject _spawnPoint;
        [SerializeField]
        private GameObject _rigAim;

        [SerializeField]
        private Transform _aimPoint;

        [SerializeField]
        private BotAnimatorManager _animatorManager;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private BotAnimationEventHandled _eventHandled;
        [SerializeField]
        private RigBuilder _rigBuilder;
        [SerializeField]
        private RagDollDropper _ragDoll;
        [SerializeField]
        private NavMeshModifier _navigationModifier;
        [SerializeField]
        private NavMeshAgent _navigation;
        [SerializeField]
        private BotFromPerdanskAI _ai;

        private Vector3 _velocity;
        private bool _syncRagdoll;

        void Start()
        {
            if (IsServer)
            {
                HitPoints = _hitPointsMax;
                _spawnPoint = new GameObject($"Bot Spawn Point {gameObject.name.Replace("Bot", "")}");
                _spawnPoint.transform.position = transform.position;
                _ragDoll.Init(this);
            }
            else
            {
                Destroy(_navigationModifier);
                Destroy(_animatorManager);
                Destroy(_navigation);
                _ai.DestroyPatrolPoints();
                Destroy(_ai);
                Destroy(_rigAim);
                Destroy(_eventHandled);
                Destroy(_rigBuilder);
                Destroy(_animator);
                Destroy(_aimPoint.gameObject);
                Destroy(_ragDoll.gameObject);
            }

            LateSubscribe();
        }

        public override void OnStartServer()
        {
            if (isServer)
            {
                foreach (var bone in _botBones)
                {
                    BotBonePositions.Add(bone.position);
                    BotBoneQuaternion.Add(bone.rotation);
                }
            }
        }

        public override void OnStartClient()
        {
            BotBonePositions.OnChange += OnPositionListChanged;
            BotBoneQuaternion.OnChange += OnRotationListChanged;
        }

        public async void LateSubscribe()
        {
            while (EventsBus.Instance == null)
            {
                await UniTask.Yield();
            }

            EventsBus.Instance.OnExplosionCenter.AddListener(ExplosionProcess);
        }

        public void ExplosionProcess(Vector3 center)
        {
            float damage = Vector3.Distance(transform.position, center);
            //Debug.Log($"{damage}");
            damage = damage / 5;
            damage = damage > 1 ? 1 : damage;
            damage = (1 - damage) * 200;
            //Debug.Log($"{damage}");

            if(isClient)
            {
                OnReceiveDamageOnServer(damage, 0, 0);
            }
        }

        public void DisableBot()
        {
            OnReceiveDamageOnServer(100, 0, 0);
        }

        [ContextMenu("Collect Bot Bones")]
        public void CollectBotBones()
        {
            // Собираем все кости регдолла в словарь
            if (_botBase != null)
            {
                Transform[] botBonesRaw = _botBase.GetComponentsInChildren<Transform>();
                List<Transform> botBonesList = new List<Transform>();
                foreach (Transform botBone in botBonesRaw)
                {
                    if (botBone.name.Contains("mixamorig"))
                    {
                        botBonesList.Add(botBone);
                    }
                }

                _botBones = botBonesList.ToArray();
                System.Array.Sort(_botBones, (a, b) => a.name.CompareTo(b.name));
            }
        }

        [ContextMenu("Collect RagDoll Bones")]
        public void CollectRagDollBones()
        {
            // Собираем все кости регдолла в словарь
            if (_ragDoll != null)
            {
                Transform[] ragDollBonesRaw = _ragDoll.transform.GetComponentsInChildren<Transform>();
                List<Transform> ragDollBonesList = new List<Transform>();
                foreach (Transform ragDollBone in ragDollBonesRaw)
                {
                    if (ragDollBone.name.Contains("mixamorig"))
                    {
                        ragDollBonesList.Add(ragDollBone);
                    }
                }

                _ragdollBones = ragDollBonesList.ToArray();
                System.Array.Sort(_ragdollBones, (a, b) => a.name.CompareTo(b.name));
            }
        }

        void OnPositionListChanged(SyncList<Vector3>.Operation op, int index, Vector3 value)
        {
            switch (op)
            {
                case SyncList<Vector3>.Operation.OP_ADD:
                    // value is the NEW value of the entry
                    //Debug.Log($"Element added at index {index} {value}");
                    break;

                case SyncList<Vector3>.Operation.OP_INSERT:
                    // value is the NEW value of the entry
                    //Debug.Log($"Element inserted at index {index} {value}");
                    break;

                case SyncList<Vector3>.Operation.OP_SET:

                    if (isClient && !isServer)
                    {
                        //Debug.Log($"Element changed at index {index} from {value} to {RagdollBonePositions[index]}");
                        _botBones[index].position = BotBonePositions[index];
                    }
                    // value is the OLD value of the entry
                    //Debug.Log($"Element changed at index {index} from {value} to {RagdollBonePositions[index]}");
                    break;

                case SyncList<Vector3>.Operation.OP_REMOVEAT:
                    // value is the OLD value of the entry
                    //Debug.Log($"Element removed at index {index} was {value}");
                    break;

                case SyncList<Vector3>.Operation.OP_CLEAR:
                    // value is null / default
                    // we can iterate the list to get the elements if needed.
                    /* foreach (Vector3 name in RagdollBonePositions)
                    {
                        Debug.Log($"Element cleared {name}");
                    }*/
                    break;
            }
        }

        void OnRotationListChanged(SyncList<Quaternion>.Operation op, int index, Quaternion value)
        {
            switch (op)
            {
                case SyncList<Quaternion>.Operation.OP_ADD:
                    // value is the NEW value of the entry
                    //Debug.Log($"Element added at index {index} {value}");
                    break;

                case SyncList<Quaternion>.Operation.OP_INSERT:
                    // value is the NEW value of the entry
                    //Debug.Log($"Element inserted at index {index} {value}");
                    break;

                case SyncList<Quaternion>.Operation.OP_SET:

                    if (isClient && !isServer)
                    {
                        //Debug.Log($"Element changed at index {index} from {value} to {RagdollBonePositions[index]}");
                        _botBones[index].rotation = BotBoneQuaternion[index];
                    }
                    // value is the OLD value of the entry
                    //Debug.Log($"Element changed at index {index} from {value} to {RagdollBonePositions[index]}");
                    break;

                case SyncList<Quaternion>.Operation.OP_REMOVEAT:
                    // value is the OLD value of the entry
                    //Debug.Log($"Element removed at index {index} was {value}");
                    break;

                case SyncList<Quaternion>.Operation.OP_CLEAR:
                    // value is null / default
                    // we can iterate the list to get the elements if needed.
                    /* foreach (Vector3 name in RagdollBonePositions)
                    {
                        Debug.Log($"Element cleared {name}");
                    }*/
                    break;
            }
        }

        [ContextMenu("Respawn On Server")]
        public void RespawnOnServer()
        {
            if(isServer)
            {
                RespawnPart();
                RespawnOnClients();
            }
        }

        [ClientRpc]
        public void RespawnOnClients()
        {
            //Debug.Log("RagDoll On Clients Respawn ");
            RespawnPart();
        }

        public void RespawnPart()
        {
            HitPoints = _hitPointsMax;
            IsDead = false;

            if (isServer)
            {
                _ragDoll.ResetRagdoll();
                _ai.ResetAfterDeth();
                _syncRagdoll = false;

                if (_navigation != null)
                {
                    _navigation.enabled = true;
                }

                _animatorManager.PlayClip(Clip);
            }

            //_animator.PlayClip("Idle");
        }

        [ContextMenu("PermamentDie")]
        public void PermamentDie()
        {
            if (NetworkClient.active)
            {
                OnReceiveDamageOnServer(120, 0, 0);
            }
            else
            {
                OnReceiveDamagePart(120, 0, 0);
            }
        }

        [Command(requiresAuthority = false)]
        public void OnReceiveDamageOnServer(float damage, uint sender, uint receiver)
        {
            //OnReceiveDamagePart(damage, sender, receiver);

            if (NetworkClient.active)
            {
                OnReceiveDamageOnClients(damage, sender, receiver);
            }
            else
            {
                OnReceiveDamagePart(damage, sender, receiver);
            }
        }

        [ClientRpc]
        private void OnReceiveDamageOnClients(float damage, uint sender, uint receiver)
        {
            OnReceiveDamagePart(damage, sender, receiver);
        }

        public void OnReceiveDamagePart(float damage, uint sender, uint receiver)
        {
            OnDamage?.Invoke();

            if (isServer)
            {
                HitPoints -= damage;
                HitPoints = HitPoints < 0 ? 0 : HitPoints;
            }

            if (HitPoints == 0)
            {
                if (isServer)
                {
                    _ai.ReturnToken();
                    WaitingScreenModel.Instance.PlayerKillBot(sender);
                }

                if (_navigation != null)
                {
                    if (_navigation.enabled)
                    {
                        _navigation.isStopped = true;
                    }

                    _navigation.enabled = false;
                }

                if (!NetworkClient.active)
                {
                    HitPointsChanged(0, 0);
                }
            }
        }

        public void HitPointsChanged(float oldValue, float newValue)
        {
            //Debug.Log("Hit Points Changed");

            if (newValue == 0)
            {
                //Debug.Log("RagDoll On Clients Drop ");
                OnDamage?.Invoke();

                if (isServer)
                {
                    _ragDoll.DropRagDoll();
                    _syncRagdoll = true;
                }

                if (isServer && !IsDead)
                {
                    IsDead = true;

                    //Debug.Log($"On New Round Started On Score {gameObject.name}");
                    //BotManagerNetwork.Instance.RegisterScore();
                    WaitingRespawn();
                }
            }
        }

        public async void WaitingRespawn()
        {
            await UniTask.WaitForSeconds(4);
            if(_navigation != null)
            {
                if (_navigation.enabled)
                {
                    _navigation.isStopped = true;
                }
                _navigation.enabled = false;
            }
            transform.position = _spawnPoint.transform.position;
            await UniTask.WaitForSeconds(1);
            RespawnOnServer();
        }

        public string CurrentAnimation()
        {
            return _animatorManager.CurrentAnimation();
        }

        void Update()
        {
            _aimAssystSphere.SetActive(!_isDead);

            _botStatusMarker.SetActive((BotManagerNetwork.Instance.LocalPlayerNetID == PlayerNetID) && 
                (Status == BotStatus.triggered) && !IsDead);

            if (Clip.Equals(_clipValuePrev))
            {
            }
            else
            {
                _animatorManager.PlayClip(Clip);
                _clipValuePrev = Clip;
            }

            if (isServer)
            {
                if (_syncRagdoll)
                {
                    for (int i = 0; i < _ragdollBones.Length; i++)
                    {
                        BotBonePositions[i] = _ragdollBones[i].position;
                        BotBoneQuaternion[i] = _ragdollBones[i].rotation;
                    }
                }
                else
                {
                    for (int i = 0; i < _botBones.Length; i++)
                    {
                        BotBonePositions[i] = _botBones[i].position;
                        BotBoneQuaternion[i] = _botBones[i].rotation;
                    }
                }
            }
        }
    }
}

