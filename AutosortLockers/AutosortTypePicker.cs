using System;
using System.Collections.Generic;
using System.Linq;
using Common.Mod;
using Common.Utility;
using Nautilus.Utility;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AutosortLockers
{
	public class AutosortTypePicker : MonoBehaviour
	{
		private enum Mode { Categories, Items }

		private Mode currentMode = Mode.Categories;
		private int currentPageCategories = 0;
		private int currentPageItems = 0;
		private List<AutosorterFilter> availableTypes;

		[SerializeField]
		private AutosortTarget locker;
		[SerializeField]
		private PickerButton[] currentList = new PickerButton[AutosortTarget.MaxTypes];
		[SerializeField]
		private PickerButton[] availableList = new PickerButton[AutosortTarget.MaxTypes];
		[SerializeField]
		private RawImage background;
		[SerializeField]
		private PickerCloseButton closeButton;
		[SerializeField]
		private TextMeshProUGUI pageText;
		[SerializeField]
		private PickerPageButton prevPageButton;
		[SerializeField]
		private PickerPageButton nextPageButton;
		[SerializeField]
		private PickerButton categoriesTabButton;
		[SerializeField]
		private PickerButton itemsTabButton;

		[SerializeField]
		private VerticalLayoutGroup currentFilterContainer;
	    
		[SerializeField]
        private VerticalLayoutGroup availableFilterContainer;

        public void Initialize(AutosortTarget locker)
		{
			this.locker = locker;
			closeButton.target = locker;

			RefreshCurrentFilters();
			UpdateAvailableTypes();
		}

		private void RefreshCurrentFilters()
		{
			List<AutosorterFilter> filters = locker.GetCurrentFilters();
			int i = 0;
			foreach (var filter in filters)
			{
				currentList[i].SetFilter(filter);
				i++;
			}
			while (i < AutosortTarget.MaxTypes)
			{
				currentList[i].SetFilter(null);
				i++;
			}

        }

		private void refreshHiddenFilters()
		{
            foreach (var availableFilter in availableList)
            {
                foreach (var currentFilter in currentList)
                {
					if (currentFilter?.filter?.GetString() == null || availableFilter?.filter?.GetString() == null) continue;

                    if (currentFilter.filter.GetString() == availableFilter.filter.GetString())
                    {
                        availableFilter.gameObject.SetActive(false);
                    }
                }
            }
        }

        private List<AutosorterFilter> GetAvailableTypes()
		{
			if (currentMode == Mode.Categories)
			{
				return AutosorterList.GetFilters().Where((e) => e.IsCategory()).ToList();
			}
			else
			{
				return AutosorterList.GetFilters().Where((e) => !e.IsCategory()).ToList();
			}
		}

		private void UpdateAvailableTypes()
		{
			availableTypes = GetAvailableTypes();
			int start = GetCurrentPage() * AutosortTarget.MaxTypes;
			for (int i = 0; i < AutosortTarget.MaxTypes; ++i)
			{
				var filter = (start + i) >= availableTypes.Count ? null : availableTypes[start + i];
				availableList[i].SetFilter(filter);
			}
            pageText.text = string.Format("{0}/{1}", GetCurrentPage() + 1, GetCurrentPageCount());
			prevPageButton.canChangePage = (GetCurrentPage() > 0);
			nextPageButton.canChangePage = (GetCurrentPage() + 1) < GetCurrentPageCount();

			categoriesTabButton.SetTabActive(currentMode == Mode.Categories);
			itemsTabButton.SetTabActive(currentMode == Mode.Items);
			refreshHiddenFilters();
        }

		public void OnCurrentListItemClick(AutosorterFilter filter, PickerButton button)
		{
			if (filter == null)
			{
				return;
			}

			locker.RemoveFilter(filter);
			GetPickerButtonFromFilter(filter)?.gameObject.SetActive(true);
            RefreshCurrentFilters();
        }

		public PickerButton GetPickerButtonFromFilter(AutosorterFilter filter)
		{
			foreach (var target in availableList)
			{
				if( target.filter == filter)
				{
					return target;
				}
			}
			return null;
		}

		public void OnAvailableListItemClick(AutosorterFilter filter, PickerButton button)
		{
			if (filter == null)
			{
				return;
			}

			locker.AddFilter(filter, button);
			RefreshCurrentFilters();
        }

		internal void ChangePage(int pageOffset)
		{
			var pageCount = GetCurrentPageCount();
			SetCurrentPage(Mathf.Clamp(GetCurrentPage() + pageOffset, 0, pageCount - 1));
			UpdateAvailableTypes();
        }

        private int GetCurrentPageCount()
		{
			return (int)Mathf.Ceil((float)availableTypes.Count / AutosortTarget.MaxTypes);
		}

		private void OnCategoriesButtonClick(AutosorterFilter obj, PickerButton b)
		{
			if (currentMode != Mode.Categories)
			{
				currentMode = Mode.Categories;
				UpdateAvailableTypes();
            }
		}

		private void OnItemsButtonClick(AutosorterFilter obj, PickerButton b)
		{
			if (currentMode != Mode.Items)
			{
				currentMode = Mode.Items;
				UpdateAvailableTypes();
            }
		}
		
		private int GetCurrentPage()
		{
			return currentMode == Mode.Categories ? currentPageCategories : currentPageItems;
		}

		private void SetCurrentPage(int page)
		{
			if (currentMode == Mode.Categories)
			{
				currentPageCategories = page;
			}
			else
			{
				currentPageItems = page;
			}
        }

		public static AutosortTypePicker Create(Transform parent, TextMeshProUGUI textPrefab)
		{
			var pickermenu = GameObject.Instantiate(Mod.pickerMenuBundle.LoadAsset<GameObject>("PickerMenu"));
			pickermenu.transform.SetParent(parent, false);
            pickermenu.transform.localPosition = new Vector3(0f, 0.6f, 0.25f);
			pickermenu.transform.localEulerAngles = new Vector3( 0, 180f, 0);
			pickermenu.GetComponentInChildren<Canvas>().sortingOrder = 2;
            pickermenu.gameObject.SetActive(false);

            pickermenu.AddComponent<AutosortTypePicker>();
            var pickerComp = pickermenu.GetComponent<AutosortTypePicker>();
            var pickerCanvas = pickermenu.FindChild("Canvas").GetComponent<Canvas>();

			pickerComp.background = pickerCanvas.gameObject.FindChild("Background").GetComponent<RawImage>();

			var currentText = pickerComp.background.gameObject.FindChild("Current");
			currentText.GetComponent<TextMeshProUGUI>().font = FontUtils.Aller_Rg;

            pickerComp.categoriesTabButton = pickerComp.background.gameObject.FindChild("CategoryButton").AddComponent<PickerButton>();
			pickerComp.categoriesTabButton.SetButton(pickerComp.OnCategoriesButtonClick);
            pickerComp.categoriesTabButton.Override("Categories", true);

            pickerComp.itemsTabButton = pickerComp.background.gameObject.FindChild("ItemsButton").AddComponent<PickerButton>();
            pickerComp.itemsTabButton.SetButton(pickerComp.OnItemsButtonClick);
            pickerComp.itemsTabButton.Override("Items", false);

            pickerComp.pageText = pickerComp.background.gameObject.FindChild("PageText").GetComponent<TextMeshProUGUI>();
			pickerComp.pageText.font = FontUtils.Aller_Rg;

            pickerComp.prevPageButton = pickerComp.background.gameObject.FindChild("LeftArrowButton").AddComponent<PickerPageButton>();
			pickerComp.prevPageButton.target = pickerComp;
			pickerComp.prevPageButton.pageOffset = -1;

            pickerComp.nextPageButton = pickerComp.background.gameObject.FindChild("RightArrowButton").AddComponent<PickerPageButton>();
            pickerComp.nextPageButton.target = pickerComp;
            pickerComp.nextPageButton.pageOffset = +1;

            pickerComp.closeButton = pickerComp.background.gameObject.FindChild("CloseButton").AddComponent<PickerCloseButton>();

			pickerComp.currentFilterContainer = pickerComp.background.gameObject.FindChild("CurrentFilterList").GetComponent<VerticalLayoutGroup>();
            pickerComp.availableFilterContainer = pickerComp.background.gameObject.FindChild("AvailableFilterList").GetComponent<VerticalLayoutGroup>();

            for (int i = 0; i < AutosortTarget.MaxTypes; ++i)
			{
				var cpb = pickerComp.currentFilterContainer.transform.GetChild(i);
				cpb.gameObject.AddComponent<PickerButton>();
				cpb.gameObject.GetComponent<PickerButton>().SetButton(pickerComp.OnCurrentListItemClick);
				cpb.gameObject.GetComponentInChildren<TextMeshProUGUI>().font = FontUtils.Aller_Rg;
				pickerComp.currentList[i] = cpb.GetComponent<PickerButton>();

                var apb = pickerComp.availableFilterContainer.transform.GetChild(i);
				apb.gameObject.AddComponent<PickerButton>();
                apb.gameObject.GetComponent<PickerButton>().SetButton(pickerComp.OnAvailableListItemClick);
                apb.gameObject.GetComponentInChildren<TextMeshProUGUI>().font = FontUtils.Aller_Rg;
                pickerComp.availableList[i] = apb.GetComponent<PickerButton>();
			}

            return pickerComp;
		}
    }
}
