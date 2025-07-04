using NewBotSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WallDetector : MonoBehaviour
{
    public UnityEvent<List<Vector3>> OnWallDetected = new UnityEvent<List<Vector3>>();

    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private bool _closeTheWall;
    private float _distanceToWall = 0.5f;
    [SerializeField]
    private bool forward1, forward2, forward3;

    void Start()
    {
        
    }

    void Update()
    {
        List<Vector3> contactPoints = new List<Vector3>();
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 0.75f, transform.forward, out hit, _distanceToWall, _layerMask))
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.75f, transform.forward * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
            forward1 = true;
            contactPoints.Add(hit.point);
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.75f, transform.forward * _distanceToWall, Color.white);
            //Debug.Log("Did not Hit");
            forward1 = false;
        }

        if (Physics.Raycast(transform.position + Vector3.up * 1.25f, transform.forward, out hit, _distanceToWall, _layerMask))
        {
            Debug.DrawRay(transform.position + Vector3.up * 1.25f, transform.forward * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
            forward2 = true;
            contactPoints.Add(hit.point);
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 1.25f, transform.forward * _distanceToWall, Color.white);
            //Debug.Log("Did not Hit");
            forward2 = false;
        }

        if (Physics.Raycast(transform.position + Vector3.up * 1.75f, transform.forward, out hit, _distanceToWall, _layerMask))
        {
            Debug.DrawRay(transform.position + Vector3.up * 1.75f, transform.forward * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
            forward3 = true;
            contactPoints.Add(hit.point);
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 1.75f, transform.forward * _distanceToWall, Color.white);
            //Debug.Log("Did not Hit");
            forward3 = false;
        }

        if(!_closeTheWall && (forward1 || forward2 || forward3))
        {
            Debug.Log("OnWallDetected");
            OnWallDetected?.Invoke(contactPoints);
        }

        _closeTheWall = forward1 || forward2 || forward3;
    }
}
