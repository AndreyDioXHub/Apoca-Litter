using Cysharp.Threading.Tasks;
using game.configuration;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootUI : MonoBehaviour
{
    public static RootUI Instance;

    [SerializeField]
    private GameObject _settingScreen;
    [SerializeField]
    private GameObject _termOfUseScreen;
    [SerializeField]
    private GameObject _privacyPolicyScreen;
    [SerializeField]
    private GameObject _inventoryScreen;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _settingScreen.SetActive(false);
        _inventoryScreen.SetActive(false);
        WaitingPlayer();

        PressEsc();
        PressTab();
    }
    private async void WaitingPlayer()
    {
        while (Character.Instance == null)
        {
            await UniTask.Yield(); // ќжидание следующего кадра
        }

        Character.Instance.OnPressEsc.AddListener(PressEsc);
        Character.Instance.OnPressTab.AddListener(PressTab);
    }

    /*
    private async void WaitingPlayer()
    {
        while (Character.Instance == null)
        {
            await UniTask.Yield(); // ќжидание следующего кадра
        }

        Character.Instance.OnPressEsc.AddListener(OpenSetting);
    }*/

    public void PressEsc()
    {
        Debug.Log("PressEsc");

        _termOfUseScreen.SetActive(false);
        _privacyPolicyScreen.SetActive(false);

        if (_inventoryScreen.activeSelf)
        {
            _inventoryScreen.SetActive(false);
        }
        else
        {
            if (_settingScreen.activeSelf)
            {
                _settingScreen.SetActive(false);
            }
            else
            {
                _settingScreen.SetActive(true);
            }
        }
    }

    public void PressTab()
    {
        if(GameConfig.CurrentConfiguration.GameMode == GameConfig.GameModes.Missions)
        {
            return;
        }

        if (_inventoryScreen.activeSelf) {
            _inventoryScreen.GetComponent<MenuPanel>().Close();
        } else {
            OpenInventoryScreen();
        }
    }

    public void OpenSetting()
    {
        MainMenuPanelManager.Instance.SwitchPanel(_settingScreen.name);
    }

    public void OpenTermOfUse()
    {
        MainMenuPanelManager.Instance.SwitchPanel(_termOfUseScreen.name);
    }

   
    public void OpenPrivacyPolicy()
    {
        MainMenuPanelManager.Instance.SwitchPanel(_privacyPolicyScreen.name);
    }

    

    public void OpenInventoryScreen() {
        MainMenuPanelManager.Instance.SwitchPanel(_inventoryScreen.name);
    }

    void Update()
    {
        
    }
}
