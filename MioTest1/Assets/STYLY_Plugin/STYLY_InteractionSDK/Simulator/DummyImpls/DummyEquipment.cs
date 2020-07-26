using System;
using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// Equipmentダミー実装のImplSetupper。
    /// </summary>
    public class DummyEquipmentetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as IEquipment;
            var impl = srcComponent.gameObject.AddComponent<DummyEquipment>();
            impl.OnDrop = ifComp.OnDrop;
            impl.OnEquip = ifComp.OnEquip;
            impl.OnUse = ifComp.OnUse;
            impl.OnUseEnd = ifComp.OnUseEnd;
        }
    }
    
    /// <summary>
    /// Equipmentダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummyEquipment : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent OnUse;

        public UnityEvent OnUseEnd;

        public UnityEvent OnEquip;

        public UnityEvent OnDrop;

        private void Update()
        {
            if (isEquipping)
            {
                MovePoisition();
            }
        }

        [SerializeField]
        private bool isEquipping;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (isEquipping)
            {
                if (eventData.button == PointerEventData.InputButton.Right)
                {
                    isEquipping = false;
                    Remove();
                }
                else 
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    OnUse.Invoke();
                }
            }
            else
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    isEquipping = true;
                    Equip();
                }
            }
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (isEquipping)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    OnUseEnd.Invoke();
                }
            }
        }

        private float distanceFromCamera;
        private Vector3 moveTo;
        private void MovePoisition() 
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = distanceFromCamera;

            moveTo = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = moveTo;
            transform.rotation = Camera.main.transform.rotation;
            transform.eulerAngles += new Vector3(-40,0,0 );
        }
        
        
        void Equip()
        {
            Debug.Log("Equip:");
            distanceFromCamera = 1.0f;
            OnEquip.Invoke();
        }
        
        void Remove()
        {
            Debug.Log("Equip Remove:");
            OnDrop.Invoke();
        }
        
    }
}
