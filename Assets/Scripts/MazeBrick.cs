using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBrick : MonoBehaviour
{

    [SerializeField]
    private GameObject _main;
    [SerializeField]
    private GameObject _state0;
    [SerializeField]
    private GameObject _state1;
    [SerializeField]
    private GameObject _state2;
    [SerializeField]
    private GameObject _state3;
    [SerializeField]
    private GameObject _state4;
    [SerializeField]
    private GameObject _state5;

    [SerializeField]
    private float _hitPointsMax = 100;
    [SerializeField]
    private float _hitPoints;
    [SerializeField]
    private int _hpNormalized;

    public void ReceiveDamage(float damage, Vector3 position, Vector3 normal)
    {
        Debug.Log($"MazeBrick ReceiveDamage {damage}");

        _hitPoints-= damage;
        _hitPoints = _hitPoints < 0 ? 0 : _hitPoints;

        _hpNormalized = (int)((_hitPoints / _hitPointsMax) * 100);

        bool s0 = _hpNormalized >= 100;
        bool s1 = _hpNormalized > 80 && _hpNormalized < 100;
        bool s2 = _hpNormalized > 60 && _hpNormalized <= 80;
        bool s3 = _hpNormalized > 40 && _hpNormalized <= 60;
        bool s4 = _hpNormalized > 20 && _hpNormalized <= 40;
        bool s5 = _hpNormalized > 0 && _hpNormalized <= 20;
        bool s6 = _hpNormalized == 0;

        _state0.SetActive(s0);
        _state1.SetActive(s1);
        _state2.SetActive(s2);
        _state3.SetActive(s3);
        _state4.SetActive(s4);
        _state5.SetActive(s5);

        if (s6)
        {
            Destroy(_main);
        }
    }

    void Start()
    {
        _hitPoints = _hitPointsMax;
        //ChankCollider.Instance.AddBlock(transform.position);
    }

    void Update()
    {
        
    }
}
