using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSettingsUIPause : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        PauseScreen.Instance?.SetActiveInGameSettings(true);
    }
    private void OnDisable()
    {
        PauseScreen.Instance?.SetActiveInGameSettings(false);
    }
}
