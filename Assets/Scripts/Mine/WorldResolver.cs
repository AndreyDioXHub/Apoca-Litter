using Cysharp.Threading.Tasks;
using game.configuration;
using kcp2k;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WorldResolver : NetworkBehaviour
{
    public UnityEvent OnServerMadeTheWorld = new UnityEvent();
    
    //public readonly SyncDictionary<Vector2Int, MicroChankData> MicroChankDatas = new SyncDictionary<Vector2Int, MicroChankData>();
    
    public readonly SyncDictionary<ulong, byte> Damage = new SyncDictionary<ulong, byte>();
    //public readonly SyncList<ulong> DamagePosition = new SyncList<ulong>();

    private int _maxlistCount = 4;

    public bool IsHaveClients => NetworkClient.active;

    [SerializeField]
    private GameWorld _world;

    private string _versionForSend = "1.005";

    [SerializeField, SyncVar(hook = nameof(VersionChanged))]
    private string _version = "";

    public string Version
    {
        get
        {
            return _version;
        }
        set
        {
            _version = value;
        }
    }

    [SerializeField, SyncVar]
    private bool _isWorldCreatedOnServer;

    public bool IsWorldCreatedOnServer
    {
        get
        {
            return _isWorldCreatedOnServer;
        }
        set
        {
            _isWorldCreatedOnServer = value;
        }
    }

    [SerializeField, SyncVar(hook = nameof(OnMapIndexChanged))]
    private int _mapIndex = -1;

    public int MapIndex
    {
        get
        {
            return _mapIndex;
        }
        set
        {
            _mapIndex = value;
        }
    }

    private bool _worldBuildingIsNotInProcess = true;

    [SerializeField]
    private bool _isWorldCreatedOnClient;

    private Dictionary<ulong, byte> _part1PrepuckedDamageMap = new Dictionary<ulong, byte>();
    private Dictionary<ulong, byte> _part2PrepuckedDamageMap = new Dictionary<ulong, byte>();
    private Dictionary<ulong, byte> _part3PrepuckedDamageMap = new Dictionary<ulong, byte>();
    private Dictionary<ulong, byte> _part4PrepuckedDamageMap = new Dictionary<ulong, byte>();

    void Start()
    {
        if (isServer)
        {
            //Debug.Log("isServer");
            int mapIndex = GameConfig.CurrentConfiguration.MapIndex;
            StratBuildWorld(mapIndex);
            MapIndex = mapIndex;
            Version = _versionForSend;
        }

        if(isClient)
        {
            StartAdsTimer("timer");
        }
    }

    public void VersionChanged(string oldValue, string newValue)
    {
        if(isServer) 
        {
        }
        else
        {
            if (isClient)
            {
                if (newValue.Equals(_versionForSend))
                {
                    Debug.Log("Client version is right");
                }
                else
                {
                    Debug.Log("Client version is wrong");
                    GameConfig.CurrentConfiguration.IsWrongVersion = true;
                    ControlNetworkManager.singleton.StopClient();
                }
            }
        }
    }

    private void OnDestroy()
    {
    }

    public async void StartAdsTimer(string wish)
    {
        await UniTask.Yield();
        /*
        if (wish.Equals("timer"))
        {
            int time = 300, timeCur;
            timeCur = time;

            while (timeCur > 0)
            {
                //Debug.Log($"Time befor ads {timeCur}");
                timeCur--;
                await UniTask.WaitForSeconds(1);
            }

            timeCur = 0;
            //Debug.Log($"Time befor ads {timeCur}");
            YandexRewardedAd.Instance.RequestRewardedAd(wish);
        }*/
    }

    public async void OnMapIndexChanged(int oldValue, int newValue)
    {
        //Debug.Log("OnMapIndexChanged");
        await UniTask.Yield();
        StratBuildWorld(newValue);
    }

    public async void StratBuildWorld(int index)
    {
        if (_worldBuildingIsNotInProcess)
        {
            _worldBuildingIsNotInProcess = false;
            //Debug.Log("StratBuildWorld");
            await UniTask.Yield();
            GameConfig.CurrentConfiguration.MapIndex = index;
        }
    }


    //[Command(requiresAuthority = false)]
    public void ClientIntentionСhangeBlock(Vector3Int blockWorldPos, float damage)
    {
        ulong position = WorldResolverUtils.PackPosition(blockWorldPos);
        SendToServer(position, (byte)damage);
    }

    [Command(requiresAuthority = false)]
    private void SendToServer(ulong position, byte damage)
    {
        //Damage[key] = (byte)v;
        AddVal(position, damage);
    }

    public void ClientIntentionMakeExplosion(Vector3Int blockWorldPos)
    {
        //0-handgrenage
        //1-grenage
        //2-rocket
        SendExplosionToServer(blockWorldPos);
    }
 

    [Command(requiresAuthority = false)]
    private void SendExplosionToServer(Vector3Int blockWorldPos) 
    {
        AddValExplosion(blockWorldPos);
    }

    [ContextMenu("Test")]
    public void Test()
    {
        Vector3Int blockWorldPos = new Vector3Int(12, 8, 0);
        Vector3Int kvpKey = new Vector3Int(100, -3, 55);

        Vector3Int kvpPosition = blockWorldPos + kvpKey;

        ulong position = WorldResolverUtils.PackPosition(kvpPosition);
        ulong puckBlockWorldPos = WorldResolverUtils.PackPosition(blockWorldPos);
        ulong puckKvpKey = WorldResolverUtils.PackPosition(kvpKey);

        //Debug.Log($"Test {UnPackPosition(position)} {UnPackPosition(puckBlockWorldPos + puckKvpKey)}");

    }

    public override void OnStartServer()
    {
        //TestServer();
    }

    public override void OnStartClient()
    {
        
        // Add handlers for SyncDictionary Actions
        Damage.OnAdd += OnItemAdded;
        Damage.OnSet += OnItemChanged;
        Damage.OnRemove += OnItemRemoved;
        Damage.OnClear += OnDictionaryCleared;
        Damage.OnChange += OnDictionaryChanged;

        ClientWaitingWorldCreations();
    }

    public async void ClientWaitingWorldCreations()
    {
        //Debug.Log($"Waiting world {!(_isWorldCreatedOnClient && _isWorldCreatedOnServer)}");
        while (!(_isWorldCreatedOnClient && _isWorldCreatedOnServer))
        {
            await UniTask.Yield();
        }

        foreach (var keyvalue in Damage.Keys)
        {
            //Debug.Log($"{keyvalue}");
            Damage.OnAdd.Invoke(keyvalue);
        }
    }

    public void AddVal(ulong position, byte damage)
    {
        if (isServer)
        {
            Vector3Int blockWorldPos = WorldResolverUtils.UnPackPosition(position);
            _world.ProcessBlock(blockWorldPos, (float)damage, true);

            if (Damage.TryGetValue(position, out byte oldDamage))
            {
                Damage[position] = (byte)(Damage[position] + Damage[position]);
            }
            else
            {
                Damage.Add(position, damage);
            }
        }
    }

    public async void AddValExplosion(Vector3Int blockWorldPos)
    {
        if (isServer)
        {
            float timeout = 0.1f;

            ulong position = WorldResolverUtils.PackPosition(blockWorldPos);

            int i = 0;

            foreach (var kvp in _part1PrepuckedDamageMap)
            {
                ulong kvpPosition = position + kvp.Key;

                AddVal(kvpPosition, 100);
            }

            await UniTask.WaitForSeconds(timeout);

            int x = _part2PrepuckedDamageMap.Count / 2;

            foreach (var kvp in _part2PrepuckedDamageMap)
            {
                ulong kvpPosition = position + kvp.Key;

                AddVal(kvpPosition, 100);

                if (x != 0 && i % x == 0)
                {
                    await UniTask.WaitForSeconds(timeout);
                }

                i++;
            }

            await UniTask.WaitForSeconds(timeout);

            x = _part3PrepuckedDamageMap.Count / 2;

            foreach (var kvp in _part3PrepuckedDamageMap)
            {
                ulong kvpPosition = position + kvp.Key;

                AddVal(kvpPosition, 100);

                if (x != 0 && i % x == 0)
                {
                    await UniTask.WaitForSeconds(timeout);
                }

                i++;
            }

            await UniTask.WaitForSeconds(timeout);

            x = _part4PrepuckedDamageMap.Count / 2;

            foreach (var kvp in _part4PrepuckedDamageMap)
            {
                ulong kvpPosition = position + kvp.Key;

                AddVal(kvpPosition, 100);

                if (x != 0 && i % x == 0)
                {
                    await UniTask.WaitForSeconds(timeout);
                }

                i++;
            }
        }
    }

    public override void OnStopClient()
    {

        Damage.OnAdd -= OnItemAdded;
        Damage.OnSet -= OnItemChanged;
        Damage.OnRemove -= OnItemRemoved;
        Damage.OnClear -= OnDictionaryCleared;
        Damage.OnChange -= OnDictionaryChanged;
    }

    
    public void OnItemAdded(ulong key)
    {
        //Debug.Log($"Element added {key} {Damage[key]}");

        Vector3Int blockWorldPos = WorldResolverUtils.UnPackPosition(key);
        float damage = (float)Damage[key];
        _world.ProcessBlock(blockWorldPos, damage, true);
    }

    public void OnItemChanged(ulong key, byte oldValue)
    {
        //Debug.Log($"Element changed {key} from {oldValue} to {Damage[key]}");

        Vector3Int blockWorldPos = WorldResolverUtils.UnPackPosition(key);
        float damage = (float)Damage[key];
        _world.ProcessBlock(blockWorldPos, damage, true);
    }
    /*
    public ulong PackPosition(Vector3Int position)
    {
        return ((ulong)position.x * 100000000 + (ulong)position.y * 100000 + (ulong)position.z);
    }

    public Vector3Int UnPackPosition(ulong key)
    {
        ulong x = (key - key % 100000000)/ 100000000;
        ulong y = ((key - x * 100000000) - (key - x * 100000000) % 100000) / 100000;
        ulong z = key - x * 100000000 - y * 100000; 

        return new Vector3Int((int)x, (int)y, (int)z);
    }*/

    public void OnItemRemoved(ulong key, byte oldValue)
    {
        //Debug.Log($"Element removed {key} {oldValue}");
    }

    public void OnDictionaryCleared()
    {
        foreach (KeyValuePair<ulong, byte> kvp in Damage)
        {
            //Debug.Log($"Element cleared {kvp.Key} {kvp.Value}");
        }
    }
    
    void OnDictionaryChanged(SyncIDictionary<ulong, byte>.Operation op, ulong arg2, byte arg3)
    {
        switch(op) {
            case SyncIDictionary<ulong, byte>.Operation.OP_ADD:
                //Debug.Log($"Element added {arg2} {arg3}");
                break;
            case SyncIDictionary<ulong, byte>.Operation.OP_CLEAR:
                //Debug.Log($"Dictionary cleared");
                break;
            case SyncIDictionary<ulong, byte>.Operation.OP_REMOVE:
                //Debug.Log($"Element removed {arg2} {arg3}");
                break;
            case SyncIDictionary<ulong, byte>.Operation.OP_SET:
                //Debug.Log($"Element changed {arg2} {arg3}");
                break;
        }
    }

    void Update()
    {
           
    }

    public void OnWorldCreated()
    {
        if (isServer)
        {
            OnWorldCreatedOnServer();
        }
        else
        {
            WaitingWhenServerMadeTheWorld();
        }
    }

    //[Command(requiresAuthority = false)]
    public void OnWorldCreatedOnServer() 
    {
        if(isServer)
        {
            _isWorldCreatedOnServer = true;
            OnServerMadeTheWorld?.Invoke();

            if (NetworkServer.connections.Count > 0) 
            {
                OnWorldCreatedOnClients();
            } 
            else 
            {
                //Debug.Log("Нет подключенных клиентов.");
            }
        }
    }

    public async void WaitingWhenServerMadeTheWorld()
    {
        while (_isWorldCreatedOnServer)
        {
            await UniTask.Yield();
        }

        //Debug.Log("Мир создан на сервере и клиент похавал это");
    }

    [ClientRpc]
    private void OnWorldCreatedOnClients()
    {
        OnServerMadeTheWorld?.Invoke();
    }
}

public static class WorldResolverUtils
{
    public static ulong PackPosition(Vector3Int position)
    {
        return ((ulong)position.x * 100000000 + (ulong)position.y * 100000 + (ulong)position.z);
    }

    public static Vector3Int UnPackPosition(ulong key)
    {
        ulong x = (key - key % 100000000) / 100000000;
        ulong y = ((key - x * 100000000) - (key - x * 100000000) % 100000) / 100000;
        ulong z = key - x * 100000000 - y * 100000;

        return new Vector3Int((int)x, (int)y, (int)z);
    }
}