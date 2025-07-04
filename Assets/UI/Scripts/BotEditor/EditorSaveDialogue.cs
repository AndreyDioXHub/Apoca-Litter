using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorSaveDialogue : MonoBehaviour {
    [SerializeField]
    private GameObject _saveDialogue;
    [SerializeField]
    private TMP_InputField _inputField;
    [SerializeField]
    private Button _saveButton;
    [SerializeField]
    private Button _cancelButton;

    public string SaveName => _inputField.text;

    void Start() {
        _inputField.onValueChanged.AddListener(CheckInput);
        _saveDialogue.SetActive(false);
    }

    private void CheckInput(string arg) {
        _saveButton.interactable = !string.IsNullOrEmpty(arg);
    }


    public async UniTask<bool> Show() 
    {
        _saveDialogue.SetActive(true);
        _saveButton.interactable = false;
        _inputField.text = "";
        bool issave = false;
        /*
#if UNITY_EDITOR
        var result = await UniTask.WhenAny(
            _saveButton.OnClickAsync().ContinueWith(() => true),
            _cancelButton.OnClickAsync().ContinueWith(() => false)
        );
#endif
        bool issave = result.winArgumentIndex == 0;

        _saveDialogue.SetActive(false);*/
        return issave;
    }
}
