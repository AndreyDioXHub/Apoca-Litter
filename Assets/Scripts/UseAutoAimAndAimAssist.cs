using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAutoAimAndAimAssist : MonoBehaviour
{
    [SerializeField]
    private GameObject _aimOn;
    [SerializeField]
    private GameObject _aimOff;

    [SerializeField]
    private GameObject _assistOn;
    [SerializeField]
    private GameObject _assistOff;

    [SerializeField]
    private bool _useAutoAim;
    [SerializeField]
    private bool _useAimAssist;

    void Start()
    {
        _useAutoAim = PlayerPrefs.GetInt(PlayerPrefsConsts.autoaim, 1) == 1;
        _useAimAssist = PlayerPrefs.GetInt(PlayerPrefsConsts.aimassist, 1) == 1;

        _aimOn.SetActive(_useAutoAim);
        _aimOff.SetActive(!_useAutoAim);

        _assistOn.SetActive(_useAimAssist);
        _assistOff.SetActive(!_useAimAssist);

        if(EventsBus.Instance != null)
        {
            EventsBus.Instance.OnUseAutoAim?.Invoke(_useAutoAim);
            EventsBus.Instance.OnUseAimAssist?.Invoke(_useAimAssist);
        }
    }

    void Update()
    {
        
    }

    public void SwitchAutoAim()
    {
        _useAutoAim = !_useAutoAim;
        _aimOn.SetActive(_useAutoAim);
        _aimOff.SetActive(!_useAutoAim);
        PlayerPrefs.SetInt(PlayerPrefsConsts.autoaim, _useAutoAim ? 1 : 0);
        EventsBus.Instance.OnUseAutoAim?.Invoke(_useAutoAim);
    }

    public void SwitchAimAssist()
    {
        _useAimAssist = !_useAimAssist;
        _assistOn.SetActive(_useAimAssist);
        _assistOff.SetActive(!_useAimAssist);
        PlayerPrefs.SetInt(PlayerPrefsConsts.aimassist, _useAimAssist ? 1 : 0);
        EventsBus.Instance.OnUseAimAssist?.Invoke(_useAimAssist);
    }
}
