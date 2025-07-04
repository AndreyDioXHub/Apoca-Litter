using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInitializer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _preSpawnPrefabs;
    [SerializeField]
    private List<GameObject> _preSpawnedPrefabs;


    void Start()
    {
        
        foreach(var prefab in _preSpawnPrefabs)
        {
            var go = Instantiate(prefab);
            _preSpawnedPrefabs.Add(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > 1)
        {
            for(int i=0; i< _preSpawnedPrefabs.Count; i++)
            {
                Destroy(_preSpawnedPrefabs[i]);
            }

            //Debug.Break();
            Destroy(this);
        }
    }
}
