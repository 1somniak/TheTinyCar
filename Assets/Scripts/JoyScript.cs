using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyScript : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Image background;
    public Image moveButton;
    public Vector2 initialPosition;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public bool touchStart = false;
    public bool onDisplay;
    public float backgroundWidth;
    public Vector2 direction;
    public float Vertical => direction.y < 10 ? 0 : direction.y / 100f;
    public float Horizontal => direction.x < 10 ? 0 : direction.x / 100f;

    void Start()
    {
        initialPosition = background.transform.position;
        backgroundWidth = background.GetComponent<RectTransform>().sizeDelta.x / 2f;
        touchStart = false;
        onDisplay = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!(Norm(eventData.position - initialPosition) < backgroundWidth)) return;
        
        startPoint = eventData.position;
        endPoint = eventData.position;
        moveButton.transform.position = startPoint;
        background.transform.position = startPoint;
        
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        endPoint = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touchStart = false;
    }

    private void Update()
    {
        touchStart = Input.GetMouseButton(0);
        if (touchStart)
        {
            onDisplay = true;
            Vector2 offset = endPoint - startPoint;
            direction = Vector2.ClampMagnitude(offset, 100f);
            moveButton.transform.position = (Vector3)(startPoint + direction);
        }
        else
        {
            direction = new Vector2(0f, 0f);
            background.transform.position = initialPosition;
            moveButton.transform.position = initialPosition;
            startPoint = initialPosition;
            endPoint = initialPosition;
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
                onDisplay = false;
        }
        SpawnJoystick();
    }

    private float Norm(Vector2 v) => (float)Math.Sqrt(v.x * v.x + v.y * v.y);

    private void  SpawnJoystick()
    {
        if (onDisplay)
        {
            if (background.color.a < 1)
            {
                background.color = new Color(255, 255, 255, background.color.a + 0.5f * Time.deltaTime);
                moveButton.color = new Color(255, 255, 255, moveButton.color.a + 0.5f * Time.deltaTime);
            }
            else
            {
                background.color = new Color(255, 255, 255, 1);
                moveButton.color = new Color(255, 255, 255, 1);
            }
        }
        else
        {
            if (background.color.a > 0)
            {
                background.color = new Color(255, 255, 255, background.color.a - 0.5f * Time.deltaTime);
                moveButton.color = new Color(255, 255, 255, moveButton.color.a - 0.5f * Time.deltaTime);
            }
            else
            {
                background.color = new Color(255, 255, 255, 0);
                moveButton.color = new Color(255, 255, 255, 0);
            }
        }
    }
}