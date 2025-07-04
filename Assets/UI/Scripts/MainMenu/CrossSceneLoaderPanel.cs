using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CrossSceneLoaderPanel : MonoBehaviour {

    public static CrossSceneLoaderPanel Instance;

    private Slider _slider;

    private float _targetValue;

    private float _maxTargetValue = 1f;
    [SerializeField]
    private bool _isFakeLoad = false;

    private float k = 1f;

    // Start is called before the first frame update
    public void Init(float maxvalue = 2) {
        Instance = this;
        // Находим компонент Slider в дочерних объектах
        _slider = GetComponentInChildren<Slider>();

        if (_slider != null) {
            // Устанавливаем параметры слайдера
            _slider.minValue = 0;
            _slider.maxValue = maxvalue;
            _slider.value = 0;
        } else {
            Debug.LogError("Slider component not found in children.");
        }
    }

    // Метод для установки значения слайдера
    public void SetPercents(float value) {
        Debug.Log(value);
        _targetValue = value;
    }

    public async void RunSecondPart(Scene newScene) {
        _maxTargetValue = 1.5f;
        _isFakeLoad = true;
        k = 3f;
        SceneManager.MoveGameObjectToScene(gameObject, newScene);
        await UniTask.WaitUntil(() => GameWorld.Instance != null);
        /*
        if (GameWorld.Instance.WorldStarted) 
        { 
            AddWorldPercents();
        } 
        else 
        { 
            GameWorld.Instance.OnWorldCreated.AddListener(AddWorldPercents);
        }
        */
        await UniTask.WaitUntil(() => BotsCanMove.Instance != null);
        BotsCanMove.Instance.OnEndLoading.AddListener(CompleteLoadAsync);
    }

    private async void CompleteLoadAsync() {
        Debug.LogWarning($"<color=red>COMPLETE LOAD</color>");
        await UniTask.WaitUntil(() => _slider.value >= 2);
        Destroy(gameObject, 1);
    }

    private void AddWorldPercents() {
        Debug.LogWarning("<color=red>World ready</color>");
        _maxTargetValue = 2f;
    }

    // Update is called once per frame
    void Update() {
        if (_isFakeLoad) {
            float nextAdd = UnityEngine.Random.Range(0, 1f) * 0.005f;
            _targetValue = Mathf.Min(nextAdd+_targetValue, _maxTargetValue);
        }
        _slider.value = Mathf.MoveTowards(_slider.value, _targetValue, Time.deltaTime * k);
    }


    public async void RunBuiltinPart() {
        _maxTargetValue = 1;
        _isFakeLoad = true;
        k = 1000f;
        await UniTask.WaitUntil(() => _slider.value >= 1);
        Destroy(gameObject);
    }
}
