using Cysharp.Threading.Tasks;
using NewBotSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RagDollDropper : MonoBehaviour
{
    [SerializeField]
    private BotResolver _resolver;
    [SerializeField]
    private Transform _slot;
    [SerializeField]
    private Transform _skin;
    [SerializeField]
    private Transform _ragdoll;
    [SerializeField]
    private AnimationCurve _disolvingCurve;
    [SerializeField]
    private SkinnedMeshRenderer _renderer;
    [SerializeField]
    private SkinnedMeshRenderer _originRenderer;

    [SerializeField]
    private Transform[] _ragdollBones;
    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private float _time=1, _timeCur;
    [SerializeField]
    private RagDollStatus _status;
    [SerializeField]
    private Material _materialRagdoll;
    [SerializeField]
    private Material _materialOriginal;


    [ContextMenu("CollectBones")]
    public void CollectBones()
    {
        // Собираем все кости регдолла в словарь
        if (_ragdoll != null)
        {
            _ragdollBones = _ragdoll.GetComponentsInChildren<Transform>();
            System.Array.Sort(_ragdollBones, (a, b) => a.name.CompareTo(b.name));
        }

        foreach(Transform t in _ragdollBones)
        { 
            t.gameObject.tag = "Blood"; 
        }
    }

    private void Start()
    {
        _materialRagdoll = new Material(_renderer.sharedMaterial.shader);
        _materialOriginal = new Material(_renderer.sharedMaterial.shader);

        _materialRagdoll.CopyMatchingPropertiesFromMaterial(_renderer.sharedMaterial);
        _materialOriginal.CopyMatchingPropertiesFromMaterial(_renderer.sharedMaterial);

        _materialRagdoll.name = _resolver.name;
        _materialOriginal.name = _resolver.name;

        _materialRagdoll.renderQueue = 3000;
        _materialOriginal.renderQueue = 3000;

        _renderer.SetMaterials(new List<Material> { _materialRagdoll });
        _originRenderer.SetMaterials(new List<Material> { _materialOriginal });
    }

    public void Init(BotResolver resolver)
    {
        _resolver = resolver;
        FindDamageble(_ragdoll);
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

        _timeCur = 0;
    }

    public void FindDamageble(Transform root)
    {
        if (root.TryGetComponent(out BotDamageble damageble))
        {
            damageble.Init(_resolver);
        }

        foreach(Transform child in root)
        {
            FindDamageble(child);
        }
    }

    public void CrashRagDoll()
    {
        if (_skin == null || _ragdoll == null)
        {
            return;
        }

        _status = RagDollStatus.crashed;

        SyncTransformsRecursive(_skin, _ragdollBones, 1);

        _skin.gameObject.SetActive(false);
        _ragdoll.parent = null;
        _ragdoll.gameObject.SetActive(true);
        _timeCur = 0;
    }

    private void LateUpdate()
    {
        _timeCur += Time.deltaTime;
        _timeCur = _timeCur > _time ? _time : _timeCur;

        if(_materialRagdoll == null || _materialOriginal == null)
        {

        }
        else
        {
            switch (_status)
            {
                case RagDollStatus.dead:
                    _materialRagdoll.SetFloat("_DissloveTime", _disolvingCurve.Evaluate(_timeCur / _time));

                    if (_timeCur == _time)
                    {
                        _timeCur = 0;
                        _ragdoll.parent = _slot;
                        gameObject.SetActive(false);
                        //_skin.gameObject.SetActive(true);
                        _materialRagdoll.SetFloat("_DissloveTime", 0);
                    }
                    break;
                case RagDollStatus.crashed:
                    if (_timeCur == _time)
                    {
                        _timeCur = 0;
                        _status = RagDollStatus.resecover;
                        SetActiveRigidBody(_ragdoll, true);
                        PrepareToRecover();
                        /*_resolver.RespawnOnServer();
                        SyncTransformsRecursive(_skin, _ragdollBones, 1);*/
                    }
                    break;
                case RagDollStatus.resecover:
                    if (_timeCur == _time)
                    {
                        _timeCur = 0;
                        //_status = RagDollStatus.resecover;
                        //_resolver.RespawnOnServer();
                        _status = RagDollStatus.dead;
                    }
                    else
                    {
                        SyncTransformsRecursive(_skin, _ragdollBones, _timeCur / _time);
                    }
                    break;
            }
        }        
    }

    public void SetActiveRigidBody(Transform skinBone, bool state)
    {
        if(skinBone.TryGetComponent(out Rigidbody bone))
        {
            bone.isKinematic = state;
        }

        foreach (Transform child in skinBone)
        {
            SetActiveRigidBody(child, state);
        }
    }

    public async void PrepareToRecover()
    {
        _skin.gameObject.SetActive(true);

        while (_materialRagdoll == null || _materialOriginal == null)
        {
            await UniTask.Yield();
        }

        _materialOriginal.SetFloat("_DissloveTime", 1);

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
        _resolver.transform.position = transformPosition + Vector3.up * 0.08f;
    }

    public async void ResetRagdoll()
    {
        SetActiveRigidBody(_ragdoll, false);
        _timeCur = 0;
        _ragdoll.parent = _slot;
        gameObject.SetActive(false);
        _skin.gameObject.SetActive(true);
        _originRenderer.enabled = true;

        while(_materialRagdoll == null || _materialOriginal == null)
        {
            await UniTask.Yield();
        }

        _materialOriginal.SetFloat("_DissloveTime", 0);
        _materialRagdoll.SetFloat("_DissloveTime", 0);
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

    // Вспомогательный класс для бинарного поиска по именам
    private class TransformNameComparer : System.Collections.Generic.IComparer<object>
    {
        public int Compare(object x, object y)
        {
            return ((Transform)x).name.CompareTo((string)y);
        }
    }
}
public enum RagDollStatus
{
    dead,
    crashed,
    resecover,
}