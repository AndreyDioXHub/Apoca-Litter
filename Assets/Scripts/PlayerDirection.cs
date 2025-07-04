using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerDirection : MonoBehaviour
{/*
    [SerializeField]
    private Rig _rigLayerWalk;
    [SerializeField]
    private Rig _rigLayerMelee;
    [SerializeField]
    private Rig _rigLayerRange;*/

    [SerializeField]
    private Animator _animatorDir;
    /*[SerializeField]
    private Animator _animatorPlayer;*/
    [SerializeField]
    private Transform _dir, _model;
    [SerializeField]
    private bool _w, _a, _s, _d;
    [SerializeField]
    private int _wasd;
    [SerializeField]
    private Vector3 _horizontalDirection;
    [SerializeField]
    private float _blend;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {/*
        if (_resolver.IsLocalPlayer)
        {
            _resolver.PDirWASD = _wasd;
        }
        else
        {
            _wasd = _resolver.PDirWASD;
        }*/

        float layerWalkSubtract = 0;

        //layerWalkSubtract = _inventory.CurentWeapon.AtackLayerWeight;

        /*
        if (_resolver.AnimatorSwordLayerWeight > 0 || _resolver.AimatorBowLayerWeight > 0)
        {
            layerWalkSubtract = _resolver.AnimatorSwordLayerWeight > _resolver.AimatorBowLayerWeight ? _resolver.AnimatorSwordLayerWeight : _resolver.AimatorBowLayerWeight;
        }
        */

        //_rigLayerWalk.weight = 1 - layerWalkSubtract;

        _w = (_wasd & 8) != 0;
        _a = (_wasd & 4) != 0;
        _s = (_wasd & 2) != 0;
        _d = (_wasd & 1) != 0;

        _animatorDir.SetBool("W", _w);
        _animatorDir.SetBool("A", _a);
        _animatorDir.SetBool("S", _s);
        _animatorDir.SetBool("D", _d);
        _animatorDir.SetLayerWeight(_animatorDir.GetLayerIndex("Atack Layer"), layerWalkSubtract);

        _blend = _dir.localPosition.y;
        _horizontalDirection = _dir.position;
        _horizontalDirection.y = _model.position.y;

        //_animatorPlayer.SetFloat("WS Blend", _blend);
        /*
        if(_character.IsClimb || _character.IsGlueWall)
        {
            _horizontalDirection = _model.position + _character.ClimbDirection;
            _rigLayerWalk.weight = 0;
        }*/
        /*
        _rigLayerMelee.weight = _inventory.CurentWeapon.MeleeFloat;
        _rigLayerRange.weight = _inventory.CurentWeapon.RangeFloat;
        */
        _model.LookAt(_horizontalDirection, Vector3.up);

        /*
        if (_blackController.IsGround)
        {
            //Debug.Log("IsGround");
        }
        else if (_blackController.IsOnLadder)
        {
            //Debug.Log("IsOnLadder");
            _rigLayerWalk.weight = 0;
            _model.LookAt(_model.position + _blackController.LadderDirection, Vector3.up);
        }
        else if (_blackController.IsClimb)
        {
            //Debug.Log("IsClimb");
            _rigLayerWalk.weight = 0;
            _model.LookAt(_model.position + _blackController.ClimbDirection, Vector3.up);
        }*/
    }

    public void SetLayerMeleeWeight(float weight)
    {
        //_rigLayerMelee.weight = weight;
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
    }
}
