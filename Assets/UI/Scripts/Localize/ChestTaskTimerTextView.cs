using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class ChestTaskTimerTextView : MonoBehaviour
{
    public string ChestTaskTimer;
    public LocalizeStringEvent localizeString;
    [SerializeField]
    private PlayerNetworkResolver _resolver;

    [SerializeField]
    private int _time = 0;

    void Start()
    {
        
    }

    void Update()
    {
        /*ChestTaskTimer = Random.Range(0,10).ToString();
        localizeString.RefreshString();*/
    }

    public void Init(PlayerNetworkResolver resolver)
    {
        _resolver = resolver;
        _resolver.OnGetTask.AddListener(ShowTimer);

        _resolver.OnFailTask.AddListener(HideTimer);
        _resolver.OnPushTask.AddListener(HideTimer);

        _resolver.OnGetTask.AddListener(StartTimer);

        _resolver.OnFailTask.AddListener(StopTimer);
        _resolver.OnPushTask.AddListener(StopTimer);
    }

    public async void StartTimer()
    {
        _time = (int)_resolver.DealerZoneForTask.TaskTime;
        string timeText = _time.ToString();

        while (_time >= 0)
        {
            if(_time >= 0 && _time < 10)
            {
                timeText = $"00{_time}";
            }
            else if(_time >= 10 && _time < 100)
            {
                timeText = $"0{_time}";
            }
            else
            {
                timeText = $"{_time}";
            }

            ChestTaskTimer = timeText;
            localizeString.RefreshString();
            await UniTask.WaitForSeconds(1);
            _time--;
        }

        timeText = $"000";
        ChestTaskTimer = timeText;
    }

    public void StopTimer()
    {
        _time = 0;
    }

    public void ShowTimer()
    {
        gameObject.SetActive(true);
    }

    public void HideTimer()
    {
        gameObject.SetActive(false);
    }
}
