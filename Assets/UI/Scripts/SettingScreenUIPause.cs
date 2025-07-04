using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingScreenUIPause : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private void OnEnable()
    {
        PauseScreen.Instance?.SetActiveInGameSettings(true);
    }

    private void OnDisable()
    {
        PauseScreen.Instance?.SetActiveInGameSettings(false);

        gameObject.GetComponent<Animator>().enabled = true;
        _canvasGroup.alpha = 0;
    }
}
