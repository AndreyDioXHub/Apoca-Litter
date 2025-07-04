using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailedPanel : MonoBehaviour {

    [SerializeField]
    private Button _reloadScene;
    [SerializeField]
    private Button _returnToMainScene;

    // Start is called before the first frame update
    void Awake() {
        _reloadScene.onClick.AddListener(OnReloadSceneClicked);
        _returnToMainScene.onClick.AddListener(OnReturnToMainSceneClicked);
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnReloadSceneClicked() {
        // Add logic to reload the scene
        AddressableSceneManager.Instance.ReloadCurrentScene().Forget();
    }

    private void OnReturnToMainSceneClicked() {
        // Add logic to return to the main scene
        AddressableSceneManager.Instance.LoadBuiltinScene(1).Forget();
    }
}
