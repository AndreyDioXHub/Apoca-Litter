using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance;

    [SerializeField]
    private GameObject _explosionPrefab;
    
    [SerializeField]
    private List<GameObject> _explosions = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public GameObject GetExplosion(Vector3 position, Vector3 normal)
    {
        bool botSpawned = false;

        foreach (var explosion in _explosions)
        {
            if (!explosion.activeSelf)
            {
                explosion.transform.position = position;
                explosion.transform.rotation = Quaternion.LookRotation(normal);
                explosion.SetActive(true);
                return explosion;
            }
        }

        if (!botSpawned)
        {
            var explosion = Instantiate(_explosionPrefab);
            explosion.transform.position = position;
            explosion.transform.rotation = Quaternion.LookRotation(normal);
            explosion.SetActive(true);
            _explosions.Add(_explosionPrefab);
            return explosion;
        }

        return null;
    }
}
