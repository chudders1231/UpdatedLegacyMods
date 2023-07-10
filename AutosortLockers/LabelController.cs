using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AutosortLockers
{
	class LabelController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		private bool hover;

		public RectTransform rectTransform;
		public Action<string> onModified = delegate { };
		public TextMeshProUGUI text;

		[SerializeField]
		private SaveDataEntry target;

		private void Awake()
		{
			rectTransform = transform as RectTransform;
		}

		private void Initialize(SaveDataEntry data, TextMeshProUGUI textPrefab)
		{
			target = data;

			text = GameObject.Instantiate(textPrefab);
			text.fontSize = 16;
			text.gameObject.name = "Text";
			text.rectTransform.SetParent(transform, false);
			RectTransformExtensions.SetSize(text.rectTransform, 140, 50);

			text.text = target.Label;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
                uGUI.main.userInput.RequestString("Label", "Submit", target.Label, 25, new uGUI_UserInput.UserInputCallback(SetLabel));
            }
		}

		public void SetLabel(string newLabel)
		{
			target.Label = newLabel;
			text.text = newLabel;

			onModified(newLabel);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			hover = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			hover = false;
		}

		private void Update()
		{
			if (hover)
			{
				HandReticle.main.SetIcon(HandReticle.IconType.Rename);
				HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "");
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		public static LabelController Create(SaveDataEntry data, Transform parent, TextMeshProUGUI textPrefabComp)
		{
			var textPrefab = Instantiate(textPrefabComp);
			textPrefab.fontSize = 12;
			textPrefab.color = CustomizeScreen.ScreenContentColor;

			var habitatNameController = new GameObject("LabelController", typeof(RectTransform)).AddComponent<LabelController>();
			var rt = habitatNameController.gameObject.transform as RectTransform;
			RectTransformExtensions.SetParams(rt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), parent);
			habitatNameController.Initialize(data, textPrefab);

			return habitatNameController;
		}
	}
}
