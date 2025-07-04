using game.configuration;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWeaponStarter : MonoBehaviour
{
    public static GameWeaponStarter Instance;

    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private GameObject _internalInventory;

    private List<Weapon> _weapons = new List<Weapon>();
    private List<WeaponAttachmentManager> _weaponsHuepans = new List<WeaponAttachmentManager>();
    private List<bool> _weaponAvalebles = new List<bool>();

    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        for(int i =0; i< _internalInventory.transform.childCount; i++)
        {
            if (_internalInventory.transform.GetChild(i).name.Equals("P_LPSP_WEP_Bot_Gun"))// && GameConfig.CurrentConfiguration.GameMode != GameConfig.GameModes.Sandbox)
            {
                //Destroy(_internalInventory.transform.GetChild(i).gameObject);
            }
            else
            {
                _weapons.Add(_internalInventory.transform.GetChild(i).GetComponent<Weapon>());
                _weaponsHuepans.Add(_internalInventory.transform.GetChild(i).GetComponent<WeaponAttachmentManager>());
            }
        }

        for(int i=0; i< _weapons.Count; i++)
        {
            _weaponAvalebles.Add(PlayerPrefs.GetInt($"{_weapons[i].GetName()}_avaleble", 0)==1);

            int scope = PlayerPrefs.GetInt($"{_weapons[i].GetName()}_scope", -1);
            int muzzle = PlayerPrefs.GetInt($"{_weapons[i].GetName()}_muzzle", 0);
            int laser = PlayerPrefs.GetInt($"{_weapons[i].GetName()}_laser", -1);
            int grip = PlayerPrefs.GetInt($"{_weapons[i].GetName()}_grip", -1); 
            Debug.Log($"{_weapons[i].GetName()} scope: {scope} muzzle: {muzzle} laser: {laser} grip: {grip}");
            _weaponsHuepans[i].InitWeapon(scope, muzzle, laser, grip);
        }

        //StartCoroutine(LateStart());
        SetAllAvaleble();
        //_player.SetActive(true);
    }

    [ContextMenu("Set All Avaleble")]
    public void SetAllAvaleble()
    {
        for (int i = 0; i < _weaponAvalebles.Count; i++)
        {
            if (!_weaponAvalebles[i])
            {
                _weaponAvalebles[i] = true;
                _weapons[i].gameObject.transform.SetParent(_internalInventory.transform);
            }
        }
    }

    public string RandomAvalebleName()
    {
        List<string> avaleble = new List<string>();


        foreach(var weapon in _weapons)
        {
            if (weapon != null)
            {
                string wname = weapon.GetName();

                if (wname.Contains("Handgun"))
                {
                    avaleble.Add("pistol");
                }

                if (wname.Contains("SMG"))
                {
                    avaleble.Add("smg");
                }

                if (wname.Contains("Shotgun"))
                {
                    avaleble.Add("shotgun");
                }

                if (wname.Contains("Assault Rifle"))
                {
                    avaleble.Add("rifle");
                }

                if (wname.Contains("Sniper"))
                {
                    avaleble.Add("sniperRifle");
                }

                if (wname.Contains("Grenade Launcher"))
                {
                    avaleble.Add("grenadeLaunched");
                }

                if (wname.Contains("Rocket Launcher"))
                {
                    avaleble.Add("rocket");
                }
            }
        }

        avaleble.Add("grenade");
        avaleble.Add("life");

        return avaleble[Random.Range(0, avaleble.Count)];
    }
    /*
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);

        _player.GetComponent<CharacterBehaviour>().Init();

        //yield return new WaitForSeconds(WinLose.Instance.LoadingTime - 1.1f);
        //_player.SetActive(true);
    }*/

    void Update()
    {
    }
}
