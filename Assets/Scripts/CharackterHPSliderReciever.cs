using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class CharackterHPSliderReciever : MonoBehaviour
{
    [SerializeField]
    private Slider _hpSlider;

    [SerializeField]
    private TextMeshProUGUI _text;

    void Start()
    {
        //AddListenerOnCharackter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDamageRecived(float cur, float max)
    {
        //Debug.Log($"{(int)cur} / {(int)max}");
        _hpSlider.maxValue = max;
        _hpSlider.value = cur;
        _text.text = $"{ (int)cur } / { (int)max }";
    }

    public void AddListenerOnCharackter(Character character)
    {
        character.OnDamageRecived.AddListener(OnDamageRecived);
    }
}
