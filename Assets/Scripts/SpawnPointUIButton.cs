using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPointUIButton : MonoBehaviour
{
    [SerializeField]
    private GameObject _notSelectedIconGO;
    [SerializeField]
    private GameObject _selectedIconGO;
    [SerializeField]
    private int _index;
    [SerializeField]
    private CheckPointSelectTextView _textView;
    [SerializeField]
    private Button _button;
    [SerializeField]
    private WaitingScreen _waitingScreen;

    void Start()
    {
    }

    void Update()
    {
        
    }

    public void Init(WaitingScreen waitingScreen, int index)
    {
        _index = index;
        _waitingScreen = waitingScreen;
        _textView.Init(_index);
        _button.onClick.AddListener(Click);
    }

    public void Select(int index)
    {
        _selectedIconGO.SetActive(index == _index);
    }

    public void Click()
    {
        _waitingScreen.SelectSpawnIndex(_index);
    }
}
