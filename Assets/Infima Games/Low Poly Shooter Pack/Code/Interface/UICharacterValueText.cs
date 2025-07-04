using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack.Interface
{
    public class UICharacterValueText : UICharacterValue
    {
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
                if (textMesh != null)
                {
                    textMesh.text = incomeValue.ToString();
                }
            }                
        }
    }

}
