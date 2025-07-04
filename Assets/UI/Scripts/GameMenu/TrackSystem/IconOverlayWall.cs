using MagicPigGames.Northstar;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconOverlayWall : OverlayIcon 
{
    public LayerMask _wallLayer;

    public GameObject _screenMarker;
    public TextMeshProUGUI _screenMarkerText;
    public Image _selfImage;
    public Vector2 screenPosition;
    public float _maxDistanceFromCenter = 50f;
    public AnimationCurve _maxDistanceFromCenterCurve;

    public Color _color;

    protected override bool ShouldShow() 
    {
        bool doShow = base.ShouldShow();
        try
        {
            bool InScreen = IsInScreenBounds(_trackedTargetOverlay.transform.position);

            if (ShowScreenOverlay && InScreen)
            {
                Transform camTransform = TargetCamera.transform;
                float distance = Vector3.Distance(camTransform.position, _trackedTargetOverlay.transform.position);
                Vector3 directionToTarget = (_trackedTargetOverlay.transform.position - camTransform.position).normalized;
                doShow = doShow && Physics.Raycast(camTransform.position, directionToTarget, distance, _wallLayer);
            }
        }
        catch(NullReferenceException e)
        {
            doShow = false;
        }

        return doShow;
    }

    private bool IsInScreenBounds(Vector3 worldPosition) 
    {
        Vector3 viewportPoint = TargetCamera.WorldToViewportPoint(worldPosition);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
    }

    public override float FadeValueLookingToMarker()
    {
        screenPosition = new Vector2(((RectTransform)transform).position.x - Screen.width / 2, ((RectTransform)transform).position.y - Screen.height / 2);

        float distanceFromCenter = Vector2.Distance(screenPosition, Vector2.zero);
        float t = distanceFromCenter / _maxDistanceFromCenter;
        float value = _maxDistanceFromCenterCurve.Evaluate(t); 
        
        return value;
    }

    /*
    public override void Setup(NorthstarOverlay northstarOverlayValue,
            TrackedTargetOverlay trackedTargetOverlayValue)
    {
        base.Setup(northstarOverlayValue, trackedTargetOverlayValue);

        Color color = _screenMarkerText.color;
        _screenMarkerText.color = new Color(color.r, color.g, color.b, 0);
    }
    */

    protected override void SetValues()
    {
        base.SetValues();

        Color color = _image.color;

        color.r = _color.r;
        color.g = _color.g;
        color.b = _color.b;

        _image.color = color;

        Color arrowColor = arrowImage.color;

        arrowColor.r = _color.r;
        arrowColor.g = _color.g;
        arrowColor.b = _color.b;

        arrowImage.color = arrowColor;

        float value = FadeValueLookingToMarker();

        Color colorText = _selfImage.color;
        colorText.a = value > _selfImage.color.a ? _selfImage.color.a : value;
        colorText.a = ShouldShow() ? colorText.a : 0;
        _screenMarkerText.color = colorText;
    }

    private void OnDrawGizmos() 
    {
        if (_trackedTargetOverlay != null) 
        {
            try
            {
                Transform camTransform = TargetCamera.transform;
                float distance = Vector3.Distance(camTransform.position, _trackedTargetOverlay.transform.position);
                Vector3 directionToTarget = (_trackedTargetOverlay.transform.position - camTransform.position).normalized;
                Vector3 endPoint = camTransform.position + directionToTarget * distance;

                // Рисуем линию от текущей позиции до позиции цели
                Gizmos.color = Color.red;
                Gizmos.DrawLine(camTransform.position, endPoint);

                // Рисуем сферу в начальной точке
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(camTransform.position, 0.1f);

                // Рисуем сферу в конечной точке
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(endPoint, 0.1f);
            }
            catch (NullReferenceException e)
            {
                Debug.Log("some shit is null");
            }
        }
    }
}
