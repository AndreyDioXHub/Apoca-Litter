using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
#endif


public class ModelPartSaver : MonoBehaviour
{
#if UNITY_EDITOR
    void Start()
    {
    }

    void Update()
    {

    }

    [ContextMenu("Export Game Objects")]
    public void ExportGameObjects()
    {
        ExportGameObjectsAsync("test");
    }
    /// <param name="filePath">Путь к FBX-файлу внутри Assets (например, "Assets/Models/MyModel.fbx")</param>

    public async void ExportGameObjectsAsync(string subFolder)
    {
        Object[] objects = { gameObject as Object };
        string filePath = Path.Combine(Application.dataPath, "Models", "MinePerdansk", "ChancModels", $"{subFolder}", $"{gameObject.name}.fbx");

        string filePathModel = Path.Combine("Assets", "Models", "MinePerdansk", "ChancModels", $"{subFolder}", $"{gameObject.name}.fbx");

        ExportGameObjects(objects, filePath);

        await UniTask.WaitForSeconds(1);

        //Debug.Log(File.Exists(filePath));

        AssetDatabase.Refresh();
        await UniTask.WaitForSeconds(0.1f);

        List<Mesh> meshes = new List<Mesh>();
        Object[] loadedObjects = AssetDatabase.LoadAllAssetsAtPath(filePathModel);

        foreach (Object obj in loadedObjects)
        {
            if (obj is Mesh mesh)
            {
                meshes.Add(mesh);
            }
        }

        //Debug.Log($"Loaded {meshes.Count} meshes");
        // Здесь можно сохранить все меши в массив или список
        gameObject.GetComponent<MeshFilter>().sharedMesh = meshes[0];

        DestroyImmediate(this);
    }

    public void ExportGameObjects(Object[] objects, string filePath)
    {
        
        ModelExporter.ExportObjects(filePath, objects);

        // ModelExporter.ExportObject can be used instead of 
        // ModelExporter.ExportObjects to export a single game object
    }
#endif
}
