using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField]
    private float _despawnTime = 10.0f;

    [SerializeField]
    private float _lightDuration = 0.02f;
    [SerializeField]
    private Light _lightFlash;

    [SerializeField]
    private AudioClip[] _explosionSounds;
    [SerializeField]
    private AudioSource _audioSource;

    private void Start()
    {
    }

    private void OnEnable()
    {
        DestroyTimer();
        LightFlash();

        int explosionIndex = Random.Range(0, _explosionSounds.Length);
        _audioSource.clip = _explosionSounds[explosionIndex];
        _audioSource.Play();

    }

    private async void LightFlash()
    {
        if (_lightFlash)
        {
            _lightFlash.GetComponent<Light>().enabled = true;
            await UniTask.WaitForSeconds(_lightDuration);
            _lightFlash.GetComponent<Light>().enabled = false;
        }
    }

    private async void DestroyTimer()
    {
        await UniTask.WaitForSeconds(Time.fixedDeltaTime);
        EventsBus.Instance.OnExplosionCenter?.Invoke(transform.position);
        await UniTask.WaitForSeconds(_despawnTime);
        Destroy(gameObject);

    }
}
