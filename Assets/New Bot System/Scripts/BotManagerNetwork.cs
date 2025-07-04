using Cysharp.Threading.Tasks;
using Mirror;
using NewBotSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BotManagerNetwork : NetworkBehaviour
{
    public static BotManagerNetwork Instance;

    public float AgrDistance => _distanceToPlayer;

    //public UnityEvent OnNewGameStarted = new UnityEvent();
    /*
    [SerializeField, SyncVar(hook = nameof(UpdateScore))]
    private int _score = 0;

    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
        }
    }*/

    public uint LocalPlayerNetID;

    [SerializeField]
    private List<BotFromPerdanskAI> _ais = new List<BotFromPerdanskAI>();
    [SerializeField] 
    private LayerMask _obstacleLayerMask;
    [SerializeField] 
    private bool _needAttack;
    [SerializeField] 
    private List<Transform> _patrolPoints = new List<Transform>();

    private float _delay = 1;
    private float _distanceToPlayer = 15; 
    [SerializeField] 
    private float _checkInterval = 15f;
    [SerializeField] 
    private float _clusterRadius = 5f;
    [SerializeField] 
    private int _minBotsInCluster = 4;
    [SerializeField] 
    private float _requiredClusterTime = 15f;

    private Dictionary<BotFromPerdanskAI, float> _timeInCluster = new Dictionary<BotFromPerdanskAI, float>();

    public virtual void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (isServer)
        {
            CheckDistanceBetweenBotsNPlayers();
            StartClusterCheckLoop();
        }
    }

    private async void StartClusterCheckLoop()
    {
        while (true)
        {
            await UniTask.WaitForSeconds(_checkInterval);
            CheckForClusters();
        }
    }


    private void CheckForClusters()
    {
        // Очистка от уничтоженных ботов
        _ais.RemoveAll(bot => bot == null);

        // Временное хранилище для новой информации о кластерах
        var newClusterTimes = new Dictionary<BotFromPerdanskAI, float>();

        // Проверка каждого бота
        for (int i = 0; i < _ais.Count; i++)
        {
            BotFromPerdanskAI currentBot = _ais[i];
            if (currentBot == null) continue;

            // Поиск ботов в радиусе
            List<BotFromPerdanskAI> clusterBots = new List<BotFromPerdanskAI>();
            foreach (var otherBot in _ais)
            {
                if (otherBot == null || otherBot == currentBot) continue;

                float distance = Vector3.Distance(
                    currentBot.transform.position,
                    otherBot.transform.position
                );

                if (distance <= _clusterRadius)
                {
                    clusterBots.Add(otherBot);
                }
            }

            // Проверка условий кластера
            if (clusterBots.Count >= _minBotsInCluster - 1)
            {
                // Добавляем текущего бота в кластер
                clusterBots.Add(currentBot);

                // Обновляем время для всех ботов в кластере
                foreach (var bot in clusterBots)
                {
                    float currentTime = _timeInCluster.TryGetValue(bot, out float time) ? time : 0;
                    currentTime += _checkInterval;
                    newClusterTimes[bot] = currentTime;

                    // Проверка на превышение времени
                    if (currentTime >= _requiredClusterTime)
                    {
                        Debug.Log($"{bot.gameObject.name} Stucked");
                        bot.Resolver.PermamentDie();
                    }
                }
            }
        }

        // Обновляем информацию о времени в кластерах
        _timeInCluster = newClusterTimes;
    }


    void Update()
    {

    }

    public Transform GetRandomPatrolPointCloseToMe(Vector3 myPosition)
    {
        float minradius = 1;
        float radius = 10;

        if (_patrolPoints == null || _patrolPoints.Count == 0)
        {
            //Debug.LogWarning("No patrol points available!");
            return null;
        }

        // Получаем все точки в радиусе
        List<Transform> pointsInRadius = GetPointsInRadius(myPosition, minradius, radius);

        // Если есть точки в радиусе - возвращаем случайную
        if (pointsInRadius.Count > 0)
        {
            int randomOrClose = UnityEngine.Random.Range(0, 5);

            if(randomOrClose == 0)
            {
                return _patrolPoints[UnityEngine.Random.Range(0, _patrolPoints.Count)];
            }
            else
            {
                return pointsInRadius[UnityEngine.Random.Range(0, pointsInRadius.Count)];
            }
        }

        // Если точек в радиусе нет - возвращаем любую случайную точку
        //Debug.LogWarning("No patrol points in radius. Returning random point.");
        return _patrolPoints[UnityEngine.Random.Range(0, _patrolPoints.Count)];
    }

    private List<Transform> GetPointsInRadius(Vector3 center, float minradius, float radius)
    {
        List<Transform> result = new List<Transform>();

        foreach (Transform point in _patrolPoints)
        {
            if (point == null) continue;

            float distance = Vector3.Distance(center, point.position);
            if (distance > minradius && distance < radius)
            {
                result.Add(point);
            }
        }

        return result;
    }


    public void RegisterBot(BotFromPerdanskAI ai)
    {
        _ais.Add(ai);
    }

    public void ReturnToken(BotFromPerdanskAI ai, PlayerNetworkResolver resolver)
    {
        if (resolver == null)
        {
            //Debug.Log($"{ai.gameObject.name} sended null resolver");
        }
        else
        {
            resolver.BotsAttackedPlayer.Remove(ai);
        }
    }

    public async void CheckDistanceBetweenBotsNPlayers()
    {
        while (true)
        {
            await UniTask.WaitForSeconds(_delay);

            if (_needAttack)
            {
                foreach (BotFromPerdanskAI bot in _ais)
                {
                    if (bot == null)
                    {
                        continue;
                    }

                    GameObject visiblePlayer = FindVisiblePlayerForBot(bot);

                    if (visiblePlayer != null)
                    {
                        bot.PlayerDetected(visiblePlayer);

                        if (visiblePlayer.GetComponent<PlayerNetworkResolver>().IsDead)
                        {

                        }
                        else
                        {
                        }
                    }
                }
            }
        }
    }

    public GameObject FindVisiblePlayerForBot(BotFromPerdanskAI bot)
    {
        if (bot == null)
        {
            return null;
        }

        float checkDistance = _distanceToPlayer;// bot.GetViewDistance(); 
        Vector3 botPosition = bot.transform.position;
        Vector3 botForward = bot.transform.forward;

        GameObject closestVisiblePlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in WaitingScreenModel.Instance.GetPlayers())
        {
            if (player == null)
            {
                continue;
            }

            Vector3 toPlayer = player.transform.position - botPosition;
            float distance = toPlayer.magnitude;

            // Проверка расстояния
            if (distance > checkDistance)
            {
                continue;
            }


            if (player.GetComponent<PlayerNetworkResolver>().IsPaused)
            {
                continue;
            }

            if (bot.Resolver.IsDead)
            {
                continue;
            }

            /*
            // Проверка направления (впереди бота)
            if (Vector3.Dot(botForward, toPlayer.normalized) <= 0)
            {
                continue;
            }

            // Проверка препятствий
            if (Physics.Raycast(botPosition, toPlayer.normalized, out RaycastHit hit, distance, _obstacleLayerMask))
            {
                // Если луч попал не в игрока - препятствие есть
                if (hit.collider.gameObject != player)
                {
                    continue;
                }
            }

            // Проверка обратного рейкаста (за спиной)
            Vector3 behindPosition = botPosition - botForward * 2f + Vector3.up; // Смещение назад
            Vector3 toPlayerFromBehind = player.transform.position - behindPosition;

            if (Physics.Raycast(behindPosition, toPlayerFromBehind.normalized,
                out RaycastHit hitBehind, toPlayerFromBehind.magnitude, _obstacleLayerMask))
            {
                if (hitBehind.collider.gameObject == player)
                {
                    continue;
                }
            }*/

            // Обновляем ближайшего видимого игрока
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestVisiblePlayer = player;


            }
        }

        if (closestVisiblePlayer == null)
        {

        }
        else
        {
            PlayerNetworkResolver resolver = closestVisiblePlayer.GetComponent<PlayerNetworkResolver>();

            if (resolver.BotsAttackedPlayer.Count >= 2)
            {
                closestVisiblePlayer = null;
            }
            else
            {
                if (resolver.BotsAttackedPlayer.Contains(bot))
                {

                }
                else
                {
                    resolver.BotsAttackedPlayer.Add(bot);
                }
            }
        }

        return closestVisiblePlayer;
    }
}
