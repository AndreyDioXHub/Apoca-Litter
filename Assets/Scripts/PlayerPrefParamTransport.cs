using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefParamTransport : MonoBehaviour
{
    [SerializeField]
    private string _key;
    [SerializeField]
    private string _value;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetPrefs()
    {
        PlayerPrefs.SetString( _key, _value );  
    }
}
