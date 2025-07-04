using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAttachementButton : MonoBehaviour
{

    [SerializeField]
    private ChangeAttachementScreen _screen;
    [SerializeField]
    private GameObject _active;
    [SerializeField]
    private AttachementType _type;
    [SerializeField]
    private int _index;

    void Start()
    {
        _screen = GetComponentInParent<ChangeAttachementScreen>();

        switch (_type)
        {
            case AttachementType.Scope:
                _screen.RegisterScope(this);
                break;
            case AttachementType.Muzzle:
                _screen.RegisterMuzzle(this);
                break;
            case AttachementType.Grip:
                _screen.RegisterGrip(this);
                break;
            case AttachementType.Laser:
                _screen.RegisterLaser(this);
                break;
        }
    }

    void Update()
    {
        
    }

    public void SetActiveState(bool isActive)
    {
        _active.SetActive(isActive);
    }

    public void ChangeWeapon()
    {

        switch (_type)
        {
            case AttachementType.Scope:
                _screen.SetScope(_index);
                break;
            case AttachementType.Muzzle:
                _screen.SetMuzzle(_index);
                break;
            case AttachementType.Grip:
                _screen.SetGrip(_index);
                break;
            case AttachementType.Laser:
                _screen.SetLaser(_index);
                break;
        }
    }

    public enum AttachementType
    {
        Scope,
        Muzzle,
        Grip,
        Laser
    }
}
