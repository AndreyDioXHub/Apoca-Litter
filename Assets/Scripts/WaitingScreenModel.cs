using cyraxchel.games.network.chat;
using Cysharp.Threading.Tasks;
using Mirror;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WaitingScreenModel : NetworkBehaviour
{
    public static WaitingScreenModel Instance;

    public UnityEvent OnPlayersDictionaryChanged = new UnityEvent();
    public UnityEvent OnGameReady = new UnityEvent();
    public UnityEvent<uint> OnPlayerKillBott = new UnityEvent<uint>();

    public readonly SyncDictionary<uint, PlayerLoadingInfo> Players = new SyncDictionary<uint, PlayerLoadingInfo>();

    //public List<GameObject> LivePlayers => _livePlayers;

    public int ConectedPlayersCount => _conectedPlayersCount;

    [SerializeField]
    private PlayerNetworkResolver _resolver;
    [SerializeField]
    private Dictionary<uint, GameObject> _playerGameObjects = new Dictionary<uint, GameObject>();
    /*
    [SerializeField]
    private List<GameObject> _livePlayers = new List<GameObject>();*/

    [SerializeField]
    private string _address = "localhost";
    [SerializeField]
    private uint _player;
    [SerializeField]
    private int _conectedPlayersCount = 0;

    [SerializeField, SyncVar(hook = nameof(GameReadyChanged))]
    private bool _gameReady;
    private PlayerNetworkResolver[] _allResolvers;

    public bool GameReady
    {
        get
        {
            return _gameReady;
        }
        set
        {
            _gameReady = value;
        }
    }
    [SerializeField, SyncVar]
    private int _playersCount;
    public int PlayersCount
    {
        get
        {
            return _playersCount;
        }
        set
        {
            _playersCount = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (isServer)
        {
            //SlowUpdate();
            ReadConfigFile();
            UpdatePlayerResolvers(1);
            ControlNetworkManager.singleton.ServerPlayerCountChanged.AddListener(SendToServerNewConnectionCount);
            ControlNetworkManager.singleton.ServerPlayerCountChanged.AddListener(UpdatePlayerResolvers);
            ControlNetworkManager.singleton.OnRemovePlayer.AddListener(RemoveDisconectedPlaye);
        }

        ControlNetworkManager.singleton.ServerPlayerCountChanged.AddListener(UpdateCurrentPlayersCount);
    }

    public void UpdatePlayerResolvers(int count)
    {
        Debug.Log($"PlayerResolvers Updated {count}");
        _allResolvers = FindObjectsOfType<PlayerNetworkResolver>(true);
    }

    private void ReadConfigFile()
    {
        // Получаем путь к папке с исполняемым файлом
        string exePath = Path.GetDirectoryName(Application.dataPath);
        string configPath = Path.Combine(exePath, "config.txt");

        //Debug.Log($"Попытка прочитать конфиг из: {configPath}");

        if (!File.Exists(configPath))
        {
            //Debug.LogWarning($"Файл конфигурации не найден! Используется адрес по умолчанию: {_address}");
            return;
        }

        try
        {
            // Читаем весь файл
            string fileContent = File.ReadAllText(configPath).Trim();

            if (string.IsNullOrWhiteSpace(fileContent))
            {
                //Debug.LogWarning("Файл конфигурации пуст! Используется адрес по умолчанию.");
                return;
            }

            // Убедимся, что в конце нет слэша
            if (fileContent.EndsWith("/"))
            {
                fileContent = fileContent[..^1];
            }

            _address = fileContent;
            //Debug.Log($"Успешно прочитан адрес сервера: {_address}");
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"Ошибка чтения конфигурации: {e.Message}");
        }
    }

    public void UpdateCurrentPlayersCount(int count)
    {
        _conectedPlayersCount = count;
    }

    public void SendToServerNewConnectionCount(int count)
    {
        if (isServer)
        {
            if (!_address.Equals("localhost"))
            {
                SendData(_address, count);
            }
        }
    }

    public void RemoveDisconectedPlaye(uint connection)
    {
        Players.Remove(connection);
    }

    void Update()
    {

    }

    /*
    public void AddLivePlayer(GameObject player)
    {
        if (_livePlayers.Contains(player))
        {

        }
        else
        { 
            //Debug.Log("Add Deth Player");
            _livePlayers = _livePlayers.Where(player => player != null).ToList();
            _livePlayers.Add(player);
        }
    }

    public void RemoveLivePlayer(GameObject player)
    {
        if (_livePlayers.Contains(player))
        {
            //Debug.Log("Remove Deth Player");
            _livePlayers.Remove(player);            
        }
        else
        {
        }
    }*/


    [ClientRpc]
    public void PlayerKillBot(uint sender)
    {
        Debug.Log($"AllPlayersDeadOnClient {sender}");
        OnPlayerKillBott?.Invoke(sender);
    }

    /*
    public async void SlowUpdate()
    {
        while(true)
        {
            await UniTask.WaitForSeconds(0.25f);

            _livePlayers = _livePlayers.Where(player => player != null).ToList();
        }
    }*/

    public void GameReadyChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            OnGameReady?.Invoke();
        }
    }

    public override void OnStartClient()
    {
        Players.OnChange += OnDictionaryChanged;
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        //RemovePlayer(_player);
        //Players.Remove();
        Players.OnChange -= OnDictionaryChanged;
    }

    [Command(requiresAuthority = false)]
    public void RemovePlayer(uint player)
    {
        Players.Remove(player);
    }

    void OnDictionaryChanged(SyncIDictionary<uint, PlayerLoadingInfo>.Operation op, uint player, PlayerLoadingInfo changedInfo)
    {

        switch (op)
        {
            case SyncIDictionary<uint, PlayerLoadingInfo>.Operation.OP_ADD:
                //Debug.Log($"Element added {player} - {changedInfo.name}: {changedInfo.status}");
                break;
            case SyncIDictionary<uint, PlayerLoadingInfo>.Operation.OP_CLEAR:
                //Debug.Log($"Dictionary cleared");
                break;
            case SyncIDictionary<uint, PlayerLoadingInfo>.Operation.OP_REMOVE:
                //Debug.Log($"Element removed {player} - {changedInfo.name}: {changedInfo.status}");
                break;
            case SyncIDictionary<uint, PlayerLoadingInfo>.Operation.OP_SET:
                //Debug.Log($"Element changed {player} - {changedInfo.name}: {changedInfo.status}");
                break;
        }

        List<PlayerLoadingInfo> players = new List<PlayerLoadingInfo>();

        foreach (var playerInfo in Players)
        {
            players.Add(playerInfo.Value);
        }

        PlayersCount = players.Count;

        if (isServer)
        {
            if (BotManagerNetwork.Instance == null)
            {

            }
            else
            {
                foreach (var playerinfo in Players)
                {
                    if (playerinfo.Value.status == 2)
                    {
                        //BotManagerNetwork.Instance.RegisterPlayer(_playerGameObjects[playerinfo.Key].transform);
                    }
                }

            }
        }

        OnPlayersDictionaryChanged?.Invoke();
    }

    public List<GameObject> GetPlayers()
    {
        List<GameObject> players = new List<GameObject>();
        foreach (PlayerNetworkResolver resolver in _allResolvers)
        {
            try
            {
                players.Add(resolver.gameObject);
            }
            catch (Exception e)
            {
                Debug.Log("Resolver is null but you try get them");
                UpdatePlayerResolvers(1);
            }
        }

        return players;
    }

    /*
    public PlayerNetworkResolver GetPlayerResolverByName(string playerName)
    {
        GameObject player = _livePlayers.Find(p=>p.name.Equals(playerName));

        if (player == null)
        {
            return null;
        }

        return player.GetComponent<PlayerNetworkResolver>();
    }

    public GameObject GetClosedLivePlayer(Vector3 position)
    {
        GameObject closestPlayer = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject player in _livePlayers)
        {
            // Пропускаем null-элементы и уничтоженные объекты
            if (player == null) continue;

            Vector3 directionToPlayer = player.transform.position - position;
            float dSqrToPlayer = directionToPlayer.sqrMagnitude;

            if (dSqrToPlayer < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToPlayer;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }*/

    /*
    public void Init(PlayerNetworkResolver resolver)
    {
        _resolver = resolver;
        _player = _resolver.PlayerID;
    }
    */

    /*
    public void RegisterPlayer(uint player, GameObject go)
    {
        _playerGameObjects.Add(player, go);
        //_livePlayers.Add(go);
    }*/

    [Command(requiresAuthority = false)]
    public void UpdatePlayer(uint playerID, PlayerLoadingInfo info)
    {
        if (Players.ContainsKey(playerID))
        {
            Players[playerID] = info;
        }
        else
        {
            Players.Add(playerID, info);
        }


    }

    public void SendData(string address, int connections)
    {
        StartCoroutine(SendDataCoroutine(address, connections));
    }

    public IEnumerator SendDataCoroutine(string address, int connections)
    {
        // Формируем данные для отправки
        ServerUpdateData updateData = new ServerUpdateData
        {
            address = address,
            connections = connections
        };

        string json = JsonConvert.SerializeObject(updateData);
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(json);

        // Создаем запрос
        string url = "http://217.114.0.75:3000/api/update-server";
        using (UnityEngine.Networking.UnityWebRequest request =
               new UnityEngine.Networking.UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(rawData);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Отправляем запрос
            yield return request.SendWebRequest();

            // Обрабатываем результат
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError ||
                request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Ошибка отправки данных: {request.error}");
            }
            else
            {
                Debug.Log($"Данные успешно отправлены! Ответ сервера: {request.downloadHandler.text}");
                /*
                // Обновляем локальные данные после успешной отправки
                yield return StartCoroutine(FetchServerData());*/
            }
        }
    }
}

[Serializable]
public class PlayerLoadingInfo
{
    public string name;
    public byte status;

    public PlayerLoadingInfo()
    {
        this.name = "";
        this.status = 0;
    }

    public PlayerLoadingInfo(string name, byte status)
    {
        this.name = name;
        this.status = status;
    }
}

[Serializable]
public class ServerUpdateData
{
    public string address;
    public int connections;
}