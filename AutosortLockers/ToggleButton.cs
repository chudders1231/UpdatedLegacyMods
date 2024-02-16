using Common.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace AutosortLockers
{
    public class ToggleButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public bool pointerOver;
        public RectTransform rectTransform;
        public Action onClick = delegate { };

        public void OnPointerClick(PointerEventData eventData)
        {
            if (enabled && eventData.button == PointerEventData.InputButton.Left)
            {
                Toggle toggle = GetComponent<Toggle>();
                toggle.isOn = !toggle.isOn;
            }
        }

        private void Awake()
        {
            rectTransform = transform as RectTransform;
        }

        public void Update()
        {
            var hover = enabled && pointerOver;
            transform.localScale = new Vector3(hover ? 0.15f : 0.1f, hover ? 0.15f : 0.1f, 0.1f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerOver = false;
        }
    }
}
