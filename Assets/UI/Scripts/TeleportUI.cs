using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportUI : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    void Start()
    {
        LateSubscribe();
    }

    public async void LateSubscribe()
    {
        while (EventsBus.Instance == null)
        {
            await UniTask.Yield();
        }

        EventsBus.Instance.OnTeleport.AddListener(EnableTeleportScreen);
    }

    public void EnableTeleportScreen()
    {
        _animator.SetTrigger("Trigger");
    }

    void Update()
    {
        
    }
}
