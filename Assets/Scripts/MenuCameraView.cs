using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraView : MonoBehaviour
{
    [SerializeField]
    private Transform _camera;

    [SerializeField]
    private Vector2 _maxMinAngleX;
    [SerializeField]
    private Vector2 _maxMinAngleY;
    [SerializeField]
    protected float _mouseSensitivity = 300f;
    private float _mouseX, _mouseY, _mouseXL, _mouseYL, time, timeCur;
    [SerializeField]
    protected bool _lockMouse;
    [SerializeField]
    protected bool _isClose;



    void Start()
    {
        time = 1;

        if (_lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (_isClose)
        {
            timeCur -= Time.deltaTime;
            timeCur = timeCur < 0 ? 0 : timeCur; 

            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

            _mouseY -= mouseY;
            _mouseY = Mathf.Clamp(_mouseY, _maxMinAngleY.x, _maxMinAngleY.y);

            _mouseX += mouseX;
            _mouseX = Mathf.Clamp(_mouseX, _maxMinAngleX.x, _maxMinAngleX.y);
            _mouseXL = _mouseX;
            _mouseYL = _mouseY;

        }
        else
        {
            timeCur += Time.deltaTime;
            timeCur = timeCur > time ? time : timeCur;

            _mouseY = _mouseYL - _mouseYL * timeCur / time;
            _mouseX = _mouseXL - _mouseXL * timeCur / time;
        }

        _camera.localPosition = Vector3.Lerp(new Vector3(0, 0, -1), new Vector3(0, 0, -2.5f), timeCur / time);
        transform.localRotation = Quaternion.Euler(_mouseY, _mouseX, 0f);
    }


    public void LookGun()
    {
        _isClose = true;
        _mouseSensitivity *= 4;
    }

    public void LeaveGun()
    {
        _isClose = false;
        _mouseSensitivity /= 4;
    }

}
