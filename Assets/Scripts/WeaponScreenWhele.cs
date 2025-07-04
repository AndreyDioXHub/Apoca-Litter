using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScreenWhele : MonoBehaviour
{
    [SerializeField]
    private List<WeaponSelectButton> _buttons = new List<WeaponSelectButton>();

    private PlayerNetworkResolver _resolver;
    private Character _character;
    private Inventory _inventory;

    void Start()
    {
    }

    public void Init(PlayerNetworkResolver resolver, Character character, Inventory inventory)
    {
        _resolver = resolver;
        _character = character;
        _inventory = inventory;

        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].Init(i, this, _resolver, _character, _inventory);
        }

        SetBuyedButtons();
    }

    public async void SetBuyedButtons()
    {
        await UniTask.WaitForSeconds(0.3f);

        foreach (WeaponSelectButton button in _buttons)
        {
            button.SetBuyed(false);
        }

        string buyedWeapon1 = _inventory.BuyedWeapon1;
        string buyedWeapon2 = _inventory.BuyedWeapon2;

        WeaponSelectButton buyedWeapon1Button = _buttons.Find(b=> b.WeaponName.Equals(buyedWeapon1));
        WeaponSelectButton buyedWeapon2Button = _buttons.Find(b=> b.WeaponName.Equals(buyedWeapon2));

        buyedWeapon1Button.SetBuyed(true);
        buyedWeapon2Button.SetBuyed(true);
    }

    public int GetBuyedWeaponIndex(string buyedWeapon)
    {
        int index = _buttons.FindIndex(b => b.WeaponName.Equals(buyedWeapon)); 
        return index;
    } 

    public void HideAll()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].SetActiveState(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
