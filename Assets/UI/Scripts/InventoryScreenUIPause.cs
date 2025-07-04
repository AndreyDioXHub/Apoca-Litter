using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreenUIPause : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    void Start()
    {
        
    }


    void Update()
    {

    }

    private void OnEnable()
    {
        PauseScreen.Instance?.SetActiveInventoryScreen(true);

        if(MenuChelikCamera.Instance != null)
        {
            MenuChelikCamera.Instance.MoveToInventoryPosition();
        }

        if(MenuAimPosition.Instance != null)
        {
            MenuAimPosition.Instance.MoveToInventoryPosition();
        }
    }

    private void OnDisable()
    {
        PauseScreen.Instance?.SetActiveInventoryScreen(false);

        gameObject.GetComponent<Animator>().enabled = true;
        _canvasGroup.alpha = 0;

        if (MenuChelikCamera.Instance != null)
        {
            MenuChelikCamera.Instance.MoveToMenuPosition();
        }

        if (MenuAimPosition.Instance != null)
        {
            MenuAimPosition.Instance.MoveToMenuPosition();
        }
    }
}
