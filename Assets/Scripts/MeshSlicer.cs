using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshSlicer : MonoBehaviour
{
    [SerializeField] private int _chunkSize = 8;

    [ContextMenu("Slice Mesh")]
    public void SliceMesh()
    {
        Mesh originalMesh = GetComponent<MeshFilter>().sharedMesh;
        if (originalMesh == null)
        {
            Debug.LogError("No mesh found!");
            return;
        }

        Material[] materials = GetComponent<MeshRenderer>().sharedMaterials;
        Vector3[] vertices = originalMesh.vertices;
        Vector2[] uv = originalMesh.uv;

        // Создаем родительский объект для всех чанков
        GameObject chunksParent = new GameObject($"{name}_Sliced");
        chunksParent.transform.SetParent(transform.parent);
        chunksParent.transform.position = transform.position;
        chunksParent.transform.rotation = transform.rotation;
        chunksParent.transform.localScale = Vector3.one;

        // Вычисляем границы меша
        Bounds meshBounds = originalMesh.bounds;
        Vector3 min = meshBounds.min;

        // Создаем сетку чанков
        Dictionary<Vector2Int, ChunkData> chunkMap = new Dictionary<Vector2Int, ChunkData>();

        // Обрабатываем каждый подмеш отдельно
        for (int submesh = 0; submesh < originalMesh.subMeshCount; submesh++)
        {
            int[] triangles = originalMesh.GetTriangles(submesh);

            // Проходим по всем треугольникам текущего подмеша
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int index1 = triangles[i];
                int index2 = triangles[i + 1];
                int index3 = triangles[i + 2];

                TriangleData tri = new TriangleData(
                    vertices[index1], vertices[index2], vertices[index3],
                    uv[index1], uv[index2], uv[index3],
                    submesh
                );

                // Определяем чанки для треугольника
                HashSet<Vector2Int> affectedChunks = GetAffectedChunks(tri, min);

                foreach (Vector2Int chunkCoord in affectedChunks)
                {
                    if (!chunkMap.ContainsKey(chunkCoord))
                    {
                        chunkMap[chunkCoord] = new ChunkData();
                    }
                    chunkMap[chunkCoord].AddTriangle(tri);
                }
            }
        }

        // Создаем объекты чанков
        foreach (var kvp in chunkMap)
        {
            CreateChunkObject(
                parent: chunksParent.transform,
                coord: kvp.Key,
                data: kvp.Value,
                materials: materials,
                originalName: name
            );
        }

        // Отключаем оригинальный объект
        gameObject.SetActive(false);
    }

    private HashSet<Vector2Int> GetAffectedChunks(TriangleData tri, Vector3 meshMin)
    {
        HashSet<Vector2Int> chunks = new HashSet<Vector2Int>();

        // Добавляем чанки для каждой вершины треугольника
        AddChunkForVertex(chunks, tri.v1, meshMin);
        AddChunkForVertex(chunks, tri.v2, meshMin);
        AddChunkForVertex(chunks, tri.v3, meshMin);

        return chunks;
    }

    private void AddChunkForVertex(HashSet<Vector2Int> chunks, Vector3 vertex, Vector3 meshMin)
    {
        int chunkX = Mathf.FloorToInt((vertex.x - meshMin.x) / _chunkSize);
        int chunkZ = Mathf.FloorToInt((vertex.z - meshMin.z) / _chunkSize);
        chunks.Add(new Vector2Int(chunkX, chunkZ));
    }

    private void CreateChunkObject(Transform parent, Vector2Int coord, ChunkData data, Material[] materials, string originalName)
    {
        GameObject chunk = new GameObject($"{originalName}_Chunk_{coord.x}_{coord.y}");
        chunk.transform.SetParent(parent);
        chunk.transform.localPosition = Vector3.zero;
        chunk.transform.localRotation = Quaternion.identity;
        chunk.transform.localScale = Vector3.one;

        MeshFilter mf = chunk.AddComponent<MeshFilter>();
        MeshRenderer mr = chunk.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        // Устанавливаем данные меша
        mesh.vertices = data.Vertices.ToArray();
        mesh.uv = data.UVs.ToArray();

        // Настраиваем подмеши
        mesh.subMeshCount = data.Submeshes.Count;
        for (int i = 0; i < data.Submeshes.Count; i++)
        {
            mesh.SetTriangles(data.Submeshes[i].Triangles, i);
        }

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        mf.sharedMesh = mesh;

        // Настраиваем материалы (только используемые в чанке)
        Material[] usedMaterials = data.Submeshes
            .OrderBy(s => s.SubmeshIndex)
            .Select(s => materials[s.SubmeshIndex])
            .ToArray();

        mr.sharedMaterials = usedMaterials;
    }

    private class TriangleData
    {
        public Vector3 v1, v2, v3;
        public Vector2 uv1, uv2, uv3;
        public int SubmeshIndex;

        public TriangleData(Vector3 v1, Vector3 v2, Vector3 v3,
                          Vector2 uv1, Vector2 uv2, Vector2 uv3,
                          int submeshIndex)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.uv1 = uv1;
            this.uv2 = uv2;
            this.uv3 = uv3;
            SubmeshIndex = submeshIndex;
        }
    }

    private class ChunkData
    {
        public List<Vector3> Vertices { get; } = new List<Vector3>();
        public List<Vector2> UVs { get; } = new List<Vector2>();
        public List<SubmeshData> Submeshes { get; } = new List<SubmeshData>();

        public void AddTriangle(TriangleData tri)
        {
            int index = Vertices.Count;

            // Добавляем вершины и UV
            Vertices.Add(tri.v1);
            Vertices.Add(tri.v2);
            Vertices.Add(tri.v3);

            UVs.Add(tri.uv1);
            UVs.Add(tri.uv2);
            UVs.Add(tri.uv3);

            // Находим или создаем подмеш для материала
            SubmeshData submesh = Submeshes.FirstOrDefault(s => s.SubmeshIndex == tri.SubmeshIndex);
            if (submesh == null)
            {
                submesh = new SubmeshData(tri.SubmeshIndex);
                Submeshes.Add(submesh);
            }

            // Добавляем индексы треугольников
            submesh.Triangles.Add(index);
            submesh.Triangles.Add(index + 1);
            submesh.Triangles.Add(index + 2);
        }
    }

    private class SubmeshData
    {
        public int SubmeshIndex { get; }
        public List<int> Triangles { get; } = new List<int>();

        public SubmeshData(int submeshIndex)
        {
            SubmeshIndex = submeshIndex;
        }
    }
}