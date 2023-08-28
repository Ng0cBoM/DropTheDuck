using UnityEngine;
using UnityEngine.EventSystems;
namespace Amas.Core.UI {
    public abstract class PopupScreen : ScreenItem, IPointerDownHandler {
        public RectTransform panel;
        public void OnPointerDown(PointerEventData eventData) {
            if (panel.gameObject.activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(
                     panel.GetComponent<RectTransform>(),
                     Input.mousePosition,
                     Camera.main)) {
                UiManager.I.Pop();
            }
        }
    }
}