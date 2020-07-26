using System;
using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// Draggableダミー実装のImplSetupper。
    /// </summary>
    public class DummyDraggableSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as IDraggable;
            var impl = srcComponent.gameObject.AddComponent<DummyDraggable>();
            impl.OnBeginDragging = ifComp.OnBeginDragging;
            impl.OnEndDragging = ifComp.OnEndDragging;
        }
    }
    
    /// <summary>
    /// Draggableダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummyDraggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent OnBeginDragging;
        public UnityEvent OnEndDragging;

        private void Update()
        {
            if (isDragging)
            {
                MovePoisition();
            }
        }

        [SerializeField]
        private bool isDragging;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            distanceFromCamera = Vector3.Distance(Camera.main.transform.position, this.transform.position);
            OnBeginDragging.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            OnEndDragging.Invoke();
        }

        private const float duration = 0.1f;
        private float distanceFromCamera;
        private Vector3 moveTo;
        private void MovePoisition() {

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = distanceFromCamera;

            moveTo = Camera.main.ScreenToWorldPoint(mousePos);

            var rb = gameObject.GetComponent<Rigidbody>();
            if (rb)
            {
                SetRigidbodyPosition(rb, moveTo, duration);
                SetRigidobodyRotation(rb);
            }
            else
            {
                transform.position = moveTo;
                transform.rotation = Camera.main.transform.rotation;
            }
            
        }

        void SetRigidbodyPosition(Rigidbody rb, Vector3 moveTo, float duration)
        {
            var diffPos = moveTo - gameObject.transform.position;
            if (Mathf.Approximately(diffPos.sqrMagnitude, 0f))
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.velocity = diffPos / duration;
            }
        }

        void SetRigidobodyRotation(Rigidbody rb)
        {
            rb.angularVelocity = Vector3.zero;
        }
    }
}