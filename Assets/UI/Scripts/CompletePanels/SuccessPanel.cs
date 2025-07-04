using Cysharp.Threading.Tasks;
using game.configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuccessPanel : MonoBehaviour
{
    [SerializeField]
    private Button _levelRestartButton;
    [SerializeField]
    private Button _toMainMenuButton;
    [SerializeField]
    private Button _nextLevelButton;

    // Start is called before the first frame update
    void Awake()
    {
        _levelRestartButton.onClick.AddListener(() => {
            AddressableSceneManager.Instance.ReloadCurrentScene().Forget();
        });
        _toMainMenuButton.onClick.AddListener(() => {
            AddressableSceneManager.Instance.LoadBuiltinScene(1).Forget();
        });
        _nextLevelButton.onClick.AddListener(() => {
            AddressableSceneManager.Instance.LoadNextScene(GameConfig.CurrentConfiguration.CurrentScene).Forget();
        });

        if (GameConfig.CurrentConfiguration.GameMode == GameConfig.GameModes.Missions) {
            _nextLevelButton.onClick.AddListener(() => {
                EventsBus.Instance?.OnNextLevel?.Invoke();
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
