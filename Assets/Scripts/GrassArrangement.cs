using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrassArrangement : MonoBehaviour
{

#if UNITY_EDITOR
    [SerializeField] 
    private MeshFilter _grass;
    [SerializeField] 
    private GameObject _grassPrefab;
    [SerializeField] 
    private Vector3 _offset = Vector3.zero;

    private List<GameObject> _spawnedGrass = new List<GameObject>();
    private List<Vector3> _basePositions = new List<Vector3>();

    [ContextMenu("Arrange Grass")]
    public void ArrangeGrass()
    {
        // Очищаем предыдущие объекты
        ClearGrass();

        if (_grass == null || _grass.sharedMesh == null)
        {
            Debug.LogError("Grass mesh is not assigned!");
            return;
        }

        if (_grassPrefab == null)
        {
            Debug.LogError("Grass prefab is not assigned!");
            return;
        }

        Mesh mesh = _grass.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Группируем треугольники по 6 вершин (по 2 треугольника на травинку)
        for (int i = 0; i < triangles.Length; i += 6)
        {
            if (i + 5 >= triangles.Length) break;

            // Берем вершины первого треугольника
            int index1 = triangles[i];
            int index2 = triangles[i + 1];
            int index3 = triangles[i + 2];

            // Вычисляем центр треугольника
            Vector3 center = (vertices[index1] + vertices[index2] + vertices[index3]) / 3f;

            // Преобразуем в мировые координаты
            Vector3 worldCenter = _grass.transform.TransformPoint(center);

            // Создаем экземпляр травы
            GameObject grassInstance = PrefabUtility.InstantiatePrefab(_grassPrefab) as GameObject;
            grassInstance.transform.position = worldCenter + _offset;
            grassInstance.transform.rotation = Quaternion.identity;
            grassInstance.transform.parent = transform;

            /* Instantiate(
                _grassPrefab,
                worldCenter + _offset,
                Quaternion.identity,
                transform
            );*/

            // Сохраняем базовую позицию (без офсета)
            _basePositions.Add(worldCenter);
            _spawnedGrass.Add(grassInstance);
        }

        Debug.Log($"Arranged {_spawnedGrass.Count} grass instances");
    }

    [ContextMenu("Validate Grass")]
    public void ValidateGrass()
    {
        float distance = 0.5f;
        List<GameObject> spawnedGrassTemp = new List<GameObject>();

    }

    [ContextMenu("Clear Grass")]
    public void ClearGrass()
    {
        foreach (GameObject grass in _spawnedGrass)
        {
            if (grass != null)
            {
                DestroyImmediate(grass);
            }
        }

        _spawnedGrass.Clear();
        _basePositions.Clear();
    }

    public void UpdateOffset(Vector3 newOffset)
    {
        _offset = newOffset;
        UpdateGrassPositions();
    }

    [ContextMenu("UpdateGrassPositions")]
    private void UpdateGrassPositions()
    {
        if (_spawnedGrass.Count != _basePositions.Count)
        {
            Debug.LogWarning("Position data mismatch. Re-arranging grass.");
            ArrangeGrass();
            return;
        }

        for (int i = 0; i < _spawnedGrass.Count; i++)
        {
            if (_spawnedGrass[i] != null)
            {
                _spawnedGrass[i].transform.position = _basePositions[i] + _offset;
            }
        }
    }

    // Для удобства обновления через инспектор
    private void OnValidate()
    {
        if (Application.isPlaying && _spawnedGrass.Count > 0)
        {
            UpdateGrassPositions();
        }
    }
#endif
}