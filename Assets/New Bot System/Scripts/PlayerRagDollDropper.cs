using Cysharp.Threading.Tasks;
using NewBotSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagDollDropper : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;
    [SerializeField]
    private SkinResolverListener _listener;
    [SerializeField]
    private Transform _slot;
    [SerializeField]
    private Transform _skin;
    [SerializeField]
    private Transform _ragdoll;
    [SerializeField]
    private SkinnedMeshRenderer _renderer;
    [SerializeField]
    private SkinnedMeshRenderer _ragRendererMan;
    [SerializeField]
    private SkinnedMeshRenderer _ragRendererFem;
    [SerializeField]
    private string _sexName;

    [SerializeField]
    private Transform[] _ragdollBones;
    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private RagDollStatus _status;
    [SerializeField]
    private Material _materialRagdoll;

    void Start()
    {
        WaitingOriginalRenderer();
    }

    void Update()
    {
    }

    public void WaitingOriginalRenderer()
    {
    }

    public async void Init(PlayerNetworkResolver resolver)
    {
        _resolver = resolver;
        _resolver.OnSkinChanged.AddListener(SkinChanged);

        while (_listener.ActiveRenderer == null)
        {
            await UniTask.Yield();
        }

        _sexName = _listener.SexMane;

        _ragRendererMan.gameObject.SetActive(_sexName.Equals("Man"));
        _ragRendererFem.gameObject.SetActive(!_sexName.Equals("Man"));

        _materialRagdoll = new Material(_renderer.sharedMaterial.shader);

        _materialRagdoll.CopyMatchingPropertiesFromMaterial(_renderer.sharedMaterial);

        _materialRagdoll.name = gameObject.name;

        _materialRagdoll.renderQueue = 3000;

        _ragRendererMan.SetMaterials(new List<Material> { _materialRagdoll });
        _ragRendererFem.SetMaterials(new List<Material> { _materialRagdoll });
    }
    public void SkinChanged(string name, string json)
    {
        _sexName = name;

        _ragRendererMan.gameObject.SetActive(_sexName.Equals("Man"));
        _ragRendererFem.gameObject.SetActive(!_sexName.Equals("Man"));

        _materialRagdoll = new Material(_renderer.sharedMaterial.shader);

        _materialRagdoll.CopyMatchingPropertiesFromMaterial(_renderer.sharedMaterial);

        _materialRagdoll.name = gameObject.name;

        _materialRagdoll.renderQueue = 3000;

        _ragRendererMan.SetMaterials(new List<Material> { _materialRagdoll });
        _ragRendererFem.SetMaterials(new List<Material> { _materialRagdoll });
    }

    public async void DropRagDoll()
    {
        await UniTask.WaitForSeconds(Time.fixedDeltaTime);

        if (_skin == null || _ragdoll == null)
        {
            return;
        }

        _status = RagDollStatus.dead;

        if (_ragdoll.gameObject.activeSelf)
        {

        }
        else
        {
            SyncTransformsRecursive(_skin, _ragdollBones, 1);

            _skin.gameObject.SetActive(false);
            _ragdoll.parent = null;
            _ragdoll.gameObject.SetActive(true);
        }
    }

    public void ResetRagdoll()
    {
        transform.parent = _slot;

        _skin.gameObject.SetActive(true);

        RaycastHit hit;

        Vector3 dropPosition = _ragdoll.GetChild(0).position + Vector3.up;
        Vector3 transformPosition = Vector3.zero;
        float distance = 5f;

        if (Physics.Raycast(dropPosition, Vector3.down, out hit, distance, _layerMask))
        {
            transformPosition = hit.point;
        }
        else
        {
            transformPosition = dropPosition;
        }

        //_resolver.transform.position = transformPosition + Vector3.up * 0.08f;
        gameObject.SetActive(false);
        _skin.gameObject.SetActive(true);
    }

    private void SyncTransformsRecursive(Transform skinBone, Transform[] ragdollBones, float time)
    {
        // Ищем соответствующую кость в регдолле
        int index = System.Array.BinarySearch(
            ragdollBones,
            skinBone.name,
            new TransformNameComparer()
        );

        if (index >= 0)
        {
            // Синхронизируем трансформы
            Transform ragdollBone = ragdollBones[index];
            ragdollBone.position = Vector3.Lerp(ragdollBone.position, skinBone.position, time);
            ragdollBone.rotation = Quaternion.Lerp(ragdollBone.rotation, skinBone.rotation, time);
            ragdollBone.localScale = Vector3.Lerp(ragdollBone.localScale, skinBone.localScale, time);
        }

        // Рекурсивно обрабатываем детей
        foreach (Transform child in skinBone)
        {
            SyncTransformsRecursive(child, ragdollBones, time);
        }
    }

    [ContextMenu("CollectBones")]
    public void CollectBones()
    {
        // Собираем все кости регдолла в словарь
        if (_ragdoll != null)
        {
            _ragdollBones = _ragdoll.GetComponentsInChildren<Transform>();
            System.Array.Sort(_ragdollBones, (a, b) => a.name.CompareTo(b.name));
        }

        foreach (Transform t in _ragdollBones)
        {
            t.gameObject.tag = "Blood";
        }
    }
    private class TransformNameComparer : System.Collections.Generic.IComparer<object>
    {
        public int Compare(object x, object y)
        {
            return ((Transform)x).name.CompareTo((string)y);
        }
    }
}
