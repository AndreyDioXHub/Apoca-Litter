using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;
    [SerializeField]
    private CharacterController _controller;
    [SerializeField]
    private Vector2 _center;
    [SerializeField]
    private Vector2 _curentPosition;
    [SerializeField]
    private float _radius;
    [SerializeField]
    private float _distance;
    [SerializeField]
    private LayerMask _mask;
    [SerializeField]
    private bool _isOnTeleportState;


    void Start()
    {
        
    }

    void Update()
    {
        if (_resolver.IsLocalPlayer)
        {
            if (_isOnTeleportState)
            {

            }
            else
            {
                if (transform.position.y < -200)
                {
                    _isOnTeleportState = true;
                    StartTeleporting();
                }
            }

            /*
            _curentPosition = new Vector2(transform.position.x, transform.position.z);
            _distance = Vector2.Distance(_curentPosition, _center);

            if (_isOnTeleportState)
            {

            }
            else
            {
                if (_distance > _radius)
                {
                    _isOnTeleportState = true;
                    StartTeleporting();
                }

                if (transform.position.y < -200)
                {
                    _isOnTeleportState = true;
                    StartTeleporting();
                }
            }*/
        }
    }

    public async void FastTeleportToPosition(Vector3 position)
    {
        _controller.enabled = false;
        transform.position = position;

        _controller.enabled = true;

        await UniTask.WaitForSeconds(Time.deltaTime);

        _isOnTeleportState = false;


        /*
        _center = new Vector2(position.x, position.z);

        try
        {
            _controller.enabled = false;

            RaycastHit hit;

            if (Physics.Raycast(new Vector3(_center.x, 200, _center.y), Vector3.down, out hit, Mathf.Infinity, _mask))
            {
                transform.position = hit.point;
            }

            _controller.enabled = true;

            await UniTask.WaitForSeconds(Time.deltaTime);

            _isOnTeleportState = false;
        }
        catch (MissingReferenceException e)
        {

        }*/

    }


    public void TeleportToSpawn(int index)
    {
        _center = new Vector2(GameWorld.Instance.SpawnPoints[index].position.x, GameWorld.Instance.SpawnPoints[index].position.z);
        StartTeleporting();
    }

    public async void StartTeleporting()
    {
        EventsBus.Instance.OnTeleport?.Invoke();

        await UniTask.WaitForSeconds(2);
        try
        {
            _controller.enabled = false;

            RaycastHit hit;

            if (Physics.Raycast(new Vector3(_center.x, 200, _center.y), Vector3.down, out hit, Mathf.Infinity, _mask))
            {
                transform.position = hit.point;
            }

            _controller.enabled = true;

            await UniTask.WaitForSeconds(Time.deltaTime);

            _isOnTeleportState = false;
        }
        catch (MissingReferenceException e)
        {

        }

    }
}
