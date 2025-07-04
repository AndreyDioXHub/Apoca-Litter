using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerResolver : MonoBehaviour
{
    public static AudioListenerResolver Instance { get; private set; }

    [SerializeField]
    private AudioListener _listener;

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

    public void DisableAudioListener()
    {
        _listener.enabled = false;
    }

    public void EnableAudioListener()
    {
        _listener.enabled = true;
    }
}
