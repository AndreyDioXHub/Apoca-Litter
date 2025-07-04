using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using Mirror.Examples.Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.HID;
using NewBotSystem;

public class PauseScreen : MonoBehaviour
{
    public static PauseScreen Instance;


    [SerializeField]
    private TextMeshProUGUI _text;

    /* public bool IsInventory => _isInventory;
     public bool IsPrivacy => _isPrivacy;
     public bool IsTerm => _isTerm;
     public bool IsSetting => _isSetting;*/

    public UnityEvent<bool> OnPaused = new UnityEvent<bool>();
    /*public UnityEvent OnPressEsc = new UnityEvent();
    public UnityEvent OnPressTab = new UnityEvent();*/

    public bool IsPaused => _isPaused;

    [SerializeField]
    private bool _isPaused = false;
    [SerializeField]
    private bool _isInventory = false;
    [SerializeField]
    private bool _isPrivacy = false;
    [SerializeField]
    private bool _isFailed = false;
    [SerializeField]
    private bool _isComplete = false;
    [SerializeField]
    private bool _isTerm = false;
    [SerializeField]
    private bool _isSetting = false;
    [SerializeField]
    private bool _isChat = false;
    [SerializeField]
    private bool _isWaiting = false;
    [SerializeField]
    private bool _isDeth = false;
    [SerializeField]
    private bool _isDealer = false;

    [SerializeField]
    private List<CanvasGroup> _hidingCanvasGroups = new List<CanvasGroup>();
    [SerializeField]
    private PlayerTrackActivator _tracker;
    [SerializeField]
    private CharackterHPSliderReciever _hpSlider;
    [SerializeField]
    private WaitingScreen _waitingScreen;
    [SerializeField]
    private GameObject _weaponScreen;
    [SerializeField]
    private GameObject _menu;
    [SerializeField]
    private GameObject _menuBG;
    /*
    [SerializeField]
    private AudioManager _audioManagerMain;
    [SerializeField]
    private AudioManager _audioManagerDrill;
    [SerializeField]
    private MouseSensitivityManager _sensitivityManager;
    */
    [SerializeField]
    private ChatViewExpander _chatViewExpander;
    [SerializeField]
    private GameObject _firstTimeContinueButton;
    [SerializeField]
    private MenuRetexture _pausedRenderTexture;
    [SerializeField]
    private MenuRetexture _dethRenderTexture;
    [SerializeField]
    private GameObject _pausedCamUI;
    [SerializeField]
    private GameObject _dethUI;
    [SerializeField]
    private GameObject _loseScreen;
    [SerializeField]
    private List<RoundsTextInfo> _rondsAddedInfo = new List<RoundsTextInfo>();
    [SerializeField]
    private Button _playButtonWaitingScreen;
    [SerializeField]
    private Button _respawnButtonWaitingScreen;
    [SerializeField]
    private Button _respawnButton;
    [SerializeField]
    private Button _leaveButton;
    [SerializeField]
    private Animator _damageAnimator;
    [SerializeField]
    private DealerScreen _dealerScreenUI;
    [SerializeField]
    private FindChestTextView _findChestText;
    [SerializeField]
    private ChestTaskTimerTextView _chestTaskTimer;

    private PlayerNetworkResolver _resolver;
    private Character _character;
    private Inventory _inventory;
    private ChatScreenUIPause _chatScreenUI;
    private ChangeAttachementScreen _attachementScreen;
    private ChelikiMenuModel _settingScreen;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //WaitingPlayer();
    }

    public void Init(PlayerNetworkResolver resolver, Character character, Inventory inventory)
    {
        _resolver = resolver;
        _character = character;
        _inventory = inventory;

        _character.OnPressEsc.AddListener(CloseScreen);
        _character.OnPressTab.AddListener(PressTab);
        _character.OnInsideDealer.AddListener(InsideDealer);
        _character.OnPressChat.AddListener(PressChat);
        _character.OnDie.AddListener(()=> _dethUI.SetActive(true));
        _character.OnDie.AddListener(()=> _loseScreen.SetActive(true));

        _dealerScreenUI.Init(_resolver, _character, _inventory);
        _tracker.Init(_resolver.PlayerCamera, _character);
        _hpSlider.AddListenerOnCharackter(_character);
        _waitingScreen.Init(_resolver);

        _pausedRenderTexture.SetCamera(_resolver.DethCamera);
        _dethRenderTexture.SetCamera(_resolver.DethCamera);

        //WaitingScreenModel.Instance.OnAllPlayersDead.AddListener(() => _loseScreen.SetActive(true));

        foreach (var roundInfo in _rondsAddedInfo)
        {
            _inventory.OnAmmoAdded.AddListener(roundInfo.AmmoAdded);
        }

        _respawnButtonWaitingScreen.onClick.AddListener(RealyRespawn);
        _respawnButton.onClick.AddListener(Respawn);
        _leaveButton.onClick.AddListener(LeaveGame);

        _findChestText.Init(_resolver);
        _chestTaskTimer.Init(_resolver);

        _character.OnDamageDeltaRecived.AddListener(DamageDeltaRecived);
        _character.OnFirstTimeWeaponSettings.AddListener(() => _weaponScreen.SetActive(true));
        _character.OnFirstTimeWeaponSettings.AddListener(LateEnableMenuBG);
        _character.OnFirstTimeWeaponSettings.AddListener(() => _firstTimeContinueButton.SetActive(true));
        OnPaused.AddListener(ShowHidePausedCam);

        _inventory.OnAmmoVestIsEmpty.AddListener(ShowWhenNoAmmo);
        /*
        _chatViewExpander.OnExpanded.AddListener(OnChatExpanded);
        _chatViewExpander.OnMinimized.AddListener(OnChatMinimized);*/
    }

    public async void LateEnableMenuBG()
    {
        await UniTask.WaitForSeconds(2*Time.fixedDeltaTime);
        /*_audioManagerMain.Init();
        _audioManagerDrill.Init();
        _sensitivityManager.Init();*/
        _menu.SetActive(false);
        _menuBG.SetActive(true);

    }

    public async void DamageDeltaRecived(float damage)
    {
        if(damage < 0)
        {
            _damageAnimator.SetBool("Heal", true);
        }
        else
        {
            _damageAnimator.SetTrigger("Damage");
        }

        await UniTask.WaitForSeconds(Time.fixedDeltaTime);
        _damageAnimator.SetBool("Heal", false);
    }

    public void RegisterChat(ChatScreenUIPause chatScreenUI)
    {
        _chatScreenUI = chatScreenUI;
    }

    public void RegisterSettings(ChelikiMenuModel settingScreen)
    {
        _settingScreen = settingScreen;
    }

    public void RegisterAttachementScreen(ChangeAttachementScreen attachementScreen)
    {
        _attachementScreen = attachementScreen;
    }

    private async void WaitingPlayer()
    {
        /*
        while (Character.Instance == null)
        {
            await UniTask.Yield(); // ќжидание следующего кадра
        }*/
    }

    public void PressTab()
    {
        if (_isChat)
        {
            _chatScreenUI.Hide();
        }

        if (_isSetting)
        {
            _settingScreen.Hide();
        }


        _attachementScreen.ShowHide();
    }

    public void ShowWhenNoAmmo()
    {
        //_attachementScreen.ShowWhenNoAmmo();
    }

    public void SwapToKnife()
    {
        //_attachementScreen.ShowWhenNoAmmo();
    }

    public void InsideDealer(DealerZone dealerZone)
    {
        _dealerScreenUI.Show();//.SetActive(true);
        _dealerScreenUI.SetLocalPlayer(_character.gameObject);
        _dealerScreenUI.SetDealerZone(dealerZone);
    }

    public void PressChat()
    {
        if (_isInventory)
        {
        }
        else
        {
            _chatScreenUI.Show();
        }
    }

    public void CloseScreen()
    {
        if(_isPaused)
        {
            if (_isInventory)
            {
                _attachementScreen.Hide();
            }

            if (_isChat)
            {
                _chatScreenUI.Hide();
            }

            if (_isSetting)
            {
                _settingScreen.Hide();
            }

            if (_isDealer)
            {
                _dealerScreenUI.Hide();
            }
        }
        else
        {
            _settingScreen.ShowHide();
        }
    }

    void Update()
    {
        if (_character != null)
        {
            _text.text = SceneManager.GetActiveScene().name + " : " + _character.transform.position.ToString();
            float alpha = _character.IsAiming() ? 0 : 1;
            alpha = _isPaused ? 1 : alpha;
            alpha = _resolver.IsDead ? 1 : alpha;

            foreach(var canvasGroup in _hidingCanvasGroups)
            {
                canvasGroup.alpha = alpha;
            }
        }
    }

    public void DisableCamera()
    {
        _character.DepthCamera.SetActive(!_character.DepthCamera.activeSelf);
    }

    public void SetActiveDillerScreen(bool state)
    {
        _isDealer = state;
        _isPaused = _isSetting || _isInventory || _isPrivacy || _isTerm || _isFailed
            || _isComplete || _isChat || _isWaiting || _isDeth || _isDealer;
        SetPause(_isPaused);
    }
    public void SetActiveDethCam(bool state)
    {
        _isDeth = state;
        _isPaused = _isSetting || _isInventory || _isPrivacy || _isTerm || _isFailed
            || _isComplete || _isChat || _isWaiting || _isDeth || _isDealer;
        SetPause(_isPaused);
    }

    public void SetActiveInGameSettings(bool state)
    {
        _isSetting = state;
        _isPaused = _isSetting || _isInventory || _isPrivacy || _isTerm || _isFailed
            || _isComplete || _isChat || _isWaiting || _isDeth || _isDealer;
        SetPause(_isPaused);
    }

    public void SetActiveInventoryScreen(bool state)
    {
        _isInventory = state;
        _isPaused = _isSetting || _isInventory || _isPrivacy || _isTerm || _isFailed
            || _isComplete || _isChat || _isWaiting || _isDeth || _isDealer;
        SetPause(_isPaused);
    }

    public void SetActiveChatScreen(bool state)
    {
        _isChat = state;
        _isPaused = _isSetting || _isInventory || _isPrivacy || _isTerm || _isFailed
            || _isComplete || _isChat || _isWaiting || _isDeth || _isDealer;
        SetPause(_isPaused);
    }

    public void SetActivePrivacyPolicyPanelScreen(bool state)
    {
        _isPrivacy = state;
        _isPaused = _isSetting || _isInventory || _isPrivacy || _isTerm || _isFailed
            || _isComplete || _isChat || _isWaiting || _isDeth || _isDealer;
        SetPause(_isPaused);
    }

    public void SetActiveTermOfUsePanelScreen(bool state)
    {
        _isTerm = state;
        _isPaused = _isSetting || _isInventory || _isPrivacy || _isTerm || _isFailed
            || _isComplete || _isChat || _isWaiting || _isDeth || _isDealer;
        SetPause(_isPaused);
    }

    public void SetActiveFailedScreen(bool state)
    {
        _isFailed = state;
        _isPaused = _isSetting || _isInventory || _isPrivacy || _isTerm || _isFailed
            || _isComplete || _isChat || _isWaiting || _isDeth || _isDealer;
        SetPause(_isPaused);
    }

    public void SetActiveCompeteScreen(bool state)
    {
        _isComplete = state;
        _isPaused = _isSetting || _isInventory || _isPrivacy || _isTerm || _isFailed
            || _isComplete || _isChat || _isWaiting || _isDeth || _isDealer;
        SetPause(_isPaused);
    }

    public void SetActiveWaitingScreen(bool state)
    {
        _isWaiting = state;
        _isPaused = _isSetting || _isInventory || _isPrivacy || _isTerm || _isFailed
            || _isComplete || _isChat || _isWaiting || _isDeth || _isDealer;
        SetPause(_isPaused);
    }

    public void ShowHidePausedCam(bool isPaused)
    {
        _pausedCamUI.SetActive(isPaused);
    }

    /*
    public void OnChatMinimized()
    {
        _chatCamUI.SetActive(false);
    }*/

    /*
    public void PressEsc()
    {
        OnPressEsc?.Invoke();
        //SetPause(!_isPaused);
    }*/

    public void SetPause(bool isPaused)
    {
        _isPaused = isPaused;
        OnPaused?.Invoke(_isPaused);
    }

    public void Respawn()
    {
        //_resolver.HealPlayerAndRecoverOnServer();
        _playButtonWaitingScreen.gameObject.SetActive(false);
        _respawnButtonWaitingScreen.gameObject.SetActive(true);
        _dethUI.SetActive(false);
        _loseScreen.SetActive(false);
        _waitingScreen.gameObject.SetActive(true);
    }

    public void RealyRespawn()
    {
        _resolver.HealPlayerAndRecoverOnServer();
        _playButtonWaitingScreen.onClick?.Invoke();
    }

    public void LeaveGame()
    {
        ControlNetworkManager.singleton.StopClient();

        if (_resolver.IsServer)
        {
            ControlNetworkManager.singleton.StopServer();
        }
    }
}
