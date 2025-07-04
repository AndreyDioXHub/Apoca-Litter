using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class JoypadActivator : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    public GameObject JoyButton;
    public OnScreenStick Stick;
    // Start is called before the first frame update
    void Start()
    {
        JoyButton.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(BaseEventData baseEventData) {
        PointerEventData pointerEventData = (PointerEventData)baseEventData;
        JoyButton.transform.position = pointerEventData.position;
        JoyButton.SetActive(true);
        Stick.OnPointerDown(pointerEventData);

    }

    public void OnPointerDown(PointerEventData pointerEventData) {
        JoyButton.transform.position = pointerEventData.position;
        JoyButton.SetActive(true);
        Stick.OnPointerDown(pointerEventData);
    }

    public void OnDrag(PointerEventData eventData) {
        Stick.OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData) {
        Stick.OnPointerUp(eventData);
        JoyButton.SetActive(false);
    }
}
