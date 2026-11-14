using UnityEngine;
using UnityEngine.EventSystems;

namespace Afterlight
{
    public sealed class OnScreenJoystick : MonoBehaviour,
        IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform background;
        public RectTransform handle;
        public float radius = 72f;

        public Vector2 Direction { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!background || !handle) return;
            Vector2 local;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, eventData.pressEventCamera, out local))
                return;

            Direction = Vector2.ClampMagnitude(local / Mathf.Max(1f, radius), 1f);
            handle.anchoredPosition = Direction * radius;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Direction = Vector2.zero;
            if (handle) handle.anchoredPosition = Vector2.zero;
        }
    }
}
