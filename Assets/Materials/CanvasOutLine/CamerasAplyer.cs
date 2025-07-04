using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasAplyer : MonoBehaviour
{
    [SerializeField]
    private Camera _meCamera;
    [SerializeField]
    private Transform _planeForGlow;
    public Vector3 screenhightpointposition;
    [SerializeField]
    private Camera _outLinedObjCamera;
    [SerializeField]
    private Camera _globalDepthCamera;
    [SerializeField]
    private Material _outLineMtl;
    private RenderTexture _outLinedTex;
    private RenderTexture _hiderTex;

    // Start is called before the first frame update
    void Start()
    {
        _outLinedObjCamera.fieldOfView = _meCamera.fieldOfView;
        _globalDepthCamera.fieldOfView = _meCamera.fieldOfView;

        _outLinedTex = new RenderTexture(Screen.width / 2, Screen.height / 2, 24);
        _hiderTex = new RenderTexture(Screen.width / 2, Screen.height / 2, 24);
        _outLinedTex.antiAliasing = 1;
        _hiderTex.antiAliasing = 1;
        _outLinedTex.format = RenderTextureFormat.Depth;
        _outLinedTex.filterMode = FilterMode.Point;
        _hiderTex.format = RenderTextureFormat.Depth;
        _hiderTex.filterMode = FilterMode.Point;
        _outLineMtl.SetTexture("_OtlinedObjectsTexture", _outLinedTex);
        _outLineMtl.SetTexture("_HideObjectsTexture", _hiderTex);
        _outLinedObjCamera.targetTexture = _outLinedTex;
        _globalDepthCamera.targetTexture = _hiderTex;
        _outLinedObjCamera.gameObject.SetActive(true);
        _globalDepthCamera.gameObject.SetActive(true);

        Vector2 planeSize = GetNearPlaneSize(_meCamera);
        planeSize = planeSize / 10;
        _planeForGlow.localScale = new Vector3(planeSize.x, 1f, planeSize.y);
    }

    private Vector2 GetNearPlaneSize(Camera cam)
    {
        float distance = cam.nearClipPlane;
        float halfFOV = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;

        // Вычисляем высоту
        float height = 2f * Mathf.Tan(halfFOV) * distance;

        // Вычисляем ширину с учетом аспекта
        float width = height * cam.aspect;

        return new Vector2(width, height);
    }

    private void OnDrawGizmosSelected()
    {
        if (_meCamera == null) return;

        // Визуализация near clipping plane
        Vector3[] corners = GetNearPlaneCorners(_meCamera);
        Gizmos.color = Color.cyan;

        for (int i = 0; i < corners.Length; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % corners.Length]);
        }
    }

    private Vector3[] GetNearPlaneCorners(Camera cam)
    {
        Vector2 size = GetNearPlaneSize(cam);
        Vector3 forward = cam.transform.forward * cam.nearClipPlane;
        Vector3 right = cam.transform.right * size.x * 0.5f;
        Vector3 up = cam.transform.up * size.y * 0.5f;

        return new Vector3[]
        {
            cam.transform.position + forward - right - up,
            cam.transform.position + forward + right - up,
            cam.transform.position + forward + right + up,
            cam.transform.position + forward - right + up
        };
    }
}
