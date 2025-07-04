using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineController : MonoBehaviour
{

    [SerializeField]
    private CinemachineVirtualCamera _vcam;
    [SerializeField]
    private AimParameters _aiming;

    private float _sensity = 1, _sensityAim = 1;

    private CinemachinePOV _lookBase;
    private CinemachineFramingTransposer _bodyCinema;
    private CinemachinePOV _aimCinema;

    // Start is called before the first frame update
    void Awake()
    {
        _lookBase = _vcam.GetCinemachineComponent<CinemachinePOV>();
        _bodyCinema = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        _aimCinema = _vcam.GetCinemachineComponent<CinemachinePOV>();

        if (_lookBase == null)
        {
            this.enabled = false;
        }
    }

    private void Start()
    {
        _lookBase = _vcam.GetCinemachineComponent<CinemachinePOV>();

        _sensityAim = _aiming.sensityNorn;

        _lookBase.m_VerticalAxis.m_MaxSpeed = _sensity * _sensityAim * _aiming.maxRotateSpeed.y;
        _lookBase.m_HorizontalAxis.m_MaxSpeed = _sensity * _sensityAim * _aiming.maxRotateSpeed.x;

        _aimCinema.m_VerticalAxis.m_MinValue = -90;
        _aimCinema.m_VerticalAxis.m_MaxValue = 90;
    }

    public void OnPause()
    {
        SetSensitivity(0);
    }

    public void OnResume()
    {
        SetSensitivity(_aiming.sensityNorn);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _lookBase.m_VerticalAxis.m_MaxSpeed = _sensity * _sensityAim * _aiming.maxRotateSpeed.y;
        _lookBase.m_HorizontalAxis.m_MaxSpeed = _sensity * _sensityAim * _aiming.maxRotateSpeed.x;

    }

    public void SetAxisMinMaxValue(Vector2 axis)
    {
        _aimCinema.m_VerticalAxis.m_MinValue = axis.x;
        _aimCinema.m_VerticalAxis.m_MaxValue = axis.y;
    }

    public void SetSensitivity(float val)
    {
        if (_lookBase != null)
        {
            _sensity = val;
            _lookBase.m_VerticalAxis.m_MaxSpeed = val * _sensityAim * _aiming.maxRotateSpeed.y;
            _lookBase.m_HorizontalAxis.m_MaxSpeed = val * _sensityAim * _aiming.maxRotateSpeed.x;
        }
    }

    [Serializable]
    private class AimParameters
    {
        public Vector2 maxRotateSpeed = Vector2.zero;
        public Vector2 aimingNorm = Vector2.zero;
        public float _distanceNorm = 5f;
        public float _distanceAim = 3f;
        public float sensityNorn = 1f;
    }
}
