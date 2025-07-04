using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSelectButton : MonoBehaviour
{
    public string WeaponName => _weaponName;

    [SerializeField]
    private string _weaponName;
    [SerializeField]
    private GameObject _active;
    [SerializeField]
    private GameObject _buyed;

    private PlayerNetworkResolver _resolver;
    private Character _character;
    private Inventory _inventory;
    private WeaponScreenWhele _whele;
    private int _index;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Init(int index, WeaponScreenWhele whele, PlayerNetworkResolver resolver, Character character, Inventory inventory)
    {
        _index = index;

        _whele = whele;
        _resolver = resolver;
        _character = character;
        _inventory = inventory;

        _resolver.OnWeaponChanged.AddListener(WeaponChanged);

    }

    private void WeaponChanged(int oldw, int neww)
    {
        if (_index == neww)
        {
            _active.SetActive(true);
        }
        else
        {
            _active.SetActive(false);
        }

        /*
        string buyedWeapon1 = _inventory.BuyedWeapon1;
        string buyedWeapon2 = _inventory.BuyedWeapon2;

        _buyed.SetActive(!(buyedWeapon1.Equals(_weaponName) || buyedWeapon2.Equals(_weaponName)));*/
    }

    public void SetActiveState(bool state)
    {
        _active.SetActive(state);
    }

    public async void Active()
    {
        _whele.HideAll();
        _active.SetActive(true);

        MenuPuppeteer.Instance.RelaxHoldingButtonFire();
        await UniTask.WaitForSeconds(0.3f);
        _character.Equip(_index);

        if (LocalSounds.Instance != null)
        {
            LocalSounds.Instance.PlaySound("weaponselect");
        }

        //Active(_index);
    }

    public void SetBuyed(bool state)
    {
        _buyed.SetActive(state);
    }

    /*
    public void Active(int index)
    {
        if (index == _index)
        {
            _active.SetActive(true);
        }
        else
        {
            _active.SetActive(false);
        }
    }*/
}
