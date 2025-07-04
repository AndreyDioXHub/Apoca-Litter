using NewBotSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorSection : MonoBehaviour
{
    public List<BotPatrolPoint> PatrolPoints => _patrolPoints;

    private List<Transform> _spawnPoints = new List<Transform>();
    private List<BotPatrolPoint> _patrolPoints = new List<BotPatrolPoint>();

    void Start()
    {
        GetComponent<Section>().OnSectionEnter.AddListener(ActivateSection);
        GetComponent<Section>().OnSectionExit.AddListener(DisableSection);
        var points = GetComponentsInChildren<BotSpawnPoint>();
        foreach (var point in points)
        {
            _spawnPoints.Add(point.gameObject.transform);
        }
        
        var patrolPoints = GetComponentsInChildren<BotPatrolPoint>();
        _patrolPoints = new List<BotPatrolPoint>();

        foreach (var patrolPoint in patrolPoints)
        {
            _patrolPoints.Add(patrolPoint);
        }

        StartCoroutine(StartDisablePointsCoroutine());
    }

    IEnumerator StartDisablePointsCoroutine()
    {
        yield return new WaitForEndOfFrame();

        while (_patrolPoints == null)
        {
            yield return new WaitForEndOfFrame();
        }

        foreach (var patrolPoint in _patrolPoints)
        {
            patrolPoint.DisablePoint();
        }
    }

    void Update()
    {
        
    }

    public void ActivateSection(Section section)
    {
        /*
        BotManager.Instance.ClearSpawnPoints();

        foreach (Transform t in _spawnPoints)
        {
            BotManager.Instance.RegisterSpawnPoint(t);
        }

        BotManager.Instance.ClearPatrolPoints();

        foreach (var patrolPoint in _patrolPoints)
        {
            patrolPoint.ActivatePoint();
            BotManager.Instance.RegisterPatrolPoint(patrolPoint.transform);
        }*/
    }

    public void DisableSection(Section section)
    {
        //BotManager.Instance.CheckSections();

        foreach (var patrolPoint in _patrolPoints)
        {
            patrolPoint.DisablePoint();
        }
    }

    [ContextMenu("ShowZoneRendererInChild")]
    public void ShowZoneRendererInChild()
    {
        var sections = GetComponentsInChildren<SectionTrigger>();
        foreach (var section in sections)
        {
            section.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    [ContextMenu("HideZoneRendererInChild")]
    public void HideZoneRendererInChild()
    {
        var sections = GetComponentsInChildren<SectionTrigger>();
        foreach (var section in sections)
        {
            section.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

    }
}
