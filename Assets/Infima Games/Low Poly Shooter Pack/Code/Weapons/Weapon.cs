//Copyright 2022, Infima Games. All Rights Reserved.

using InfimaGames.LowPolyShooterPack.Legacy;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using UnityEngine.TextCore.Text;

namespace InfimaGames.LowPolyShooterPack
{
    public class Weapon : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        public string WeaponName => weaponName;
        public int WeaponDamage => (int)_weaponDamage;

        public WeaponAttachmentManager AttachmentManager => _attachmentManager;

        [SerializeField]
        private string weaponName;
        [SerializeField]
        private bool _isAvailable;

        [SerializeField]
        private AmmoType _ammoType;

        [SerializeField]
        private float multiplierMovementSpeed = 1.0f;

        [SerializeField]
        private float _weaponDamage = 1.0f;
        
        [SerializeField]
        private bool automatic;
        
        [SerializeField]
        private bool boltAction;
        
        [SerializeField]
        private int _shotCount = 1;

        [SerializeField]
        private float spread = 0.25f;

        [SerializeField]
        private float projectileImpulse = 400.0f;
        
        [SerializeField]
        private int roundsPerMinutes = 200;
        
        [SerializeField]
        private bool cycledReload;

        //[SerializeField]
        private bool _isNeedFeelOnStart = true;

        [SerializeField]
        private bool canReloadWhenFull = true;

        [SerializeField]
        private bool automaticReloadOnEmpty;

        [SerializeField]
        private float automaticReloadOnEmptyDelay = 0.25f;
        
        [SerializeField]
        private Transform socketEjection;

        [SerializeField]
        private Offsets weaponOffsets;

        [SerializeField]
        private float swaySmoothValue = 10.0f;

        [SerializeField]
        private Sway sway;

        [SerializeField]
        private GameObject prefabCasing;

        [SerializeField]
        private GameObject prefabProjectile;

        [SerializeField]
        public RuntimeAnimatorController controller;

        [SerializeField]
        private Sprite spriteBody;

        [SerializeField]
        private AudioClip audioClipHolster;

        [SerializeField]
        private AudioClip audioClipUnholster;

        [SerializeField]
        private AudioClip audioClipReload;

        [SerializeField]
        private AudioClip audioClipReloadEmpty;

        [SerializeField]
        private AudioClip audioClipReloadOpen;

        [SerializeField]
        private AudioClip audioClipReloadInsert;

        [SerializeField]
        private AudioClip audioClipReloadClose;

        [SerializeField]
        private AudioClip audioClipFireEmpty;

        [SerializeField]
        private AudioClip audioClipBoltAction;


        #endregion

        #region FIELDS
        private Animator animator;
        private WeaponAttachmentManager _attachmentManager;
        [SerializeField]
        private int _ammunitionCurrent;

        #region Attachment Behaviours
        private Scope _scopeBehaviour;
        private Magazine _magazineBehaviour;
        private Muzzle _muzzleBehaviour;
        private Laser _laserBehaviour;
        private Grip _gripBehaviour;

        #endregion

        private Character _character;
        private PlayerNetworkResolver _resolver;
        private Inventory _inventory;

        private Transform playerCamera;

        #endregion

        #region UNITY

        protected void Awake()
        {
        }

        protected void Start()
        {

        }

        public void Init(Character character, Inventory inventory, WeaponAttachmentManager attachmentManager, PlayerNetworkResolver resolver)
        {
            //Debug.Log($"Weapon {weaponName} Init");

            _inventory = inventory;
            _character = character;
            _resolver = resolver;

            animator = GetComponent<Animator>();
            _attachmentManager = attachmentManager;
            playerCamera = _character.GetCameraWorld().transform;

            NextFrameReEquipWeaponKits(true);
        }

        public async void NextFrameReEquipWeaponKits(bool isNeedFeelOnStart)
        {
            await UniTask.Yield(); // Ожидание следующего кадра
            ReEquipWeaponKits(isNeedFeelOnStart);
            _inventory.OnWeaponAttachmentManageкSelected?.Invoke(_attachmentManager);
        }

        /*
        public async void NextFrameReEquipWeaponKits(bool mayCalcBullets)
        {
            await UniTask.Yield(); // Ожидание следующего кадра
            _isNeedFeelOnStart = mayCalcBullets;
            ReEquipWeaponKits();
            _inventory.OnWeaponAttachmentManageкSelected?.Invoke(_attachmentManager);
        }*/

        public void ReEquipWeaponKits(bool isNeedFeelOnStart)
        {
            Debug.Log("ReEquip Weapon Kits");

            _scopeBehaviour = _attachmentManager.GetEquippedScope();
            _magazineBehaviour = _attachmentManager.GetEquippedMagazine();
            _muzzleBehaviour = _attachmentManager.GetEquippedMuzzle();
            _laserBehaviour = _attachmentManager.GetEquippedLaser();
            _gripBehaviour = _attachmentManager.GetEquippedGrip();
            
            if(isNeedFeelOnStart)
            {
                _ammunitionCurrent = _magazineBehaviour.GetAmmunitionTotal();
            }
            else
            {
                if (_ammunitionCurrent > 0)
                {

                }
            }

            //_ammunitionCurrent = isNeedFeelOnStart ? _magazineBehaviour.GetAmmunitionTotal() : 0;
            //_ammunitionCurrent = _magazineBehaviour.GetAmmunitionTotal();


            if (weaponName.Equals("Jackhammer"))
            {
                _ammunitionCurrent = _magazineBehaviour.GetAmmunitionTotal();
            }

            if (weaponName.Equals("Rocket Launcher 01"))
            {
                if(_ammunitionCurrent == 0)
                {
                    _magazineBehaviour.gameObject.SetActive(false);
                }
            }

            if(weaponName.Equals("Grenade_Launcher_01"))
            {
                if(_ammunitionCurrent == 0)
                {
                    _magazineBehaviour.gameObject.SetActive(false);
                }
            }

            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountCurent, new ValueCurentMax(_ammunitionCurrent, _magazineBehaviour.GetAmmunitionTotal()));
        }

        #endregion

        #region GETTERS

        public Offsets GetWeaponOffsets() => weaponOffsets;

        public float GetFieldOfViewMultiplierAim()
        {
            if (_scopeBehaviour != null)
            {
                return _scopeBehaviour.GetFieldOfViewMultiplierAim();
            }

            return 1.0f;
        }

        public float GetFieldOfViewMultiplierAimWeapon()
        {
            if (_scopeBehaviour != null)
            {
                return _scopeBehaviour.GetFieldOfViewMultiplierAimWeapon();
            }
            
            return 1.0f;
        }

        public Animator GetAnimator() => animator;

        public Sprite GetSpriteBody() => spriteBody;
        public float GetMultiplierMovementSpeed() => multiplierMovementSpeed;

        public AudioClip GetAudioClipHolster() => audioClipHolster;
        public AudioClip GetAudioClipUnholster() => audioClipUnholster;

        public AudioClip GetAudioClipReload() => audioClipReload;
        public AudioClip GetAudioClipReloadEmpty() => audioClipReloadEmpty;

        public AudioClip GetAudioClipReloadOpen() => audioClipReloadOpen;
        public AudioClip GetAudioClipReloadInsert() => audioClipReloadInsert;
        public AudioClip GetAudioClipReloadClose() => audioClipReloadClose;

        public AudioClip GetAudioClipFireEmpty() => audioClipFireEmpty;
        public AudioClip GetAudioClipBoltAction() => audioClipBoltAction;

        public AudioClip GetAudioClipFire() => _muzzleBehaviour.GetAudioClipFire();

        public int GetAmmunitionCurrent() => _ammunitionCurrent;

        public int GetAmmunitionTotal()
        {
            if (_magazineBehaviour == null)
            {
                return 0;
            }
            else
            {
                return _magazineBehaviour.GetAmmunitionTotal();
            }
        }

        public bool HasCycledReload() => cycledReload;

        public bool IsAutomatic() => automatic;
        public bool IsBoltAction() => boltAction;

        public bool GetAutomaticallyReloadOnEmpty() => automaticReloadOnEmpty;
        public float GetAutomaticallyReloadOnEmptyDelay() => automaticReloadOnEmptyDelay;

        public bool CanReloadWhenFull() => canReloadWhenFull;
        public float GetRateOfFire() => roundsPerMinutes;

        public bool IsFull() => _ammunitionCurrent == _magazineBehaviour.GetAmmunitionTotal();
        public bool HasAmmunition() => _ammunitionCurrent > 0;

        public RuntimeAnimatorController GetAnimatorController() => controller;

        public Sway GetSway() => sway;
        public float GetSwaySmoothValue() => swaySmoothValue;

        #endregion

        #region METHODS

        public void Reload()
        {
            
            if (cycledReload)
            {
                if (HasAmmunition())
                {
                    animator.Play("Reload Open", 0, 0.0f);
                }
            }
            else
            {
                //Play Reload Animation.
                if (HasAmmunition())
                {
                    animator.Play("Reload", 0, 0.0f);
                }
                else
                {
                    animator.Play("Reload Empty", 0, 0.0f);
                }
            }

            //animator.Play(cycledReload ? "Reload Open" : (HasAmmunition() ? "Reload" : "Reload Empty"), 0, 0.0f);

            //Set Reloading Bool. This helps cycled reloads know when they need to stop cycling.
            const string boolName = "Reloading";
            animator.SetBool(boolName, true);

        }

        public void StopReload()
        {
            //Set Reloading Bool. This helps cycled reloads know when they need to stop cycling.
            const string boolName = "Reloading";
            animator.SetBool(boolName, false);
        }
        public void SetAmmunitionCurrent(int count)
        {
            _ammunitionCurrent = count;
            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountCurent, new ValueCurentMax(_ammunitionCurrent, _magazineBehaviour.GetAmmunitionTotal()));
        }

        /*
        public void Reload()
        {
            //Set Reloading Bool. This helps cycled reloads know when they need to stop cycling.
            const string boolName = "Reloading";
            animator.SetBool(boolName, true);

            //Play Reload Animation.
            animator.Play(cycledReload ? "Reload Open" : (HasAmmunition() ? "Reload" : "Reload Empty"), 0, 0.0f);
        }*/

        public void Recover(string weaponName)
        {
            try
            {
                _ammunitionCurrent = _magazineBehaviour.GetAmmunitionTotal();
            }
            catch(NullReferenceException e)
            {
                //Debug.Log($"{weaponName} not initialized");
            }
            ////Debug.Log($"{ammunitionCurrent} {magazineBehaviour.GetAmmunitionTotal()} {GetAmmunitionTotal()}");
        }

        [ContextMenu("RemoveWeapon")]
        public void RemoveWeapon()
        {
            Inventory inv = GetComponentInParent<Inventory>();
            inv.RemoveWeapon(this);
        }
        
        public void Fire(float spreadMultiplier = 1.0f)
        {
            //We need a muzzle in order to fire this weapon!
            if (_muzzleBehaviour == null)
            {
                return;
            }

            //Make sure that we have a camera cached, otherwise we don't really have the ability to perform traces.
            if (playerCamera == null)
            {
                return;
            }

            //Play the firing animation.
            const string stateName = "Fire";
            animator.Play(stateName, 0, 0.0f);

            if (weaponName.Equals("Jackhammer"))
            {

            }
            else
            {
                _ammunitionCurrent = Mathf.Clamp(_ammunitionCurrent - 1, 0, _magazineBehaviour.GetAmmunitionTotal());
            }

            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountCurent, new ValueCurentMax(_ammunitionCurrent, _magazineBehaviour.GetAmmunitionTotal()));
            //Set the slide back if we just ran out of ammunition.
            if (_ammunitionCurrent == 0)
            {
                SetSlideBack(1);
            }

            _muzzleBehaviour.Effect();

            for (var i = 0; i < _shotCount; i++)
            {
                //Debug.Log("new bullet spawned");

                Vector3 spreadValue = UnityEngine.Random.insideUnitSphere * (spread * spreadMultiplier);
                spreadValue.z = 0;
                spreadValue = playerCamera.TransformDirection(spreadValue);

                GameObject projectile = Instantiate(prefabProjectile, playerCamera.position, Quaternion.Euler(playerCamera.eulerAngles + spreadValue));

                if (projectile.TryGetComponent(out Projectile prj))
                {
                    prj.Init(_weaponDamage, projectileImpulse, _character.gameObject, _resolver);
                }
            }
        }
        
        public void FillAmmunition(int amount)
        {
            //Update the value by a certain amount.
            /*//Debug.Log($"FillAmmunition Weapon {amount}");
            //Debug.Log($"FillAmmunition Weapon 1 {ammunitionCurrent}");*/

            _ammunitionCurrent += amount;//amount != 0 ? Mathf.Clamp(ammunitionCurrent + amount, 0, GetAmmunitionTotal()) : magazineBehaviour.GetAmmunitionTotal();
            _ammunitionCurrent = _ammunitionCurrent > GetAmmunitionTotal() ? GetAmmunitionTotal() : _ammunitionCurrent;

            ////Debug.Log($"FillAmmunition Weapon 2 {ammunitionCurrent}");

            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountCurent, new ValueCurentMax(_ammunitionCurrent, _magazineBehaviour.GetAmmunitionTotal()));
        }
        
        public void SetSlideBack(int back)
        {
            //Set the slide back bool.
            const string boolName = "Slide Back";
            animator.SetBool(boolName, back != 0);
        }
        
        public void EjectCasing()
        {
            //Spawn casing prefab at spawn point.
            if (prefabCasing != null && socketEjection != null)
                Instantiate(prefabCasing, socketEjection.position, socketEjection.rotation);
        }

        public AmmoType GetAmmoType() => _ammoType;

        public string GetName() => weaponName;

        #endregion
    }

    [Serializable]
    public class ValueCurentMax
    {
        public float current;
        public float total;

        public ValueCurentMax(float current, float total)
        {
            this.current = current;
            this.total = total;
        }
    }
}

