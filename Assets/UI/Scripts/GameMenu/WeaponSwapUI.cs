using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwapUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _text;
    private Image _emblemImage;
    private Dictionary<AmmoType, string> _emblemDict;


    // Start is called before the first frame update
    void Awake()
    {
        _emblemImage = GetComponent<Image>();

        // Инициализация словаря с иконками для каждого типа боеприпасов
        _emblemDict = new Dictionary<AmmoType, string>()
        {
            {AmmoType.pistol, "pistol_ammo_icon"},
            {AmmoType.grenade, "grenade_ammo_icon"},
            {AmmoType.grenadeLaunched, "Grenade-Launcher-Icon"},
            {AmmoType.rocket, "Rocket-Launcher-Icon"},
            {AmmoType.rifle, "rifle_ammo_icon"},
            {AmmoType.smg, "smg-icon"},
            {AmmoType.shotgun, "ShotGun-Icon"},
            {AmmoType.sniperRifle, "SR-Icon"},
            {AmmoType.pgun, "pgun_ammo_icon"},
            {AmmoType.jackhammer, "Drill-Icon"}
        };
    }

    async void Start()
    {
        await UniTask.WaitUntil(() => Inventory.Instance != null);
        Inventory.Instance.OnWeaponSelected.AddListener(SwapEmblem);
        SwapEmblem(Inventory.Instance.GetEquipped());
    }

    private void SwapEmblem(GameObject newWeapon)
    {
        Weapon wp = newWeapon.GetComponent<Weapon>();
        SwapEmblem(wp);
    }
    private void SwapEmblem(Weapon wp)
    {
        if (wp != null)
        {
            AmmoType ammoType = wp.GetAmmoType();

            if (ammoType == AmmoType.pgun || ammoType == AmmoType.jackhammer)
            {
                _text.SetActive(false);
            }
            else
            {
                _text.SetActive(true);
            }

            // Получаем имя иконки из словаря
            string emblemName = _emblemDict[ammoType];
            // Загружаем спрайт из ресурсов и устанавливаем его в Image компонент
            Sprite emblemSprite = Resources.Load<Sprite>($"{emblemName}");
            if (emblemSprite != null)
            {
                _emblemImage.sprite = emblemSprite;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
