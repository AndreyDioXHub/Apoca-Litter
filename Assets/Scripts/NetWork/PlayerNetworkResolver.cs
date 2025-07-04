using Cysharp.Threading.Tasks;
using game.configuration;
using InfimaGames.LowPolyShooterPack;
using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerNetworkResolver : NetworkBehaviour
{
    public bool IsLocalPlayer => isLocalPlayer;
    public bool IsServer => isServer;
    public uint PlayerID => _identity.netId;

    public DealerZone DealerZoneCurent => _dealerZoneCurent;
    public DealerZone DealerZoneForTask => _dealerZoneForTask;

    public Camera PlayerCamera => _playerCamera;
    public Camera DethCamera => _dethCamera;

    public UnityEvent<float, uint, uint> OnReceiveDamage = new UnityEvent<float, uint, uint>(); //damage, sender, receiver
    public UnityEvent<int, int> OnWeaponChanged = new UnityEvent<int, int>(); //old new

    public UnityEvent<int, int, int, int> OnAttachmentChanged = new UnityEvent<int, int, int, int>(); //old new
    public UnityEvent<GameObject> OnWeaponSelected = new UnityEvent<GameObject>();
    public UnityEvent<WeaponAttachmentManager> OnWeaponAttachmentManagerSelected = new UnityEvent<WeaponAttachmentManager>();

    public UnityEvent<Vector3, Vector3, string> OnCollidedEffect = new UnityEvent<Vector3, Vector3, string>();

    public UnityEvent<bool> OnAim = new UnityEvent<bool>();
    public UnityEvent<Vector3, float> OnPjectileSpawned = new UnityEvent<Vector3, float>();
    public UnityEvent<bool> OnMagazineFasten = new UnityEvent<bool>();
    public UnityEvent OnBulletCollided = new UnityEvent();
    public UnityEvent OnReload = new UnityEvent();
    public UnityEvent OnFire = new UnityEvent();

    public UnityEvent OnGetTask = new UnityEvent();
    public UnityEvent OnFailTask = new UnityEvent();
    public UnityEvent OnPushTask = new UnityEvent();

    public UnityEvent<string, string> OnSkinChanged = new UnityEvent<string, string>();

    //public UnityEvent OnWeaponPut = new UnityEvent();
    //public UnityEvent OnMagazineInArm = new UnityEvent();
    //public UnityEvent OnWeaponPull = new UnityEvent();

    //public UnityEvent OnReloadingOpen = new UnityEvent();
    //public UnityEvent OnReloadingInsert = new UnityEvent();
    //public UnityEvent OnBoltAction = new UnityEvent();
    /*
    [SerializeField]
    private GameObject _dethCamUIGO;
    [SerializeField]
    private GameObject _loseScreenGO;*/

    public List<BotFromPerdanskAI> BotsAttackedPlayer = new List<BotFromPerdanskAI>();
    public int BotsCountAttackPlayer = 0;

    [SerializeField]
    private PlayerCanvasSpawner _playerCanvasSpawner;

    [SerializeField]
    private NetworkIdentity _identity;

    [SerializeField]
    private Transform _lookTarget;

    [SerializeField]
    private PauseBoxView _pauseBoxView;
    [SerializeField]
    private Camera _playerCamera;
    [SerializeField]
    private Camera _dethCamera;
    [SerializeField]
    private GameObject _cameraControll3rdPerson;
    [SerializeField]
    private MenuPlayerPuppeteer _puppeteer;
    [SerializeField]
    private AudioListener _listener;
    [SerializeField]
    private GameObject _characterRoot;
    [SerializeField]
    private GameObject _assystSphere;
    [SerializeField]
    private GameObject _inputComponents;
    [SerializeField]
    private UnityEngine.InputSystem.PlayerInput _playerInput;
    [SerializeField]
    private CharacterController _characterController;
    [SerializeField]
    private WallDetector _wallDetector;
    [SerializeField]
    private Movement _movement;
    [SerializeField]
    private FootstepPlayer _footstepPlayer;
    [SerializeField]
    private Inventory _inventory;
    [SerializeField]
    private Character _character;
    [SerializeField]
    private CharacterKinematics _characterKinematics;
    [SerializeField]
    private SkinResolverListener _skinListener;
    [SerializeField]
    private PlayerRagDollDropper _ragDoll;
    [SerializeField]
    private DrillSound3D _drillSound;
    [SerializeField]
    private GameObject _zombieImpacktCube;
    [SerializeField]
    private GameObject _taskImpacktCube;
    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private List<GameObject> _damagebles = new List<GameObject>();
    [SerializeField]
    private Transform _alivePlayer;
    [SerializeField]
    private float _timeToLookForPlayer = 6, _timeToLookForPlayerCur;

    [SerializeField, SyncVar]
    private int _currentWeaponIndex;

    public int CurrentWeaponIndex
    {
        get
        {
            return _currentWeaponIndex;
        }
        set
        {
            _currentWeaponIndex = value;
        }
    }

    [SerializeField, SyncVar]
    private bool _isPushTask;

    public bool IsPushTask
    {
        get
        {
            return _isPushTask;
        }
        set
        {
            _isPushTask = value;
        }
    }

    [SerializeField, SyncVar]
    private bool _isHaveTask;

    public bool IsHaveTask
    {
        get
        {
            return _isHaveTask;
        }
        set
        {
            _isHaveTask = value;
        }
    }

    [SerializeField, SyncVar]
    private int _taskID = -1;

    public int TaskID
    {
        get
        {
            return _taskID;
        }
        set
        {
            _taskID = value;
        }
    }

    [SerializeField, SyncVar]
    private float _taskTimeCur;

    public float TaskTimeCur
    {
        get
        {
            return _taskTimeCur;
        }
        set
        {
            _taskTimeCur = value;
        }
    }

    [SerializeField]
    private DealerZone _dealerZoneCurent;
    [SerializeField]
    private DealerZone _dealerZoneForTask;

    [SerializeField, SyncVar]
    private int _wasd;

    public int WASD
    {
        get
        {
            return _wasd;
        }
        set
        {
            _wasd = value;
        }
    }

    [SerializeField, SyncVar]
    private string _playerName;

    public string PlayerName
    {
        get
        {
            return _playerName;
        }
        set
        {
            _playerName = value;
        }
    }

    [SerializeField, SyncVar]
    private bool _isInsideDealer;

    public bool IsInsideDealer
    {
        get
        {
            return _isInsideDealer;
        }
        set
        {
            _isInsideDealer = value;
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

    [SerializeField, SyncVar]
    private bool _isHoldingButtonFire;

    public bool IsHoldingButtonFire
    {
        get
        {
            return _isHoldingButtonFire;
        }
        set
        {
            _isHoldingButtonFire = value;
        }
    }

    [SerializeField, SyncVar]
    private bool _isCrouch;

    public bool IsCrouch
    {
        get
        {
            return _isCrouch;
        }
        set
        {
            _isCrouch = value;
        }
    }

    [SerializeField, SyncVar]
    private bool _isRun;

    public bool IsRun
    {
        get
        {
            return _isRun;
        }
        set
        {
            _isRun = value;
        }
    }

    [SerializeField, SyncVar]
    private bool _isGrounded;
    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
        set
        {
            _isGrounded = value;
        }
    }

    [SerializeField, SyncVar]
    private Vector3 _lookTargetPosition;
    public Vector3 LookTargetPosition
    {
        get
        {
            return _lookTargetPosition;
        }
        set
        {
            _lookTargetPosition = value;
        }
    }

    [SerializeField, SyncVar]
    private string _nameSkin;
    public string NameSkin
    {
        get
        {
            return _nameSkin;
        }
        set
        {
            _nameSkin = value;
        }
    }
    [SerializeField, SyncVar]
    private string _jsonSkin;
    public string JSONSkin
    {
        get
        {
            return _jsonSkin;
        }
        set
        {
            _jsonSkin = value;
        }
    }

    [SerializeField, SyncVar]
    private bool _isPaused;
    public bool IsPaused
    {
        get
        {
            return _isPaused;
        }
        set
        {
            _isPaused = value;
        }
    }


    void Start()
    {
        _ragDoll.Init(this);

        if (isLocalPlayer)
        {
            _playerCanvasSpawner.SpawnCanvas(_inventory, _character);

            for (int i = 0; i < _damagebles.Count; i++)
            {
                Destroy(_damagebles[i]);
            }

            Destroy(_assystSphere);

            if (AudioListenerResolver.Instance == null)
            {

            }
            else
            {
                AudioListenerResolver.Instance.DisableAudioListener();
            }

            _listener.enabled = true;

            int sex = PlayerPrefs.GetInt("bodytype", 0);
            string name = sex == 0 ? "Man" : "Woman";
            string json = PlayerPrefs.GetString(name, "");

            NameSkin = name;
            JSONSkin = json;
            PlayerName = GameConfig.CurrentConfiguration.PlayerName;
            _nameText.text = PlayerName;

            WaitingScreenModel.Instance.UpdatePlayer(PlayerID, new PlayerLoadingInfo(PlayerName, 2));
            //GameWorld.Instance.OnWorldCreated.AddListener(WorldCreated);
            //BotManagerNetwork.Instance.OnNewRoundStarted.AddListener(HealPlayerAndRecoverOnServer);
            PauseScreen.Instance.OnPaused.AddListener(TurnIntoBox);
            EventsBus.Instance.OnNameUpdated.AddListener(OnNameUpdated);

            _character.OnDie.AddListener(PlayerDieOnServer);
            _character.OnDie.AddListener(DeadProcessing);

            OnFire.AddListener(DeadProcessing);

            _dethCamera.transform.parent = null;
            _cameraControll3rdPerson.transform.parent = null;
            //OnLoadSkinOnServer(name, json);
            /*
            if (!isServer)
            {
            }*/

            LateStartLocal();
        }
        else
        {
            _lookTarget.SetParent(transform);

            Destroy(_dethCamera.gameObject);
            Destroy(_cameraControll3rdPerson);
            Destroy(_playerCanvasSpawner);
            Destroy(_characterRoot);
            Destroy(_inputComponents);
            Destroy(_playerInput);
            Destroy(_characterController);
            Destroy(_wallDetector);
            Destroy(_movement);
            Destroy(_footstepPlayer);
            Destroy(_character);
            Destroy(_characterKinematics);
            Destroy(_puppeteer);
            //Destroy(_damageble);

            string layerName = "AnotherPlayer";

            // �������� ������ ���� �� ��� ��������
            int layerIndex = LayerMask.NameToLayer(layerName);

            // ���������, ���������� �� ����� ����
            if (layerIndex == -1)
            {
                //Debug.LogError("���� � ������ " + layerName + " �� ������!");
                return;
            }

            // ���������� ��������� ���� ���� �������� ��������
            SetLayerRecursively(transform, layerIndex);

            LateStart();
        }

        LateStartSkin();

        if (isServer)
        {
            /*
            if (BotManagerNetwork.Instance == null)
            {

            }
            else
            {
                BotManagerNetwork.Instance.RegisterPlayer(transform);
            }*/
            /*
            if (WaitingScreenModel.Instance == null)
            {

            }
            else
            {
                WaitingScreenModel.Instance.RegisterPlayer(PlayerID, gameObject);
            }*/
        }

        if (isLocalPlayer)
        {

        }
        else
        {
            if (isServer)
            {

            }
            else
            {
                Destroy(_zombieImpacktCube);
                Destroy(_taskImpacktCube);
            }
        }
    }

    public void TossACoin(uint sender)
    {
        if (sender == netId)
        {
            CoinsManager.Instance.AddCoins(5);

            if (LocalSounds.Instance != null)
            {
                LocalSounds.Instance.PlaySound("moneyget");
            }
        }
    }

    public async void TurnIntoBox(bool isPaused)
    {
        await UniTask.Yield();

        IsPaused = isPaused;

        if (NetworkClient.active)
        {
            TurnIntoBoxOnServer(isPaused);
        }
        else
        {

        }
    }

    [Command(requiresAuthority = false)]
    public async void TurnIntoBoxOnServer(bool isPaused)
    {
        await UniTask.Yield();
        await UniTask.Yield();

        if (IsDead)
        {

        }
        else
        {
            TurnIntoBoxOnClient(isPaused);
        }
    }

    [ClientRpc]
    public void TurnIntoBoxOnClient(bool isPaused)
    {
        _pauseBoxView.ShowHideBox(isPaused);
    }

    public void AddRandomBullet()
    {
        _inventory.AddRandomBullet();
    }

    public async void AddMoreRandomBullet()
    {
        for (int i = 0; i < 30; i++)
        {
            await UniTask.WaitForSeconds(0.5f);
            AddRandomBullet();
        }
    }

    public async void DeadProcessing()
    {
        while (!IsDead)
        {
            await UniTask.Yield();
        }

        if (IsDead)
        {
            PlayerNetworkResolver[] foundResolvers = FindObjectsOfType<PlayerNetworkResolver>();
            foreach (var resolver in foundResolvers)
            {
                if (!resolver.IsDead)
                {
                    _alivePlayer = resolver.transform;
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void PlayerDieOnServer()
    {
        IsDead = true;
        PlayerDieOnClient();
        //WaitingScreenModel.Instance.RemoveLivePlayer(gameObject);
    }

    [ClientRpc]
    public void PlayerDieOnClient()
    {
        IsDead = true;
        _ragDoll.DropRagDoll();

        if (_assystSphere != null)
        {
            _assystSphere.SetActive(false);
        }

        if (isLocalPlayer)
        {
            _characterController.enabled = false;
        }
    }

    [Command(requiresAuthority = false)]
    public void PlayerEmtyDrillOnServet(bool state)
    {
        PlayerEmtyDrillOnClient(state);
    }

    [ClientRpc]
    public void PlayerEmtyDrillOnClient(bool state)
    {
        if (state)
        {
            _drillSound.DrillOn();
        }
        else
        {
            _drillSound.DrillOff();
        }
    }

    public void GoIntoDealerZone(DealerZone dealerZone)
    {
        _dealerZoneCurent = dealerZone;
        _character.OnInsideDealer?.Invoke(dealerZone);
    }

    public void SetTaskDealerZone(DealerZone dealerZone)
    {
        _dealerZoneForTask = dealerZone;
        IsHaveTask = _dealerZoneForTask != null;
        _taskTimeCur = _dealerZoneForTask == null ? 0 : _dealerZoneForTask.TaskTime;

        if (dealerZone != null)
        {
            OnGetTask?.Invoke();
        }
    }

    public void SetDealerInside(bool isInside)
    {
        if (NetworkClient.active)
        {
            SetDealerInsideOnServer(isInside);
        }
    }

    [Command(requiresAuthority = false)]
    public void SetDealerInsideOnServer(bool isInside)
    {
        SetDealerInsideOnClient(isInside);
    }

    [ClientRpc]
    public void SetDealerInsideOnClient(bool isInside)
    {
        IsInsideDealer = isInside;
    }

    [Command(requiresAuthority = false)]
    public async void HealPlayerAndRecoverOnServer()
    {
        await UniTask.WaitForSeconds(3);
        /*
        _dethCamUIGO.SetActive(false);
        _loseScreenGO.SetActive(false);
        */
        _timeToLookForPlayerCur = 0;
        IsDead = false;
        HealPlayerAndRecoverOnClient();
        //WaitingScreenModel.Instance.AddLivePlayer(gameObject);
    }

    [ClientRpc]
    public void HealPlayerAndRecoverOnClient()
    {
        IsDead = false;
        //Debug.Log("On New Round Started On Client Resolve");
        _ragDoll.ResetRagdoll();

        if (isLocalPlayer)
        {
            _characterController.enabled = true;
        }

        if (isLocalPlayer)
        {
            _character.SetNeedHealPlayer();
            //_characterController.enabled = true;
        }
    }
    /*
    public async void WorldCreated()
    {
        await UniTask.WaitForSeconds(1);
        //Debug.Log("WorldCreated WaitForSeconds");
        WaitingScreenModel.Instance.UpdatePlayer(PlayerID, new PlayerLoadingInfo(PlayerName, 2));
    }*/

    private async void LateStartLocal()
    {
        await UniTask.WaitForSeconds(0.1f);
        OnNameUpdated(PlayerName);
        WaitingScreenModel.Instance.OnPlayerKillBott.AddListener(TossACoin);
        BotManagerNetwork.Instance.LocalPlayerNetID = _identity.netId;
    }

    private async void LateStart()
    {
        await UniTask.Yield();

        //Debug.Log("Set AimAssyst Layer");
        _assystSphere.layer = LayerMask.NameToLayer("AimAssyst");

        string layerzombieImpacktName = "ZombieImpact";
        int layerzombieImpacktIndex = LayerMask.NameToLayer(layerzombieImpacktName);

        if (_zombieImpacktCube != null)
        {
            _zombieImpacktCube.layer = layerzombieImpacktIndex;
        }

        _nameText.text = PlayerName;
    }

    public void OnNameUpdated(string name)
    {
        PlayerName = name;
        OnNameUpdatedOnServer(name);
    }

    [Command(requiresAuthority = false)]
    public void OnNameUpdatedOnServer(string name)
    {
        OnNameUpdatedOnClients(name);
    }


    [ClientRpc]
    public void OnNameUpdatedOnClients(string name)
    {
        _nameText.text = name;
    }


    private async void LateStartSkin()
    {
        await UniTask.Yield();

        while (string.IsNullOrEmpty(NameSkin) && string.IsNullOrEmpty(JSONSkin))
        {
            await UniTask.Yield();
        }

        if (isServer)
        {
            if (NetworkClient.active)
            {
                OnLoadSkin(NameSkin, JSONSkin);
            }
        }
        else
        {
            OnLoadSkin(NameSkin, JSONSkin);
        }
        string layerzombieImpacktName = "ZombieImpact";
        int layerzombieImpacktIndex = LayerMask.NameToLayer(layerzombieImpacktName);

        if (_zombieImpacktCube != null)
        {
            _zombieImpacktCube.layer = layerzombieImpacktIndex;
        }
    }

    private void SetLayerRecursively(Transform parent, int layerIndex)
    {
        if (parent.gameObject.name.Equals("CameraControll 3rdPerson"))
        {

        }
        else if (parent.gameObject.name.Equals("DethCamera"))
        {

        }
        else
        {
            parent.gameObject.layer = layerIndex;
        }

        foreach (Transform child in parent)
        {
            SetLayerRecursively(child, layerIndex);
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            LookTargetPosition = _lookTarget.position;
            IsHoldingButtonFire = _character.IsHoldingButtonFire();
            IsGrounded = _movement.IsGrounded();

            if (IsDead)
            {
                _timeToLookForPlayerCur += Time.deltaTime;

                if (_timeToLookForPlayerCur > _timeToLookForPlayer)
                {
                    _timeToLookForPlayerCur = _timeToLookForPlayer;



                    if (_alivePlayer == null)
                    {

                    }
                    else
                    {
                        //Debug.Log($"Alive Player {_alivePlayer}");
                        //transform.position = _alivePlayer.position;
                    }
                }
            }
        }
        else
        {
            _lookTarget.position = LookTargetPosition;
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (IsHaveTask)
            {
                TaskTimeCur -= Time.fixedDeltaTime;

                if ((TaskTimeCur < 0) || IsDead)
                {
                    DealerZoneForTask.HideTaskMarker();
                    DealerZonesManager.Instance.ShowDefaultMarkers();
                    DealerZonesManager.Instance.AssignRandomSiblings();
                    SetTaskDealerZone(null);
                    TaskTimeCur = 0;
                    OnFailTask?.Invoke();
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void OnReceiveDamageOnServer(float damage, uint sender, uint receiver)
    {
        //Debug.Log("�� ���� ������ �����, � � 11 ���");
        //Debug.Log("��� ���� �������� ��������� � ���� ���� sender ���� receiver");

        //OnReceiveDamage?.Invoke(damage, sender, receiver);
        OnReceiveDamageOnClients(damage, sender, receiver);
    }

    [ClientRpc]
    private void OnReceiveDamageOnClients(float damage, uint sender, uint receiver)
    {
        OnReceiveDamage?.Invoke(damage, sender, receiver);
    }

    [Command(requiresAuthority = false)]
    public void OnEffectOnServer(Vector3 position, Vector3 normal, string tag)
    {
        OnCollidedEffect?.Invoke(position, normal, tag);
        OnEffectOnClients(position, normal, tag);
    }

    [ClientRpc]
    private void OnEffectOnClients(Vector3 position, Vector3 normal, string tag)
    {
        OnCollidedEffect?.Invoke(position, normal, tag);
    }

    public void OnLoadSkin(string name, string json)
    {
        if (NetworkServer.connections.Count > 0)
        {
            OnLoadSkinOnServer(name, json);
        }
        else
        {
            _skinListener.Init(name, json);
        }
    }

    [Command(requiresAuthority = false)]
    public void OnLoadSkinOnServer(string name, string json)
    {
        _skinListener.Init(name, json);
        OnLoadSkinOnClients(name, json);
    }

    [ClientRpc]
    private void OnLoadSkinOnClients(string name, string json)
    {
        //Debug.Log($"OnLoadSkinOnClients {PlayerID} {name} {json}");
        _skinListener.Init(name, json);
    }

    [Command(requiresAuthority = false)]
    public void OnBulletCollidedOnServer()
    {
        OnBulletCollided?.Invoke();
        OnBulletCollidedOnClients();
    }

    [ClientRpc]
    private void OnBulletCollidedOnClients()
    {
        OnBulletCollided?.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void OnReloadOnServer()
    {
        OnReload?.Invoke();
        OnReloadOnClients();
    }

    [ClientRpc]
    private void OnReloadOnClients()
    {
        OnReload?.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void OnPjectileSpawnedOnServer(Vector3 endPosition, float simulationStepTime)
    {
        OnPjectileSpawned?.Invoke(endPosition, simulationStepTime);
        OnPjectileSpawnedOnClients(endPosition, simulationStepTime);
    }

    [ClientRpc]
    private void OnPjectileSpawnedOnClients(Vector3 endPosition, float simulationStepTime)
    {
        OnPjectileSpawned?.Invoke(endPosition, simulationStepTime);
    }

    [Command(requiresAuthority = false)]
    public void OnChangedSkinOnServer(string name, string json)
    {
        OnSkinChanged?.Invoke(name, json);
        OnChangedSkinOnClients(name, json);
    }

    [ClientRpc]
    private void OnChangedSkinOnClients(string name, string json)
    {
        OnSkinChanged?.Invoke(name, json);
    }

    [Command(requiresAuthority = false)]
    public void OnMagazineFastenOnServer(bool isFasten)
    {
        OnMagazineFasten?.Invoke(isFasten);
        OnMagazineFastenOnClients(isFasten);
    }

    [ClientRpc]
    private void OnMagazineFastenOnClients(bool isFasten)
    {
        OnMagazineFasten?.Invoke(isFasten);
    }

    [Command(requiresAuthority = false)]
    public void OnSetAttachmentOnServer(int scope, int muzzle, int laser, int grip, string sender)
    {
        //Debug.Log($"OnSetAttachmentOnServer {sender} scope: {scope} muzzle: {muzzle} laser: {laser} grip: {grip} ");
        OnSetAttachmentOnClients(scope, muzzle, laser, grip);
        OnAttachmentChanged?.Invoke(scope, muzzle, laser, grip);
    }

    [ClientRpc]
    private void OnSetAttachmentOnClients(int scope, int muzzle, int laser, int grip)
    {
        OnAttachmentChanged?.Invoke(scope, muzzle, laser, grip);
    }

    [Command(requiresAuthority = false)]
    public void OnFireOnServer()
    {
        OnFire?.Invoke();
        OnFireOnClients();
    }

    [ClientRpc]
    private void OnFireOnClients()
    {
        OnFire?.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void OnAimOnServer(bool isAim)
    {
        OnAim?.Invoke(isAim);
        OnAimOnClients(isAim);
    }

    [ClientRpc]
    private void OnAimOnClients(bool isAim)
    {
        OnAim?.Invoke(isAim);
    }

    [Command(requiresAuthority = false)]
    public void OnWeaponChangedOnServer(int indexOld, int indexNew)
    {
        //Debug.Log($"{_identity.netId} Call on server");
        OnWeaponChanged?.Invoke(indexOld, indexNew);
        OnWeaponChangedOnClients(indexOld, indexNew);
    }

    [ClientRpc]
    private void OnWeaponChangedOnClients(int indexOld, int indexNew)
    {
        //Debug.Log($"{_identity.netId} Call on client");
        OnWeaponChanged?.Invoke(indexOld, indexNew);
    }
}
