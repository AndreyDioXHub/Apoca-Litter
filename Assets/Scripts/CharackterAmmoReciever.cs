using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using InfimaGames.LowPolyShooterPack.Interface;

public class CharackterAmmoReciever : UICharacterValue
{
    [SerializeField]
    private TextMeshProUGUI textMesh;
    [SerializeField]
    string curent;
    [SerializeField]
    string total;

    public override void Awake()
    {
        base.Awake();

        textMesh = GetComponent<TextMeshProUGUI>();
        CharacterValues.OnLanguageChanged += UpdateValue;
    }
    public override void UpdateValue(CharacterValueKey key, object incomeValue)
    {
        base.UpdateValue(key, incomeValue);

        string message0 = "";

        if (key == CharacterValueKey.AmmoCountCurent)
        {
            if (textMesh != null)
            {
                ValueCurentMax value = (ValueCurentMax)incomeValue;
                curent = $"{value.current}";
                message0 = $"{curent}/{total}"; 
                //message0 = (ValueCurentMax)incomeValue.cu.ToString();
                //Debug.Log(message0);
                textMesh.text = message0;
            }
        }

        if (key == CharacterValueKey.AmmoCountTotal)
        {
            if (textMesh != null)
            {
                total = incomeValue.ToString();
                message0 = $"{curent}/{total}";
                //message0 = (ValueCurentMax)incomeValue.cu.ToString();
                //Debug.Log(message0);
                textMesh.text = message0;
            }
        }        
    }
}
