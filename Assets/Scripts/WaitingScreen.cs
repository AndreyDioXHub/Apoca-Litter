using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingScreen : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;
    [SerializeField]
    private WaitingScreenModel _model;
    [SerializeField]
    private GameObject _screen;
    [SerializeField]
    private GameObject _startButton;

    [SerializeField]
    private int _spawnIndex = -1;

    [SerializeField]
    private List<SpawnPointUIButton> _spawnButtons = new List<SpawnPointUIButton>();

    /* [SerializeField]
     private List<Button> _spawnButtons = new List<Button>();*/

    /*
    [SerializeField]
    private Slider _loadingSlider;
    [SerializeField]
    private List<WaitingScreenPlayerSlot> _players = new List<WaitingScreenPlayerSlot>();*/

    void Start()
    {
        PauseScreen.Instance?.SetActiveWaitingScreen(true);
        /*
        for (int i = 0; i < _spawnButtons.Count; i++)
        {
            _spawnButtons[i].onClick.AddListener(() =>
            {
                Debug.Log($"Selected new spawn index {i}");
                _spawnIndex = i;

            });
        }*/
    }

    void Update()
    {
    }

    public async void Init(PlayerNetworkResolver resolver)
    {
        _resolver = resolver;

        _startButton.SetActive(false);
        Button playbutton = _startButton.GetComponent<Button>();
        playbutton.onClick.AddListener(StartNewGame);
        //_startButton.GetComponent<Button>().interactable = false;

        _model = WaitingScreenModel.Instance;

        //_model.OnPlayersDictionaryChanged.AddListener(UpdateView);
        _model.OnGameReady.AddListener(FinalRegisterPlayer);

        for (int i = 0; i < _spawnButtons.Count; i++)
        {
            _spawnButtons[i].Init(this, i);
        }

        while (string.IsNullOrEmpty(_resolver.PlayerName))
        {
            await UniTask.Yield();
        }

        //_model.Init(_resolver);
        _model.UpdatePlayer(_resolver.PlayerID, new PlayerLoadingInfo(_resolver.PlayerName, 1));

    }

    public void SelectSpawnIndex(int spawnIndex)
    {
        Debug.Log($"Selected new spawn index {spawnIndex}");
        _spawnIndex = spawnIndex;
        _startButton.SetActive(true);

        for (int i = 0; i < _spawnButtons.Count; i++)
        {
            _spawnButtons[i].Select(spawnIndex);
        }
    }

    /*
    public void UpdateView()
    {
        List<PlayerLoadingInfo> players = new List<PlayerLoadingInfo>();

        foreach (var playerInfo in _model.Players)
        {
            players.Add(playerInfo.Value);
        }

        foreach(var playerSlot in _players)
        {
            playerSlot.UpdateItem(new PlayerLoadingInfo("", 0));
        }

        for(int i=0; i < players.Count; i++)
        {
            _players[i].UpdateItem(players[i]);
            //Debug.Log( $"UpdateView {players[i].status}");
        }

        bool mayStart = true;

        foreach (var player in players)
        {
            if(player.status != 2)
            {
                mayStart = false;
                break;
            }
        }

        _startButton.GetComponent<Button>().interactable = mayStart;

        if(_model.GameReady && mayStart)
        {
            FinalRegisterPlayer();
        }
    }*/

    public void StartNewGame()
    {
        _model.GameReady = true;
        Teleport teleport = _resolver.GetComponent<Teleport>();
        teleport.TeleportToSpawn(_spawnIndex);
        gameObject.SetActive(false);
    }

    public void LeaveGame()
    {
        ControlNetworkManager.singleton.StopClient();
        if (_resolver.IsServer)
        {
            ControlNetworkManager.singleton.StopServer();
        }

    }

    public void FinalRegisterPlayer()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        PauseScreen.Instance?.SetActiveWaitingScreen(true);
    }

    private void OnDisable()
    {
        PauseScreen.Instance?.SetActiveWaitingScreen(false);
    }
}
