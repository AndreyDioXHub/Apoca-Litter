//Copyright 2022, Infima Games. All Rights Reserved.

using Cysharp.Threading.Tasks;
using kcp2k;
using System;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.Events;

namespace InfimaGames.LowPolyShooterPack
{
    public class Inventory : MonoBehaviour
    {
        //public UnityEvent<int, int> OnWeaponChanged = new UnityEvent<int, int>(); //old new
        public UnityEvent OnAmmoVestIsEmpty = new UnityEvent();
        public UnityEvent<GameObject> OnWeaponSelected = new UnityEvent<GameObject>();
        public UnityEvent<AmmoType, int> OnAmmoAdded = new UnityEvent<AmmoType, int>();
        public UnityEvent<WeaponAttachmentManager> OnWeaponAttachmentManageкSelected = new UnityEvent<WeaponAttachmentManager>();

        public static Inventory Instance;

        public string BuyedWeapon1 => _buyedWeapon1;
        public string BuyedWeapon2 => _buyedWeapon2;

        [SerializeField]
        private PlayerNetworkResolver _resolver;

        [SerializeField]
        private string _buyedWeapon1;
        [SerializeField]
        private string _buyedWeapon2;

        /*
        [SerializeField]
        private int _scope, _muzzle, _laser, _grip;*/

        [SerializeField]
        private Weapon[] weapons;
        /*[SerializeField]
        private GameObject _testWeapon;*/
        /*[SerializeField]
        private List<GameObject> _weapons = new List<GameObject>();*/

        private Dictionary<AmmoType, int> _ammosCount = new Dictionary<AmmoType, int>()
        {
            {AmmoType.pistol, 50},
            {AmmoType.grenade, 5},
            {AmmoType.grenadeLaunched, 5},
            {AmmoType.rocket, 3},
            {AmmoType.rifle, 90},
            {AmmoType.smg, 150},
            {AmmoType.shotgun, 32},
            {AmmoType.sniperRifle, 40},
            {AmmoType.pgun, 40},
            {AmmoType.jackhammer, 600}
        };

        private Dictionary<AmmoType, int> _ammosCountMax = new Dictionary<AmmoType, int>()
        {
            {AmmoType.pistol, 50},
            {AmmoType.grenade, 5},
            {AmmoType.grenadeLaunched, 5},
            {AmmoType.rocket, 3},
            {AmmoType.rifle, 90},
            {AmmoType.smg, 150},
            {AmmoType.shotgun, 32},
            {AmmoType.sniperRifle, 40},
            {AmmoType.pgun, 40},
            {AmmoType.jackhammer, 600}
        };

        private Dictionary<AmmoType, int> _ammoProbabilities = new Dictionary<AmmoType, int>()
        {
            {AmmoType.pistol, 20},
            {AmmoType.grenade, 5000},
            {AmmoType.grenadeLaunched, 500},
            {AmmoType.rocket, 500},
            {AmmoType.rifle, 10},
            {AmmoType.smg, 20},
            {AmmoType.shotgun, 20},
            {AmmoType.sniperRifle, 30}
        };

        [SerializeField]
        private Weapon equipped;
        private int equippedIndex = -1;

        private bool _buyWeapon1 = true;

        /*
        private void Start()
        {
            CharacterValues.UpdateValue(CharacterValueKey.AmmosCountTotal, _ammosCount[type].ToString());
        }*/

        private void Awake()
        {
            Instance = this;
        }

        public void Init(Character character)
        {
            _buyedWeapon1 = PlayerPrefs.GetString(PlayerPrefsConsts.buyedWeapon1, "SMG 01");
            _buyedWeapon2 = PlayerPrefs.GetString(PlayerPrefsConsts.buyedWeapon2, "Handgun 01");

            //Cache all weapons. Beware that weapons need to be parented to the object this component is on!
            weapons = GetComponentsInChildren<Weapon>(true);

            //Disable all weapons. This makes it easier for us to only activate the one we need.
            foreach (Weapon weapon in weapons)
            {
                WeaponAttachmentManager attachmentManager = weapon.transform.GetComponent<WeaponAttachmentManager>();
                attachmentManager.ResetAttachment();
                weapon.Init(character, this, attachmentManager, _resolver);
                weapon.gameObject.SetActive(false);
            }

            equipped = weapons[0];
            //equippedIndex = PlayerPrefs.GetInt(PlayerPrefsConsts.equippedIndex, 0);
            Equip(0);
        }

        public int GetBuyedWeapon1Index()
        {
            int index = 0;
            List<Weapon> weaponsList = new List<Weapon>();

            foreach (var weapon in weapons)
            {
                weaponsList.Add(weapon);
            }

            index = weaponsList.FindIndex(w => w.GetName().Equals(_buyedWeapon1));
            index = index < 0 ? 0 : index;
            return index;
        }

        public void SetNewBuyedWeapon(string weaponName)
        {
            if (_buyWeapon1)
            {
                _buyedWeapon1 = weaponName;
                PlayerPrefs.SetString(PlayerPrefsConsts.buyedWeapon1, _buyedWeapon1);
                _buyWeapon1 = false;
            }
            else
            {
                _buyedWeapon2 = weaponName;
                PlayerPrefs.SetString(PlayerPrefsConsts.buyedWeapon2, _buyedWeapon2);
                _buyWeapon1 = true;
            }
        }

        public void Equip(int index)
        {
            equipped.gameObject.SetActive(false);

            EquipGunPart(index);
        }

        public void EquipSpawnGun()
        {
            int index = 0;

            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i].name.Equals("P_LPSP_WEP_Bot_Gun"))
                {
                    index = i;
                }
            }

            EquipGunPart(index);
        }

        private void EquipGunPart(int index)
        {
            equippedIndex = index;
            //Update equipped.
            equipped = weapons[equippedIndex];
            //Activate the newly-equipped weapon.
            equipped.gameObject.SetActive(true);

            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountTotal, _ammosCount[equipped.GetAmmoType()].ToString());
            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountCurent, new ValueCurentMax(equipped.GetAmmunitionCurrent(), equipped.GetAmmunitionTotal()));


            int scope = 0;
            int muzzle = 0;
            int laser = 0;
            int grip = 0;

            if (equipped.AttachmentManager.IsInited)
            {
                scope = equipped.AttachmentManager.ScopeIndex;
                muzzle = equipped.AttachmentManager.MuzzleIndex;
                laser = equipped.AttachmentManager.LaserIndex;
                grip = equipped.AttachmentManager.GripIndex;
            }
            else
            {

                scope = PlayerPrefs.GetInt($"{equipped.GetName()}_scope", -1);
                muzzle = PlayerPrefs.GetInt($"{equipped.GetName()}_muzzle", 0);
                laser = PlayerPrefs.GetInt($"{equipped.GetName()}_laser", -1);
                grip = PlayerPrefs.GetInt($"{equipped.GetName()}_grip", -1);

                equipped.AttachmentManager.IsInited = true;

                //Debug.Log($"{equipped.AttachmentManager.IsInited} {equipped.GetName()} Set scope: {scope} muzzle: {muzzle} laser: {laser} grip: {grip}");
                //SetAttachmentOnStartLater(scope, muzzle, laser, grip, $"inventory Late {equipped.GetName()}");
            }

            SetAttachment(scope, muzzle, laser, grip, $"inventory {equipped.GetName()}");

            OnWeaponSelected?.Invoke(equipped.gameObject);
        }

        public async void SetAttachmentOnStartLater(int scope, int muzzle, int laser, int grip, string sender)
        {
            await UniTask.WaitForSeconds(1);

            SetAttachment(scope, muzzle, laser, grip, sender);
        }

        public void RemoveWeapon(Weapon weapon)
        {
            List<Weapon> weaponsList = new List<Weapon>();

            foreach (var w in weapons)
            {
                if (w != weapon)
                {
                    weaponsList.Add(w);
                }
            }

            weapons = weaponsList.ToArray();
            Destroy(weapon.gameObject);
        }

        [ContextMenu("ClearAmmo")]
        public void ClearAmmo()
        {
            _ammosCount = new Dictionary<AmmoType, int>()
            {
                {AmmoType.pistol, 0},
                {AmmoType.grenade, 0},
                {AmmoType.grenadeLaunched, 0},
                {AmmoType.rocket, 0},
                {AmmoType.rifle, 0},
                {AmmoType.smg, 0},
                {AmmoType.shotgun, 0},
                {AmmoType.sniperRifle, 0},
                {AmmoType.pgun, 0},
                {AmmoType.jackhammer, 0}

            };

            equipped.SetAmmunitionCurrent(1);

            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountCurent, new ValueCurentMax(1, 0));
            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountTotal, 0);

        }

        public void BuyAmmoForBuyedWeapon()//string buyedWeapon1, string buyedWeapon2)
        {

            List<Weapon> weaponsList = new List<Weapon>();

            foreach (var weapon in weapons)
            {
                weaponsList.Add(weapon);
            }

            int index1 = weaponsList.FindIndex(w => w.GetName().Equals(_buyedWeapon1));
            index1 = index1 < 0 ? 0 : index1;

            int index2 = weaponsList.FindIndex(w => w.GetName().Equals(_buyedWeapon2));
            index2 = index2 < 0 ? 0 : index2;

            AddAmmo(weaponsList[index1].GetAmmoType(), _ammosCountMax[weaponsList[index1].GetAmmoType()]);
            AddAmmo(weaponsList[index2].GetAmmoType(), _ammosCountMax[weaponsList[index2].GetAmmoType()]);

        }


        [ContextMenu("AddWeapon")]
        public void AddWeapon()
        {
            /*var t = Instantiate(_testWeapon);
            t.transform.SetParent(transform);
            t.transform.localPosition = Vector3.zero;
            t.transform.localRotation = Quaternion.Euler(0,0,0);
            t.SetActive(false);

            WeaponAttachmentManagerBehaviour wa = t.GetComponent<WeaponAttachmentManagerBehaviour>();
            wa.InitWeapon(1,1,-1,-1);
            AddWeapon(t.GetComponent<WeaponBehaviour>());*/

            //AddWeapon("P_LPSP_WEP_Sniper_03");
            //AddWeapon("Sniper_03");
        }

        /*
        [ContextMenu("Set Attachment")]
        public void SetAttachment()
        {
            equipped.GetComponent<WeaponAttachmentManager>().InitWeapon(_scope, _muzzle, _laser, _grip);
            equipped.NextFrameReEquipWeaponKits();
            _resolver.OnSetAttachmentOnServer(_scope, _muzzle, _laser, _grip);
        }*/

        public void SetAttachment(int scope, int muzzle, int laser, int grip, string sender)
        {
            equipped.GetComponent<WeaponAttachmentManager>().InitWeapon(scope, muzzle, laser, grip);
            equipped.NextFrameReEquipWeaponKits(false);
            _resolver.OnSetAttachmentOnServer(scope, muzzle, laser, grip, sender);
        }

        /*

        public GameObject GetWeapon(string weaponName, int scope, int muzzle, int laser, int grip)
        {
            GameObject weaponGO = _weapons.Find(w => w.name.Equals(weaponName));

            var t = Instantiate(weaponGO);
            t.transform.SetParent(transform);
            t.transform.localPosition = Vector3.zero;
            t.transform.localRotation = Quaternion.Euler(0, 0, 0);
            t.SetActive(false);

            WeaponAttachmentManager wa = t.GetComponent<WeaponAttachmentManager>();
            wa.InitWeapon(scope, muzzle, laser, grip);

            return t;
        }

        public void AddWeapon(string weaponName, int scope = 1, int muzzle = 1, int laser = -1, int grip = -1)
        {
            bool weaponExist = false;

            foreach (GameObject weaponGO in _weapons)
            {
                if (weaponGO.name.Equals(weaponName))
                {
                    weaponExist = true;
                }
            }

            if(weaponExist)
            {
                GameObject weaponGO = _weapons.Find(w=>w.name.Equals(weaponName));

                var t = Instantiate(weaponGO);
                t.transform.SetParent(transform);
                t.transform.localPosition = Vector3.zero;
                t.transform.localRotation = Quaternion.Euler(0, 0, 0);
                t.SetActive(false);

                WeaponAttachmentManager wa = t.GetComponent<WeaponAttachmentManager>();
                wa.InitWeapon(scope, muzzle, laser, grip);
                AddWeapon(t.GetComponent<Weapon>());
            }
        }*/

        /*
        public void RecoverWeapon(string weaponName)
        {
            List<Weapon> weaponsList = new List<Weapon>();

            foreach (var wea in weapons)
            {
                weaponsList.Add(wea);
            }

            Weapon weapon = weaponsList.Find(w => ((Weapon)w).WeaponName.Equals(weaponName));

            weapon.Recover(weaponName);

            if (((Weapon)equipped).WeaponName.Equals(weaponName))
            {
                CharacterValues.UpdateValue(CharacterValueKey.AmmoCountCurent, new ValueCurentMax(equipped.GetAmmunitionCurrent(), equipped.GetAmmunitionTotal()));
            }

            FillMaxAmmo(weapon.GetAmmoType());
        }
        */

        /*
        [ContextMenu("ReEquipWeaponKits")]
        public void ReEquipWeaponKits()
        {
            WeaponAttachmentManager wa = equipped.gameObject.GetComponent<WeaponAttachmentManager>();
            wa.InitWeapon(1, 1, -1, -1);
            equipped.NextFrameReEquipWeaponKits();
        }*/

        public void AddWeapon(Weapon weapon)
        {
            List<Weapon> weaponsList = new List<Weapon>();

            foreach (var w in weapons)
            {
                weaponsList.Add(w);
            }

            weaponsList.Add(weapon);

            weapons = weaponsList.ToArray();
        }

        public int GetNextIndex()
        {
            //Get next index with wrap around.
            int newIndex = 0;// equippedIndex + 1;
            /*
            if (newIndex > weapons.Length - 1)
            {
                newIndex = 0;
            }

            if (weapons[newIndex].name.Equals("P_LPSP_WEP_Bot_Gun"))
            {
                newIndex++;

                if (newIndex > weapons.Length - 1)
                {
                    newIndex = 0;
                }
            }*/

            List<Weapon> weaponsList = new List<Weapon>();

            foreach (var weapon in weapons)
            {
                weaponsList.Add(weapon);
            }

            if (weapons[equippedIndex].GetName().Equals(_buyedWeapon1))
            {
                newIndex = weaponsList.FindIndex(w => w.GetName().Equals(_buyedWeapon2));
            }

            if (weapons[equippedIndex].GetName().Equals(_buyedWeapon2))
            {
                newIndex = weaponsList.FindIndex(w => w.GetName().Equals(_buyedWeapon1));
            }

            //Return.
            return newIndex;
        }

        public int GetLastIndex()
        {
            //Get last index with wrap around.
            int newIndex = 0;// equippedIndex - 1;
            /*
            if (newIndex < 0)
            {
                newIndex = weapons.Length - 1;
            }
            */


            List<Weapon> weaponsList = new List<Weapon>();

            foreach (var weapon in weapons)
            {
                weaponsList.Add(weapon);
            }

            if (weapons[equippedIndex].GetName().Equals(_buyedWeapon1))
            {
                newIndex = weaponsList.FindIndex(w => w.GetName().Equals(_buyedWeapon2));
            }

            if (weapons[equippedIndex].GetName().Equals(_buyedWeapon2))
            {
                newIndex = weaponsList.FindIndex(w => w.GetName().Equals(_buyedWeapon1));
            }

            //Return.
            return newIndex;
        }


        public Weapon GetEquipped() => equipped;
        public int GetEquippedIndex() => equippedIndex;

        public int GetAmmoCount(AmmoType type) => _ammosCount[type];
        public void RemoveAmmo(AmmoType type, int count)
        {
            _ammosCount[type] -= count;
            _ammosCount[type] = _ammosCount[type] < 0 ? 0 : _ammosCount[type];
            //Debug.Log($"RemoveAmmo {type} {_ammosCount[type]}");

            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountTotal, _ammosCount[type].ToString());
        }

        public bool AmmoVestIsEmpty()
        {
            bool result = false;

            List<Weapon> weaponsList = new List<Weapon>();

            foreach (var weapon in weapons)
            {
                weaponsList.Add(weapon);
            }

            Weapon weapon1 = weaponsList.Find(w => w.GetName().Equals(_buyedWeapon1));
            Weapon weapon2 = weaponsList.Find(w => w.GetName().Equals(_buyedWeapon2));

            result = _ammosCount[weapon1.GetAmmoType()] + _ammosCount[weapon2.GetAmmoType()] == 0;

            if (result)
            {
                OnAmmoVestIsEmpty?.Invoke();
            }

            return result;
        }

        /*
        public void ClearAmmo()
        {
            foreach (var weapon in _ammosCount)
            {

            }
            
        }*/


        public void AddAmmo(AmmoType type, int count)
        {
            _ammosCount[type] += count;
            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountTotal, _ammosCount[equipped.GetAmmoType()].ToString());
            //Debug.Log($"AddAmmo {type} {_ammosCount[type]}");
        }

        /*
        public void FillMaxAmmo(AmmoType type)
        {
            int ammoCountMax = _ammosCountMax[type];
            ammoCountMax = _ammosCount[type] > ammoCountMax ? _ammosCount[type] : ammoCountMax;
            _ammosCount[type] = ammoCountMax;

            CharacterValues.UpdateValue(CharacterValueKey.AmmoCountTotal, _ammosCount[equipped.GetAmmoType()].ToString());
            //Debug.Log($"AddAmmo {type} {_ammosCount[type]}");
        }*/

        [ContextMenu("AddRandomBullet")]
        internal void AddRandomBullet()
        {
            AmmoType selectedType = GetRandomAmmoType();

            if (selectedType == AmmoType.jackhammer)
            {
                selectedType = AmmoType.pistol;
            }

            int addedCount = UnityEngine.Random.Range(1, 5);

            if (selectedType == AmmoType.grenade || selectedType == AmmoType.grenadeLaunched
                || selectedType == AmmoType.rocket)
            {
                addedCount = 1;
            }

            if (_ammosCount[selectedType] > 500)
            {
                selectedType = AmmoType.rifle;
                addedCount = 1;
            }

            _ammosCount[selectedType] += addedCount;
            //Debug.Log($"Добавлен патрон: {selectedType}. Новое количество: {_ammosCount[selectedType]}");
            OnAmmoAdded?.Invoke(selectedType, addedCount);
        }

        private AmmoType GetRandomAmmoType()
        {
            List<AmmoType> possibleTypes = new List<AmmoType>();

            // Генерация списка возможных типов
            foreach (var kvp in _ammoProbabilities)
            {
                if (UnityEngine.Random.Range(1, kvp.Value + 1) == 1)
                {
                    possibleTypes.Add(kvp.Key);
                }
            }

            // Если ничего не выбрано, возвращаем пистолет
            if (possibleTypes.Count == 0)
            {
                return AmmoType.pistol;
            }

            // Выбор случайного типа из возможных
            return possibleTypes[UnityEngine.Random.Range(0, possibleTypes.Count)];
        }
    }

    public enum AmmoType
    {
        pistol,
        grenade,
        grenadeLaunched,
        rocket,
        rifle,
        smg,
        shotgun,
        sniperRifle,
        pgun,
        jackhammer
    }
}