using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerRoomCamera : MonoBehaviour
{
    public static DealerRoomCamera Instance;
    public Camera DealerCamera;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
