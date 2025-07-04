using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelectMap : MonoBehaviour
{
    [SerializeField]
    private GameObject _status;

    private int _index;
    private Button _button;
    private MenuMapModel _model;

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void Init(int index, MenuMapModel model)
    {
        _index = index;
        _model = model;

        _button = GetComponent<Button>();
        _button.onClick.AddListener(SelectMap);
    }

    public void SelectMap()
    {
        _model.SelectIndex(_index);
    }

    public void SetActiveStatus(bool isActive)
    {
        _status.SetActive(isActive);
    }


}
