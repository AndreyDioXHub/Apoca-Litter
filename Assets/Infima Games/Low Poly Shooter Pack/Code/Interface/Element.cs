//Copyright 2022, Infima Games. All Rights Reserved.

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace InfimaGames.LowPolyShooterPack.Interface
{
    /// <summary>
    /// Interface Element.
    /// </summary>
    public abstract class Element : MonoBehaviour
    {
        #region FIELDS
        
        /// <summary>
        /// Game Mode Service.
        /// </summary>
        protected IGameModeService gameModeService;
        
        /// <summary>
        /// Player Character.
        /// </summary>
        protected Character characterBehaviour;
        /// <summary>
        /// Player Character Inventory.
        /// </summary>
        protected Inventory inventoryBehaviour;

        /// <summary>
        /// Equipped Weapon.
        /// </summary>
        protected Weapon equippedWeaponBehaviour;
        
        #endregion

        #region UNITY

        /// <summary>
        /// Awake.
        /// </summary>
        protected virtual void Awake()
        {
            //Get Game Mode Service. Very useful to get Game Mode references.
            gameModeService = ServiceLocator.Current.Get<IGameModeService>();

            WaitingPlayer();
        }

        private async void WaitingPlayer()
        {
            while (Character.Instance == null)
            {
                await UniTask.Yield(); // Ожидание следующего кадра
            }

            //Get Player Character.
            characterBehaviour = Character.Instance; //gameModeService.GetPlayerCharacter();
            //Get Player Character Inventory.
            inventoryBehaviour = characterBehaviour.GetInventory();
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            //Ignore if we don't have an Inventory.
            if (Equals(inventoryBehaviour, null))
                return;

            //Get Equipped Weapon.
            equippedWeaponBehaviour = inventoryBehaviour.GetEquipped();
            
            //Tick.
            Tick();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Tick.
        /// </summary>
        protected virtual void Tick() {}

        #endregion
    }
}