using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class WeaponStatisticInfoScreenText : MonoBehaviour
{
    public string Damage = "0";
    public string ScopeIndex = "-";
    public string MuzzleIndex = "-";
    public string GripIndex = "-";
    public string Lazerndex = "-";
    public LocalizeStringEvent localizeString;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void UpdateAttachments(int damage, int scope, int mazzle, int gripp, int lazer)
    {
        Damage = damage < 10 ? $"0{damage}" : $"{damage}";
        ScopeIndex = scope < 10 ? $"0{scope}" : $"{scope}";
        MuzzleIndex = mazzle < 10 ? $"0{mazzle}" : $"{mazzle}";
        GripIndex = gripp < 10 ? $"0{gripp}" : $"{gripp}";
        Lazerndex = lazer < 10 ? $"0{lazer}" : $"{lazer}";
        localizeString.RefreshString();
    }
}
