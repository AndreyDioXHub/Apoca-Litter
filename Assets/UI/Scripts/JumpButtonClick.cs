using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks; // Добавьте эту директиву для использования UniTask

public class JumpButtonClick : MonoBehaviour {
    [SerializeField]
    private Image Gradient;
    [SerializeField]
    private Image RectImage;
    [SerializeField]
    private Mask RectMask;
    [SerializeField]
    private Button JumpButton;
    [SerializeField]
    private float JumpReloadTime = 1f;

    // Start is called before the first frame update
    void Start() {
        ConfigureActiveState();
        JumpButton.onClick.AddListener(FreezeClick);
    }

    private void ConfigureActiveState() {
        RectMask.showMaskGraphic = true;
        Gradient.gameObject.SetActive(false);
        JumpButton.interactable = true;
    }

    // Update is called once per frame
    void Update() {
    }

    private async void FreezeClick() {
        Gradient.gameObject.SetActive(true);
        JumpButton.interactable = false;
        RectMask.showMaskGraphic = false;

        float elapsedTime = 0f;
        while (elapsedTime < JumpReloadTime) {
            Gradient.fillAmount = elapsedTime / JumpReloadTime;
            elapsedTime += Time.deltaTime;
            await UniTask.Yield(); // Ожидание следующего кадра
        }

        Gradient.fillAmount = 1f;
        //await UniTask.Delay(TimeSpan.FromSeconds(0.1f)); // Небольшая задержка перед сбросом анимации
        //await UniTask.WaitForSeconds(0.2f);
        Gradient.gameObject.SetActive(false);
        Gradient.fillAmount = 0f;
        ConfigureActiveState();
    }
}
