﻿//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Helper StateMachineBehaviour that allows us to more easily play a specific weapon sound.
    /// </summary>
    public class PlaySoundCharacterBehaviour : StateMachineBehaviour
    {
        /// <summary>
        /// Type of weapon sound.
        /// </summary>
        private enum SoundType
        {
            //Character Actions.
            GrenadeThrow, Melee,
            //Holsters.
            Holster, Unholster,
            //Normal Reloads.
            Reload, ReloadEmpty,
            //Cycled Reloads.
            ReloadOpen, ReloadInsert, ReloadClose,
            //Firing.
            Fire, FireEmpty,
            //Bolt.
            BoltAction
        }

        #region FIELDS SERIALIZED

        [Title(label: "Setup")]
        
        [Tooltip("Delay at which the audio is played.")]
        [SerializeField]
        private float delay;
        
        [Tooltip("Type of weapon sound to play.")]
        [SerializeField]
        private SoundType soundType;
        
        [Title(label: "Audio Settings")]

        [Tooltip("Audio Settings.")]
        [SerializeField]
        private AudioSettings audioSettings = new AudioSettings(1.0f, 0.0f, true);

        #endregion

        #region FIELDS

        /// <summary>
        /// Player Character.
        /// </summary>
        private Character playerCharacter;

        /// <summary>
        /// Player Inventory.
        /// </summary>
        private Inventory playerInventory;

        /// <summary>
        /// The service that handles sounds.
        /// </summary>
        private IAudioManagerService audioManagerService;

        #endregion
        
        #region UNITY

        /// <summary>
        /// On State Enter.
        /// </summary>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Character.Instance == null)
            {
                return;
            }

            //We need to get the character component.
            playerCharacter ??= Character.Instance; //ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();

            //Get Inventory.
            playerInventory ??= playerCharacter.GetInventory();

            //Try to get the equipped weapon's Weapon component.
            if (!(playerInventory.GetEquipped() is { } weaponBehaviour))
                return;
            
            //Try grab a reference to the sound managing service.
            audioManagerService ??= ServiceLocator.Current.Get<IAudioManagerService>();

            #region Select Correct Clip To Play

            //Switch.
            AudioClip clip = soundType switch
            {
                //Grenade Throw.
                SoundType.GrenadeThrow => playerCharacter.GetAudioClipsGrenadeThrow().GetRandom(),
                //Melee.
                SoundType.Melee => playerCharacter.GetAudioClipsMelee().GetRandom(),
                
                //Holster.
                SoundType.Holster => weaponBehaviour.GetAudioClipHolster(),
                //Unholster.
                SoundType.Unholster => weaponBehaviour.GetAudioClipUnholster(),
                
                //Reload.
                SoundType.Reload => weaponBehaviour.GetAudioClipReload(),
                //Reload Empty.
                SoundType.ReloadEmpty => weaponBehaviour.GetAudioClipReloadEmpty(),
                
                //Reload Open.
                SoundType.ReloadOpen => weaponBehaviour.GetAudioClipReloadOpen(),
                //Reload Insert.
                SoundType.ReloadInsert => weaponBehaviour.GetAudioClipReloadInsert(),
                //Reload Close.
                SoundType.ReloadClose => weaponBehaviour.GetAudioClipReloadClose(),
                
                //Fire.
                SoundType.Fire => weaponBehaviour.GetAudioClipFire(),
                //Fire Empty.
                SoundType.FireEmpty => weaponBehaviour.GetAudioClipFireEmpty(),
                
                //Bolt Action.
                SoundType.BoltAction => weaponBehaviour.GetAudioClipBoltAction(),
                
                //Default.
                _ => default
            };

            #endregion

            //Play with some delay. Granted, if the delay is set to zero, this will just straight-up play!
            audioManagerService.PlayOneShotDelayed(clip, audioSettings, delay);
        }
        
        #endregion
    }
}