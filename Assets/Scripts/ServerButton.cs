using kcp2k;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerButton : MonoBehaviour
{
    [SerializeField]
    private ServerInfo _info;
    [SerializeField]
    private int _index;
    [SerializeField]
    private ServerText _text;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Button _button;
    [SerializeField]
    private GameObject _serverNameText;
    [SerializeField]
    private GameObject _localGameText;
    [SerializeField]
    private Color _common;
    [SerializeField]
    private Color _disabled;

    private bool _isTest = false;

    public void Init(ServerInfo info, int index)
    {
        _info = info;

        if (_isTest)
        {
            _serverNameText.SetActive(true);
            _localGameText.SetActive(false);
        }
        else
        {
            _serverNameText.SetActive(!_info.address.Equals("localhost"));
            _localGameText.SetActive(_info.address.Equals("localhost"));
        }

        _button.interactable = _info.connections < 16;
        _image.color = _info.connections < 16 ? _common : _disabled;
        _text.UpdateValue($"{_info.name} {_info.connections}/16");
    }

    public void Connect()
    {
        if(_isTest)
        {
            //Debug.Log($"Try connect to server {_info.address} : {_info.port}");
            ControlNetworkManager.singleton.networkAddress = _info.address;
            ControlNetworkManager.singleton.gameObject.GetComponent<KcpTransport>().Port = _info.port;
            ControlNetworkManager.singleton.StartClient();
        }
        else
        {
            if (_info.address.Equals("localhost"))
            {
                //Debug.Log($"Start Host");
                ControlNetworkManager.singleton.StartHost();
            }
            else
            {
                //Debug.Log($"Try connect to server {_info.address} : {_info.port}");
                ControlNetworkManager.singleton.networkAddress = _info.address;
                ControlNetworkManager.singleton.gameObject.GetComponent<KcpTransport>().Port = _info.port;
                ControlNetworkManager.singleton.StartClient();
            }
        }

    }
}
