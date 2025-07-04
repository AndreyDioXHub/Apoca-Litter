using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStarter : MonoBehaviour
{
    void Start()
    {
        ControlNetworkManager.singleton.StartServer();
    }
}
