using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuKitsUI : MonoBehaviour
{
    public static MenuKitsUI Instance;

    //[field: SerializeField]
    private MenuWeapon _curentWeapon;

    [SerializeField]
    private GameObject _canBuyButton;
    [SerializeField]
    private GameObject _cantBuyButton;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SetNewWeapon(MenuWeapon weapon)
    {
        _curentWeapon = weapon;

        _canBuyButton.GetComponent<Texter>().UpdateText(_curentWeapon.Cost.ToString());
        _cantBuyButton.GetComponent<Texter>().UpdateText(_curentWeapon.Cost.ToString());

        if (_curentWeapon.Avaleble)
        {

            _canBuyButton.SetActive(false);
            _cantBuyButton.SetActive(false);
        }
        else
        {
            if (CoinsManager.Instance.Coins >= _curentWeapon.Cost)
            {
                _canBuyButton.SetActive(true);
                _cantBuyButton.SetActive(false);
            }
            else
            {
                _canBuyButton.SetActive(false);
                _cantBuyButton.SetActive(true);
            }
        }
    }

    public void SetNewWeapon()
    {
        _canBuyButton.GetComponent<Texter>().UpdateText(_curentWeapon.Cost.ToString());
        _cantBuyButton.GetComponent<Texter>().UpdateText(_curentWeapon.Cost.ToString());

        if (_curentWeapon.Avaleble)
        {

            _canBuyButton.SetActive(false);
            _cantBuyButton.SetActive(false);
        }
        else
        {
            if (CoinsManager.Instance.Coins >= _curentWeapon.Cost)
            {
                _canBuyButton.SetActive(true);
                _cantBuyButton.SetActive(false);
            }
            else
            {
                _canBuyButton.SetActive(false);
                _cantBuyButton.SetActive(true);
            }
        }
    }

    public void BuyWeapon()
    {
        if (CoinsManager.Instance.TryBuy(_curentWeapon.Cost))
        {
            _curentWeapon.Avaleble = true;
            _canBuyButton.SetActive(false);
            _cantBuyButton.SetActive(false);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Next(int type)
    {
        _curentWeapon.Next(type);
    }

    public void Prev(int type)
    {
        _curentWeapon.Prev(type);
    }
}
