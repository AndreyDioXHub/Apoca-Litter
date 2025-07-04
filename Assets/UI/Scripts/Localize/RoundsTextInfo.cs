using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class RoundsTextInfo : MonoBehaviour
{
    [SerializeField]
    private AmmoType _ammoType;

    public int PistolCount = 0;
    public int GrenadeCount = 0;
    public int GrenadeLaunchedCount = 0;
    public int RocketCount = 0;
    public int RifleCount = 0;
    public int SMGCount = 0;
    public int ShotgunCount = 0;
    public int SniperRifleCount = 0;

    public LocalizeStringEvent localizeString;
    public Animator _animator;

    void Start()
    {
        
    }

    public void AmmoAdded(AmmoType type, int count)
    {
        if (_ammoType == type)
        {
            switch (type)
            {
                case AmmoType.pistol:
                    PistolCount = count;
                    break;
                case AmmoType.grenade:
                    GrenadeCount = count;
                    break;
                case AmmoType.grenadeLaunched:
                    GrenadeLaunchedCount = count;
                    break;
                case AmmoType.rocket:
                    RocketCount = count;
                    break;
                case AmmoType.rifle:
                    RifleCount = count;
                    break;
                case AmmoType.smg:
                    SMGCount = count;
                    break;
                case AmmoType.shotgun:
                    ShotgunCount = count;
                    break;
                case AmmoType.sniperRifle:
                    SniperRifleCount = count;
                    break;
            }

            localizeString.RefreshString();
            _animator.SetTrigger("Trigger");
        }
    }
}

/* {AmmoType.pistol, 0},
            {AmmoType.grenade, 0},
            {AmmoType.grenadeLaunched, 0},
            {AmmoType.rocket, 0},
            {AmmoType.rifle, 0},
            {AmmoType.smg, 0},
            {AmmoType.shotgun, 0},
            {AmmoType.sniperRifle, 0},
            {AmmoType.pgun, 0},
            {AmmoType.jackhammer, 600}*/