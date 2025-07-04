using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ChangeAttachementScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject _buyWeaponButton;
    [SerializeField]
    private GameObject _buyAmmoForBuyedWeaponButton;
    [SerializeField]
    private GameObject _noAmmoBaner;

    [SerializeField]
    private WeaponScreenWhele _weaponWhele;

    [SerializeField]
    private RectTransform _attachementsRect;
    [SerializeField]
    private WeaponStatisticInfoScreenText _descriptionScript;
    [SerializeField]
    private List<GameObject> _weaponTitles = new List<GameObject>();


    [SerializeField]
    private TextMeshProUGUI _equipedWeapon1InfoName;
    [SerializeField]
    private TextMeshProUGUI _equipedWeapon2InfoName;
    [SerializeField]
    private TextMeshProUGUI _selectedWeaponInfoName;

    [SerializeField]
    private TextMeshProUGUI _equipedWeapon1InfoDamage;
    [SerializeField]
    private TextMeshProUGUI _equipedWeapon2InfoDamage;
    [SerializeField]
    private TextMeshProUGUI _selectedWeaponInfoDamage;

    private int _curentWeaponIndex = 0;

    private int _scopeCurent = 0;
    private int _scopesMaxCount = 8;
    private int _muzzleCurent = 0;
    private int _muzzlesMaxCount = 4;
    private int _laserCurent = 0;
    private int _lasersMaxCount = 2;
    private int _gripCurent = 0;
    private int _gripsMaxCount = 3;

    private List<SelectAttachementButton> _scopesUIElements = new List<SelectAttachementButton>();
    private List<SelectAttachementButton> _muzzlesUIElements = new List<SelectAttachementButton>();
    private List<SelectAttachementButton> _gripsUIElements = new List<SelectAttachementButton>();
    private List<SelectAttachementButton> _lasersUIElements = new List<SelectAttachementButton>();

    private PlayerNetworkResolver _resolver;
    private Character _character;
    private Inventory _inventory;
    private Weapon _weapon;

    private string _name;

    void Start()
    {

    }

    void Update()
    {

    }

    [ContextMenu("Init")]
    public void Init(PlayerNetworkResolver resolver, Character character, Inventory inventory)
    {
        _resolver = resolver;
        _character = character;
        _inventory = inventory;
        _resolver.OnWeaponChanged.AddListener(WeaponChanged);
        _weaponWhele.Init(resolver, _character, inventory);

        PauseScreen.Instance.RegisterAttachementScreen(this);

        //EquipWeaponAfterInit();
    }

    /*
    public async void EquipWeaponAfterInit()
    {
        await UniTask.Yield();
        int equippedIndex = PlayerPrefs.GetInt(PlayerPrefsConsts.equippedIndex, 0);
        Debug.Log($"equippedIndex {equippedIndex}");
        _character.Equip(equippedIndex);
    }*/

    public void RegisterScope(SelectAttachementButton scope)
    {
        _scopesUIElements.Add(scope);
    }
    public void RegisterMuzzle(SelectAttachementButton muzzle)
    {
        _muzzlesUIElements.Add(muzzle);
    }

    public void RegisterGrip(SelectAttachementButton grip)
    {
        _gripsUIElements.Add(grip);
    }

    public void RegisterLaser(SelectAttachementButton laser)
    {
        _lasersUIElements.Add(laser);
    }

    public void ShowWhenNoAmmo()
    {
        if (gameObject.activeSelf)
        {

        }
        else
        {
            ShowHide();
        }

        _noAmmoBaner.SetActive(true);
    }

    public void ShowHide()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            EquipBuyedWeaponBeforGame();
        }
        else
        {
            gameObject.SetActive(true);

            GetComponent<Animator>().enabled = true;
            GetComponent<CanvasGroup>().alpha = 0f;
        }

        WaitingUIElements();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        EquipBuyedWeaponBeforGame();
        WaitingUIElements();
    }

    public async void WaitingUIElements()
    {
        while (_scopesUIElements.Count == 0 && _muzzlesUIElements.Count == 0 &&
            _gripsUIElements.Count == 0 && _lasersUIElements.Count == 0)
        {
            await UniTask.Yield(); // ќжидание следующего кадра
        }

        _weapon = _inventory.transform.GetChild(_inventory.GetEquippedIndex()).GetComponent<Weapon>();
        _name = _weapon.WeaponName;

        WaitingAttachmentManager();
    }

    private void WeaponChanged(int oldw, int neww)
    {
        //Debug.Log($"{oldw} {neww}");

        _weapon = _inventory.transform.GetChild(neww).GetComponent<Weapon>();
        _name = _weapon.WeaponName;

        _curentWeaponIndex = neww;

        PlayerPrefs.SetInt(PlayerPrefsConsts.equippedIndex, neww);
        //Debug.Log($"equippedIndex WeaponChanged {neww}");

        foreach (var title in _weaponTitles)
        {
            title.SetActive(true);
        }

        _weaponTitles[neww].SetActive(true);

        string buyedWeapon1 = _inventory.BuyedWeapon1;
        string buyedWeapon2 = _inventory.BuyedWeapon2;

        int indexBuyedWeapon1 = _weaponWhele.GetBuyedWeaponIndex(buyedWeapon1);
        int indexBuyedWeapon2 = _weaponWhele.GetBuyedWeaponIndex(buyedWeapon2);

        _equipedWeapon1InfoName.text = _weaponTitles[indexBuyedWeapon1].GetComponent<TextMeshProUGUI>().text;
        _equipedWeapon2InfoName.text = _weaponTitles[indexBuyedWeapon2].GetComponent<TextMeshProUGUI>().text;
        _selectedWeaponInfoName.text = _weaponTitles[neww].GetComponent<TextMeshProUGUI>().text;

        _equipedWeapon1InfoDamage.text = _inventory.transform.GetChild(indexBuyedWeapon1).GetComponent<Weapon>().WeaponDamage.ToString();
        _equipedWeapon2InfoDamage.text = _inventory.transform.GetChild(indexBuyedWeapon2).GetComponent<Weapon>().WeaponDamage.ToString();
        _selectedWeaponInfoDamage.text = _weapon.WeaponDamage.ToString();

        _buyWeaponButton.SetActive(!(buyedWeapon1.Equals(_weapon.GetName()) || buyedWeapon2.Equals(_weapon.GetName())));
        _buyAmmoForBuyedWeaponButton.SetActive((buyedWeapon1.Equals(_weapon.GetName()) || buyedWeapon2.Equals(_weapon.GetName())));

        WaitingAttachmentManager();
    }

    public void BuyWeapon()
    {
        if (LocalSounds.Instance != null)
        {
            LocalSounds.Instance.PlaySound("weaponbuy");
        }

        BuyWeaponAsync();
    }

    public void BuyAmmoForBuyedWeapon()
    {
        _inventory.BuyAmmoForBuyedWeapon();
    }

    public async void BuyWeaponAsync()
    {
        await UniTask.Yield();
        _inventory.SetNewBuyedWeapon(_weapon.GetName());
        await UniTask.Yield();

        WeaponChanged(_curentWeaponIndex, _curentWeaponIndex);
        _weaponWhele.SetBuyedButtons();

        /*
        string buyedWeapon1 = _inventory.BuyedWeapon1;
        string buyedWeapon2 = _inventory.BuyedWeapon2;
        _buyButton.SetActive(!(buyedWeapon1.Equals(_weapon.GetName()) || buyedWeapon2.Equals(_weapon.GetName())));*/
    }

    public void EquipBuyedWeaponBeforGame()
    {
        _character.EquipbuyedWeapon1();
    }

    private async void WaitingAttachmentManager()
    {
        while (_weapon.AttachmentManager == null)
        {
            await UniTask.Yield(); // ќжидание следующего кадра
        }

        _scopesMaxCount = _weapon.AttachmentManager.ScopesMaxCount - 1;
        _muzzlesMaxCount = _weapon.AttachmentManager.MuzzlesMaxCount - 1;
        _lasersMaxCount = _weapon.AttachmentManager.LasersMaxCount - 1;
        _gripsMaxCount = _weapon.AttachmentManager.GripsMaxCount - 1;

        _scopeCurent = _weapon.AttachmentManager.ScopeIndex;
        _muzzleCurent = _weapon.AttachmentManager.MuzzleIndex;
        _laserCurent = _weapon.AttachmentManager.LaserIndex;
        _gripCurent = _weapon.AttachmentManager.GripIndex;

        UpdateUI();
        UpdateElements();
    }

    public void UpdateUI()
    {
        if (_name.Equals("Rocket Launcher 01"))
        {
            for (int i = 1; i < _scopesUIElements.Count; i++)
            {
                _scopesUIElements[i].gameObject.SetActive(false);
            }

            for (int i = 1; i < _muzzlesUIElements.Count; i++)
            {
                _muzzlesUIElements[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 1; i < _scopesUIElements.Count; i++)
            {
                _scopesUIElements[i].gameObject.SetActive(true);
            }

            for (int i = 1; i < _muzzlesUIElements.Count; i++)
            {
                _muzzlesUIElements[i].gameObject.SetActive(true);
            }
        }
    }

    public async void UpdateElements()
    {
        foreach (var scope in _scopesUIElements)
        {
            scope.SetActiveState(false);
        }

        foreach (var muzzle in _muzzlesUIElements)
        {
            muzzle.SetActiveState(false);
        }

        foreach (var grip in _gripsUIElements)
        {
            grip.SetActiveState(false);
        }

        foreach (var laser in _lasersUIElements)
        {
            laser.SetActiveState(false);
        }

        await UniTask.Yield();

        /*
        if (_weapon == null)
        {
            _weapon = GetComponent<Weapon>();
        }*/

        if (_weapon.AttachmentManager.IsInited)
        {
            //Debug.Log($"{_weapon.GetName()} Set scope: {_scopeCurent} muzzle: {_muzzleCurent} laser: {_laserCurent} grip: {_gripCurent}");

            PlayerPrefs.SetInt($"{_weapon.GetName()}_scope", _scopeCurent);
            PlayerPrefs.SetInt($"{_weapon.GetName()}_muzzle", _muzzleCurent);
            PlayerPrefs.SetInt($"{_weapon.GetName()}_laser", _laserCurent);
            PlayerPrefs.SetInt($"{_weapon.GetName()}_grip", _gripCurent);
        }
        else
        {
            while (!_weapon.AttachmentManager.IsInited)
            {
                await UniTask.Yield();
            }

            _scopeCurent = PlayerPrefs.GetInt($"{_weapon.GetName()}_scope", -1);
            _muzzleCurent = PlayerPrefs.GetInt($"{_weapon.GetName()}_muzzle", 0);
            _laserCurent = PlayerPrefs.GetInt($"{_weapon.GetName()}_laser", -1);
            _gripCurent = PlayerPrefs.GetInt($"{_weapon.GetName()}_grip", -1);

            _inventory.SetAttachmentOnStartLater(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent, $"ChangeAttachementScreen Late {_weapon.GetName()}");

        }

        await UniTask.Yield();

        if (_scopesUIElements.Count == 0 && _muzzlesUIElements.Count == 0 &&
            _gripsUIElements.Count == 0 && _lasersUIElements.Count == 0)
        {
            //Debug.Log("UIElements not register yet");
        }
        else
        {
            try
            {
                _scopesUIElements[_scopeCurent].SetActiveState(true);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Log($"ArgumentOutOfRangeException {_weapon.GetName()} scope {_scopeCurent}");
                _scopesUIElements[0].SetActiveState(true);
            }

            try
            {
                _muzzlesUIElements[_muzzleCurent].SetActiveState(true);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Log($"ArgumentOutOfRangeException {_weapon.GetName()} muzzle {_muzzleCurent}");
                _muzzlesUIElements[0].SetActiveState(true);
            }

            try
            {
                _gripsUIElements[_gripCurent].SetActiveState(true);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Log($"ArgumentOutOfRangeException {_weapon.GetName()} grip {_gripCurent}");
                _gripsUIElements[0].SetActiveState(true);
            }

            try
            {
                _lasersUIElements[_laserCurent].SetActiveState(true);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Log($"ArgumentOutOfRangeException {_weapon.GetName()} laser {_laserCurent}");
                _lasersUIElements[0].SetActiveState(true);
            }

        }

        await UniTask.Yield();

        _descriptionScript.UpdateAttachments(_weapon.WeaponDamage, _scopeCurent, _muzzleCurent, _gripCurent, _laserCurent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_attachementsRect);

        await UniTask.Yield();

        //Debug.Log();
    }

    public void SetScope(int scope)
    {
        _scopeCurent = scope;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent, "ChangeAttachementScreen SetScope");
        UpdateElements();
    }

    public void SetMuzzle(int muzzle)
    {
        _muzzleCurent = muzzle;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent, "ChangeAttachementScreen SetMuzzle");
        UpdateElements();
    }

    public void SetLaser(int laser)
    {
        _laserCurent = laser;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent, "ChangeAttachementScreen SetLaser");
        UpdateElements();
    }

    public void SetGrip(int grip)
    {
        _gripCurent = grip;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent, "ChangeAttachementScreen SetGrip");
        UpdateElements();
    }

    /*
    [ContextMenu("Next Scope")]

    public void NextScope()
    {
        _scopeCurent++;
        _scopeCurent = _scopeCurent > _scopesMaxCount ? 0: _scopeCurent;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent);

        UpdateText();
    }

    [ContextMenu("Prev Scope")]

    public void PrevScope()
    {
        _scopeCurent--;
        _scopeCurent = _scopeCurent <= 0 ? _scopesMaxCount : _scopeCurent;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent);

        UpdateText();
    }

    public void NextMuzzle()
    {
        _muzzleCurent++;
        _muzzleCurent = _muzzleCurent > _muzzlesMaxCount ? 0 : _muzzleCurent;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent);

        UpdateText();
    }

    public void PrevMuzzle()
    {
        _muzzleCurent--;
        _muzzleCurent = _muzzleCurent <= 0 ? _muzzlesMaxCount : _muzzleCurent;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent);

        UpdateText();
    }

    public void NextLaser()
    {
        _laserCurent++;
        _laserCurent = _laserCurent > _lasersMaxCount ? 0 : _laserCurent;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent);

        UpdateText();
    }

    public void PrevLaser()
    {
        _laserCurent--;
        _laserCurent = _laserCurent <= 0 ? _lasersMaxCount : _laserCurent;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent);

        UpdateText();
    }

    public void NextGrip()
    {
        _gripCurent++;
        _gripCurent = _gripCurent > _gripsMaxCount ? 0 : _gripCurent;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent);

        UpdateText();
    }

    public void PrevGrip()
    {
        _gripCurent--;
        _gripCurent = _gripCurent <= 0 ? _gripsMaxCount : _gripCurent;
        _inventory.SetAttachment(_scopeCurent, _muzzleCurent, _laserCurent, _gripCurent);

        UpdateText();
    }
    */
}
