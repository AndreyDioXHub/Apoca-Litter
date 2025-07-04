using Cysharp.Threading.Tasks;
using game.configuration;
using kcp2k;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telepathy;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingWindow : MonoBehaviour
{
    [SerializeField]
    private ServerListResponse _serverList = new ServerListResponse();

    [SerializeField]
    private bool _isTest;
    [SerializeField]
    private GameObject _connectButton;
    [SerializeField]
    private GameObject _playButton;
    [SerializeField]
    private GameObject _serverButtonPrefab;
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private RefreshServerListTimer _timer;
    [SerializeField]
    private int _timeBetweenRefresh=10, _timeCur;
    [SerializeField]
    private List<GameObject> _spawnedButtons = new List<GameObject>();

    private const string API_URL = "http://217.114.0.75:3000/api/servers";
    void Start()
    {
        RefreshServerList();

#if UNITY_EDITOR
        _isTest = true;
#else
        _isTest = false;
#endif
        _connectButton.SetActive(_isTest);
        _playButton.SetActive(false);
    }

    void Update()
    {
        
    }

    public void RefreshServerList()
    {
        StartCoroutine(FetchServerData());
    }

    public async void RefreshServerListAsync()
    {
        for (int i = 0; i < _spawnedButtons.Count; i++)
        {
            Destroy(_spawnedButtons[i]);
        }

        _spawnedButtons.Clear();
        _spawnedButtons = new List<GameObject>();

        for(int i=0; i < _serverList.servers.Count; i++)
        {
            GameObject buttonGO = Instantiate(_serverButtonPrefab);
            buttonGO.transform.parent = _content;
            buttonGO.GetComponent<ServerButton>().Init(_serverList.servers[i], i);
            _spawnedButtons.Add(buttonGO);
        }

        await UniTask.Yield();

        LayoutRebuilder.ForceRebuildLayoutImmediate(_content.GetComponent<RectTransform>());

    }

    public async void Timer()
    {
        _timeCur = _timeBetweenRefresh;

        while (_timeCur > 0)
        {
            _timer.UpdateValue(_timeCur);
            _timeCur--;
            await UniTask.WaitForSeconds(1);
        }

        _timeCur = 0;
        _timer.UpdateValue(_timeCur);
    }

    private IEnumerator FetchServerData()
    {
        Timer();
        using (UnityEngine.Networking.UnityWebRequest webRequest =
               UnityEngine.Networking.UnityWebRequest.Get(API_URL))
        {
            // Отправляем запрос и ждем ответ
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                //Debug.LogError($"Ошибка: {webRequest.error}");
                _serverList = new ServerListResponse();
                _serverList.servers = new List<ServerInfo>
                {
                    new ServerInfo()
                };

                _serverList.servers[0].name = "localhost";
                _serverList.servers[0].address = "localhost";
                _serverList.servers[0].connections = 0;
                _serverList.servers[0].port = 7777;

                RefreshServerListAsync();
                _playButton.SetActive(true);
                yield break;
            }

            try
            {
                // Десериализация JSON
                _serverList = JsonConvert.DeserializeObject<ServerListResponse>(webRequest.downloadHandler.text);
                //Debug.Log("Данные серверов успешно обновлены!");
            }
            catch (System.Exception e)
            {
                //Debug.LogError($"Ошибка десериализации: {e.Message}");
                _serverList = new ServerListResponse();
                _serverList.servers = new List<ServerInfo>
                {
                    new ServerInfo()
                };
                _serverList.servers[0].name = "localhost";
                _serverList.servers[0].address = "localhost";
                _serverList.servers[0].connections = 0;
                _serverList.servers[0].port = 7777;

            }
        }

        RefreshServerListAsync();
        _playButton.SetActive(true);
    }

    public void PlayHost()
    {
        GameConfig.CurrentConfiguration.ServerName = "localhost";
        GameConfig.CurrentConfiguration.Server = "localhost";
        GameConfig.CurrentConfiguration.Port = "7777";

        ControlNetworkManager.singleton.StartHost();
    }

    public void PlayGame()
    {
        List<ServerInfo> servers = new List<ServerInfo>();

        for(int i=0; i< _serverList.servers.Count; i++)
        {
            if (!_serverList.servers[i].address.Equals("localhost"))
            {
                servers.Add(new ServerInfo(_serverList.servers[i].name, 
                    _serverList.servers[i].address, _serverList.servers[i].connections, 
                    _serverList.servers[i].port));
            }
        }

        ServerInfo server = servers.OrderBy(server => server.connections).FirstOrDefault();

        if (server.connections == 16)
        {
            ControlNetworkManager.singleton.StartHost();
        }
        else
        {
            GameConfig.CurrentConfiguration.ServerName = server.name;
            GameConfig.CurrentConfiguration.Server = server.address;
            GameConfig.CurrentConfiguration.Port = server.port.ToString();

            ControlNetworkManager.singleton.networkAddress = server.address;
            ControlNetworkManager.singleton.gameObject.GetComponent<KcpTransport>().Port = server.port;
            ControlNetworkManager.singleton.StartClient();
        }
    }

    public void PlayClient()
    {
        ControlNetworkManager.singleton.StartClient();
    }

}

[Serializable]
public class ServerInfo
{
    [JsonProperty("name")]
    public string name; 
    [JsonProperty("address")]
    public string address;
    [JsonProperty("connections")]
    public int connections;
    [JsonProperty("port")]
    public ushort port;

    public ServerInfo()
    {
        this.name = "";
        this.address = "";
        this.connections = 0;
        this.port = 7777;
    }

    public ServerInfo(string name, string address, int connections, ushort port)
    {
        this.name = name;
        this.address = address;
        this.connections = connections;
        this.port = port;
    }
}

[Serializable]
public class ServerListResponse
{
    [JsonProperty("servers")]
    public List<ServerInfo> servers = new List<ServerInfo>();
}
