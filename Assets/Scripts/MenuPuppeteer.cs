using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPuppeteer : MonoBehaviour
{
    public static MenuPuppeteer Instance;
    [SerializeField]
    private TPVInputController _inputController;
    [SerializeField]
    private TPVInventory _inventory;
    [SerializeField]
    private string _menuSceneName = "MenuNetwork";

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
    }

    void OnEnable()
    {
        //Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        //Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        await UniTask.WaitForSeconds(0.5f);

        //Debug.Log("OnSceneLoaded: " + scene.name);
        //Debug.Log(mode);
        try
        {
            if (scene.name.Equals(_menuSceneName))
            {
                int equippedIndex = PlayerPrefs.GetInt(PlayerPrefsConsts.equippedIndex, 0);
                string weaponName = _inventory.ExternalWeapon[equippedIndex].GetComponent<TPVWeapon>().WeaponName;

                WeaponChanged(0, equippedIndex);

                int scope = PlayerPrefs.GetInt($"{weaponName}_scope", -1);
                int muzzle = PlayerPrefs.GetInt($"{weaponName}_muzzle", 0);
                int laser = PlayerPrefs.GetInt($"{weaponName}_laser", -1);
                int grip = PlayerPrefs.GetInt($"{weaponName}_grip", -1);

                //Debug.Log($"{weaponName} {scope} {muzzle} {laser} {grip}");
                AttachmentChangedLater(equippedIndex, scope, muzzle, laser, grip);
            }
        }
        catch(NullReferenceException ex)
        {

        }
    }

    public async void AttachmentChangedLater(int equippedIndex, int scope, int muzzle, int laser, int grip)
    {
        await UniTask.WaitForSeconds(1);
        try
        {
            _inventory.ExternalWeapon[equippedIndex].GetComponent<TPVWeapon>().AttachmentChanged(scope, muzzle, laser, grip);
        }
        catch(Exception e)
        {
            //Debug.Log($"Exception {e.Message}");
        }
    }

    void Update()
    {

    }

    public void RelaxHoldingButtonFire()
    {
        _inputController.RelaxHoldingButtonFire();
    }

    public void WeaponChanged(int old, int neww)
    {
        _inputController.WeaponChanged( old,  neww);
    }

    public void WeaponSelected(GameObject weaponGO)
    {
        _inputController.WeaponSelected(weaponGO);
    }

    public void WeaponAttachmentManagerSelected(WeaponAttachmentManager am)
    {
        _inputController.WeaponAttachmentManagerSelected(am);
    }

    public void Aim(bool isaim)
    {
        _inputController.Aim(isaim);
    }

    public void MagazineFasten(bool isfasten)
    {
        _inputController.MagazineFasten(isfasten);
    }

    public void AttachmentChanged(int scope, int muzzle, int laser, int grip)
    {
        _inputController.AttachmentChanged(scope, muzzle, laser, grip);
    }
}
