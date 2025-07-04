using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InfimaGames.LowPolyShooterPack.Interface
{
    public class UICharacterValueBar : UICharacterValue
    {
        private Image _imgBar;

        public override void Awake()
        {
            base.Awake();

            _imgBar = GetComponent<Image>();
            CharacterValues.OnLanguageChanged += UpdateValue;
        }
        public override void UpdateValue(CharacterValueKey key, object incomeValue)
        {
            base.UpdateValue(key, incomeValue);

            if (key == _key)
            {
                ValueCurentMax val = (ValueCurentMax)incomeValue; //new AmmunitionCurrent(ammunitionCurrent, magazineBehaviour.GetAmmunitionTotal());

                if (_imgBar != null)
                {
                    float life = val.current / val.total;// inventoryBehaviour.GetAmmoCount(equippedWeaponBehaviour.GetAmmoType());// equippedWeaponBehaviour.GetAmmunitionTotal();

                    life = life < 0 ? 0 : life;
                    life = life > 1 ? 1 : life;

                    //Update Text.
                    _imgBar.fillAmount = life;
                }
            }
        }
    }
}
