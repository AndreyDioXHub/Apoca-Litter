using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

public class SphereDamageGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] 
    private int _radius = 3;
    [SerializeField] 
    private AnimationCurve _damageFalloff = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] 
    private GameObject _cubePrefab;

    [Header("Debug")]
    [SerializeField] private Dictionary<Vector3Int, float> _damageMap = new Dictionary<Vector3Int, float>();

    [ContextMenu("Generate Damage Map")]
    public void GenerateDamageMap()
    {
        _damageMap.Clear();
        float maxDistance = _radius;

        for (int x = -_radius; x <= _radius; x++)
        {
            for (int y = -_radius; y <= _radius; y++)
            {
                for (int z = -_radius; z <= _radius; z++)
                {
                    Vector3Int position = new Vector3Int(x, y, z);
                    float distance = Vector3.Distance(Vector3.zero, position);

                    if (distance <= _radius)
                    {
                        float normalizedDistance = distance / maxDistance;
                        float damage = Mathf.Clamp01(_damageFalloff.Evaluate(normalizedDistance)) * 100f;
                        _damageMap[position] = damage;
                    }
                }
            }
        }

        Debug.Log($"_damageMap {_damageMap.Count}");
    }

    [ContextMenu("Spawn Damage Cubes")]
    public void SpawnDamageCubes()
    {
        if (_cubePrefab == null) return;

        foreach (var kvp in _damageMap)
        {
            GameObject cube = Instantiate(_cubePrefab, kvp.Key, Quaternion.identity, transform);
            SetCubeColor(cube, kvp.Value);
        }
    }

    private void SetCubeColor(GameObject cube, float damage)
    {
        /*
        var renderer = cube.GetComponent<Renderer>();
        if (renderer == null) return;

        Color color = Color.Lerp(Color.red, Color.green, damage / 100f);
        renderer.material.color = color;*/
    }

    [ContextMenu("Export to JSON")]
    public void ExportToJson()
    {
        if (_damageMap.Count == 0)
        {
            Debug.LogError("Damage map is empty!");
            return;
        }

        // Конвертируем в сериализуемый словарь
        Dictionary<string, float> serializableDict = new Dictionary<string, float>();
        foreach (var kvp in _damageMap)
        {
            string key = $"{kvp.Key.x},{kvp.Key.y},{kvp.Key.z}";
            serializableDict[key] = kvp.Value;
        }

#if UNITY_EDITOR
        string json = JsonConvert.SerializeObject(serializableDict, Formatting.Indented);
        string path = EditorUtility.SaveFilePanel("Save Damage Map", "", "damage_map.json", "json");

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            Debug.Log($"Exported {_damageMap.Count} entries to: {path}");
        }
#endif
    }

    [ContextMenu("Clear All")]
    public void ClearAll()
    {
        _damageMap.Clear();
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}