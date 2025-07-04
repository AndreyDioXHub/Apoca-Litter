using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DevMenuList : MonoBehaviour
{
    public GameObject TogglePrefab;
    public RectTransform Container;
    public ToggleGroup TGGroup;

    [Header("List of Menus GO")]
    public UIItems[] Items;
    public Toggle RootMenuToggle;

    // Start is called before the first frame update
    void Start()
    {
        foreach (UIItems item in Items)
        {
            // Создаем Toggle из префаба
            GameObject toggleGO = Instantiate(TogglePrefab, Container, false);
            
            // Получаем компонент Toggle и привязываем к группе
            Toggle toggle = toggleGO.GetComponent<Toggle>();
            toggle.group = TGGroup;
            toggle.SetIsOnWithoutNotify(false);
            
            // Находим и устанавливаем текст, используя TextMeshProUGUI вместо Text
            TextMeshProUGUI toggleText = toggleGO.GetComponentInChildren<TextMeshProUGUI>();
            if (toggleText != null)
            {
                toggleText.text = item.displayName;
            }

            // Добавляем обработчик изменения состояния Toggle
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    
                    
                    // Выключаем все остальные GameObject'ы
                    foreach (UIItems otherItem in Items)
                    {
                        if (otherItem != item)
                        {
                            otherItem.menuObject.SetActive(false);
                        }
                    }
                    // Включаем GameObject текущего элемента
                    item.menuObject.SetActive(true);
                    gameObject.SetActive(false);
                    RootMenuToggle.SetIsOnWithoutNotify(false);
                }
            });

            // Изначально выключаем все GameObject'ы
            item.menuObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[Serializable]
public class UIItems
{
    public GameObject menuObject;
    public string displayName;
}
