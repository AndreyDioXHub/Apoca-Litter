using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
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

        EventsBus.Instance.OnExplosionCenter.AddListener(EnableTeleportScreen);
    }

    public void EnableTeleportScreen(Vector3 center)
    {
        float distance = Vector3.Distance(transform.position, center);

        if (distance < 10)
        {
            _animator.SetTrigger("Trigger");
        }
    }
}
