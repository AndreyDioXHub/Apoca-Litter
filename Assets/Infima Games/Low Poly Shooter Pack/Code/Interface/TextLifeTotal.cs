using InfimaGames.LowPolyShooterPack;
using InfimaGames.LowPolyShooterPack.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLifeTotal : MonoBehaviour
{
    [SerializeField]
    private Image image;

    protected IGameModeService gameModeService;
    protected Character character;

    private void Start()
    {
        gameModeService = ServiceLocator.Current.Get<IGameModeService>();
        character = Character.Instance;
        character.OnDamageRecived.AddListener(RecieveDamage);
    }

    public void RecieveDamage(float lifeCurent, float lifeTotal)
    {
        float life = lifeCurent / lifeTotal;// inventoryBehaviour.GetAmmoCount(equippedWeaponBehaviour.GetAmmoType());// equippedWeaponBehaviour.GetAmmunitionTotal();

        life = life < 0 ? 0 : life;
        life = life > 1 ? 1 : life;

        //Update Text.
        image.fillAmount = life;
    }


}
