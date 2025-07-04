using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BotsCanMove : MonoBehaviour
{
    public UnityEvent OnEndLoading = new UnityEvent();

    public static BotsCanMove Instance;

    public bool CanMove = false;

    private bool _startCountDown;
    private float _time = 5f, _timeCur;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _startCountDown = true;
        //GetComponent<GameWorld>().OnWorldCreated.AddListener(StartCountDown);
    }

    void Update()
    {
        if(_startCountDown)
        {
            _timeCur += Time.deltaTime;
            CanMove = _timeCur > _time;

            if (CanMove)
            {
                OnEndLoading?.Invoke();
                _startCountDown = false;
            }
        }
    }

    public void StartCountDown()
    {
        _startCountDown = true;
    }
}
