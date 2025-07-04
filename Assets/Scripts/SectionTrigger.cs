using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    [SerializeField]
    private Section _section;

    public bool PlayerInside;

    void Start()
    {
        _section = GetComponentInParent<Section>();

        if (TryGetComponent(out MeshRenderer renderer))
        {
            renderer.enabled = false;
        }
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerInside = true;
            _section.UpdateSection();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerInside = false;
            _section.UpdateSection();
        }
    }
}
