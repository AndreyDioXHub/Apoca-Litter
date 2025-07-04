using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Censored;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;



#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CensoredList", menuName = "Chat/CensoredList")]
public class CensoredList : ScriptableObject {
    Censor _censor;

    [SerializeField]
    List<string> _filterList;

    [SerializeField]
    string _locale = "ru";

    [SerializeField]
    bool _replace = false;

    [SerializeField]
    private TextAsset _textAsset; // Поле для TextAsset

    // Start is called before the first frame update
    async void OnEnable() {
        if (_filterList == null || _filterList.Count == 0) {
            if (_textAsset != null) {
                await LoadFromTextAssetAsync(_textAsset);
            }
        }
        Init();
    }

    private void Init() {
        _censor = new Censor(_filterList);
    }

    public string ReplaceText(string text) {
        return _censor.CensorText(text);
    }

    [ContextMenu("Load list from file")]
    private void LoadFromFile() {
        #region Unity editor
#if UNITY_EDITOR
        string tfile = EditorUtility.OpenFilePanel("Select file", Application.dataPath, "txt");
        string[] listItems = File.ReadAllLines(tfile);
        if (_replace) {
            _filterList = listItems.ToList<string>();
        } else {
            _filterList.AddRange(listItems);
        }
#endif
        #endregion
    }

    private async UniTask LoadFromTextAssetAsync(TextAsset textAsset) {
        using (StringReader reader = new StringReader(textAsset.text)) {
            string line;
            while ((line = await reader.ReadLineAsync()) != null) {
                if (_replace) {
                    _filterList = new List<string> { line };
                    _replace = false; // После первой строки заменяем на добавление
                } else {
                    _filterList.Add(line);
                }
            }
        }
    }
}
