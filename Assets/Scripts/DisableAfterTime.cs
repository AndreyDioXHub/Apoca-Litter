using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    [SerializeField]
    private float _time, _timeCur;

    void Start()
    {
        
    }

    void Update()
    {
        _timeCur += Time.deltaTime;
        if (_timeCur > _time)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        _timeCur = 0;
    }
}
