using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LongTapPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public GameObject panelTemplate; // Ссылка на панель, которую нужно открыть
    private bool isPointerDown = false;
    private float pointerDownTimer = 0f;
    public float requiredHoldTime = 1f; // Время, необходимое для длительного нажатия (в секундах)

    public event Action OnStartOpenContext;

    private static GameObject _panel;

    private void Awake() {
        if (_panel == null) {
            Transform rootCanvas = GetRootCanvas(transform);
            if (rootCanvas != null) {
                _panel = Instantiate(panelTemplate, rootCanvas);
                _panel.SetActive(false);
            } else {
                Debug.LogError("Root Canvas not found in parent objects.");
            }
        }
    }

    void Update() {
        if (isPointerDown) {
            pointerDownTimer += Time.deltaTime;

            // Если время удержания превышает требуемое, открываем панель
            if (pointerDownTimer >= requiredHoldTime) {
                OpenPanel();
                Reset();
            }
        }

        // Закрытие панели при нажатии вне её области
        if (_panel != null && _panel.activeSelf) {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
                if (!IsPointerOverPanel()) {
                    ClosePanel();
                }
            } else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) {
                if (!IsPointerOverPanel()) {
                    ClosePanel();
                }
            }
        }
        
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("Pointer down");
        isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        Debug.Log("Pointer up");
        Reset();
    }

    private void OpenPanel() {
        if (_panel != null) {
            _panel.SetActive(false);
            // Устанавливаем позицию панели под курсор
            Vector2 cursorPosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Touchscreen.current.primaryTouch.position.ReadValue();
            RectTransform panelRectTransform = _panel.GetComponent<RectTransform>();
            panelRectTransform.position = cursorPosition;

            // Проверяем, не выходит ли панель за границы экрана
            Vector3[] panelCorners = new Vector3[4];
            panelRectTransform.GetWorldCorners(panelCorners);
            Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

            Vector3 offset = Vector3.zero;
            if (panelCorners[0].x < screenRect.xMin) {
                offset.x = screenRect.xMin - panelCorners[0].x;
            }
            if (panelCorners[2].x > screenRect.xMax) {
                offset.x = screenRect.xMax - panelCorners[2].x;
            }
            if (panelCorners[0].y < screenRect.yMin) {
                offset.y = screenRect.yMin - panelCorners[0].y;
            }
            if (panelCorners[2].y > screenRect.yMax) {
                offset.y = screenRect.yMax - panelCorners[2].y;
            }

            panelRectTransform.position += offset;
            OnStartOpenContext?.Invoke();
            _panel.SetActive(true); // Активируем панель
        }
    }

    private void ClosePanel() {
        if (_panel != null) {
            _panel.SetActive(false); // Деактивируем панель
        }
    }

    private void OnDisable() {
        Reset();
        ClosePanel();
    }

    private void Reset() {
        isPointerDown = false;
        pointerDownTimer = 0f;
    }

    // Проверка, находится ли курсор над панелью
    private bool IsPointerOverPanel() {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        if (Mouse.current != null) {
            eventData.position = Mouse.current.position.ReadValue(); // Используем новое Input System для мыши
        } else if (Touchscreen.current != null) {
            eventData.position = Touchscreen.current.primaryTouch.position.ReadValue(); // Используем новое Input System для сенсорного экрана
        }

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results) {
            if (result.gameObject == _panel || result.gameObject.transform.IsChildOf(_panel.transform)) {
                return true;
            }
        }
        return false;
    }

    // Метод для поиска корневого Canvas в родительских объектах
    private Transform GetRootCanvas(Transform current) {
        while (current != null) {
            if (current.GetComponent<Canvas>() != null) {
                return current;
            }
            current = current.parent;
        }
        return null;
    }
}
