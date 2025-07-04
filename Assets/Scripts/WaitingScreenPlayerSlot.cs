using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaitingScreenPlayerSlot : MonoBehaviour
{
    [SerializeField]
    private PlayerLoadingInfo _info;
    [SerializeField]
    private TextMeshProUGUI _playerNameText;
    [SerializeField]
    private TextMeshProUGUI _waitingText;
    [SerializeField]
    private Image _status;
    /*[SerializeField]
    private string _defaultText;*/
    [SerializeField]
    private Color _statusOffline;
    [SerializeField]
    private Color _statusPrepare;
    [SerializeField]
    private Color _statusReady;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void UpdateItem(PlayerLoadingInfo info)
    {
        _info = info;
        //_playerNameText.text = string.IsNullOrEmpty(_info.name) ? _defaultText : _info.name;

        switch(info.status) 
        {
            case 0:
                _status.color = _statusOffline;
                _waitingText.gameObject.SetActive(true);
                _playerNameText.gameObject.SetActive(false);
                break;
            case 1:
                _waitingText.gameObject.SetActive(false);
                _playerNameText.gameObject.SetActive(true);
                _playerNameText.text = _info.name;
                _status.color = _statusPrepare;
                break;
            case 2:
                _waitingText.gameObject.SetActive(false);
                _playerNameText.gameObject.SetActive(true);
                _playerNameText.text = _info.name;
                _status.color = _statusReady;
                break;
        }
    }
}
