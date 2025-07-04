using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrapBase : MonoBehaviour
{
    public UnityEvent<Collider> OnEnter = new UnityEvent<Collider>();
    public UnityEvent<Collider> OnExit = new UnityEvent<Collider>();
    public UnityEvent<Collider> OnStay = new UnityEvent<Collider>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            OnEnter?.Invoke(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            OnExit?.Invoke(other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            OnStay?.Invoke(other);
        }
    }
}
