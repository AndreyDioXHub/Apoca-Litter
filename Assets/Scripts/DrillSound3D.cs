using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillSound3D : MonoBehaviour
{

    [SerializeField]
    private AudioSource _emptyStart;
    [SerializeField]
    private AudioSource _emptyMid;
    [SerializeField]
    private AudioSource _emptyEnd;
    
    [SerializeField]
    private bool _isOn;
    [SerializeField]
    private float _volume;


    void Start()
    {
        _volume = PlayerPrefs.GetFloat(PlayerPrefsConsts.audiodrill, 1);
        UpdateVolume(_volume);
        LateSubscribe();
    }

    public async void LateSubscribe()
    {
        while (EventsBus.Instance == null)
        {
            await UniTask.Yield();
        }

        EventsBus.Instance.OnAudioDrillLVLChanged.AddListener(UpdateVolume);
    }

    public void UpdateVolume(float volume)
    {
        _volume = volume;
        _emptyStart.volume = volume;
        _emptyMid.volume = volume;
        _emptyEnd.volume = volume;
    }

    void Update()
    {
        
    }

    public async void DrillOn()
    {
        if (_isOn)
        {

        }
        else
        {
            _isOn = true;
            _emptyStart.Play();
            await UniTask.WaitForSeconds(1.76f);
            while (_isOn)
            {
                _emptyMid.Play();
                await UniTask.WaitForSeconds(1.95f);
            }
        }
    }

    public async void DrillOff()
    {
        if (_isOn)
        {
            _isOn = false;
            _emptyEnd.Play();
            await UniTask.WaitForSeconds(0.3f);
            _emptyMid.Stop();
        }
        else
        {
        }
    }
}
