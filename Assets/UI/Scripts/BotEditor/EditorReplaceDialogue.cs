using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorReplaceDialogue : MonoBehaviour
{
    [SerializeField]
    private GameObject _replaceDialogue;
    // Start is called before the first frame update
    void Awake()
    {
        _replaceDialogue.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
