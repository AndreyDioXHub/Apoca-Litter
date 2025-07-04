using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAimButtonListener : MonoBehaviour
{
    [SerializeField]
    private bool _useAutoAim;

    void Start()
    {
        _useAutoAim = PlayerPrefs.GetInt(PlayerPrefsConsts.autoaim, 1) == 1;

        if (EventsBus.Instance != null)
        {
            EventsBus.Instance.OnUseAutoAim.AddListener(UpdateUseAutoAim);
        }

        gameObject.SetActive(!_useAutoAim);
    }

    public void UpdateUseAutoAim(bool useAutoAim)
    {
        _useAutoAim = useAutoAim;
        gameObject.SetActive(!_useAutoAim);
    }

    void Update()
    {
        
    }
}
