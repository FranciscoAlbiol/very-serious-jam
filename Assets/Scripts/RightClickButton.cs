using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RightClickButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Right Click Event")]
    public UnityEvent onRightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            onRightClick?.Invoke();
        }
    }
}
