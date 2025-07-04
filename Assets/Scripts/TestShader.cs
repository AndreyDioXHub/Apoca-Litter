using NewBotSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShader : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer _renderer;


    [SerializeField]
    private float _time = 1, _timeCur;
    [SerializeField]
    private Material _materialRagdoll;

    private void Start()
    {
        _materialRagdoll = new Material(_renderer.sharedMaterial.shader);
        _materialRagdoll.CopyMatchingPropertiesFromMaterial(_renderer.sharedMaterial);
        _materialRagdoll.name = "test";
        _materialRagdoll.renderQueue = 3000;
        //_materialOriginal.
        _renderer.SetMaterials(new List<Material> { _materialRagdoll });
    }

    private void LateUpdate()
    {
        _timeCur += Time.deltaTime;
        _timeCur = _timeCur > _time ? _time : _timeCur;

        _renderer.sharedMaterial.SetFloat("_DissloveTime", _timeCur / _time);

        if (_timeCur == _time)
        {
            _timeCur = 0;
            _renderer.sharedMaterial.SetFloat("_DissloveTime", 0);
        }
    }
}
