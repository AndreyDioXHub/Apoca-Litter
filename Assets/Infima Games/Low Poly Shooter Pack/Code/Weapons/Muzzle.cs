//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using System.Collections;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Muzzle.
    /// </summary>
    public class Muzzle : MonoBehaviour//MuzzleBehaviour
    {
        #region FIELDS SERIALIZED

        [SerializeField]
        private Transform socket;

        [SerializeField]
        private Sprite sprite;

        [SerializeField]
        private AudioClip audioClipFire;

        [SerializeField]
        private GameObject prefabFlashParticles;

        [SerializeField]
        private int flashParticlesCount = 5;

        [SerializeField]
        private GameObject prefabFlashLight;

        [SerializeField]
        private float flashLightDuration;

        [SerializeField]
        private Vector3 flashLightOffset;

        /*
        [SerializeField]
        private bool _isDrill;
        [SerializeField]
        private AudioClip _startDrill;
        [SerializeField]
        private AudioClip _drillContinue;
        [SerializeField]
        private AudioClip _drillEnd;*/

        #endregion

        #region FIELDS

        /// <summary>
        /// Instantiated Particle System.
        /// </summary>
        private ParticleSystem particles;
        /// <summary>
        /// Instantiated light.
        /// </summary>
        private Light flashLight;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Awake.
        /// </summary>
        private void Awake()
        {
            //Null Check.
            if(prefabFlashParticles != null)
            {
                //Instantiate Particles.
                GameObject spawnedParticlesPrefab = Instantiate(prefabFlashParticles, socket);
                //Reset the position.
                spawnedParticlesPrefab.transform.localPosition = default;
                //Reset the rotation.
                spawnedParticlesPrefab.transform.localEulerAngles = default;
                
                //Get Reference.
                particles = spawnedParticlesPrefab.GetComponent<ParticleSystem>();
            }

            //Null Check.
            if (prefabFlashLight)
            {
                //Instantiate.
                GameObject spawnedFlashLightPrefab = Instantiate(prefabFlashLight, socket);
                //Reset the position.
                spawnedFlashLightPrefab.transform.localPosition = flashLightOffset;
                //Reset the rotation.
                spawnedFlashLightPrefab.transform.localEulerAngles = default;
                
                //Get reference.
                flashLight = spawnedFlashLightPrefab.GetComponent<Light>();
                //Disable.
                flashLight.enabled = false;
            }
        }

        #endregion

        #region GETTERS

        public void Effect()
        {
            //Try to play the fire particles from the muzzle!
            if(particles != null)
            {
                particles.Emit(flashParticlesCount);
                //Debug.Break();
            }

            //Make sure that we have a light to flash!
            if (flashLight != null)
            {
                //Enable the light.
                flashLight.enabled = true;
                //Disable the light after a few seconds.
                StartCoroutine(nameof(DisableLight));
            }
        }

        public Transform GetSocket() => socket;

        public Sprite GetSprite() => sprite;
        public AudioClip GetAudioClipFire() => audioClipFire;
        
        public ParticleSystem GetParticlesFire() => particles;
        public int GetParticlesFireCount() => flashParticlesCount;
        
        public Light GetFlashLight() => flashLight;
        public float GetFlashLightDuration() => flashLightDuration;

        #endregion

        #region METHODS

        private IEnumerator DisableLight()
        {
            //Wait.
            yield return new WaitForSeconds(flashLightDuration);
            //Disable.
            flashLight.enabled = false;
        }

        #endregion
    }
}