using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AutosortLockers
{
	public class PickerPageButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		private static readonly Color DisabledColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
		private static readonly Color NormalColor = new Color(0.7f, 0.7f, 0.7f, 1f);
		private static readonly Color HoverColor = Color.white;

		public bool canChangePage;
		public bool pointerOver;
		public AutosortTypePicker target;
		public int pageOffset;
		public Image image;

		public void Awake()
		{
			image = GetComponent<Image>();
		}
		
		public void OnPointerClick(PointerEventData eventData)
		{
			if (canChangePage && eventData.button == PointerEventData.InputButton.Left)
			{
				target.ChangePage(pageOffset);
			}
		}

		public void Update()
		{
			if (canChangePage)
			{
				image.color = pointerOver ? HoverColor : NormalColor;
				transform.localScale = pointerOver ? new Vector3(1.2f, 1.2f, 1) : new Vector3(1f, 1f, 1); 
			} else
			{
				image.color = DisabledColor;
				transform.localScale = new Vector3( 1, 1, 1);
			}
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
