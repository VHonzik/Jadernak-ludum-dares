using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(Button))]
public class TextualButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler,IPointerExitHandler
{
    public Color hover = Color.red;
    public Color click = Color.green;
    public Color normal = new Color();
    public Color disabled = Color.grey;

    public Events.RewardEvent resultEvent;

    private bool clicked = false;
    private bool canClick = true;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(canClick && !clicked) GetComponentInChildren<Text>().color = hover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(canClick && !clicked) GetComponentInChildren<Text>().color = normal;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canClick && resultEvent != null)
        {
            GetComponentInChildren<Text>().color = click;
            clicked = true;

            Events.EventManager.Instance.ResultOfEvent(resultEvent);
        }
    }

    void Update()
    {
        if (resultEvent != null)
        {
            canClick = resultEvent.Affordable();
            if (!canClick) GetComponentInChildren<Text>().color = disabled;
        }
    }

}
