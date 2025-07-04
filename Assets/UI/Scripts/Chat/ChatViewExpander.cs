using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChatViewExpander : MonoBehaviour
{
    public UnityEvent OnExpanded = new UnityEvent();
    public UnityEvent OnMinimized = new UnityEvent();

    [SerializeField]
    private RectTransform _chatView;

    [SerializeField]
    private List<GameObject> _showInMinimized;

    [SerializeField]
    private List<GameObject> _showInExpandView;

    [SerializeField]
    private bool _minimizeOnStart = false;

    public bool IsExpanded { get; private set; } = false;

    private float _lastcallTime = 0;
    // Start is called before the first frame update
    void Awake() {
    }

    private void Start() {
        Expand(!_minimizeOnStart);
    }

    public void Expand(bool isExpand) {
        if (Time.time - _lastcallTime < 0.5f) {
            return;
        }
        _lastcallTime = Time.time;
        IsExpanded = isExpand;
        if (isExpand) {
            _chatView.sizeDelta = new Vector2(660, 400);
        } else {
            _chatView.sizeDelta = new Vector2(660, 60);
        }
        foreach (var item in _showInMinimized) {
            item.SetActive(!isExpand);
        }
        foreach (var item in _showInExpandView) {
            item.SetActive(isExpand);
        }

        if (isExpand) {
            OnExpanded.Invoke();
        } else {
            OnMinimized.Invoke();
        }
    }
}
