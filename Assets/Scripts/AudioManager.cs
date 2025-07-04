using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public enum AudioType
    {
        main,
        drill,
        music
    }

    [SerializeField]
    private AudioType managerType;

    [SerializeField]
    private GameObject _audioOn;
    [SerializeField]
    private GameObject _audioOff;
    [SerializeField]
    private Slider _audioSlider;
    [SerializeField]
    private TextMeshProUGUI _sliderValueText;
    [SerializeField]
    private float _volume = 1;
    [SerializeField]
    private bool _recoverAudioLVL;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        //Debug.Log("PlayerPrefs Init AudioManager");
        if (_recoverAudioLVL)
        {
            RecoverAudioLVL();
        }
        _audioSlider.onValueChanged.AddListener(UpdateVolume);
    }

    public void RecoverAudioLVL()
    {
        switch (managerType)
        {
            case AudioType.main:
                _volume = PlayerPrefs.GetFloat(PlayerPrefsConsts.audio, 1);
                break;
            case AudioType.drill:
                _volume = PlayerPrefs.GetFloat(PlayerPrefsConsts.audiodrill, 0.2f);
                break;
            case AudioType.music:
                _volume = PlayerPrefs.GetFloat(PlayerPrefsConsts.audiomusic, 0.5f);
                break;
        }

        _volume = _volume > 1 ? 1 : _volume;

        switch (managerType)
        {
            case AudioType.main:
                AudioListener.volume = _volume;
                break;
            case AudioType.drill:
                break;
            case AudioType.music:
                break;
        }

        if (_audioOn != null && _audioOff != null && _audioSlider != null)
        {
            if (_volume == 0)
            {
                _audioOn.SetActive(false);
                _audioOff.SetActive(true);
            }
            else
            {
                _audioOn.SetActive(true);
                _audioOff.SetActive(false);
            }
        }

        if (_audioSlider != null)
        {
            _audioSlider.value = _volume;
        }

        if(_sliderValueText != null)
        {
            _sliderValueText.text = $"{(int)(_volume * 100)}";
        }

        switch (managerType)
        {
            case AudioType.main:
                EventsBus.Instance?.OnAudioLVLChanged?.Invoke(_volume);
                break;
            case AudioType.drill:
                EventsBus.Instance?.OnAudioDrillLVLChanged?.Invoke(_volume);
                break;
            case AudioType.music:
                EventsBus.Instance?.OnAudioMusicLVLChanged?.Invoke(_volume);
                break;
        }
    }
    /*
    private void Update()
    {
        //Ебанейший колхоз который надо приявязать к загрузке уровня
         
        if(WinLose.Instance != null)
        {
            if (Time.timeSinceLevelLoad > WinLose.Instance.LoadingTime && !_recoverAudioLVL)
            {
                RecoverAudioLVL();
            }
        }
    }*/

    public void UpdateVolume(float value)
    {
        _volume = value;
        _volume = _volume > 1 ? 1 : _volume;

        switch (managerType)
        {
            case AudioType.main:
                PlayerPrefs.SetFloat(PlayerPrefsConsts.audio, _volume);
                break;
            case AudioType.drill:
                PlayerPrefs.SetFloat(PlayerPrefsConsts.audiodrill, _volume);
                break;
            case AudioType.music:
                PlayerPrefs.SetFloat(PlayerPrefsConsts.audiomusic, _volume);
                break;
        }

        switch (managerType)
        {
            case AudioType.main:
                AudioListener.volume = _volume;
                break;
            case AudioType.drill:
                break;
            case AudioType.music:
                break;
        }

        if (_audioSlider != null)
        {
            _audioSlider.value = _volume;
        }

        if (_audioOn != null && _audioOff != null)
        {
            if (_volume == 0)
            {
                _audioOn.SetActive(false);
                _audioOff.SetActive(true);
            }
            else
            {
                _audioOn.SetActive(true);
                _audioOff.SetActive(false);
            }
        }

        if (_sliderValueText != null)
        {
            _sliderValueText.text = $"{(int)(_volume * 100)}";
        }

        switch (managerType)
        {
            case AudioType.main:
                EventsBus.Instance?.OnAudioLVLChanged?.Invoke(_volume);
                break;
            case AudioType.drill:
                EventsBus.Instance?.OnAudioDrillLVLChanged?.Invoke(_volume);
                break;
            case AudioType.music:
                EventsBus.Instance?.OnAudioMusicLVLChanged?.Invoke(_volume);
                break;
        }
    }

    public void SwitchVolume()
    {
        if (_volume == 0)
        {
            _volume = 1;
        }
        else
        {
            _volume = 0;
        }

        UpdateVolume(_volume);

        _sliderValueText.text = $"{(int)(_volume * 100)}";
        EventsBus.Instance.OnAudioLVLChanged?.Invoke(_volume);
    }
}
