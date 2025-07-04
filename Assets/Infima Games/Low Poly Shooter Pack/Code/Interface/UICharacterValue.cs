//Copyright 2022, Infima Games. All Rights Reserved.
using UnityEngine;
using System.Globalization;
using TMPro;

namespace InfimaGames.LowPolyShooterPack.Interface
{
    public class UICharacterValue : MonoBehaviour 
    {
        [SerializeField]
        protected CharacterValueKey _key;

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {

        }

        public virtual void UpdateValue(CharacterValueKey key, object incomeValue)
        {
            //Debug.Log($"TextAmmunitionTotal {incomeValue}");
            
        }

        public virtual void OnDestroy()
        {

            CharacterValues.OnLanguageChanged -= UpdateValue;
        }
    }
}
    /*
    /// <summary>
    /// Total Ammunition Text.
    /// </summary>
    public class TextAmmunitionTotal : ElementText
    {
        #region METHODS

        /// <summary>
        /// Tick.
        /// </summary>
        protected override void Tick()
        {
            //Total Ammunition.
            float ammunitionTotal = inventoryBehaviour.GetAmmoCount(equippedWeaponBehaviour.GetAmmoType());// equippedWeaponBehaviour.GetAmmunitionTotal();

            //Update Text.
            textMesh.text = ammunitionTotal.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }*/
