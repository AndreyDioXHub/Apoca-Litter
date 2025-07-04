using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraSkinSelectButton : MonoBehaviour
{
    private ChelikiMenuModel _model;
    private ExtraSkinMenuController _controller;

    public int Index => _index;
    public bool IsSelected => _isSelected;

    [SerializeField]
    private Image _skinView;
    [SerializeField]
    private Animator _winedAnimatorStatus;
    [SerializeField]
    private GameObject _winedStatus;
    [SerializeField]
    private GameObject _previewStatus;
    [SerializeField]
    private int _index;
    [SerializeField]
    private bool _isSelected;
    private string _name = "haircut";

    private AudioSource _sourceSelect;
    private AudioClip _clipSelect;

    void Start()
    {
        _sourceSelect = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        _clipSelect = Resources.Load<AudioClip>($"SelectSound");

        _sourceSelect.clip = _clipSelect;

        _sourceSelect.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(ExtraSkinMenuController controller, ChelikiMenuModel model, Sprite sprite, int iconIndex)
    {
        _controller = controller;
        _model = model;

        _skinView.sprite = sprite;
        int defaultIndexesBeforExtra = 7;
        _index = iconIndex + defaultIndexesBeforExtra;

        _controller.OnPreviewIndexChanged.AddListener(UpdatePreviewStatus);
    }

    public void PlaySelectSound()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Triggered {gameObject.name}");
    }

    public void Equip()
    {
        _model.Equip(_name, _index);
    }

    public void PreviewSkin()
    {   
        if(_isSelected)
        {

        }
        else
        {
            if (_sourceSelect)
            {
                if (_sourceSelect.isPlaying)
                {

                }
                else
                {
                    _sourceSelect.Play();
                }
            }
        }
        _model.Equip(_name, _index);
        _controller.OnPreviewIndexChanged?.Invoke(_index);
    }

    public void UpdatePreviewStatus(int index)
    {
        _isSelected = index == _index;
        _previewStatus.SetActive(index == _index);
    }
    
    public void UpdateWinningStatus(int index)
    {
        if(index == _index)
        {
            _winedAnimatorStatus.SetTrigger("Trigger");
        }
        _winedStatus.SetActive(index == _index);
    }
}
