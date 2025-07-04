using Cysharp.Threading.Tasks;
using MagicPigGames.Northstar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class ChestMarkerUIDistance : MonoBehaviour
{
    public OverlayIcon Icon;
    public string Distance;
    public LocalizeStringEvent localizeString;

    void Start()
    {
        SlowUpdate();
    }

    void Update()
    {
        
    }

    public async void SlowUpdate()
    {
        while (true)
        {
            await UniTask.WaitForSeconds(0.1f);
            UpdateDistance((int)Icon.DistanceToTarget);
        }
    }

    public void UpdateDistance(int distance)
    {
        Distance = distance.ToString();
        localizeString.RefreshString();
    }
}
