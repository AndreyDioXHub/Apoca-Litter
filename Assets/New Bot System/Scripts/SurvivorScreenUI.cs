using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NewBotSystem;
using System;
using game.configuration;
using static game.configuration.GameConfig;

public class SurvivorScreenUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _hpBar;
    [SerializeField]
    private GameObject _prepareWaveGO;
    [SerializeField]
    private GameObject _endingPrepareWaveGO;
    [SerializeField]
    private GameObject _waveCountDownGO;
    [SerializeField]
    private GameObject _waveStartGO;
    [SerializeField]
    private GameObject _waveEndedGO;
    [SerializeField]
    private GameObject _waveEnemyCount;
    [SerializeField]
    private TextMeshProUGUI _waveCountDownText;
    [SerializeField]
    private TextMeshProUGUI _waveStartText;
    [SerializeField]
    private TextMeshProUGUI _waveEndedText;
    [SerializeField]
    private TextMeshProUGUI _waveEnemyCountText;

    void Start()
    {
        EventsBus.Instance.OnEnemyCount.AddListener(WaveEnemyCount);
        EventsBus.Instance.OnSandBoxStart.AddListener(SandBoxStart);
        EventsBus.Instance.OnSandBoxAborted.AddListener(SandBoxAborted);

        BotManager.Instance.OnSurvivorPrepare.AddListener(PrepareWave);
        BotManager.Instance.OnTimeCountDown.AddListener(WaveCountDown);
        BotManager.Instance.OnWaveStarted.AddListener(WaveStarted);
        BotManager.Instance.OnWaveEnded.AddListener(WaveEnded);
        BotManager.Instance.OnWavePaused.AddListener(WavePaused);

        _hpBar.SetActive(false);
        _prepareWaveGO.SetActive(false);
    }

    void Update()
    {
    }

    [ContextMenu("SandBoxStart")]
    public void StartSanboxMission()
    {
        BotManager.Instance.StartSanboxMission();
    }

    [ContextMenu("SandBoxAborted")]
    public void AbortSanboxMission()
    {
        BotManager.Instance.AbortSanboxMission();
        BotManager.Instance.ClearSandboxBiomass();
    }

    public void SandBoxStart()
    {
        _hpBar.SetActive(true);
        _waveEnemyCount.SetActive(true);

        _prepareWaveGO.SetActive(false);
        _waveCountDownGO.SetActive(false);
        _waveStartGO.SetActive(false);
        _waveEndedGO.SetActive(false);
        _endingPrepareWaveGO.SetActive(false);
    }

    public void SandBoxAborted()
    {
        _hpBar.SetActive(false);
        _waveCountDownGO.SetActive(false);
        _waveStartGO.SetActive(false);
        _waveEndedGO.SetActive(false);
        _prepareWaveGO.SetActive(false);
        _waveEnemyCount.SetActive(false);

        _endingPrepareWaveGO.SetActive(true);

        _waveEnemyCountText.text = $"{0}/{0}";
    }

    public void PrepareWave()
    {
        Debug.Log("PrepareWave");

        _hpBar.SetActive(false);
        _waveCountDownGO.SetActive(false);
        _waveStartGO.SetActive(false);
        _waveEndedGO.SetActive(false);
        _waveEnemyCount.SetActive(false);

        if (GameConfig.CurrentConfiguration.GameMode == GameModes.Sandbox)
        {
            _endingPrepareWaveGO.SetActive(true);
            _waveEnemyCount.SetActive(false);
        }
        else
        {
            _prepareWaveGO.SetActive(true);
            _waveEnemyCount.SetActive(true);
        }
    }

    public void WaveCountDown(int timer, int index, float maxtime)
    {
        _hpBar.SetActive(false);
        _prepareWaveGO.SetActive(false);
        _waveCountDownGO.SetActive(true);
        _waveStartGO.SetActive(false);
        _waveEndedGO.SetActive(false);

        if (GameConfig.CurrentConfiguration.GameMode == GameModes.Sandbox)
        {
            _waveEnemyCount.SetActive(false);
        }
        else
        {
            _waveEnemyCount.SetActive(true);
        }

        _waveCountDownText.text = $"Wave {index}  starts in \r\n{(int)(maxtime - timer)}";
    }

    public void WaveStarted(int index)
    {
        StartCoroutine(WaveStartedCoroutine(index));
    }

    IEnumerator WaveStartedCoroutine(int index)
    {

        _prepareWaveGO.SetActive(false);
        _waveCountDownGO.SetActive(false);
        _waveStartGO.SetActive(true);
        _waveEndedGO.SetActive(false);

        if (GameConfig.CurrentConfiguration.GameMode == GameModes.Sandbox)
        {
            _waveEnemyCount.SetActive(false);
        }
        else
        {
            _waveEnemyCount.SetActive(true);
        }

        _waveStartText.text = $"Wave {index} is complete.";

        yield return new WaitForSeconds(1);

        _hpBar.SetActive(true);
        _prepareWaveGO.SetActive(false);
        _waveCountDownGO.SetActive(false);
        _waveStartGO.SetActive(false);
        _waveEndedGO.SetActive(false);
    }

    public void WaveEnded(int index, float time, int timeCurPrev)
    {
        _hpBar.SetActive(false);
        _prepareWaveGO.SetActive(false);
        _waveCountDownGO.SetActive(false);
        _waveStartGO.SetActive(false);

        if (GameConfig.CurrentConfiguration.GameMode == GameModes.Sandbox)
        {
            _waveEnemyCount.SetActive(false);
        }
        else
        {
            _waveEnemyCount.SetActive(true);
        }

        _waveEndedGO.SetActive(true);

        _waveEndedText.text = $"Wave {index} completed.\nSupply update {(int)time - timeCurPrev}";
    }

    public void WavePaused(float time)
    {
        _hpBar.SetActive(true);
        _prepareWaveGO.SetActive(false);
        _waveCountDownGO.SetActive(false);
        _waveStartGO.SetActive(false);

        if (GameConfig.CurrentConfiguration.GameMode == GameModes.Sandbox)
        {
            _waveEnemyCount.SetActive(false);
        }
        else
        {
            _waveEnemyCount.SetActive(true);
        }

        _waveEndedGO.SetActive(false);
    }

    public void WaveEnemyCount(int countCur, int countMax)
    {
        /*
        _prepareWaveGO.SetActive(false);
        _waveCountDownGO.SetActive(false);
        _waveStartGO.SetActive(false);
        //_waveEnemyCount.SetActive(true);
        _waveEndedGO.SetActive(false);*/

        _waveEnemyCountText.text = $"{countCur}/{countMax}";
    }
}
