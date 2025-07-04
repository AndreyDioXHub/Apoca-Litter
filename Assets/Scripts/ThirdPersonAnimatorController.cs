using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class ThirdPersonAnimatorController : MonoBehaviour
{

    [SerializeField]
    private TPVInputController _input;
    [SerializeField]
    private TPVInventory _inventory;
    [SerializeField]
    private TPVInventorySoundPutPull _inventoryPutPullSound;

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Animator _animatorThermometerAim;
    [SerializeField]
    private Animator _animatorThermometerFall;
    [SerializeField]
    private Animator _animatorThermometerCrouch;
    [SerializeField]
    private Animator _animatorThermometerRun;
    [SerializeField]
    private Animator _animatorThermometerPutPull;
    [SerializeField]
    private Transform _thermometerAimFloat;
    [SerializeField]
    private Transform _thermometerFallFloat;
    [SerializeField]
    private Transform _thermometerCrouchFloat;
    [SerializeField]
    private Transform _thermometerRunFloat;
    [SerializeField]
    private Transform _thermometerPutPullFloat;
    [SerializeField]
    private int _wasd;
    private float _reloadLayerWeight = 0;

    [SerializeField]
    private Rig _rigARFiring;
    [SerializeField]
    private Rig _rigARArmFollower;

    AnimatorClipInfo[] _infosBase;
    AnimatorClipInfo[] _infosRun;
    AnimatorClipInfo[] _infosRunUp;

    private bool _isAim;
    private bool _needAim;

    private int _baseLayer = 0;
    private int _crouchLayer = 0;
    private int _crouchUpLayer = 0;
    private int _walkLayer = 0;
    private int _walkUpLayer = 0;
    private int _runLayer = 0;
    private int _runUpLayer = 0;
    private int _fallLayer = 0;

    void Start()
    {
        _inventory.Init(_input);
        _inventoryPutPullSound.Init(_input);

        _baseLayer = _animator.GetLayerIndex("Base");

        _crouchLayer = _animator.GetLayerIndex("Crouch");
        _crouchUpLayer = _animator.GetLayerIndex("CrouchUP");

        _walkLayer = _animator.GetLayerIndex("Walk");
        _walkUpLayer = _animator.GetLayerIndex("WalkUp");

        _runLayer = _animator.GetLayerIndex("Run");
        _runUpLayer = _animator.GetLayerIndex("RunUp");
        
        _fallLayer = _animator.GetLayerIndex("Fall");

        _infosBase = _animator.GetCurrentAnimatorClipInfo(_baseLayer);
        _infosRun = _animator.GetCurrentAnimatorClipInfo(_runLayer);
        _infosRunUp = _animator.GetCurrentAnimatorClipInfo(_runUpLayer);

        _input.OnReload.AddListener(OnReloading);
        _input.OnAim.AddListener(OnAim);
        _input.OnWeaponChanged.AddListener(OnWeaponChanged);
    }

    public void OnWeaponChanged(int indexOld, int indexNew)
    {
        PlayClip("Rifle Put Away", _baseLayer, _infosBase);
        _animatorThermometerPutPull.SetBool("IsActive", true);
    }

    public void OnWeaponPut()
    {
        //Debug.Log("OnWeaponPut");
        _input.OnWeaponPut?.Invoke();
    }

    public void OnMagazineInArm()
    {
        //Debug.Log("OnMagazineInArm");
        _input.OnMagazineInArm?.Invoke();
    }

    public void OnWeaponPull()
    {
        //Debug.Log("OnWeaponPull");
        _input.OnWeaponPull?.Invoke();
        _animatorThermometerPutPull.SetBool("IsActive", false);
    }

    public void OnReloadingOpen()
    {
        _input.OnReloadingOpen?.Invoke();
    }

    public void OnReloadingInsert()
    {
        _input.OnReloadingInsert?.Invoke();
    }

    public void OnBoltAction()
    {
        _input.OnBoltAction?.Invoke();
    }

    void Update()
    {
        _needAim = _isAim || _input.IsHoldingButtonFire;
        _needAim = _needAim && _reloadLayerWeight == 0;

        _animatorThermometerAim.SetBool("IsActive", _needAim);
        _animatorThermometerFall.SetBool("IsActive", _input.IsGrounded);
        _animatorThermometerCrouch.SetBool("IsActive", _input.IsCrouch);
        _animatorThermometerRun.SetBool("IsActive", _input.IsRun);

        _animator.SetInteger("WASD", _input.WASD);

        _animator.SetBool("IsAim", _isAim);
        _animator.SetBool("IsReloading", _reloadLayerWeight > 0);

        float rigARArmFollowerFloat = (1 - _thermometerPutPullFloat.localPosition.y) - _reloadLayerWeight;
        rigARArmFollowerFloat = rigARArmFollowerFloat < 0 ? 0 : rigARArmFollowerFloat;
        _rigARArmFollower.weight = rigARArmFollowerFloat;

        float ARFiringWeight = _thermometerAimFloat.localPosition.y;
        ARFiringWeight = ARFiringWeight < 0.45f ? 0.45f : ARFiringWeight;

        _rigARFiring.weight = ARFiringWeight;

        float crouchLayerFloat = _thermometerCrouchFloat.localPosition.y * _thermometerFallFloat.localPosition.y;
        crouchLayerFloat = crouchLayerFloat < 0 ? 0 : crouchLayerFloat;

        float runLayerFloat = _thermometerFallFloat.localPosition.y * _thermometerRunFloat.localPosition.y;

        float walkLayerFloat = 1 - crouchLayerFloat;
        walkLayerFloat = walkLayerFloat - runLayerFloat;
        walkLayerFloat = walkLayerFloat<0 ? 0 : walkLayerFloat;

        _animator.SetLayerWeight(_crouchLayer, crouchLayerFloat);
        _animator.SetLayerWeight(_walkLayer, walkLayerFloat);
        _animator.SetLayerWeight(_runLayer, runLayerFloat);
        _animator.SetLayerWeight(_fallLayer, 1 - _thermometerFallFloat.localPosition.y);

        _reloadLayerWeight = _animator.GetFloat("ReloadingLayerWeight");
        _animator.SetLayerWeight(_runUpLayer, (1 - _reloadLayerWeight) * runLayerFloat * (1 - _thermometerAimFloat.localPosition.y));
        _animator.SetLayerWeight(_walkUpLayer, (1 - _reloadLayerWeight) * walkLayerFloat * (1 - _thermometerAimFloat.localPosition.y));
        _animator.SetLayerWeight(_crouchUpLayer, (1 - _reloadLayerWeight) * crouchLayerFloat * (1 - _thermometerAimFloat.localPosition.y));
    }

    public void OnAim(bool isAim)
    {
        _isAim = isAim;
    }

    public void PlayClip(string clip, int layer, AnimatorClipInfo[] infos)
    {
        if (infos.Length > 0)
        {

            if (infos[0].clip.name.Equals(clip))
            {
            }
            else
            {
                _animator.CrossFade(clip, 0.05f, layer, 0);
            }
        }
    }

    public void OnReloading()
    {
        PlayClip("Reloading", _baseLayer, _infosBase);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        float x = context.ReadValue<Vector2>().x;
        x = x > 0 ? 1 : x;
        x = x < 0 ? -1 : x;

        float y = context.ReadValue<Vector2>().y;
        y = y > 0 ? 1 : y;
        y = y < 0 ? -1 : y;

        int w = y > 0 ? 8 : 0;
        int a = x < 0 ? 4 : 0;
        int s = y < 0 ? 2 : 0;
        int d = x > 0 ? 1 : 0;

        _wasd = w | a | s | d;

        _input.WASD = _wasd;
    }
}
