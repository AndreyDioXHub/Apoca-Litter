using InfimaGames.LowPolyShooterPack.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffect : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;
    [SerializeField]
    private List<Transform> _bloodImpactPrefabs = new List<Transform>();
    [SerializeField]
    private List<Transform> _metalImpactPrefabs = new List<Transform>();
    [SerializeField]
    private List<Transform> _dirtImpactPrefabs = new List<Transform>();
    [SerializeField]
    private List<Transform> _drillImpactPrefabs = new List<Transform>();
    [SerializeField]
    private List<Transform> _concreteImpactPrefabs = new List<Transform>();
    [SerializeField]
    private List<Transform> _explosionImpactPrefabs = new List<Transform>();

    void Start()
    {
        _resolver.OnCollidedEffect.AddListener(RecieveDamage);
    }

    void Update()
    {

    }

    public virtual void RecieveDamage(Vector3 position, Vector3 normal, string tag)
    {
        switch (tag)
        {
            case "Blood":
                Instantiate(_bloodImpactPrefabs[Random.Range(0, _bloodImpactPrefabs.Count)], position, Quaternion.LookRotation(normal));
                break;
            case "Metal":
                Instantiate(_metalImpactPrefabs[Random.Range(0, _metalImpactPrefabs.Count)], position, Quaternion.LookRotation(normal));
                break;
            case "Dirt":
                Instantiate(_dirtImpactPrefabs[Random.Range(0, _dirtImpactPrefabs.Count)], position, Quaternion.LookRotation(normal));
                break;
            case "Drill":
                Instantiate(_drillImpactPrefabs[Random.Range(0, _drillImpactPrefabs.Count)], position, Quaternion.LookRotation(normal));
                break;
            case "Concrete":
                Instantiate(_concreteImpactPrefabs[Random.Range(0, _concreteImpactPrefabs.Count)], position, Quaternion.LookRotation(normal));
                break;
            case "Explosion":
                Instantiate(_explosionImpactPrefabs[Random.Range(0, _explosionImpactPrefabs.Count)], position, Quaternion.LookRotation(normal));
                break;
            default:
                break;
        }
    }
}

