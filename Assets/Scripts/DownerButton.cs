using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class DownerButton : Button
{
    public event Action onPointerDown;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (onPointerDown != null && interactable)
        {
            onPointerDown();
        }
    }
}