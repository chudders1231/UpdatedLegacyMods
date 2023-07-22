using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Mod;
using Common.Utility;
using UnityEngine;
using UnityEngine.UI;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Ingredient = CraftData.Ingredient;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using TMPro;
using Newtonsoft.Json;

namespace AutosortLockers
{
	public class AutosortTarget : MonoBehaviour
	{
		public const int MaxTypes = 7;
		public const float MaxDistance = 3;

		private bool initialized;
		private Constructable constructable;
		private StorageContainer container;
		private AutosortTypePicker picker;
		private CustomizeScreen customizeScreen;
		private Coroutine plusCoroutine;
        private SaveDataEntry saveData;

        [SerializeField]
        private bool isStandingLocker = false;
		[SerializeField]
		private TextMeshProUGUI textPrefab; // Problem child needs to be created before anything else is called.
		[SerializeField]
		private Image background;
		[SerializeField]
		private Image icon;
		[SerializeField]
		private ConfigureButton configureButton;
		[SerializeField]
		private Image configureButtonImage;
		[SerializeField]
		private ConfigureButton customizeButton;
		[SerializeField]
		private Image customizeButtonImage;
		[SerializeField]
		private TextMeshProUGUI text;
		[SerializeField]
		private TextMeshProUGUI label;
		[SerializeField]
		private TextMeshProUGUI plus;
		[SerializeField]
		private TextMeshProUGUI quantityText;
		[SerializeField]
		private List<AutosorterFilter> currentFilters = new List<AutosorterFilter>();

		private void Awake()
		{
			constructable = GetComponent<Constructable>();
			container = gameObject.GetComponent<StorageContainer>();
        }

		private void Start()
		{
            if (!initialized && constructable._constructed && transform.parent != null)
            {
                Initialize();
            }
        }

		public void SetPicker(AutosortTypePicker picker)
		{
			this.picker = picker;

            if (!isStandingLocker)
            {
                picker.transform.localPosition = new Vector3(0.0f, -0.4f, 0.4f);
            }
        }

		public List<AutosorterFilter> GetCurrentFilters()
		{
			return currentFilters;
		}

		public void AddFilter(AutosorterFilter filter, PickerButton button)
		{
			if (currentFilters.Count >= AutosortTarget.MaxTypes)
			{
				return;
			}
			if (ContainsFilter(filter))
			{
				return;
			}
			if (AnAutosorterIsSorting())
			{
				return;
			}
            button.gameObject.SetActive(false);
            currentFilters.Add(filter);
			UpdateText();
		}

		private bool ContainsFilter(AutosorterFilter filter)
		{

			foreach (var f in currentFilters)
			{
				if (f.IsSame(filter))
				{
					return true;
				}
			}

			return false;
		}

		public void RemoveFilter(AutosorterFilter filter)
		{
			if (AnAutosorterIsSorting())
			{
				return;
			}
			foreach (var f in currentFilters)
			{
				if (f.IsSame(filter))
				{
					currentFilters.Remove(f);
					break;
				}
			}
			UpdateText();
		}

		private void UpdateText()
		{
			if (text != null)
			{
				if (currentFilters == null || currentFilters.Count == 0)
				{
					text.text = "[Any]";
				}
				else
				{
					string filtersText = string.Join("\n", currentFilters.Select((f) => f.IsCategory() ? "[" + f.GetString() + "]" : f.GetString()).ToArray());
					text.text = filtersText;
				}

				saveData.FilterData = GetNewVersion(currentFilters);
				SettingModified();
            }
		}

		internal void AddItem(Pickupable item)
		{
			container.container.AddItem(item);

			if (plusCoroutine != null)
			{
				StopCoroutine(plusCoroutine);
			}
			plusCoroutine = StartCoroutine(ShowPlus());
		}

		internal bool CanAddItemByItemFilter(Pickupable item)
		{
			bool allowed = IsTypeAllowedByItemFilter(item.GetTechType());
			return allowed && container.container.HasRoomFor(item);
		}

		internal bool CanAddItemByCategoryFilter(Pickupable item)
		{
			bool allowed = IsTypeAllowedByCategoryFilter(item.GetTechType());
			return allowed && container.container.HasRoomFor(item);
		}

		internal bool CanAddItem(Pickupable item)
		{
			bool allowed = CanTakeAnyItem() || IsTypeAllowed(item.GetTechType());
			return allowed && container.container.HasRoomFor(item);
		}

		internal bool CanTakeAnyItem()
		{
			return currentFilters == null || currentFilters.Count == 0;
		}

		internal bool CanAddItems()
		{
			return constructable.constructed;
		}

		internal bool HasCategoryFilters()
		{
			foreach (var filter in currentFilters)
			{
				if (filter.IsCategory())
				{
					return true;
				}
			}
			return false;
		}

		internal bool HasItemFilters()
		{
			foreach (var filter in currentFilters)
			{
				if (!filter.IsCategory())
				{
					return true;
				}
			}
			return false;
		}

		private bool IsTypeAllowedByCategoryFilter(TechType techType)
		{
			foreach (var filter in currentFilters)
			{
				if (filter.IsCategory() && filter.IsTechTypeAllowed(techType))
				{
					return true;
				}
			}

			return false;
		}

		private bool IsTypeAllowedByItemFilter(TechType techType)
		{
			foreach (var filter in currentFilters)
			{
				if (!filter.IsCategory() && filter.IsTechTypeAllowed(techType))
				{
					return true;
				}
			}

			return false;
		}

		private bool IsTypeAllowed(TechType techType)
		{
			foreach (var filter in currentFilters)
			{
				if (filter.IsTechTypeAllowed(techType))
				{
					return true;
				}
			}

			return false;
		}

		private void Update()
		{
            if (!initialized || !constructable._constructed)
			{
				return;
			}

			if (Player.main != null)
			{
				float distSq = (Player.main.transform.position - transform.position).sqrMagnitude;
				bool playerInRange = distSq <= (MaxDistance * MaxDistance);
				configureButton.enabled = playerInRange;
				customizeButton.enabled = playerInRange;

				if (picker != null && picker.isActiveAndEnabled && !playerInRange)
				{
					picker.gameObject.SetActive(false);
				}
				if (customizeScreen != null && customizeScreen.isActiveAndEnabled && !playerInRange)
				{
					customizeScreen.gameObject.SetActive(false);
				}
			}

			container.enabled = ShouldEnableContainer();

			UpdateQuantityText();
		}

		private bool AnAutosorterIsSorting()
		{
			var root = GetComponentInParent<SubRoot>();
			if (root != null && root.isBase)
			{
				var autosorters = root.GetComponentsInChildren<AutosortLocker>();
				foreach (var autosorter in autosorters)
				{
					if (autosorter.IsSorting)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool ShouldEnableContainer()
		{
			return (picker == null || !picker.isActiveAndEnabled)
				&& (customizeScreen == null || !customizeScreen.isActiveAndEnabled)
				&& (!configureButton.pointerOver || !configureButton.enabled)
				&& (!customizeButton.pointerOver || !customizeButton.enabled);
		}

		internal void ShowConfigureMenu()
		{
			foreach (var otherPicker in GameObject.FindObjectsOfType<AutosortTarget>())
			{
				otherPicker.HideAllMenus();
			}
			picker.gameObject.SetActive(true);
		}

		internal void ShowCustomizeMenu()
		{
			foreach (var otherPicker in GameObject.FindObjectsOfType<AutosortTarget>())
			{
				otherPicker.HideAllMenus();
			}
			customizeScreen.gameObject.SetActive(true);
		}

		internal void HideConfigureMenu()
		{
			if (picker != null)
			{
				picker.gameObject.SetActive(false);
			}
		}

		internal void HideCustomizeMenu()
		{
			if (customizeScreen != null)
			{
				customizeScreen.gameObject.SetActive(false);
			}
		}

		internal void HideAllMenus()
		{
			if (initialized)
			{
				HideConfigureMenu();
				HideCustomizeMenu();
			}
		}

		private void Initialize()
		{
			background.gameObject.SetActive(true);
			icon.gameObject.SetActive(true);
			text.gameObject.SetActive(true);

            background.sprite = ImageUtils.TextureToSprite(Utilities.GetTexture("LockerScreen"));
            icon.sprite = ImageUtils.TextureToSprite(Utilities.GetTexture("Receptacle"));

			configureButtonImage.sprite = ImageUtils.TextureToSprite(Utilities.GetTexture("Configure"));
			customizeButtonImage.sprite = ImageUtils.TextureToSprite(Utilities.GetTexture("Edit"));

			configureButton.onClick = ShowConfigureMenu;
			customizeButton.onClick = ShowCustomizeMenu;

			saveData = GetSaveData();

            InitializeFromSaveData();

			InitializeFilters();

			UpdateText();

            UWE.CoroutineHost.StartCoroutine(CreateCustomizeScreen(background, saveData));

            CreatePicker();

            initialized = true;

        }

		private void InitializeFromSaveData()
		{
			AutosortLogger.Log("Object Initialized from Save Data");
			label.text = saveData.Label;
			label.color = saveData.LabelColor.ToColor();
			icon.color = saveData.IconColor.ToColor();
			configureButtonImage.color = saveData.ButtonsColor.ToColor();
			customizeButtonImage.color = saveData.ButtonsColor.ToColor();
			text.color = saveData.OtherTextColor.ToColor();
			quantityText.color = saveData.ButtonsColor.ToColor();
            SetLockerColor(saveData.LockerColor.ToColor());
		}

		private void SetLockerColor(Color color)
		{
			var meshRenderers = GetComponentsInChildren<MeshRenderer>();
			foreach (var meshRenderer in meshRenderers)
			{
				meshRenderer.material.color = color;
			}
		}

		private SaveDataEntry GetSaveData()
		{
			var prefabIdentifier = GetComponent<PrefabIdentifier>();
			var id = prefabIdentifier.id;

			return Mod.GetSaveData(id);
		}

		private void InitializeFilters()
		{
			if (saveData == null)
			{
				currentFilters = new List<AutosorterFilter>();
				return;
			}

			currentFilters = GetNewVersion(saveData.FilterData);
		}

		private List<AutosorterFilter> GetNewVersion(List<AutosorterFilter> filterData)
		{
			Dictionary<string, AutosorterFilter> validItems = new Dictionary<string, AutosorterFilter>();
			Dictionary<string, AutosorterFilter> validCategories = new Dictionary<string, AutosorterFilter>();
			var filterList = AutosorterList.GetFilters();
			foreach (var filter in filterList)
			{
				if (filter.IsCategory())
				{
					validCategories[filter.Category] = filter;
				}
				else
				{
					validItems[filter.Types[0]] = filter;
				}
			}

			var newData = new List<AutosorterFilter>();
			foreach (var filter in filterData)
			{
				if (validCategories.ContainsKey(filter.Category) || filter.Category == "")
				{
					newData.Add(filter);
					continue;
				}

				if (filter.Category == "0")
				{
					filter.Category = "";
					newData.Add(filter);
					continue;
				}

				var newTypes = AutosorterList.GetOldFilter(filter.Category, out bool success, out string newCategory);
				if (success)
				{
					newData.Add(new AutosorterFilter() { Category = newCategory, Types = newTypes });
					continue;
				}

				newData.Add(filter);
			}
			return newData;
		}

		private void CreatePicker()
		{
			SetPicker(AutosortTypePicker.Create(transform, textPrefab));

            picker.Initialize(this);
			picker.gameObject.SetActive(false);
		}

		private void SettingModified() {

			var id = this.GetComponent<PrefabIdentifier>().id;

            Mod.saveData.Entries[id] = saveData;

			InitializeFromSaveData();
        }

		private IEnumerator CreateCustomizeScreen(Image background, SaveDataEntry saveData)
		{
			TaskResult<CustomizeScreen> result = new TaskResult<CustomizeScreen>();
			yield return CustomizeScreen.Create(background.transform, saveData, result);
			customizeScreen = result.Get();

            customizeScreen.onModified += SettingModified;
            customizeScreen.Initialize(saveData);
            customizeScreen.gameObject.SetActive(false);
		}

		public IEnumerator ShowPlus()
		{
			plus.color = new Color(plus.color.r, plus.color.g, plus.color.b, 1);
			float t = 0;
			float rate = 0.5f;
			while (t < 1.0)
			{
				t += Time.deltaTime * rate;
				plus.color = new Color(plus.color.r, plus.color.g, plus.color.b, Mathf.Lerp(1, 0, t));
				yield return null;
			}
		}

		private void UpdateQuantityText()
		{
			var count = container.container.count;
			quantityText.text = count == 0 ? "empty" : count.ToString();
		}

		internal class AutosortTargetBuildable
		{
			public static PrefabInfo Info { get; private set; }

			public static void Patch()
			{
				Info = Utilities.CreateBuildablePrefabInfo(
					"AutosortTarget", 
					"Autosort Receptacle", 
					"Wall locker linked to an Autosorter that receives sorted items.", 
					Utilities.GetSprite("AutosortTarget"));

				var customPrefab = new CustomPrefab(Info);
				var clonePrefab = new CloneTemplate(Info, TechType.SmallLocker);

				clonePrefab.ModifyPrefab += obj =>
				{
					var TriggerCull = obj.GetComponentInChildren<TriggerCull>();
					var StorageContainer = obj.GetComponent<StorageContainer>();

					StorageContainer.width = AutosortConfig.ReceptacleWidth.Value;
                    StorageContainer.height = AutosortConfig.ReceptacleHeight.Value;

                    StorageContainer.container.Resize(AutosortConfig.ReceptacleWidth.Value, AutosortConfig.ReceptacleHeight.Value);

					var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
					foreach ( var meshRenderer in meshRenderers )
					{
						meshRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);
					}

					var canvas = LockerPrefabShared.CreateCanvas(obj.transform);
					var autosortTarget = obj.AddComponent<AutosortTarget>();
                    autosortTarget.isStandingLocker = false;
                    autosortTarget.background = LockerPrefabShared.CreateBackground(canvas.transform);

					TriggerCull.objectToCull = canvas.gameObject;

					var tPrefab = obj.GetComponentInChildren<TextMeshProUGUI>();


                    tPrefab.transform.parent = canvas.transform;
					tPrefab.gameObject.SetActive(false);

                    autosortTarget.textPrefab = tPrefab;
					var label = obj.FindChild("Label");
					DestroyImmediate(label);

					autosortTarget.icon = LockerPrefabShared.CreateIcon(autosortTarget.background.transform, autosortTarget.textPrefab.color, 70);
					autosortTarget.text = LockerPrefabShared.CreateText(autosortTarget.background.transform, autosortTarget.textPrefab, autosortTarget.textPrefab.color, -20, 10, "Any");

					autosortTarget.label = LockerPrefabShared.CreateText(autosortTarget.background.transform, autosortTarget.textPrefab, autosortTarget.textPrefab.color, 100, 12, "Locker");

					autosortTarget.background.gameObject.SetActive(false);
					autosortTarget.icon.gameObject.SetActive(false);
					autosortTarget.text.gameObject.SetActive(false);

					autosortTarget.plus = LockerPrefabShared.CreateText(autosortTarget.background.transform, autosortTarget.textPrefab, autosortTarget.textPrefab.color, 0, 30, "+");
					autosortTarget.plus.color = new Color(autosortTarget.textPrefab.color.r, autosortTarget.textPrefab.color.g, autosortTarget.textPrefab.color.g, 0);
					autosortTarget.plus.rectTransform.anchoredPosition += new Vector2(30, 70);

					autosortTarget.quantityText = LockerPrefabShared.CreateText(autosortTarget.background.transform, autosortTarget.textPrefab, autosortTarget.textPrefab.color, 0, 8, "XX");
					autosortTarget.quantityText.rectTransform.anchoredPosition += new Vector2(-32, -104);

					autosortTarget.configureButton = ConfigureButton.Create(autosortTarget.background.transform, autosortTarget.textPrefab.color, 40);
					autosortTarget.configureButtonImage = autosortTarget.configureButton.GetComponent<Image>();

					autosortTarget.customizeButton = ConfigureButton.Create(autosortTarget.background.transform, autosortTarget.textPrefab.color, 20);
					autosortTarget.customizeButtonImage = autosortTarget.customizeButton.GetComponent<Image>();
				};

				var recipe = new RecipeData
				{
					craftAmount = 1,
					Ingredients = AutosortConfig.EasyBuild.Value
                    ? new List<Ingredient>
                    {
                        new Ingredient(TechType.Titanium, 2)
                    }
                    : new List<Ingredient>
                    {
                        new Ingredient(TechType.Titanium, 2),
                        new Ingredient(TechType.Magnetite, 1)
                    }
                };

				customPrefab.SetGameObject(clonePrefab);
				customPrefab.SetRecipe(recipe);
				customPrefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

				customPrefab.Register();
			}
		}

        internal class AutosortStandingTargetBuildable
        {
			public static PrefabInfo Info;
			private static TextMeshProUGUI textPrefab;

            public static void Patch()
            {
                Info = Utilities.CreateBuildablePrefabInfo(
                    "AutosortTargetStanding",
                    "Standing Autosort Receptacle",
                    "Large locker linked to an Autosorter that receives sorted items.", 
					Utilities.GetSprite("AutosortTargetStanding"));
				UWE.CoroutineHost.StartCoroutine(SetTextMeshProPrefab());

                var customPrefab = new CustomPrefab(Info);
                var clonePrefab = new CloneTemplate(Info, TechType.Locker);

				clonePrefab.ModifyPrefab += obj =>
				{
					StorageContainer container = obj.GetComponent<StorageContainer>();
					container.width = AutosortConfig.StandingReceptacleWidth.Value;

					container.height = AutosortConfig.StandingReceptacleHeight.Value;

                    container.container.Resize(AutosortConfig.StandingReceptacleWidth.Value, AutosortConfig.StandingReceptacleHeight.Value);

					var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
					foreach (var meshRenderer in meshRenderers)
					{
						meshRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);
					}

					var autosortTarget = obj.AddComponent<AutosortTarget>();
					autosortTarget.isStandingLocker = true;

					autosortTarget.textPrefab = Instantiate(textPrefab, obj.transform);

                    var canvas = LockerPrefabShared.CreateCanvas(obj.transform);
					canvas.transform.localPosition = new Vector3(0, 1.1f, 0.25f);
					canvas.sortingOrder = 1;

                    autosortTarget.background = LockerPrefabShared.CreateBackground(canvas.transform);
					autosortTarget.icon = LockerPrefabShared.CreateIcon(autosortTarget.background.transform, autosortTarget.textPrefab.color, 70);
					autosortTarget.text = LockerPrefabShared.CreateText(autosortTarget.background.transform, autosortTarget.textPrefab, autosortTarget.textPrefab.color, -20, 10, "Any");

					autosortTarget.label = LockerPrefabShared.CreateText(autosortTarget.background.transform, autosortTarget.textPrefab, autosortTarget.textPrefab.color, 100, 12, "Locker");

					autosortTarget.background.gameObject.SetActive(false);
					autosortTarget.icon.gameObject.SetActive(false);
					autosortTarget.text.gameObject.SetActive(false);

					autosortTarget.plus = LockerPrefabShared.CreateText(autosortTarget.background.transform, autosortTarget.textPrefab, autosortTarget.textPrefab.color, 0, 30, "+");
					autosortTarget.plus.color = new Color(autosortTarget.textPrefab.color.r, autosortTarget.textPrefab.color.g, autosortTarget.textPrefab.color.g, 0);
					autosortTarget.plus.rectTransform.anchoredPosition += new Vector2(30, 70);

					autosortTarget.quantityText = LockerPrefabShared.CreateText(autosortTarget.background.transform, autosortTarget.textPrefab, autosortTarget.textPrefab.color, 0, 10, "XX");
					autosortTarget.quantityText.rectTransform.anchoredPosition += new Vector2(-32, -104);

					autosortTarget.configureButton = ConfigureButton.Create(autosortTarget.background.transform, autosortTarget.textPrefab.color, 40);
					autosortTarget.configureButtonImage = autosortTarget.configureButton.GetComponent<Image>();
					autosortTarget.customizeButton = ConfigureButton.Create(autosortTarget.background.transform, autosortTarget.textPrefab.color, 20);
					autosortTarget.customizeButtonImage = autosortTarget.customizeButton.GetComponent<Image>();
				};

                var recipe = new RecipeData
                {
                    craftAmount = 1,
                    Ingredients = AutosortConfig.EasyBuild.Value
                    ? new List<Ingredient>
                    {
                        new Ingredient(TechType.Titanium, 2),
                        new Ingredient(TechType.Quartz, 1)
                    }
                    : new List<Ingredient>
                    {
                        new Ingredient(TechType.Titanium, 2),
                        new Ingredient(TechType.Quartz, 1),
                        new Ingredient(TechType.Magnetite, 1)
                    }
                };

                customPrefab.SetGameObject(clonePrefab);
                customPrefab.SetRecipe(recipe);
                customPrefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);
				customPrefab.Register();
            }

			public static IEnumerator SetTextMeshProPrefab()
			{
				CoroutineTask<GameObject> task = CraftData.GetBuildPrefabAsync(TechType.SmallLocker);
				yield return task;
				GameObject prefab = task.GetResult();
				textPrefab = prefab.GetComponentInChildren<TextMeshProUGUI>();
			}
        }
	}
}
