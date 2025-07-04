using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XInput;

public class TPVInputController : MonoBehaviour
{

    public int CurrentWeaponIndex
    {
        get
        {
            if (_resolver == null)
            {
                return 0;
            }
            else
            {
                return _resolver.CurrentWeaponIndex;
            }
        }
    }

    public bool IsLocalPlayer
    {
        get
        {
            if (_resolver == null)
            {
                return false;
            }
            else
            {
                return _resolver.IsLocalPlayer;
            }
        }
    }

    public bool IsHoldingButtonFire
    {
        get
        {
            if (_resolver == null)
            {
                return _isHoldingButtonFire;
            }
            else
            {
                return _resolver.IsHoldingButtonFire;
            }
        }
    }

    private bool _isHoldingButtonFire;

    public bool IsGrounded
    {
        get
        {
            if (_resolver == null)
            {
                return true;
            }
            else
            {
                return _resolver.IsGrounded;
            }
        }
    }

    public bool IsCrouch
    {
        get
        {
            if (_resolver == null)
            {
                return false;
            }
            else
            {
                return _resolver.IsCrouch;
            }
        }
    }

    public bool IsRun
    {
        get
        {
            if (_resolver == null)
            {
                return false;
            }
            else
            {
                return _resolver.IsRun;
            }
        }
    }

    public int WASD
    {
        get
        {
            if (_resolver == null)
            {
                return 8;
            }
            else
            {
                return _resolver.WASD;
            }
        }
        set
        {
            if (_resolver == null)
            {
            }
            else
            {
                _resolver.WASD = value;
            }
        }
    }

    public UnityEvent<int, int> OnWeaponChanged = new UnityEvent<int, int>(); //old new

    public UnityEvent<int, int, int, int> OnAttachmentChanged = new UnityEvent<int, int, int, int>(); //old new
    public UnityEvent<GameObject> OnWeaponSelected = new UnityEvent<GameObject>();
    public UnityEvent<WeaponAttachmentManager> OnWeaponAttachmentManagerSelected = new UnityEvent<WeaponAttachmentManager>();

    public UnityEvent<bool> OnAim = new UnityEvent<bool>();
    public UnityEvent<bool> OnMagazineFasten = new UnityEvent<bool>();
    public UnityEvent OnReload = new UnityEvent();
    public UnityEvent OnFire = new UnityEvent();

    //local animator events
    public UnityEvent OnWeaponPut = new UnityEvent();
    public UnityEvent OnMagazineInArm = new UnityEvent();
    public UnityEvent OnWeaponPull = new UnityEvent();

    public UnityEvent OnReloadingOpen = new UnityEvent();
    public UnityEvent OnReloadingInsert = new UnityEvent();
    public UnityEvent OnBoltAction = new UnityEvent();

    [SerializeField]
    private PlayerNetworkResolver _resolver;

    private void Awake()
    {

    }

    void Start()
    {
        if (_resolver == null)
        {
            //OnWeaponPut.AddListener(()=> _isHoldingButtonFire=false);
            OnWeaponPull.AddListener(LateHoldingButtonFire);
        }
        else
        {
            _resolver.OnWeaponChanged.AddListener(WeaponChanged);

            _resolver.OnWeaponSelected.AddListener(WeaponSelected);

            _resolver.OnAttachmentChanged.AddListener(AttachmentChanged);

            _resolver.OnWeaponAttachmentManagerSelected.AddListener(WeaponAttachmentManagerSelected);

            _resolver.OnAim.AddListener(Aim);

            _resolver.OnMagazineFasten.AddListener(MagazineFasten);

            _resolver.OnReload.AddListener(() => OnReload?.Invoke());
            _resolver.OnFire.AddListener(() => OnFire?.Invoke());
        }
    }


    public void RelaxHoldingButtonFire()
    {
        _isHoldingButtonFire = false;
    }

    public async void LateHoldingButtonFire()
    {
        await UniTask.WaitForSeconds(0.3f);
        _isHoldingButtonFire = true;
    }

    void Update()
    {

    }

    public void WeaponChanged(int old, int neww)
    {
        OnWeaponChanged?.Invoke(old, neww);
    }

    public void WeaponSelected(GameObject weaponGO)
    {
        OnWeaponSelected?.Invoke(weaponGO);
    }

    public void WeaponAttachmentManagerSelected(WeaponAttachmentManager am)
    {
        OnWeaponAttachmentManagerSelected?.Invoke(am);
    }

    public void Aim(bool isaim)
    {
        OnAim?.Invoke(isaim);
    }

    public void MagazineFasten(bool isfasten)
    {
        OnMagazineFasten?.Invoke(isfasten);
    }

    public void AttachmentChanged(int scope, int muzzle, int laser, int grip)
    {
        OnAttachmentChanged?.Invoke(scope, muzzle, laser, grip);
    }
}
