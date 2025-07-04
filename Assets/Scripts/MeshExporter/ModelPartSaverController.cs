using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ModelPartSaverController : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private List<GameObject > _parts = new List<GameObject>();
    [SerializeField]
    private List<ModelPartSaver> _savers = new List<ModelPartSaver>();

    void Start()
    {
        
    }

    void Update()
    {

    }

    [ContextMenu("ProcessFullPipeLine")]
    public void ProcessFullPipeLine()
    {
        CollectParts();
        AddModelPartSaver();
        ProcessModelPartSaver();
    }

    public void CollectParts()
    {
        for(int i=0; i< transform.childCount; i++)
        {
            _parts.Add(transform.GetChild(i).GetChild(0).gameObject);
        }
    }

    public void AddModelPartSaver()
    {
        for(int i=0; i< _parts.Count; i++)
        {
            ModelPartSaver saver = _parts[i].AddComponent<ModelPartSaver>();
            _savers.Add(saver);
        }
    }

    public void ClearModelPartSaver()
    {
        for (int i = 0; i < _savers.Count; i++)
        {
            DestroyImmediate(_savers[i]);
        }
    }

    public void ProcessModelPartSaver()
    {
        ProcessModelPartSaverAsync();
    }

    public async void ProcessModelPartSaverAsync()
    {
        for (int i = 0; i < _savers.Count; i++)
        {
            _savers[i].ExportGameObjectsAsync(gameObject.name);
            await UniTask.WaitForSeconds(1);
        }
    }


    [ContextMenu("Recreate Game Part Objects")]
    public void RecreateGamePartObjects()
    {
        //for (int i = 0; i < transform.childCount; i++)
        //for (int i = 0; i < 3; i++)
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject part = transform.GetChild(i).GetChild(0).gameObject;
            GameObject empty = new GameObject(part.name);

            MeshFilter meshFilter = empty.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = empty.AddComponent<MeshRenderer>();
            MeshCollider collider = empty.AddComponent<MeshCollider>();

            meshFilter.sharedMesh = part.GetComponent<MeshFilter>().sharedMesh;
            List<Material> materials = new List<Material>();
            part.GetComponent<MeshRenderer>().GetSharedMaterials(materials);
            meshRenderer.SetSharedMaterials(materials);
            collider.sharedMesh = meshFilter.sharedMesh;

            LODGroup groupParent = part.transform.parent.GetComponent<LODGroup>();
            LOD[] lODs = groupParent.GetLODs();

            lODs[0] = new LOD(lODs[0].screenRelativeTransitionHeight, new Renderer[] { meshRenderer });
            groupParent.SetLODs(lODs);
            groupParent.RecalculateBounds();

            empty.tag = part.tag;
            empty.layer = part.layer;

            empty.transform.parent = part.transform.parent;
            empty.transform.localPosition = Vector3.zero;

            DestroyImmediate(part);
        }
    }
#endif
}
