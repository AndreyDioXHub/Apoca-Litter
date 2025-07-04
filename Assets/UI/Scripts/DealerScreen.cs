using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerScreen : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;
    [SerializeField]
    private Character _character;
    [SerializeField]
    private Inventory _inventory;
    [SerializeField]
    private DealerZone _dealerZone;
    [SerializeField]
    private GameObject _localPlayer;

    [SerializeField]
    private GameObject _healButton;
    [SerializeField]
    private GameObject _healButtonAds;
    [SerializeField]
    private GameObject _weaponButton;
    [SerializeField]
    private GameObject _weaponButtonAds;
    [SerializeField]
    private GameObject _buttonGetTask;
    [SerializeField]
    private GameObject _buttonGetTaskAds;
    [SerializeField]
    private GameObject _buttonPushTask;
    [SerializeField]
    private GameObject _wrongDealerZone;
    [SerializeField]
    private GameObject _kill2Zombies;
    [SerializeField]
    private int _taskDropZoneID;
    private bool _shoodShowetTask;

    private int _costTask = 10;
    private int _costHeal = 20;
    private int _costAmmo = 30;
    private int _rewardTask = 40;

    private bool _isMayShowAds = false;

    public void SetDealerZone(DealerZone dealerZone)
    {
        _dealerZone = dealerZone;
    }

    public void SetLocalPlayer(GameObject localPlayer)
    {
        _localPlayer = localPlayer;
    }

    public void Init(PlayerNetworkResolver resolver, Character character, Inventory inventory)
    {
        _resolver = resolver;
        _character = character;
        _inventory = inventory;
        CoinsManager.Instance.OnCoinsCountChanged.AddListener(UpdateButtons);
    }

    public void UpdateButtons()
    {
        bool needShowHeal = _character.GetLife() != _character.GetLifeTotal();
        bool enoughCoinsForHeal = CoinsManager.Instance.Coins >= _costHeal;

        _healButton.SetActive(needShowHeal && enoughCoinsForHeal);
        _healButtonAds.SetActive(needShowHeal && !enoughCoinsForHeal && _isMayShowAds);

        bool enoughCoinsForWeapon = CoinsManager.Instance.Coins >= _costAmmo;
        _weaponButton.SetActive(enoughCoinsForWeapon); 
        _weaponButtonAds.SetActive(!enoughCoinsForWeapon && _isMayShowAds); 

        bool enoughCoinsForTask = CoinsManager.Instance.Coins >= _costTask;
        _buttonGetTask.SetActive(enoughCoinsForTask && _shoodShowetTask);
        _buttonGetTaskAds.SetActive(!enoughCoinsForTask && _shoodShowetTask && _isMayShowAds);

        _kill2Zombies.SetActive(!enoughCoinsForTask && _shoodShowetTask && !_isMayShowAds);
    }

    public void Show()
    {

        if (LocalSounds.Instance!=null)
        {
            LocalSounds.Instance.PlaySound("chestopen");
        }

        gameObject.SetActive(true);

        _wrongDealerZone.SetActive(false);


        if (_resolver.DealerZoneForTask == null)
        {
            _shoodShowetTask = true;
            _buttonPushTask.SetActive(false);
        }
        else
        {
            if( _resolver.DealerZoneCurent == null)
            {
                _shoodShowetTask = false;
                _buttonPushTask.SetActive(false);
            }
            else
            {
                if(_resolver.DealerZoneCurent == _resolver.DealerZoneForTask)
                {
                    _shoodShowetTask = false;
                    _buttonPushTask.SetActive(true);
                }
                else
                {
                    _shoodShowetTask = false;
                    _buttonPushTask.SetActive(false);
                    _wrongDealerZone.SetActive(true);
                }
            }
        }

        UpdateButtons();
    }

    public void HealPlayer()
    {
        if (CoinsManager.Instance.TryBuy(_costHeal))
        {
            _character.SetNeedHealPlayer();

            if (LocalSounds.Instance != null)
            {
                LocalSounds.Instance.PlaySound("moneyspend");
            }
        }
    }

    public void BuyAmmo()
    {
        if (CoinsManager.Instance.TryBuy(_costAmmo))
        {
            _inventory.BuyAmmoForBuyedWeapon();

            if (LocalSounds.Instance != null)
            {
                LocalSounds.Instance.PlaySound("moneyspend");
            }

            if (LocalSounds.Instance != null)
            {
                LocalSounds.Instance.PlaySound("casing");
            }
        }
    }
    public void HealPlayerAds()
    {
        _character.SetNeedHealPlayer();

        if (LocalSounds.Instance != null)
        {
            LocalSounds.Instance.PlaySound("moneyspend");
        }
    }

    public void BuyAmmoAds()
    {
        _inventory.BuyAmmoForBuyedWeapon();

        if (LocalSounds.Instance != null)
        {
            LocalSounds.Instance.PlaySound("moneyspend");
        }

        if (LocalSounds.Instance != null)
        {
            LocalSounds.Instance.PlaySound("casing");
        }
    }

    public void Hide()
    {
        if (LocalSounds.Instance != null)
        {
            LocalSounds.Instance.PlaySound("chestclose");
        }

        gameObject.SetActive(false);
        _resolver.SetDealerInside(false);

        try
        {
            _localPlayer.GetComponent<Teleport>().FastTeleportToPosition(_dealerZone.DealerOut.position);
        }
        catch (NullReferenceException ex)
        {

        }

        _localPlayer = null;
        _dealerZone = null;
    }

    void Start()
    {
        
    }

    void Update()
    {
    }

    public void GetTask()
    {
        if (CoinsManager.Instance.TryBuy(_costTask))
        {
            _resolver.SetTaskDealerZone(_dealerZone.SiblingZone);
            _resolver.DealerZoneForTask.ShowTaskMarker();
            _buttonGetTask.SetActive(false);
            _wrongDealerZone.SetActive(true);
            DealerZonesManager.Instance.HideDefaultMarkers();

            if (LocalSounds.Instance != null)
            {
                LocalSounds.Instance.PlaySound("moneyspend");
            }

            Hide();
        }
    }

    public void GetTaskAds()
    {
        _resolver.SetTaskDealerZone(_dealerZone.SiblingZone);
        _resolver.DealerZoneForTask.ShowTaskMarker();
        _buttonGetTask.SetActive(false);
        _wrongDealerZone.SetActive(true);
        DealerZonesManager.Instance.HideDefaultMarkers();

        Hide();
    }

    public async void LateHide()
    {
        await UniTask.WaitForSeconds(1);
        Hide();
        _resolver.IsPushTask = false;
    } 

    public void PushTask()
    {
        _resolver.DealerZoneForTask.HideTaskMarker();
        _resolver.SetTaskDealerZone(null);
        _resolver.OnPushTask?.Invoke();
        _buttonGetTask.SetActive(false);
        _buttonPushTask.SetActive(false);
        _wrongDealerZone.SetActive(true);
        DealerZonesManager.Instance.ShowDefaultMarkers();
        DealerZonesManager.Instance.AssignRandomSiblings();
        CoinsManager.Instance.AddCoins(_rewardTask);

        if (LocalSounds.Instance != null)
        {
            LocalSounds.Instance.PlaySound("moneyget");
        }

        Hide();
    }
}
