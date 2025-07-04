using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using static UnityEngine.Rendering.DebugUI;

public class TimerText : MonoBehaviour
{
    public string Value;
    public LocalizeStringEvent localizeString;

    void Start()
    {
        localizeString.RefreshString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateValue(string value)
    {
        Value = value;
        localizeString.RefreshString();
    }
}
