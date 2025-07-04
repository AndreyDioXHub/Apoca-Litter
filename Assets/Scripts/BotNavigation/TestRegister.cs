using NewBotSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRegister : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    [ContextMenu("Register Player")]
    public void Register()
    {
        //BotManagerNetwork.Instance.RegisterPlayer(transform);
    }

    [ContextMenu("Remove Player")]
    public void RemovePlayer()
    {
        //BotManagerNetwork.Instance.RemovePlayer(transform);
    }
}
