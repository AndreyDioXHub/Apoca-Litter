using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODDistanceListener : MonoBehaviour
{
    [SerializeField]
    private LODGroup _lod;
    [SerializeField]
    private float _value;
    [SerializeField]
    private float _minValue = 0.5f;
    [SerializeField]
    private float _midValue = 0.07f;
    [SerializeField]
    private float _maxValue = 0.01f;
    [SerializeField]
    private float transitionValue = 0.01f;

    void Start()
    {
        _value = PlayerPrefs.GetFloat(PlayerPrefsConsts.drawingdistance, 0.7f);
        ReValue(_value);
        AwateEventsBus();
    }

    public async void AwateEventsBus()
    {
        while (EventsBus.Instance==null)
        {
            await UniTask.Yield();
        }

        EventsBus.Instance.OnDrawingDistanceChanged.AddListener(ReValue);
    }

    [ContextMenu("RecalculateBounds")]
    public void RecalculateBounds()
    {
        _lod.RecalculateBounds();
    }

    [ContextMenu("ReValue")]
    public void ReValue()
    {
        ReValue(_value);
    }

    public void ReValue(float value)
    {


        LOD[] lods = _lod.GetLODs();

         transitionValue = 0f;

        if(value >= 0 && value <= 0.7f)
        {
            float t = value / 0.7f;
            transitionValue = Mathf.Lerp(_minValue, _midValue, t);
        }
        else if (value > 0.7f && value <= 1f)
        {
            float t = value;
            transitionValue = Mathf.Lerp(_midValue, _maxValue, t);
        }
        else
        {
            Debug.LogWarning($"Значение _value ({value}) вне допустимого диапазона [0, 1].");
        }

        // Обновляем параметр fadeTransitionWidth для первого уровня детализации
        lods[0].screenRelativeTransitionHeight = transitionValue;
        // Применяем изменения к LODGroup
        _lod.SetLODs(lods);
        _lod.RecalculateBounds();
        //_lod.size = 8;
    }

    void Update()
    {
        
    }

    [ContextMenu("CollectLOD")]
    public void CollectLOD()
    {
        _lod =GetComponent<LODGroup>();
    }
}
