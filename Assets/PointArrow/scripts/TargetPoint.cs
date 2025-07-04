using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetPoint : MonoBehaviour
{
    /*
    [SerializeField]
    private GameObject _canvas;*/
    [SerializeField]
    private Transform _pointUIContainer;
    [SerializeField]
    private Transform _pointArrow;
    [SerializeField]
    private RectTransform _point;
    [SerializeField]
    private RectTransform _description;
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private LayerMask _mask;
    [SerializeField]
    private bool _isAutoInit;
    [SerializeField]
    private bool _isAlwaysOnScreen;
    [SerializeField]
    private bool _isActive = true;

    private bool _isAiming;
    private bool _isPause;

    private Camera _mainCamera;
    private Transform _pointTarget;
    private TextMeshProUGUI _descriptionText;
    private Vector2 _pointSize = Vector2.zero;
    private Vector3 _screenPos = Vector3.zero;
    private Vector3 _pointPos = Vector3.zero;
    private int _screenWidth;
    private int _screenHeight;
    private float _arrowAngle;

    private void Awake()
    {
    }

    private void Start()
    {
        LateStart();
    }

    private async void LateStart()
    {
        await UniTask.WaitUntil(() => Character.Instance != null);

        _pointTarget = transform;
        _mainCamera = Camera.main;
        _pointSize = new Vector2(_point.rect.width, _point.rect.height);
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
        _canvasGroup = _pointUIContainer.GetComponent<CanvasGroup>();

        Debug.Log("Character.Instance.OnAim.AddListener(OnAim);");
        
        PauseScreen.Instance.OnPaused.AddListener(OpenSetting);

        if (_isAutoInit)
        {
            Init(transform.parent.gameObject, "");
        }

        _point.GetChild(0).gameObject.GetComponent<Image>().raycastTarget = false;
        _point.GetChild(1).gameObject.GetComponent<Image>().raycastTarget = false;

        //Destroy(_canvas);

    }

    public void Init(GameObject root, string text)
    {
        LateInit(root, text);
    }

    public void LateInit(GameObject root, string text)
    {
        if (RootUI.Instance == null)
        {
            Destroy(gameObject);
        }

        GameObject rootUI = RootUI.Instance.gameObject;

        transform.SetParent(root.transform, false);
        Canvas rootcanvas = rootUI.GetComponentInParent<Canvas>();
        _pointUIContainer.SetParent(rootcanvas.transform, false);
        SetDescriptionText(text);
    }

    public void OnHide()
    {

    }

    public void OnRestore()
    {

    }

    public void DestroyEvent()
    {
        Debug.Log("Character.Instance.OnAim.RemoveListener(OnAim);");
        Debug.Log("Character.Instance.OnAim.RemoveListener(OpenSetting);");
        
        
        Destroy(_pointUIContainer.gameObject);
        Destroy(gameObject);
    }

    public void SetDescriptionText(string textforset)
    {
        _descriptionText = _description.GetChild(0).GetComponent<TextMeshProUGUI>();
        _descriptionText.text = textforset;
        _descriptionText.raycastTarget = false;
    }

    public void OnAim(bool isaim)
    {
        _isAiming = isaim;
        float value = _isPause || _isAiming ? 0 : 1;
        _canvasGroup.alpha = value;
    }

    public void OpenSetting(bool isPause)
    {
        _isPause = isPause;
        float value = _isPause || _isAiming ? 0 : 1;
        _canvasGroup.alpha = value;
    }

    public void SetActive(bool isActive)
    {
        _isActive = isActive;
    }

    void Update()
    {
        if (_mainCamera == null || !_isActive)
        {
            _point.gameObject.SetActive(false);
            _description.gameObject.SetActive(false);
            return;
        }
        else
        {
            _point.gameObject.SetActive(true);
            _description.gameObject.SetActive(true);
        }

        _screenPos = _mainCamera.WorldToScreenPoint(_pointTarget.position);
        _pointPos = _screenPos;

        if (_pointPos.x < _pointSize.x / 2)
        {
            _pointPos.x = _pointSize.x / 2;
        }

        if (_pointPos.x > _screenWidth - _pointSize.x / 2)
        {
            _pointPos.x = _screenWidth - _pointSize.x / 2;
        }

        if (_pointPos.y < _pointSize.y / 2)
        {
            _pointPos.y = _pointSize.y / 2;
        }

        if (_screenPos.y > _screenHeight - _pointSize.y / 2)
        {
            _pointPos.y = _screenHeight - _pointSize.y / 2;
        }

        if (_pointPos == _screenPos)
        {
            _pointArrow.gameObject.SetActive(false);
        }
        else
        {
            _pointArrow.gameObject.SetActive(false);
        }

        Vector3 targetDir = _screenPos - _pointPos;
        Vector3 forward = new Vector3(0, 1, 0);
        _arrowAngle = Vector3.SignedAngle(targetDir, forward, new Vector3(0, 0, -1));

        _pointArrow.localEulerAngles = new Vector3(0, 0, _arrowAngle);

        _point.anchoredPosition = new Vector2(_pointPos.x, _pointPos.y);
        _description.anchoredPosition = new Vector2(_screenPos.x, _screenPos.y);

        Vector3 directionToCam = _mainCamera.transform.position - transform.position;
        Vector3 forvardCamera = _mainCamera.transform.forward;
        directionToCam.y = 0;
        forvardCamera.y = 0;

        if (Vector3.Dot(forvardCamera.normalized, directionToCam.normalized) > 0)
        {
            //Debug.Log("is not Visible");
            _description.gameObject.SetActive(false);

            if (_pointPos.x > _screenWidth / 2)
            {
                _point.anchoredPosition = new Vector2(_point.sizeDelta.x / 2, _pointPos.y);
                //_description.anchoredPosition = new Vector2(_description.sizeDelta.x / 2, _screenPos.y);
            }
            else
            {
                _point.anchoredPosition = new Vector2(_screenWidth - _point.sizeDelta.x / 2, _pointPos.y);
                //_description.anchoredPosition = new Vector2(_screenWidth- _description.sizeDelta.x / 2, _screenPos.y);
            }
        }
        else
        {
            _description.gameObject.SetActive(true);

            if (_isAiming || _isPause)
            {
                _canvasGroup.alpha = 0;
            }
            else
            {
                RaycastHit hit;
                Vector3 direction = _mainCamera.transform.position - transform.position;

                if (_isAlwaysOnScreen)
                {

                }
                else
                {
                    if (Physics.Raycast(transform.position, direction.normalized, out hit, direction.magnitude, _mask))
                    {
                        _canvasGroup.alpha = 200f / 255f;
                    }
                    else
                    {
                        _canvasGroup.alpha = 0;
                    }
                }
            }
        }
    }
}
