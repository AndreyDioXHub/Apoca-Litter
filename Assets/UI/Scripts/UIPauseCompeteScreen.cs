using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseCompeteScreen : MonoBehaviour
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
        PauseScreen.Instance?.SetActiveCompeteScreen(true);
    }
    private void OnDisable()
    {
        PauseScreen.Instance?.SetActiveCompeteScreen(false);
    }
}
