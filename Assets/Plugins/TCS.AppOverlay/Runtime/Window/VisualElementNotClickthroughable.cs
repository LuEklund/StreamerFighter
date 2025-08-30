using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace TCS {
    public class VisualElementNotClickthroughable : MonoBehaviour {
        UIDocument m_document;
        VisualElement m_root;

        [Header("Element tags to make not clickthroughable")]
        public List<string> m_elementTags = new();
        readonly List<VisualElement> m_childrenNotClickthroughable = new();

        void Awake() {
            m_document = GetComponent<UIDocument>();
            m_root = m_document.rootVisualElement;
        }

        void OnEnable() {
            //root.RegisterCallback<MouseEnterEvent>(_ => OnMouseEnter());
            //root.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

            foreach (string s in m_elementTags) {
                var element = m_root.Q<VisualElement>(s);
                if (element == null) continue;
                element.RegisterCallback<MouseEnterEvent>(_ => OnMouseEnter());
                element.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
                m_childrenNotClickthroughable.Add(element);
            }
        }

        void OnDisable() {
            //root.UnregisterCallback<MouseEnterEvent>(_ => OnMouseEnter());
            //root.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);

            foreach (var element in m_childrenNotClickthroughable) {
                element.UnregisterCallback<MouseEnterEvent>(_ => OnMouseEnter());
                element.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
            }

            m_childrenNotClickthroughable.Clear();
        }

        void OnMouseEnter() {
#if !UNITY_EDITOR
            TransparentWindowEvents.OnForceClickThrough?.Invoke();
#endif
            Debug.Log("OnForceNotClickThrough event invoked");
        }

        void OnMouseLeave(MouseLeaveEvent evt) {
#if !UNITY_EDITOR
            TransparentWindowEvents.OnForceNotClickThrough?.Invoke();
#endif
            Debug.Log("OnForceClickThrough event invoked");
        }
    }
}