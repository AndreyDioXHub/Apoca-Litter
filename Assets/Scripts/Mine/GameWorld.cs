using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using static UnityEngine.Mesh;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;

public class GameWorld : MonoBehaviour
{
    public const string CHANKS_LAYER_NAME = "chanks";
    public static GameWorld Instance;

    public List<Transform> SpawnPoints => _spawnPoints;

    [SerializeField]
    private WorldResolver _worldResolver;

    [SerializeField]
    private List<Transform> _spawnPoints = new List<Transform>();



    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
    }

    private void Update()
    {
    }

    public void IntentionСhangeBlock(Vector3 point, Vector3 normal, float damage)
    {
    }

    public void ProcessBlock(Vector3Int blockWorldPos, float damage, bool collect = false)
    {
    }

    public async UniTask<bool> MyTask()
    {
        // Имитация асинхронной операции
        await UniTask.Delay(1000); // Ждём 1 секунду

        // Возвращаем результат
        return true;

        // Для обработки ошибок можно использовать try/catch:
        /*
        try 
        {
            // Ваш код
            return true;
        }
        catch
        {
            return false;
        }
        */
    }

}