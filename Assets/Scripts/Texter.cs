using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Texter : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();

    [SerializeField]
    private bool _dublicate;

    private void Start()
    {
        if (_dublicate)
        {
            string origin = _texts[0].text;
            UpdateText(origin);
        }

        foreach (var t in _texts)
        {
            t.gameObject.SetActive(true);
        }
    }

    public void UpdateText(string text)
    {
        foreach(var t in _texts)
        {
            t.text = text;
        }
    }

    private void Update()
    {
        if (_dublicate)
        {
            string origin = _texts[0].text;
            UpdateText(origin);
        }
    }
}
