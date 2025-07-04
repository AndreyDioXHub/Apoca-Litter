using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GypsyDoor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource SourceSelect => _sourceSelect;

    [SerializeField]
    private int _childIndex = -1;


    private AudioSource _sourceSelect;
    private AudioSource _sourceTap;
    private AudioClip _clipSelect;
    private AudioClip _clipTap;
    private Button _buttonTap;


    void Start()
    {
        _sourceSelect = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        _sourceTap = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        _clipSelect = Resources.Load<AudioClip>($"SelectSound");
        _clipTap = Resources.Load<AudioClip>($"TapSound");

        _sourceSelect.clip = _clipSelect;
        _sourceTap.clip = _clipTap;

        _sourceSelect.playOnAwake = false;
        _sourceTap.playOnAwake = false;

        _buttonTap = GetComponent<Button>();
        _buttonTap.onClick.AddListener(()=> _sourceTap.Play());

        if (_childIndex >= 0)
        {
            transform.SetSiblingIndex(_childIndex);
        }
    }   
    
    void Update()
    {

    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
        
    }*/

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter");
        _sourceSelect.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }


}
