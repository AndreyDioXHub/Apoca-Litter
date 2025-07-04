//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using System;

namespace InfimaGames.LowPolyShooterPack.Legacy
{
	public class ImpactScript : MonoBehaviour
	{
		public float despawnTimer = 10.0f;

		public AudioClip[] impactSounds;

		public AudioSource audioSource;

		public bool IsDrill;

		private void Start()
		{
			DespawnTimer();
            audioSource.clip = impactSounds[UnityEngine.Random.Range(0, impactSounds.Length)];
			audioSource.Play();

			if (IsDrill)
			{
				float volume = PlayerPrefs.GetFloat(PlayerPrefsConsts.audiodrill, 1);
                audioSource.volume = volume;
            }
        }

        private async void DespawnTimer()
		{
			await UniTask.WaitForSeconds(despawnTimer);

			try
            {
                try
                {
                    Destroy(gameObject);
                }
                catch (NullReferenceException e)
                {

                }
            }
			catch(MissingReferenceException e)
			{

			}
		}
	}
}