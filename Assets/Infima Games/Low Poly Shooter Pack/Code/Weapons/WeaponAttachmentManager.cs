//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Weapon Attachment Manager. Handles equipping and storing a Weapon's Attachments.
    /// </summary>
    public class WeaponAttachmentManager : MonoBehaviour
    {
        public bool IsInited;

        public int ScopesMaxCount => scopeArray.Length;
        public int MuzzlesMaxCount => muzzleArray.Length;
        public int LasersMaxCount => laserArray.Length;
        public int GripsMaxCount => gripArray.Length;

        public int ScopeIndex => scopeIndex;
        public int MuzzleIndex => muzzleIndex;
        public int LaserIndex => laserIndex;
        public int GripIndex => gripIndex;
        public int MagazineIndex => magazineIndex;

        #region FIELDS SERIALIZED
        [SerializeField]
        private Scope scopeDefaultBehaviour;

        [SerializeField]
        private Scope[] scopeArray;
        [SerializeField]
        private Muzzle[] muzzleArray;
        [SerializeField]
        private Laser[] laserArray;
        [SerializeField]
        private Grip[] gripArray;
        [SerializeField]
        private Magazine[] magazineArray;
        #endregion

        #region FIELDS
        private int scopeIndex = 0;
        private int muzzleIndex = 0;
        private int laserIndex = 0;
        private int gripIndex = 0;
        private int magazineIndex = 0;

        private Scope scopeBehaviour;
        private Muzzle muzzleBehaviour;
        private Laser laserBehaviour;
        private Grip gripBehaviour;
        private Magazine magazineBehaviour;
        //private Weapon _weapon;
        #endregion

        #region UNITY FUNCTIONS

        protected void Awake()
        {
        }

        private void Start()
        {
            //ResetAttachment();
        }

        [ContextMenu("ResetAttachment")]
        public void ResetAttachment()
        {
            scopeBehaviour = scopeArray.SelectAndSetActive(scopeIndex);

            if (scopeBehaviour == null)
            {
                scopeBehaviour = scopeArray[0]; 
                scopeBehaviour.gameObject.SetActive(true);
            }
            else
            {
                ////Debug.Log("scopeBehaviour == null");
            }

            muzzleBehaviour = muzzleArray.SelectAndSetActive(muzzleIndex);
            laserBehaviour = laserArray.SelectAndSetActive(laserIndex);
            gripBehaviour = gripArray.SelectAndSetActive(gripIndex);
            magazineBehaviour = magazineArray.SelectAndSetActive(magazineIndex);
        }

        internal void InitWeapon(int scope, int muzzle, int laser, int grip)
        {
            //Debug.Log($"{scope} {scopeArray.Length}");
            scopeIndex = scope;
            muzzleIndex = muzzle;
            laserIndex = laser;
            gripIndex = grip;

            /*
            PlayerPrefs.SetInt(PlayerPrefsConsts.scopeIndex, scopeIndex);
            PlayerPrefs.SetInt(PlayerPrefsConsts.muzzleIndex, muzzleIndex);
            PlayerPrefs.SetInt(PlayerPrefsConsts.laserIndex, laserIndex);
            PlayerPrefs.SetInt(PlayerPrefsConsts.gripIndex, gripIndex);
            */

            if (scopeIndex == -1)
            {
                scopeBehaviour = scopeArray[0];
            }
            else
            {
                scopeBehaviour = scopeArray.SelectAndSetActive(scopeIndex);
                /*
                foreach (var sc in scopeArray)
                {
                    sc.gameObject.SetActive(false);
                }

                scopeBehaviour = scopeArray[scopeIndex];
                scopeBehaviour.gameObject.SetActive(true);*/
                //scopeArray.SelectAndSetActive(scopeIndex);
            }

            muzzleBehaviour = muzzleArray.SelectAndSetActive(muzzleIndex);
            laserBehaviour = laserArray.SelectAndSetActive(laserIndex);
            gripBehaviour = gripArray.SelectAndSetActive(gripIndex);
            magazineBehaviour = magazineArray.SelectAndSetActive(magazineIndex);
        }

        protected void Update()
        {

        }

        #endregion

        #region GETTERS

        public Scope GetEquippedScope() => scopeBehaviour;
        public Scope GetEquippedScopeDefault() => scopeArray[0];

        public Magazine GetEquippedMagazine() => magazineBehaviour;
        public Muzzle GetEquippedMuzzle() => muzzleBehaviour;

        public Laser GetEquippedLaser() => laserBehaviour;
        public Grip GetEquippedGrip() => gripBehaviour;

        #endregion
    }
}