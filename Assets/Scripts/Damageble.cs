using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.Events;

public class Damageble : MonoBehaviour
{
    public float DamageMultiplier => _damageMultiplier;

    //public uint PlayerID => _resolver.PlayerID;

    [SerializeField]
    protected PlayerNetworkResolver _resolver;

    [SerializeField]
    protected float _damageMultiplier = 1;

    public virtual void Start()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public void Init()
    {

    }

    public virtual void ReceiveDamage(float damage, uint sender, bool multyplierEffect = true)
    {
        uint receiver = _resolver.PlayerID;
        float damageMultiplier = multyplierEffect ? _damageMultiplier : 1;
        _resolver.OnReceiveDamageOnServer(damage * damageMultiplier, sender, receiver);
    }
}
