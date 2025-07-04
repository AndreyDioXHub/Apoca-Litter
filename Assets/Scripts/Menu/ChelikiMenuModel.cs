using game.configuration;
using InfimaGames.LowPolyShooterPack;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChelikiMenuModel : MonoBehaviour
{
    public UnityEvent<string, int> OnSelectNameIndex = new UnityEvent<string, int>(); 
    public UnityEvent<string, string> OnSkinChanged = new UnityEvent<string, string>(); 

    [SerializeField]
    private GameObject _selectBodyMenu;
    [SerializeField]
    private GameObject _extraSkinMenu;
    [SerializeField]
    private GameObject _extraSkinButton;
    [SerializeField]
    private GameObject _inventoryScreen;
    [SerializeField]
    private GameObject _policyScreen;

    [SerializeField]
    private List<GameObject> _bodies = new List<GameObject>();

    [SerializeField]
    private int _bodyIndex = 0;

    [SerializeField]
    private GameObject _gameSettingWindow;
    [SerializeField]
    private GameObject _connectionErrorMenu;
    [SerializeField]
    private GameObject _versionErrorMenu;
    [SerializeField]
    private ServerInfoText _serverInfoText;
    /*
    [SerializeField]
    private GameObject _commonGame;
    [SerializeField]
    private GameObject _steamGame;
    */

    [SerializeField]
    private ExtraSkinMenuController _extraSkinMenuController;

    /*
    [SerializeField]
    private GameObject _skinMenu;
    [SerializeField]
    private GameObject _hairMenu;*/
    /*
    [SerializeField]
    private GameObject _prevWindow;

    [SerializeField]
    private GameObject _curentWindow;*/

    private PlayerNetworkResolver _resolver;
    private Character _character;
    private Inventory _inventory;

    void Start()
    {
        if (WaitingScreenModel.Instance == null)
        {
            if (GameConfig.CurrentConfiguration.IsWrongVersion)
            {
                GameConfig.CurrentConfiguration.IsWrongVersion = false;
                _versionErrorMenu.SetActive(true);
            }
        }
        else
        {
            if (_serverInfoText.gameObject.activeSelf)
            {
                Debug.Log("ServerInfoText sub");
                WaitingScreenModel.Instance.OnPlayersDictionaryChanged.AddListener(ServerPlayerCountChanged);
            }
        }

    }

    public void ServerPlayerCountChanged()
    {
        Debug.Log("ServerInfoText invoke");
        int count = WaitingScreenModel.Instance.Players.Count;
        _serverInfoText.UpdateValue(count);
    }

    public void SkinChanged(string name, string json)
    {
        _resolver.OnChangedSkinOnServer(name, json);
    }

    private void OnEnable()
    {
        ControlNetworkManager.singleton.OnClientDisconected.AddListener(ShowConnectionErrorMenu);
    }

    private void OnDisable()
    {
        if (ControlNetworkManager.singleton != null)
        {
            ControlNetworkManager.singleton.OnClientDisconected.RemoveListener(ShowConnectionErrorMenu);
        }
    }

    private void OnDestroy()
    {
        if (ControlNetworkManager.singleton != null)
        {
            if (_serverInfoText.gameObject.activeSelf)
            {
                WaitingScreenModel.Instance.OnPlayersDictionaryChanged.RemoveListener(ServerPlayerCountChanged);
            }
        }
    }

    void Update()
    {

    }

    [ContextMenu("ClearPref")]
    public void ClearPref()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Init(PlayerNetworkResolver resolver, Character character, Inventory inventory)
    {
        _resolver = resolver;
        _character = character;
        _inventory = inventory;

        if (_resolver == null)
        {

        }
        else
        {
            OnSkinChanged.AddListener(_resolver.OnChangedSkinOnServer);
        }

        if(PauseScreen.Instance == null)
        {

        }
        else
        {
            PauseScreen.Instance.RegisterSettings(this);
        }

        _extraSkinMenuController.Init(this);

        var equippedStatuses = GetComponentsInChildren<SkinPartEquippedStatus>(true);

        foreach (var status in equippedStatuses)
        {
            status.Init(this);
        }

        var defaultSkinModels = GetComponentsInChildren<DefaultSkinModel>(true);

        foreach (var chelik in defaultSkinModels)
        {
            chelik.Init(this);

            string json = PlayerPrefs.GetString(chelik.Name, "");

            DefaultSkinConfig config = new DefaultSkinConfig();

            if (string.IsNullOrEmpty(json))
            {
                config = new DefaultSkinConfig();
                config.sex = "Man";
                config.isActive = true;
                config.sexIndex = 0;
                config.hairCutIndex = Random.Range(0, 6);
                config.hairColorIndex = Random.Range(0, 11);
                config.faceIndex = 0;
                config.bodycolorIndex = 0;
                config.tshirtcolorIndex = Random.Range(0, 11);
                config.pantscolorIndex = Random.Range(0, 11);
                config.shoescolorIndex = Random.Range(0, 11);
            }
            else
            {
                config = JsonConvert.DeserializeObject<DefaultSkinConfig>(json);
            }

            OnSelectNameIndex?.Invoke("bodytype", PlayerPrefs.GetInt("bodytype", 0));
            OnSelectNameIndex?.Invoke("haircut", config.hairCutIndex);
            OnSelectNameIndex?.Invoke("haircolor", config.hairColorIndex);
            OnSelectNameIndex?.Invoke("face", config.faceIndex);
            OnSelectNameIndex?.Invoke("bodycolor", config.bodycolorIndex);
            OnSelectNameIndex?.Invoke("tshirtcolor", config.tshirtcolorIndex);
            OnSelectNameIndex?.Invoke("pantscolor", config.pantscolorIndex);
            OnSelectNameIndex?.Invoke("shoescolor", config.shoescolorIndex);
        }
    }

    public int GetIndexByName(string name)
    {
        int index = -1;

        var defaultSkinModels = GetComponentsInChildren<DefaultSkinModel>(true);

        foreach (var chelik in defaultSkinModels)
        {
            chelik.Init(this);

            string json = PlayerPrefs.GetString(chelik.Name, "");

            DefaultSkinConfig config = new DefaultSkinConfig();

            if (string.IsNullOrEmpty(json))
            {
                config = new DefaultSkinConfig();
                config.sex = "Man";
                config.isActive = true;
                config.sexIndex = 0;
                config.hairCutIndex = Random.Range(0, 6);
                config.hairColorIndex = Random.Range(0, 11);
                config.faceIndex = 0;
                config.bodycolorIndex = 0;
                config.tshirtcolorIndex = Random.Range(0, 11);
                config.pantscolorIndex = Random.Range(0, 11);
                config.shoescolorIndex = Random.Range(0, 11);
            }
            else
            {
                config = JsonConvert.DeserializeObject<DefaultSkinConfig>(json);
            }

            switch (name)
            {
                case "haircut":
                    index = config.hairCutIndex;
                    break;
                case "haircolor":
                    index = config.hairColorIndex;
                    break;
                case "face":
                    index = config.faceIndex;
                    break;
                case "bodycolor":
                    index = config.bodycolorIndex;
                    break;
                case "tshirtcolor":
                    index = config.tshirtcolorIndex;
                    break;
                case "pantscolor":
                    index = config.pantscolorIndex;
                    break;
                case "shoescolor":
                    index = config.shoescolorIndex;
                    break;
            }
        }

        return index;
    }

    public void OpenGameSettingWindow()
    {
        _gameSettingWindow.SetActive(true);

        /*
        if (GameConfig.CurrentConfiguration.IsSteam)
        {
            _steamGame.SetActive(true);
        }
        else
        {
            _commonGame.SetActive(true);
        }*/
    }

    public void HideGameSettingWindow()
    {
        _gameSettingWindow.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenInventory()
    {
        _inventoryScreen.SetActive(true);
        gameObject.SetActive(false);

        gameObject.GetComponent<Animator>().enabled = true;
        gameObject.GetComponent<CanvasGroup>().alpha = 0f;

        _inventoryScreen.GetComponent<Animator>().enabled = false;
        _inventoryScreen.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public void CloseInventory()
    {
        _inventoryScreen.SetActive(false);
        gameObject.SetActive(true);
        gameObject.GetComponent<Animator>().enabled = false;
        gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;

        _inventoryScreen.GetComponent<Animator>().enabled = true;
        _inventoryScreen.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void ShowHide()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OpenSelectBodyMenu()
    {
        _selectBodyMenu.SetActive(true);
    }
    public void CloseBodyWindow()
    {
        _selectBodyMenu.SetActive(false);
    }

    public void OpenPolicyScreen()
    {
        _policyScreen.SetActive(true);
    }
    public void ClosePolicyScreen()
    {
        _policyScreen.SetActive(false);
    }

    public void OpenExtraSkinMenu()
    {
        _extraSkinMenu.SetActive(true);
        _extraSkinButton.SetActive(false);
    }
    public void CloseExtraSkinMenu()
    {
        _extraSkinMenu.SetActive(false);
        _extraSkinButton.SetActive(true);
    }

    public void SelectBody(int index)
    {
        _bodyIndex = index;
        PlayerPrefs.SetInt("bodytype", _bodyIndex);
    }

    public void ShowConnectionErrorMenu()
    {
        if (_gameSettingWindow.activeSelf)
        {
            HideGameSettingWindow();
        }

        _connectionErrorMenu.SetActive(true);
    }

    public void HideConnectionErrorMenu()
    {
        _connectionErrorMenu.SetActive(false);
    }

    public void ShowGameSettingFromConnectionError()
    {
        HideConnectionErrorMenu();
        OpenGameSettingWindow();
    }

    public void TuningBody()
    {
        foreach (GameObject body in _bodies)
        {
            body.SetActive(false);
        }

        Debug.Log(_bodyIndex);
        _bodies[_bodyIndex].SetActive(true);
        //_curentWindow = _bodies[index];
    }

    public void Equip(string name, int index)
    {
        OnSelectNameIndex?.Invoke(name, index);

        switch (name)
        {
            case "bodytype":
                SelectBody(index);
                break;
            case "tuningbodytype":
                TuningBody();
                break;
        }

    }
    public void LeaveGame()
    {
        ControlNetworkManager.singleton.StopClient();

        if(_resolver.IsServer)
        {
            ControlNetworkManager.singleton.StopServer();
        }
    }

}
