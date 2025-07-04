using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    public UnityEvent OnReward;

    [SerializeField]
    private int _reward = 100;

    public bool IsPaused { get; private set; }
    public string TimeString { get
        {
            string result = "";

            if (_timeCur < 10)
            {
                result = $"0{ (int)_timeCur }/60";
            }
            else
            {
                result = $"{ (int)_timeCur }/60";
            }
            //_timeCur.ToString(".#");
            /*
            double t = Math.Round(_timeCur, 2);
            int timeD = (int)t;
            string timestring = $"{timeD}:{t - timeD}";*/
            return result;
        }
    }

    [SerializeField]
    private float _time;
    [SerializeField]
    private float _timeCur;
    [SerializeField]
    private  int _timeCurPrev;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        IsPaused = PlayerPrefs.GetInt(PlayerPrefsConsts.adsPause, 0) == 1;
        _timeCur = PlayerPrefs.GetFloat(PlayerPrefsConsts.adsTimer, 0);
        //_time = _infoYG.fullscreenAdInterval; 
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("Game") || scene.name.Equals("Menu"))
        {
            //_sdk._FullscreenShow();
        }
        //do stuff
    }

    public void ShowRewardedAd()
    {
        //_sdk._FullscreenShow();
        //_sdk._RewardedShow(1);
    }

    public void GetReward()
    {
        OnReward?.Invoke();
        CoinsManager.Instance.AddCoins(_reward);
        IsPaused = true;
    }

    void Update()
    {
        if (IsPaused)
        {
            _timeCur += Time.deltaTime;

            if(_timeCur - _timeCur%1 > _timeCurPrev)
            {
                Debug.Log("_timeCur");
                _timeCurPrev = (int)_timeCur;
                PlayerPrefs.SetFloat(PlayerPrefsConsts.adsTimer, _timeCur);
                PlayerPrefs.SetInt(PlayerPrefsConsts.adsPause, 1);
            }

            if(_timeCur > _time)
            {
                _timeCur = 0;
                IsPaused = false;

                PlayerPrefs.SetFloat(PlayerPrefsConsts.adsTimer, 0);
                PlayerPrefs.SetInt(PlayerPrefsConsts.adsPause, 0);

            }
        }
    }
}
