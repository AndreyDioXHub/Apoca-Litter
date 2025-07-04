using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class CheckPointSelectTextView : MonoBehaviour
{
    public string PointIndex;
    public LocalizeStringEvent localizeString;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Init(int pointIndex)
    {
        PointIndex = (pointIndex + 1).ToString();

        localizeString.RefreshString();
    }
}
