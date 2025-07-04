using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _lvl;
    [SerializeField]
    private float _time = 14, _timeCur;
    [SerializeField]
    private List<GameObject> _gameObjects = new List<GameObject>();

    void Start()
    {
        foreach(var go in _gameObjects)
        {
            var gos = Instantiate(go);
            Destroy(gos, 5);
        }
    }

    void Update()
    {
        _timeCur += Time.deltaTime;

        if(_timeCur > _time)
        {
            _lvl.SetActive(true);
            Destroy(this);
        }


    }
}
