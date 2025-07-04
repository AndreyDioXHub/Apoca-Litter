using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System;
using game.configuration;
using System.Threading.Tasks;
using NewBotSystem;

public class WinLose : MonoBehaviour
{
    public static WinLose Instance;

    public UnityEvent OnWin = new UnityEvent();
    public UnityEvent OnFall = new UnityEvent();

    /// <summary>
    /// Get from @GameMenuPanelLocator
    /// </summary>
    private GameObject _winPanel;
    private GameObject _losePanel;

    private Transform _player;
    [SerializeField]
    private int _playerKillsCount = 0;

    private bool _calc = false;

    private bool _needUnmute = true;

    private HashSet<int> _alreadyKilled = new HashSet<int>();

    private void Awake()
    {
        Instance = this;

    }

    async void Start()
    {
        //StartCoroutine(LoadingCoroutine());
        await WaitPlayer();
        //OnWin.AddListener(() => MoveToCenterDelay(1));
        //OnFall.AddListener(ReturnToCenter);
        Character.Instance.OnDie.AddListener(ShowLosePanel);
        _winPanel = GameMenuPanelLocator.Instance.WinPanel;
        _losePanel = GameMenuPanelLocator.Instance.LosePanel;
        EventsBus.Instance.OnBotDieFromKiller.AddListener(CountBot);

        EventsBus.Instance.OnSandBoxStart.AddListener(SandBoxAborted);
        EventsBus.Instance.OnSandBoxAborted.AddListener(SandBoxAborted);

        int killsInLevel = GameConfig.CurrentConfiguration.GameMode == GameConfig.GameModes.Sandbox ? BotManager.Instance.KillingCountSandBox : GameConfig.CurrentConfiguration.KillsInLevel;

        EventsBus.Instance.OnEnemyCount?.Invoke(_playerKillsCount, killsInLevel);
    }

    private void SandBoxAborted()
    {
        _playerKillsCount = 0;
    }

    private void CountBot(GameObject bot, GameObject killer)
    {
        int killsInLevel = GameConfig.CurrentConfiguration.GameMode == GameConfig.GameModes.Sandbox ? 
            BotManager.Instance.KillingCountSandBox : 
            GameConfig.CurrentConfiguration.KillsInLevel;

        //TODO
        if (_alreadyKilled.Add(bot.GetInstanceID()))
        {
            if (killer == _player.gameObject)
            {
                _playerKillsCount++;

                EventsBus.Instance.OnEnemyCount?.Invoke(_playerKillsCount, killsInLevel);
            }
            else
            {
                BotManager.Instance.KillingCountSandBox--; 

                killsInLevel = GameConfig.CurrentConfiguration.GameMode == GameConfig.GameModes.Sandbox ?
                    BotManager.Instance.KillingCountSandBox :
                    GameConfig.CurrentConfiguration.KillsInLevel;

                EventsBus.Instance.OnEnemyCount?.Invoke(_playerKillsCount, killsInLevel);
            }
        }
        else
        {

        }

        bool win = _playerKillsCount >= killsInLevel;
        win = GameConfig.CurrentConfiguration.GameMode == GameConfig.GameModes.Sandbox ?
            BotManager.Instance.SandboxReady && win : win;


        if (win) 
        {
            //Show Win panel
            //OnWin?.Invoke();
            _winPanel.SetActive(true);
            GameConfig.CurrentConfiguration.Level = GameConfig.CurrentConfiguration.CurrentScene.LevelIndex + 1;
        }
    }
    private void OnDisable()
    {
        EventsBus.Instance?.OnBotDieFromKiller.RemoveListener(CountBot);
    }

    private void ShowLosePanel() 
    {
        _losePanel.SetActive(true);
        OnFall?.Invoke();
    }

    private async Task WaitPlayer()
    {
        await UniTask.WaitUntil(() => Character.Instance != null);
        await UniTask.WaitUntil(() => Character.Instance.gameObject.transform != null);

        _player = Character.Instance.gameObject.transform;
    }

    void FixedUpdate()
    {/*
        if (_needUnmute && _loadingBar != null)
        {
            _loadingAmount += Time.fixedDeltaTime;

            _loadingAmount = _loadingAmount > 100 ? 100 : _loadingAmount;
            _loadingAmount = Time.timeSinceLevelLoad > 14 ? 100 : _loadingAmount;
            _loadingBar.fillAmount = _loadingAmount / 100;
        }*/

        if (_player != null)
        {
            if (_calc)
            {
                if (_player.position.y < 0)
                {
                    OnFall?.Invoke();

                }
            }                
        }

        if(_player!= null)
        {
            if (_calc)
            {
                if (Vector2.Distance(new Vector2(_player.position.x, _player.position.z), new Vector2(128, 128)) > 65)
                {
                    //_calc = false;
                    OnWin?.Invoke();
                }
            }
        }
    }

    public void ReturnToCenter()
    {
        _player.gameObject.SetActive(false);
        _player.position = new Vector3(128, 40, 128);
        _player.gameObject.SetActive(true);
    }

    public void MoveToCenterDelay(float delay)
    {
        StartCoroutine(LoadSceneDelayCoroutine(delay));
    }

    IEnumerator LoadSceneDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (_player != null)
        {
            _player.GetComponent<CharacterController>().enabled = false;
            _player.transform.position = new Vector3(128, _player.transform.position.y, 128);
            _player.GetComponent<CharacterController>().enabled = true;
            //gameObject.SetActive(false);
        }
    }

    public void Loaded()
    {
        AudioListener.volume = PlayerPrefs.GetFloat(PlayerPrefsConsts.audio, 1);
    }

    public void SaveProgress()
    {
        //PlayerPrefs.SetFloat("PlayerPrefsConsts", Time.timeSinceLevelLoad);
    } 
}
