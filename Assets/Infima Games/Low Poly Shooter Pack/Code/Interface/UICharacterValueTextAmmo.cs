using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack.Interface
{
    public class UICharacterValueTextAmmo : UICharacterValue
    {
        [SerializeField]
        private bool _updateColor = true;
        [SerializeField]
        private Color _emptyColor = Color.red;

        private TextMeshProUGUI textMesh;
        public override void Awake()
        {
            base.Awake();

            textMesh = GetComponent<TextMeshProUGUI>();
            CharacterValues.OnLanguageChanged += UpdateValue;
        }

        public override void UpdateValue(CharacterValueKey key, object incomeValue)
        {
            base.UpdateValue(key, incomeValue);

            if (key == _key)
            {
                //Debug.Log($"{gameObject.name} {incomeValue.ToString()}");

                ValueCurentMax ammunition = (ValueCurentMax)incomeValue; //new AmmunitionCurrent(ammunitionCurrent, magazineBehaviour.GetAmmunitionTotal());

                if (textMesh != null)
                {
                    textMesh.text = ammunition.current.ToString();

                    if (_updateColor)
                    {
                        float colorAlpha = (ammunition.current / ammunition.total);
                        textMesh.color = Color.Lerp(_emptyColor, Color.white, colorAlpha);
                    }
                }
            }
        }
    }

}
