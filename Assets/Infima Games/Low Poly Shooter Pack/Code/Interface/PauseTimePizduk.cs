using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTimePizduk : MonoBehaviour
{

    public static PauseTimePizduk Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        //Time.timeScale = 0;
    }

    private void OnDisable()
    {
        //Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        //Time.timeScale = 1;
    }
}
