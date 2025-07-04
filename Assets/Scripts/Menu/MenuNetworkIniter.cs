using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNetworkIniter : MonoBehaviour
{
    [SerializeField]
    private ChelikiMenuModel _model;

    void Start()
    {
        LateStart();
    }

    private async void LateStart()
    {
        await UniTask.Yield();
        _model.Init(null, null, null);
    }

    void Update()
    {
        
    }
}
