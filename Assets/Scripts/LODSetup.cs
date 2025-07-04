using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LODSetup : MonoBehaviour
{
    [SerializeField] 
    private float _transition = 0.1f;
    [SerializeField] 
    private string _tag = "Dirt";
    [SerializeField] 
    private LayerMask _layer;

    private LODGroup _lodGroup;

    [ContextMenu("Setup LOD and Collider")]
    public void SetupLOD()
    {
        // Создаем новый родительский объект
        GameObject lodParent = new GameObject($"{name}_LODParent");

        // Настраиваем позицию и иерархию
        lodParent.transform.position = transform.position;
        lodParent.transform.rotation = transform.rotation;
        lodParent.transform.localScale = Vector3.one;
        lodParent.transform.SetParent(transform.parent);

        // Настраиваем тег и слой
        lodParent.tag = _tag;
        int layer = LayerMaskToLayer(_layer);
        lodParent.layer = layer;
        gameObject.layer = layer;
        gameObject.tag = _tag;

        // Переносим текущий объект в нового родителя
        transform.SetParent(lodParent.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // Добавляем MeshCollider к текущему объекту
        MeshCollider collider = gameObject.AddComponent<MeshCollider>();
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh != null)
        {
            collider.sharedMesh = mesh;
        }

        // Добавляем LOD Group к родительскому объекту
        _lodGroup = lodParent.AddComponent<LODGroup>();

        // Создаем два уровня LOD
        LOD[] lods = new LOD[1];

        // LOD0 - оригинальный меш
        Renderer renderer = GetComponent<Renderer>();
        lods[0] = new LOD(_transition, new Renderer[] { renderer });
        //lods[0].re
        // Настраиваем LOD Group
        _lodGroup.SetLODs(lods);
        //_lodGroup.RecalculateBounds();
        //SetCustomBounds(_customBounds);
        // Настраиваем переходы
        _lodGroup.fadeMode = LODFadeMode.None;
        _lodGroup.animateCrossFading = true;

        DestroyImmediate(this);
    }// Вспомогательная функция для преобразования LayerMask в индекс слоя
    private int LayerMaskToLayer(LayerMask layerMask)
    {
        int layerNumber = 0;
        int layer = layerMask.value;
        while (layer > 1)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber;
    }
    private void SetCustomBounds(Bounds bounds)
    {
        if (_lodGroup == null) return;

        // Используем рефлексию для установки внутренних границ
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        PropertyInfo property = typeof(LODGroup).GetProperty("localBounds", flags);

        if (property != null)
        {
            property.SetValue(_lodGroup, bounds, null);
            Debug.Log("Custom bounds set: " + bounds);
        }
        else
        {
            Debug.LogWarning("Failed to set custom bounds. Using automatic bounds.");
            _lodGroup.RecalculateBounds();
        }
    }
}