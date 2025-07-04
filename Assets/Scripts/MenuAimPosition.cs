using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAimPosition : MonoBehaviour
{
    public static MenuAimPosition Instance;

    [SerializeField]
    private Transform _menuPosition;
    [SerializeField]
    private Transform _inventoryPosition;
    [SerializeField]
    private Vector3 _positionCurent;
    [SerializeField]
    private Vector3 _position;

    private float _speed = 20;

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
        transform.position = Vector3.MoveTowards(transform.position, _position, _speed * Time.deltaTime);
    }

    public void MoveToInventoryPosition()
    {
        _position = _inventoryPosition.position;
        _positionCurent = transform.position;
    }

    public void MoveToMenuPosition()
    {
        _position = _menuPosition.position;
        _positionCurent = transform.position;
    }
}
