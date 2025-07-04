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
        // ������� ����� ������������ ������
        GameObject lodParent = new GameObject($"{name}_LODParent");

        // ����������� ������� � ��������
        lodParent.transform.position = transform.position;
        lodParent.transform.rotation = transform.rotation;
        lodParent.transform.localScale = Vector3.one;
        lodParent.transform.SetParent(transform.parent);

        // ����������� ��� � ����
        lodParent.tag = _tag;
        int layer = LayerMaskToLayer(_layer);
        lodParent.layer = layer;
        gameObject.layer = layer;
        gameObject.tag = _tag;

        // ��������� ������� ������ � ������ ��������
        transform.SetParent(lodParent.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // ��������� MeshCollider � �������� �������
        MeshCollider collider = gameObject.AddComponent<MeshCollider>();
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh != null)
        {
            collider.sharedMesh = mesh;
        }

        // ��������� LOD Group � ������������� �������
        _lodGroup = lodParent.AddComponent<LODGroup>();

        // ������� ��� ������ LOD
        LOD[] lods = new LOD[1];

        // LOD0 - ������������ ���
        Renderer renderer = GetComponent<Renderer>();
        lods[0] = new LOD(_transition, new Renderer[] { renderer });
        //lods[0].re
        // ����������� LOD Group
        _lodGroup.SetLODs(lods);
        //_lodGroup.RecalculateBounds();
        //SetCustomBounds(_customBounds);
        // ����������� ��������
        _lodGroup.fadeMode = LODFadeMode.None;
        _lodGroup.animateCrossFading = true;

        DestroyImmediate(this);
    }// ��������������� ������� ��� �������������� LayerMask � ������ ����
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

        // ���������� ��������� ��� ��������� ���������� ������
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