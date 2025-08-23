using UnityEngine;
using UnityEngine.EventSystems;
namespace OverlayCore.Window {
    public class Clickthroughable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        void OnMouseEnter() {
            Debug.Log("OnMouseEnter called");
            TransparentWindowEvents.OnForceNotClickThrough?.Invoke();
            Debug.Log("OnForceNotClickThrough event invoked");
        }

        void OnMouseExit() {
            Debug.Log("OnMouseExit called");
            TransparentWindowEvents.OnForceClickThrough?.Invoke();
            Debug.Log("OnForceClickThrough event invoked");
        }

        public void OnPointerEnter(PointerEventData eventData) {
            Debug.Log("OnPointerEnter called");
            TransparentWindowEvents.OnForceNotClickThrough?.Invoke();
            Debug.Log("OnForceNotClickThrough event invoked");
        }

        public void OnPointerExit(PointerEventData eventData) {
            Debug.Log("OnPointerExit called");
            TransparentWindowEvents.OnForceClickThrough?.Invoke();
            Debug.Log("OnForceClickThrough event invoked");
        }
    }
}