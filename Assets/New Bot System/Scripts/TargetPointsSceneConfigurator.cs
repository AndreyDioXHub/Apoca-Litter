using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetPointsSceneConfigurator : MonoBehaviour
{
    public static TargetPointsSceneConfigurator Instance;



    [SerializeField]
    private List<TargetPointSpawner> _targetPoints = new List<TargetPointSpawner>();
    [SerializeField]
    private bool _useTargetPoints = false;
    [SerializeField]
    private float _time = 3, _timeCur;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (_useTargetPoints)
        {
            _timeCur += Time.deltaTime;

            if(_timeCur > _time)
            {
                _timeCur = _time;
                _useTargetPoints = false;
                ProcessPoints();
            }
        }
    }

    public void ProcessPoints()
    {
        for(int i=0; i< _targetPoints.Count; i++)
        {
            _targetPoints[i].Sleep(null, null);
            Destroy(_targetPoints[i]);
        }

        BotsEnablerDisabler.Instance.BotTargetsInited = true;
    }

    public void Register(TargetPointSpawner targetPointSpawner)
    {
        _targetPoints.Add(targetPointSpawner);
        _useTargetPoints = true;
    }
}
