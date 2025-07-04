using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearFogController : MonoBehaviour
{

    [SerializeField]
    private float _startDistance = 300f;
    [SerializeField]
    private float _endDistance = 300f;

    private float _value;

    private float _minStartDistance = 2f;
    private float _midStartDistance = 30f;
    private float _maxStartDistance = 80;

    private float _minEndDistance = 5f;
    private float _midEndDistance = 40f;
    private float _maxEndDistance = 100f;

    void Start()
    {
        _value = PlayerPrefs.GetFloat(PlayerPrefsConsts.drawingdistance, 0.7f);
        ReValue(_value);
        EventsBus.Instance.OnDrawingDistanceChanged.AddListener(ReValue);
    }

    [ContextMenu("ReValue")]
    public void ReValue()
    {
        ReValue(_value);
    }

    public void ReValue(float value)
    {

        if (value >= 0 && value <= 0.7f)
        {
            float t = value / 0.7f;
            _startDistance = Mathf.Lerp(_minStartDistance, _midStartDistance, t);
        }
        else if (value > 0.7f && value <= 1f)
        {
            float t = value;
            _startDistance = Mathf.Lerp(_midStartDistance, _maxStartDistance, t);
        }

        if (value >= 0 && value <= 0.7f)
        {
            float t = value / 0.7f;
            _endDistance = Mathf.Lerp(_minEndDistance, _midEndDistance, t);
        }
        else if (value > 0.7f && value <= 1f)
        {
            float t = value;
            _endDistance = Mathf.Lerp(_midEndDistance, _maxEndDistance, t);
        }

        RenderSettings.fogStartDistance = _startDistance;
        RenderSettings.fogEndDistance = _endDistance;
    }


    void Update()
    {
        
    }
}
