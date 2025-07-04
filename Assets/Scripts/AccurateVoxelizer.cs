using UnityEngine;
using System.Collections.Generic;


public class AccurateVoxelizer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _voxelSize = 0.1f;
    [SerializeField] private Vector3 _localOffset = Vector3.zero;
    [SerializeField] private float _checkRadiusMultiplier = 0.5f;
    [SerializeField] private GameObject _voxelPrefab;
    [SerializeField] private LayerMask _meshLayer;

    private List<Vector3> _voxelPoints = new List<Vector3>();
    private MeshCollider _meshCollider;
    private Bounds _adjustedBounds;

    [SerializeField] private bool _useDoubleCheck = true;
    private void Awake()
    {
        _meshCollider = GetComponent<MeshCollider>();
        UpdateBounds();
    }

    private void UpdateBounds()
    {
        // Рассчитываем границы с учетом offset
        Bounds originalBounds = _meshCollider.sharedMesh.bounds;
        _adjustedBounds = new Bounds(
            originalBounds.center + _localOffset,
            originalBounds.size
        );
    }

    [ContextMenu("Generate Voxels with Offset")]
    public void GenerateVoxelsWithOffset()
    {
        _voxelPoints.Clear();
        UpdateBounds();

        Vector3 start = _adjustedBounds.min;
        Vector3 end = _adjustedBounds.max;

        for (float x = start.x; x <= end.x; x += _voxelSize)
        {
            for (float y = start.y; y <= end.y; y += _voxelSize)
            {
                for (float z = start.z; z <= end.z; z += _voxelSize)
                {
                    // Применяем offset в локальных координатах
                    Vector3 localPoint = new Vector3(x, y, z) + _localOffset;
                    Vector3 worldPoint = transform.TransformPoint(localPoint);

                    if (IsPointInsideMesh(worldPoint))
                    {
                        _voxelPoints.Add(worldPoint);
                    }
                }
            }
        }

        Debug.Log($"Generated {_voxelPoints.Count} voxels with offset");
    }

    private bool IsPointInsideMesh(Vector3 worldPoint)
    {
        float checkRadius = _voxelSize * _checkRadiusMultiplier;
        bool collisionCheck = Physics.CheckSphere(worldPoint, checkRadius, _meshLayer);

        if (!_useDoubleCheck) return collisionCheck;

        return collisionCheck && IsInsideAdjustedBounds(worldPoint);
    }

    private bool IsInsideAdjustedBounds(Vector3 worldPoint)
    {
        // Преобразуем мировые координаты в локальные с учетом offset
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint) - _localOffset;
        return _meshCollider.sharedMesh.bounds.Contains(localPoint);
    }

    [ContextMenu("Visualize Voxels")]
    public void VisualizeVoxels()
    {
        if (_voxelPrefab == null) return;

        foreach (var point in _voxelPoints)
        {
            Instantiate(_voxelPrefab, point, Quaternion.identity, transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация границ с offset
        if (_meshCollider != null && _meshCollider.sharedMesh != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_adjustedBounds.center, _adjustedBounds.size);
        }
    }

    [ContextMenu("Center Offset")]
    private void CenterOffset()
    {
        // Автоматический центр смещения
        _localOffset = _meshCollider.sharedMesh.bounds.center;
        UpdateBounds();
    }
}