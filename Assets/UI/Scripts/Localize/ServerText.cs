using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class ServerText : MonoBehaviour
{
    public string Value;
    public LocalizeStringEvent localizeString;

    public void UpdateValue(string value)
    {
        Value = value;
        localizeString.RefreshString();
    }
}
