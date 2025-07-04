using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;
using game.configuration;

public class AddressableSceneManager : MonoBehaviour {
    private static AddressableSceneManager _instance;

    public static GameObject CanvasCrossLoader { get; set; }

    public static AddressableSceneManager Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject("AddressableSceneManager");
                _instance = go.AddComponent<AddressableSceneManager>();
                DontDestroyOnLoad(go);
#if UNITY_EDITOR
                if (CanvasCrossLoader == null) {
                    // Получить префаб из Assets по пути: Assets/UI/Prefabs/CanvasLoadCrossScene.prefab
                    CanvasCrossLoader = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UI/Prefabs/CanvasLoadCrossScene.prefab");
                }
#endif
            }
            return _instance;
        }
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private AsyncOperationHandle<SceneInstance>? _currentSceneHandle;

    public async UniTask<bool> LoadScene(string sceneName) {
        Debug.Log($"Loading scene: {sceneName}");

        GameObject sliderPanel = Instantiate(CanvasCrossLoader);
        DontDestroyOnLoad(sliderPanel);
        CrossSceneLoaderPanel loaderPanel = sliderPanel.AddComponent<CrossSceneLoaderPanel>();
        loaderPanel.Init();
        await UniTask.Yield();

        // Загрузка addressable с тегом "bots"
        var loadBotsHandle = Addressables.LoadAssetsAsync<object>("bots", null);

        // Отслеживаем прогресс загрузки addressable с тегом "bots"
        while (!loadBotsHandle.IsDone) {
            float progress = loadBotsHandle.PercentComplete * 0.5f; // 50% на загрузку addressable
            loaderPanel.SetPercents(progress);
            await UniTask.Yield();
        }

        if (loadBotsHandle.Status != AsyncOperationStatus.Succeeded) {
            Debug.LogError("Failed to load addressable assets with tag 'bots'");
            return false;
        }

        // Выгружаем предыдущую addressable сцену
        if (_currentSceneHandle.HasValue) {
            await Addressables.UnloadSceneAsync(_currentSceneHandle.Value).ToUniTask();
            _currentSceneHandle = null;
        }

        // Загружаем новую сцену аддитивно
        var loadSceneHandle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // Отслеживаем прогресс загрузки сцены
        while (!loadSceneHandle.IsDone) {
            float progress = 0.5f + loadSceneHandle.PercentComplete * 0.5f; // 50% на загрузку сцены
            loaderPanel.SetPercents(progress);
            await UniTask.Yield();
        }

        if (loadSceneHandle.Status == AsyncOperationStatus.Succeeded) {
            _currentSceneHandle = loadSceneHandle;
            SceneManager.SetActiveScene(loadSceneHandle.Result.Scene);
            loaderPanel.RunSecondPart(loadSceneHandle.Result.Scene);
            return true;
        }

        Debug.LogError($"Failed to load scene: {sceneName}");
        return false;
    }

    public async UniTask<bool> LoadBuiltinScene(int sceneIndex) {
        // Проверяем, что сцена существует
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings) {
            Debug.LogError($"Scene with index {sceneIndex} not found in build settings");
            return false;
        }

        GameObject sliderPanel = Instantiate(CanvasCrossLoader);
        DontDestroyOnLoad(sliderPanel);
        CrossSceneLoaderPanel loaderPanel = sliderPanel.AddComponent<CrossSceneLoaderPanel>();
        loaderPanel.Init(1);
        await UniTask.Yield();

        // Запускаем асинхронную загрузку сцены
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        // Отслеживаем прогресс загрузки
        while (!asyncOperation.isDone) {
            float progress = asyncOperation.progress;
            loaderPanel.SetPercents(progress);
            await UniTask.Yield();
        }

        // Проверяем успешность загрузки
        if (asyncOperation.isDone) {
            loaderPanel.RunBuiltinPart();
            return true;
        }

        Debug.LogError($"Failed to load scene with index: {sceneIndex}");
        return false;
    }

    public async UniTask<bool> ReloadCurrentScene() {
        Scene currentScene = SceneManager.GetActiveScene();

        // Проверяем, что текущая сцена не является builtin
        if (currentScene.buildIndex != -1) {
            Debug.LogWarning("Cannot reload builtin scene through AddressableSceneManager");
            return false;
        }

        // Получаем имя текущей сцены
        string currentSceneName = currentScene.name;

        // Используем существующий метод LoadScene для перезагрузки
        return await LoadScene(currentSceneName);
    }

    private void OnDestroy() {
        if (_currentSceneHandle.HasValue) {
            Addressables.UnloadSceneAsync(_currentSceneHandle.Value).Completed += (handle) => {
                Addressables.Release(handle);
            };
        }
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public async UniTask<bool> LoadNextScene(SceneInfo currentScene) {
        // Получаем индекс текущей сцены
        int currentIndex = GameConfig.CurrentConfiguration.AllScenes.MissionScenes.IndexOf(currentScene);

        // Определяем индекс следующей сцены
        int nextIndex = currentIndex + 1;

        bool loadLocal = nextIndex < GameConfig.CurrentConfiguration.AllScenes.MissionScenes.Count ? false : true;
        if (loadLocal) {
            // Если следующая сцена не найдена, загружаем сцену с индексом 1
            GameConfig.CurrentConfiguration.CurrentScene = null;
            await SceneManager.LoadSceneAsync(1, LoadSceneMode.Single).ToUniTask();
            return false;
        } else {
            string nextSceneName = GameConfig.CurrentConfiguration.AllScenes.MissionScenes[nextIndex].SceneName;
            bool success = await LoadScene(nextSceneName);
            if (success) {
                GameConfig.CurrentConfiguration.CurrentScene = GameConfig.CurrentConfiguration.AllScenes.MissionScenes[nextIndex];
            }
            // Загружаем следующую сцену из списка
            return success;
        }
    }
}
