using InfimaGames.LowPolyShooterPack;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZombieImpackt : NetworkBehaviour
{
    [SerializeField]
    private Character _character;
    [SerializeField]
    private PlayerNetworkResolver _resolver;

    [SerializeField, SyncVar]
    private float _damageForSendFronServerToClients;

    [SerializeField, SyncVar]
    private uint _senderForSendFronServerToClients;

    [SerializeField, SyncVar]
    private uint _receiverForSendFronServerToClients;

    [SerializeField, SyncVar(hook = nameof(ChangeRecieveDamageChanged))]
    private bool _isChangeRecieveDamage;

    public bool IsChangeRecieveDamage
    {
        get
        {
            return _isChangeRecieveDamage;
        }
        set
        {
            _isChangeRecieveDamage = value;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReceiveDamage(float damage, uint sender, uint receiver)
    {
        //Debug.Log($"ReceiveDamage {NetworkClient.active}");

        if (NetworkClient.active)
        {
            //OnReceiveDamageOnServer(damage, sender, receiver);
            if(isServer)
            {
                if (_character != null)
                {
                    _character.ReceiveDamage(damage, sender, receiver);
                }
            }
        }
        else
        {
            IsChangeRecieveDamage = !IsChangeRecieveDamage;
            _damageForSendFronServerToClients = damage;
            _senderForSendFronServerToClients = sender;
            _receiverForSendFronServerToClients = receiver;
        }
    }
    public void ChangeRecieveDamageChanged(bool oldValue, bool newValue)
    {
        //Debug.Log($"ChangeRecieveDamageChanged {oldValue} => {newValue}");

        if (_character != null)
        {
            _character.ReceiveDamage(_damageForSendFronServerToClients, _senderForSendFronServerToClients, _receiverForSendFronServerToClients);
        }
        //OnReceiveDamage?.Invoke(_damageForSendFronServerToClients, _senderForSendFronServerToClients, _receiverForSendFronServerToClients);
    }
}
