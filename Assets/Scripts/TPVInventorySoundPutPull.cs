using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TPVInventorySoundPutPull : MonoBehaviour
{
    /*[SerializeField]
    private PlayerNetworkResolver _resolver;*/

    private TPVInputController _input;
    [SerializeField]
    private AudioSource _putSound;
    [SerializeField]
    private AudioSource _pullSound;

    void Start()
    {
    }

    public void Init(TPVInputController input)
    {
        _input = input;

        _input.OnWeaponPut.AddListener(WeaponPut);
        _input.OnWeaponPull.AddListener(WeaponPull);
    }

    void Update()
    {

    }

    public void WeaponPut()
    {
        if (!_input.IsLocalPlayer)
        {
            _putSound.Stop();
            _pullSound.Stop();
            _putSound.Play();
        }
    }

    public void WeaponPull()
    {
        if (!_input.IsLocalPlayer)
        {
            _putSound.Stop();
            _pullSound.Stop();
            _pullSound.Play();
        }
    }
}
