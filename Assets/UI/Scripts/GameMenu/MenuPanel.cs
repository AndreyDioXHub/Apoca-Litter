using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{

    public bool IsMainPanel = false;
    public bool EnableOnStart = false;
    // Start is called before the first frame update
    async void Awake()
    {
        await UniTask.WaitUntil(() => MainMenuPanelManager.Instance != null);
        MainMenuPanelManager.Instance.RegisterPanel(this);
    }

    #region Для подключения через редактор
    public void ShowMenu(string menuGameObjectName) {
         MainMenuPanelManager.Instance.SwitchPanel(menuGameObjectName);
    }

    public void Back() {
        MainMenuPanelManager.Instance.Back();
    }

    public void Close() {
        MainMenuPanelManager.Instance.Close(this);
    }
    #endregion
}
