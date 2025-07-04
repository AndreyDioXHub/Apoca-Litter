using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;
    void Start()
    {
        itemPrefab = Resources.Load<GameObject>("RandomItem");
    }

    void Update()
    {
        
    }

    public void Sleep(GameObject target, GameObject killer)
    {
        GameObject item = Instantiate(itemPrefab);
        item.transform.position = transform.position;
    }
}
