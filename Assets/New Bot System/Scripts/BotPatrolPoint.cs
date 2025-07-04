using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotPatrolPoint : MonoBehaviour
{
    private TargetPoint _targetPoint;
    private string _name = "P_LPSP_WEP_Bot_Gun";
    private bool _isBotGunInArms = false;
    private bool _isPlayerInsideSection = false;

    void Start()
    {
        _targetPoint = GetComponentInChildren<TargetPoint>();
        WaitingPlayer();
    }

    private async void WaitingPlayer()
    {
        while (Inventory.Instance == null)
        {
            await UniTask.Yield(); // ќжидание следующего кадра
        }

        Inventory.Instance.OnWeaponSelected.AddListener(WeaponSelected);
    }

    public void WeaponSelected(GameObject weapon)
    {
        _isBotGunInArms = weapon.name.Equals(_name);
        _targetPoint.SetActive(_isPlayerInsideSection && _isBotGunInArms);
    }

    void Update()
    {
        
    }

    public void ActivatePoint()
    {
        _isPlayerInsideSection = true;
        _targetPoint.SetActive(_isPlayerInsideSection && _isBotGunInArms);
    }

    public void DisablePoint()
    {
        _isPlayerInsideSection = false;
        _targetPoint.SetActive(false);
    }
}
