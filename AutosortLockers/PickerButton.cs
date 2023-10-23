using Common.Mod;
using Nautilus.Utility;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AutosortLockers
{
	public class PickerButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		private static readonly Color inactiveColor = new Color(0.8f, 0.8f, 0.8f, 1f);
		private static readonly Color inactiveHoverColor = new Color(1f, 1f, 1f, 1f);
		private static readonly Color upColor = new Color(0.85f, 0.85f, 0.85f, 1f);
		private static readonly Color hoverColor = new Color(1f, 1f, 1f, 1f);

		private bool hover;
		private bool tabActive = true;
		internal AutosorterFilter filter;

		public Action<AutosorterFilter,PickerButton> onClick = delegate { };

		[SerializeField]
		private RawImage background;
		[SerializeField]
		private TextMeshProUGUI text;

		public AutosorterFilter GetTechType()
		{
			return filter;
		}

		public void Override(string text, bool category)
		{
			filter = null;
			this.text.text = text;
			SetBackgroundSprite(category);
			gameObject.SetActive(true);
		}

		public void SetFilter(AutosorterFilter value)
		{
			filter = value;
			if (filter != null)
			{
				text.text = filter.GetString();
				SetBackgroundSprite(filter.IsCategory());
			}

			gameObject.SetActive(filter != null);
		}

		private void SetBackgroundSprite(bool category)
		{
			if (background != null)
			{
				var spriteName = category ? "MainMenuButton" : "MainMenuButtonStandard";
				background.texture = Utilities.GetTexture(Mod.GetAssetPath(spriteName));
			}
		}

		public void Update()
		{
			if (background != null)
			{
				if (tabActive)
				{
					background.color = hover ? hoverColor : upColor;
					background.transform.localScale = hover ? new Vector3(1.05f, 1.05f, 1f) : new Vector3(1.0f, 1.0f, 1.0f);
				}
				else
				{
					background.color = hover ? inactiveHoverColor : inactiveColor;
                    background.transform.localScale = hover ? new Vector3(1.05f, 1.05f, 1f) : new Vector3(1.0f, 1.0f, 1.0f);
                }
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
                onClick.Invoke(filter, this);
				hover = false;
            }
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			hover = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			hover = false;
		}

		public void SetTabActive(bool active)
		{
			tabActive = active;
		}

		public void SetButton(Action<AutosorterFilter,PickerButton> action)
		{
			text = GetComponentInChildren<TextMeshProUGUI>();
			text.font = FontUtils.Aller_Rg;

            background = GetComponent<RawImage>();
			background.texture = Utilities.GetTexture(Mod.GetAssetPath("MainMenuButton"));
            onClick += action;
		}
	}
}
