using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourRemover : MonoBehaviour
{
    [SerializeField]
    private List<MonoBehaviour> _monoBehaviours = new List<MonoBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    [ContextMenu("Remove")]
    public void Remove()
    {
        RemoveM(transform);
    }

    [ContextMenu("Delete")]
    public void Delete()
    {
        for(int i=0; i< _monoBehaviours.Count; i++)
        {
            DestroyImmediate(_monoBehaviours[i]);
        }
    }
    
    public void RemoveM(Transform parent)
    {
        if (parent.TryGetComponent(out MonoBehaviour mono))
        {
            _monoBehaviours.Add(mono);
        }

        // Проходим по всем дочерним объектам и рекурсивно вызываем эту же функцию
        foreach (Transform child in parent)
        {
            RemoveM(child);
        }
    }


}
