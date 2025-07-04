using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{

	public class CharacterAnimationEventHandler : MonoBehaviour
	{
		#region FIELDS

		private Character _charktr;

        private Character playerCharacter { get {
				if(_charktr == null) {
					_charktr = Character.Instance;
				}
				return _charktr;
			} set { _charktr = Character.Instance; } }

		#endregion

		#region UNITY

		private void Awake()
		{
			//Grab a reference to the character component.
			playerCharacter = Character.Instance;// ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
		}

		#endregion

		#region ANIMATION


		private void OnEjectCasing()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.EjectCasing();
		}

		private void OnAmmunitionFill(int amount = 0)
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.FillAmmunition(amount);
		}

		private void OnSetActiveKnife(int active)
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.SetActiveKnife(active);
		}

		private void OnGrenade()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.Grenade();
		}

		private void OnSetActiveMagazine(int active)
		{
			if(playerCharacter != null)
            {
                playerCharacter.SetActiveMagazine(active);
            }
		}

		private void OnAnimationEndedBolt()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedBolt();
		}

		private void OnAnimationEndedReload()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedReload();
		}

		private void OnAnimationEndedGrenadeThrow()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedGrenadeThrow();
		}

		private void OnAnimationEndedMelee()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedMelee();
		}

		private void OnAnimationEndedInspect()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedInspect();
		}

		private void OnAnimationEndedHolster()
		{
			//Debug.Log($"{nameof(OnAnimationEndedHolster)} Invoked. Charackter is {playerCharacter}");
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedHolster();
		}

		private void OnSlideBack(int back)
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.SetSlideBack(back);
		}

		#endregion
	}   
}