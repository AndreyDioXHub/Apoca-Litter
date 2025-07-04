using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuRetexture : MonoBehaviour
{
    public enum CameraType
    {
        menu,
        deth,
        dealer
    }

    [SerializeField]
    private RawImage rawImage;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private CameraType _type;
    [SerializeField]
    private string _leakMemoryText;

    private RenderTexture _renderTexture;



    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        //Debug.Log(_leakMemoryText);
        switch (_type)
        {
            case CameraType.menu:
                _camera = MenuChelikCamera.Instance.ChelikCamera;
                break;
            case CameraType.deth:

                break;
            case CameraType.dealer:
                _camera = DealerRoomCamera.Instance.DealerCamera;
                _camera.enabled = true;
                break;
        }
        Retexture();
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
        Retexture();
    }

    private void OnDisable()
    {
        // Освобождаем ресурсы RenderTexture при уничтожении объекта
        if (_renderTexture != null)
        {
            _renderTexture.Release();
        }
    }

    void OnDestroy()
    {
        // Освобождаем ресурсы RenderTexture при уничтожении объекта
        if (_renderTexture != null)
        {
            _renderTexture.Release();
        }
    }

    public void Retexture()
    {
        if(_camera == null)
        {
            // Освобождаем ресурсы RenderTexture при уничтожении объекта
            if (_renderTexture != null)
            {
                _renderTexture.Release();
            }
        }
        else
        {
            // Получаем текущее разрешение экрана
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            // Создаем RenderTexture с текущим разрешением экрана
            _renderTexture = new RenderTexture(screenWidth, screenHeight, 24); // 24 — глубина буфера (можно изменить)
            _renderTexture.Create(); // Инициализируем RenderTexture

            // Пример использования: присваиваем RenderTexture камере
            _camera.targetTexture = _renderTexture;
            rawImage.texture = _renderTexture;
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        if (gameObject.activeSelf)
        {
            Retexture();
        }

        //Debug.Log($"Window dimensions changed to {Screen.width}x{Screen.height}");
    }
}
