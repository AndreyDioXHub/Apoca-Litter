using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieCollider : MonoBehaviour
{


    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Character character))
        {
            if (LocalSounds.Instance != null)
            {
                LocalSounds.Instance.PlaySound("water");
            }
            character.Permach();
        }
    }
}
