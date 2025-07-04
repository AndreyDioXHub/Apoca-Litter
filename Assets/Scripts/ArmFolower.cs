using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmFolower : MonoBehaviour
{
    [SerializeField]
    private Transform _hand;

    void Start()
    {
    }

    void Update()
    {
        _hand.position = transform.position;
        _hand.eulerAngles = transform.eulerAngles;
    }
}
