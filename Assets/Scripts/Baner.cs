using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baner : MonoBehaviour
{
    [SerializeField]
    private Transform _camera;

    void Start()
    {
        WaitingPlayer();
    }

    private async void WaitingPlayer()
    {
        while (Inventory.Instance == null)
        {
            await UniTask.Yield(); // ќжидание следующего кадра
        }

        _camera = Camera.main.transform;
    }


    void Update()
    {
        if (_camera != null)
        {
            transform.rotation = _camera.rotation;
        }
    }
}
