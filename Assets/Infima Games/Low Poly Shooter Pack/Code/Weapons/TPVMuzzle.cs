using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

public class TPVMuzzle : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;

    private GameObject _parentWeapon;
    [SerializeField]
    private int _flashParticlesCount = 5;
    [SerializeField]
    private float _flashTime = 0.1f;
    [SerializeField]
    private ParticleSystem _particles;
    [SerializeField]
    private Light _flashLight;
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private GameObject _tpvBulletPrefab;
    [SerializeField]
    private Transform _socket;

    void Start()
    {
    }

    void Update()
    {

    }

    public void Init(GameObject parentWeapon)
    {
        _parentWeapon = parentWeapon;

        if(_resolver == null)
        {
            //Debug.Log("TPVMuzzle resolver is null");
        }
        else
        {
            _resolver.OnFire.AddListener(Effect);
            _resolver.OnPjectileSpawned.AddListener(SpawnTPVBullet);
        }
    }

    public async void Effect()
    {
        if (_parentWeapon.activeSelf)
        {
            if (!_resolver.IsLocalPlayer)
            {
                if(_audioSource != null)
                {
                    //Debug.Log("_audioSource.Play();");
                    _audioSource.Play();
                }

                if (_particles != null)
                {
                    _particles.Emit(_flashParticlesCount);
                }

                if (_flashLight != null)
                {
                    _flashLight.enabled = true;
                    await UniTask.WaitForSeconds(_flashTime);
                    _flashLight.enabled = false;
                }
            }
        }
    }

    public void SpawnTPVBullet(Vector3 endPosition, float simulationStepTime)
    {
        if (_parentWeapon.activeSelf)
        {
            if (!_resolver.IsLocalPlayer)
            {

                var go = Instantiate(_tpvBulletPrefab);

                go.transform.position = _socket.position;
                go.GetComponent<TPVProjectileTrajectory>().Init(endPosition, simulationStepTime, _resolver);

                //Debug.Log($"SpawnTPVBullet {go.name} {gameObject.activeSelf}");
            }
        }
    }
}
