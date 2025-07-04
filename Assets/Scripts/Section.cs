using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SectionRegistrator))]
public class Section : MonoBehaviour
{
    public UnityEvent<Section> OnSectionEnter = new UnityEvent<Section>();
    public UnityEvent<Section> OnSectionExit = new UnityEvent<Section>();

    public bool IsActiveSection => _isActiveSection;

    /*
    [SerializeField]
    private List<Transform> _spawnPoints = new List<Transform>();*/
    [SerializeField]
    private List<SectionTrigger> _triggers = new List<SectionTrigger>();
    [SerializeField]
    private bool _isActiveSection;
    [SerializeField]
    private bool _isActiveSectionPrev;

    void Start()
    {
        SectionTrigger[] sectionTriggers = GetComponentsInChildren<SectionTrigger>();

        foreach (SectionTrigger trigger in sectionTriggers)
        {
            _triggers.Add(trigger);
        }
    }

    void Update()
    {
        
    }

    public void UpdateSection()
    {
        _isActiveSectionPrev = _isActiveSection;
        _isActiveSection = false;

        foreach(var trigger in _triggers)
        {
            if (trigger.PlayerInside)
            {
                _isActiveSection = true;
                break;
            }
        }

        if(_isActiveSectionPrev != _isActiveSection)
        {
            if (_isActiveSection)
            {
                SectionEnter();
            }
            else
            {
                SectionExit();
            }
        }
    }

    public void SectionEnter()
    {
        //Debug.Log("OnSectionEnter");
        OnSectionEnter?.Invoke(this);
        /*foreach (var spawnPoint in _spawnPoints)
        {
            BotsManager.Instance.SpawnBot(spawnPoint.position);
        }*/

    }

    public void SectionExit()
    {
        OnSectionExit?.Invoke(this);
        //Debug.Log("OnSectionExit");

    }

}
