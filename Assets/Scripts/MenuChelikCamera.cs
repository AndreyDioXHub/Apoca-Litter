using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChelikCamera : MonoBehaviour
{
    public static MenuChelikCamera Instance;

    public Camera ChelikCamera => _camera;

    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Transform _menuPosition;
    [SerializeField]
    private Transform _inventoryPosition;
    [SerializeField]
    private Vector3 _positionCurent;
    [SerializeField]
    private Vector3 _position;
    [SerializeField] 
    private float _speed = 1f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _positionCurent = _inventoryPosition.position;
        _position = _menuPosition.position;
    }

    void Update()
    {
        _camera.transform.position = Vector3.MoveTowards(_camera.transform.position, _position, _speed * Time.deltaTime);
    }

    public void MoveToInventoryPosition()
    {
        _position = _inventoryPosition.position;
        _positionCurent = _camera.transform.position;
        //_camera.transform.position = _position;
    }

    public void MoveToMenuPosition()
    {
        _position = _menuPosition.position;
        _positionCurent = _camera.transform.position;
        //_camera.transform.position = _position;
    }
}
