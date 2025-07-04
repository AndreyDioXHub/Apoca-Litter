using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSounds : MonoBehaviour
{
    public static LocalSounds Instance;

    [SerializeField]
    private AudioSource _chestOpen;
    [SerializeField]
    private AudioSource _chestClose;
    [SerializeField]
    private AudioSource _moneySpend;
    [SerializeField]
    private AudioSource _moneyGet;
    [SerializeField]
    private AudioSource _casing;
    [SerializeField]
    private AudioSource _water;
    [SerializeField]
    private AudioSource _die;
    [SerializeField]
    private AudioSource _weaponSelect;
    [SerializeField]
    private AudioSource _weaponBuy;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlaySound(string sound)
    {
        switch (sound)
        {
            case "chestopen":
                _chestOpen.Play();
                break;
            case "chestclose":
                _chestClose.Play();
                break;
            case "moneyspend":
                _moneySpend.Play();
                break;
            case "moneyget":
                _moneyGet.Play();
                break;
            case "casing":
                _casing.Play();
                break;
            case "water":
                _water.Play();
                break;
            case "die":
                _die.Play();
                break;
            case "weaponselect":
                _weaponSelect.Play();
                break;
            case "weaponbuy":
                _weaponBuy.Play();
                break;
        }
    }
}
