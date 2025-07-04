using Cysharp.Threading.Tasks;
using game.configuration;
using InfimaGames.LowPolyShooterPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils;

namespace NewBotSystem
{
    public class BotManager : MonoBehaviour
    {
        public static BotManager Instance;

        public int KillingCountSandBox;

        public bool SandboxReady => _sandboxReady;
        public List<BotConfig> BotsData => _botsData;
        public List<GameObject> BioMass => _bioMass;
        public List<GameObject> BioMassSandbox => _bioMassSandbox;

        public UnityEvent<int, int> OnEnemyCount = new UnityEvent<int, int>();

        public UnityEvent OnSurvivorPrepare = new UnityEvent();
        public UnityEvent<int, int, float> OnTimeCountDown = new UnityEvent<int, int, float>();
        public UnityEvent<int> OnWaveStarted = new UnityEvent<int>();
        public UnityEvent<int, float, int> OnWaveEnded = new UnityEvent<int, float, int>(); //Номер закончившейся волны и Время до следующей волны и текущий отсчет
        public UnityEvent<float> OnWavePaused = new UnityEvent<float>(); //Номер закончившейся волны и Время до следующей волны

        [SerializeField]
        protected bool _sandboxReady;

        protected List<GameObject> _bioMass = new List<GameObject>();
        protected List<GameObject> _bioMassSandbox = new List<GameObject>();

        protected int _spawnedIndex;

        protected GameObject _botPrefab;

        protected GameObject _lastBot;

        protected int _attackTokensCount = 3;

        protected List<GameObject> _crowdAttackers = new List<GameObject>();

        [SerializeField]
        protected List<BotConfig> _botsData;

        public HashSet<string> BotThemes { get; private set; } = new HashSet<string>();
        public static readonly string CUSTOM_BOT_THEME = "custom";

        private int _waveIndex = 1;
        private float _time = 5, _timeCur;
        private int _timeCurPrev;
        private int _score = 0;


        [SerializeField]
        private List<Transform> _players = new List<Transform>();



        public virtual void Awake()
        {
            Instance = this;
        }
        
        private async void LoadBotData()
        {
            BotCollection botcoll = DataLoader.ReadJson<BotCollection>("bot.json");
            //BotCollection botcoll = await DataLoader.LoadJsonFile<BotCollection>("bot.json");

            if (botcoll == null)
            {
                _botsData = new List<BotConfig>();
                _botsData.Add(new BotConfig());
                _botsData[0].Name = "Default";
                _botsData[0].SkinIndex = 0;
                _botsData[0].Addrlink = "Toxin";
                _botsData[0].ConfigureInternal(10, 2, 5);
            }
            else
            {
                _botsData = botcoll.bots;
            }
            GenerateGroups(_botsData);
            //Set default bot for sandbox mode
            GameConfig.CurrentConfiguration.BotConfig = _botsData[UnityEngine.Random.Range(0,_botsData.Count-1)];
            StartCoroutine(PreloadAssets("bots"));
        }

        private void GenerateGroups(List<BotConfig> botsData) {
            BotThemes = new HashSet<string>();
            foreach (var bot in botsData) {
                BotThemes.Add(bot.BotThemeName);
            }
            BotThemes.Add(CUSTOM_BOT_THEME);
        }

        public IEnumerator PreloadAssets(string key)
        {

            AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(key);
            yield return getDownloadSize;

            //If the download size is greater than 0, download all the dependencies.
            if (getDownloadSize.Result > 0)
            {
                AsyncOperationHandle downloadDependencies = Addressables.DownloadDependenciesAsync(key);
                downloadDependencies.Completed += (AsyncOperationHandle handle) => {
                    handle.Release();
                };
                yield return downloadDependencies;

                if (downloadDependencies.IsDone)
                {
                    //log.text += "Load Complete";
                    Debug.Log("Load Complete");
                }
                else
                {
                    //log.text += $"Load error: {downloadDependencies.Status}";
                    Debug.Log($"Load error: {downloadDependencies.Status}");
                }
            }
            else
            {
                Debug.Log("No required downloads for bots");
            }
        }

        public virtual void Start()
        {
            GameObject targetPointPrefab = Resources.Load<GameObject>("TargetPoint");// AssetDatabase.LoadAssetAtPath("Assets/PointArrow/TargetPoint.prefab", typeof(GameObject));
            _botPrefab = Resources.Load<GameObject>("Bot");
        }

        public void StartSanboxMission()
        {
            if(_bioMassSandbox.Count == 0)
            {
                AbortSandBox();
            }
            else
            {
                bool allEmpty = true;

                for(int i =0; i< _bioMassSandbox.Count; i++)
                {
                    if (_bioMassSandbox[i] == null)
                    {

                    }
                    else
                    {
                        allEmpty = false;
                    }
                }

                if(allEmpty)
                {
                    AbortSandBox();
                }
                else
                {
                    _sandboxReady = true;

                    KillingCountSandBox = 0;

                    for (int i = 0; i < _bioMassSandbox.Count; i++)
                    {
                        if (_bioMassSandbox[i] != null)
                        {
                            KillingCountSandBox++;
                        }
                    }

                    EventsBus.Instance.OnSandBoxStart?.Invoke();
                    EventsBus.Instance.OnEnemyCount?.Invoke(0, KillingCountSandBox);
                }
            }
        }

        public void AbortSandBox()
        {
            _sandboxReady = false;
            EventsBus.Instance.OnSandBoxAborted?.Invoke();

            if (CantStartSandBox.Instance == null)
            {

            }
            else
            {
                CantStartSandBox.Instance.Show();
            }
        }

        public void AbortSanboxMission()
        {
            _sandboxReady = false;
            EventsBus.Instance.OnSandBoxAborted?.Invoke();
        }

        public void ClearSandboxBiomass()
        {
            for(int i=0; i< _bioMassSandbox.Count; i++)
            {
                if (_bioMassSandbox[i] != null)
                {
                    Destroy(_bioMassSandbox[i]);
                }
            }

            _bioMassSandbox.Clear();
            _bioMassSandbox = new List<GameObject>();
        }

        public virtual void BotDead(GameObject bot, GameObject killer)
        {
            _bioMass.Remove(bot);
            OnEnemyCount?.Invoke(_bioMass.Count, _waveIndex * 2);

            if (killer == Character.Instance.gameObject)
            {
                AddScore(1);
            }
        }

        private void AddScore(int v)
        {
            _score += v;
        }

        public virtual void Update()
        {

        }

        [ContextMenu("Spawn New Bot")]
        public virtual void SpawnNewBot()
        {

        }

        public virtual void SpawnNewBot(Vector3 position)
        {
            GameObject bot = Instantiate(_botPrefab);
            _lastBot = bot;
        }

        protected virtual BotConfig GetConfig(string botName, string theme)
        {
            var config = _botsData.Find(bot => bot.Name == botName && bot.BotTheme == theme);
            if (config == null)
            {
                Debug.LogWarning($"Конфигурация для бота {botName} с темой {theme} не найдена!");
                return _botsData[0]; // Возвращаем первую доступную конфигурацию как запасной вариант
            }
            return config;
        }

        public virtual void RegisterBot(GameObject dude)
        {
            _bioMass.Add(dude);
        }

        public virtual void ClearBioMass()
        {
            for (int i = 0; i < _bioMass.Count; i++)
            {
                Destroy(_bioMass[i]);
            }
            _bioMass.Clear();
            _bioMass = new List<GameObject>();
        }

        public bool CanIHitThePlayer()
        {
            bool result = true;
            result = _crowdAttackers.Count < _attackTokensCount;
            return result;
        }

        public Transform CetTarget()
        {
            Transform result = null;
            /*
            if (GameWorld.Instance.IsWorldCreated)
            {
                if (_players.Count > 0)
                {
                    Debug.Log("_players.Count > 0");
                    result = _players[0];
                }
            }
            */
            return result;
        }

        public void RegisterPlayer(Transform player)
        {
            _players.Add(player);
        }

        public void RemovePlayer(Transform player)
        {
            _players.Remove(player);
        }

        public void TakeToken(GameObject bot)
        {
            if (_crowdAttackers.Contains(bot))
            {

            }
            else
            {
                _crowdAttackers.Add(bot);
            }

            //_attackTokensCurent++;
        }

        public void SetTokenCount(int attackTokensCount)
        {
            _attackTokensCount = attackTokensCount;
        }

        public void ReturnToken(GameObject bot)
        {
            if (_crowdAttackers.Contains(bot))
            {
                _crowdAttackers.Remove(bot);
            }
            else
            {
            }
            //_attackTokensCurent--;
        }

    }

    public class BotCollection
    {
        public List<BotConfig> bots;
    }
}
