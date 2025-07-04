using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinPartEquippedStatus : MonoBehaviour
{

    private ChelikiMenuModel _model;

    [SerializeField]
    private GameObject _status;
    [SerializeField]
    private string _name;
    [SerializeField]
    private int _index;

    void Start()
    {
    }

    public void Init(ChelikiMenuModel model)
    {
        _model = model;
        _model.OnSelectNameIndex.AddListener(Equip);
    }

    void Update()
    {
        
    }

    public void Equip()
    {
        if (_name.Equals("haircut"))
        {
            PlayerPrefs.SetInt(PlayerPrefsConsts.haircutwiningindex, -1);
        }

        _model.Equip(_name, _index);
    }

    public void Equip(string name, int index)
    {
        if (_name.Equals(name))
        {
            _status.SetActive(_index == index);
        }
    }
}
