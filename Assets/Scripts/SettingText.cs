using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingText : MonoBehaviour
{

    [SerializeField]
    private List<TextLocalizer> _textLocalizers = new List<TextLocalizer>();

    void Start()
    {

        foreach(var t in _textLocalizers)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
