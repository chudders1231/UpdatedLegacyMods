using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Mod;
using UnityEngine;
using UnityEngine.UI;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Ingredient = CraftData.Ingredient;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using TMPro;
using System;
using Nautilus.Handlers;
using FMOD;

namespace AutosortLockers
{
    public class AutosortLocker : MonoBehaviour
    {
        private static readonly Color MainColor = new Color(1f, 0.2f, 0.2f);
        private static readonly Color PulseColor = Color.white;

        private bool initialized;
        private Constructable constructable;
        private StorageContainer container;
        private readonly List<AutosortTarget> singleItemTargets = new List<AutosortTarget>();
        private readonly List<AutosortTarget> categoryTargets = new List<AutosortTarget>();
        private readonly List<AutosortTarget> anyTargets = new List<AutosortTarget>();

        private int unsortableItems = 0;

        [SerializeField]
        private Image background;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private TextMeshProUGUI sortingText;
        [SerializeField]
        private bool sortToggle = true;
        [SerializeField]
        private GameObject sortToggleButton;
        [SerializeField]
        private bool isSorting;
        [SerializeField]
        private bool sortedItem;
        [SerializeField]

        public bool IsSorting => isSorting;

        private void Awake()
        {
            constructable = GetComponent<Constructable>();
            container = GetComponent<StorageContainer>();
            container.hoverText = "Open autosorter";
            container.storageLabel = "Autosorter";
        }

        private void Update()
        {
            if (!initialized && constructable._constructed && transform.parent != null)
            {
                Initialize();
            }

            if (!initialized || !constructable._constructed)
            {
                return;
            }
            UpdateSortToggle();
            UpdateText();
        }

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Mathf.Max(0, AutosortConfig.SortInterval.Value - (unsortableItems / 60.0f)));

                yield return Sort();
            }
        }

        private void UpdateSortToggle()
        {

            if (sortToggleButton)
            {
                Toggle sortToggleComponent = sortToggleButton.GetComponent<Toggle>();
                if (sortToggleComponent.isOn)
                {
                    sortToggle = true;
                }
                else
                {
                    sortToggle = false;
                }
            }
        }

        private void UpdateText()
        {
            string output = "";
            if (sortToggle) {

                output = "Sorting Disabled";

            } else if (isSorting)
            {
                output = "Sorting...";
            }
            else if (unsortableItems > 0)
            {
                output = "Unsorted Items: " + unsortableItems;
            }
            else
            {
                output = "Ready to Sort";
            }

            sortingText.text = output;
        }

        private void Initialize()
        {
            background.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            text.gameObject.SetActive(true);
            sortingText.gameObject.SetActive(true);
            sortToggleButton.SetActive(true);

            background.sprite = Common.Utility.ImageUtils.TextureToSprite(Utilities.GetTexture("LockerScreen"));
            icon.sprite = Common.Utility.ImageUtils.TextureToSprite(Utilities.GetTexture("Sorter"));

            initialized = true;
        }

        private void AccumulateTargets()
        {
            singleItemTargets.Clear();
            categoryTargets.Clear();
            anyTargets.Clear();

            SubRoot subRoot = gameObject.GetComponentInParent<SubRoot>();
            if (subRoot == null)
            {
                return;
            }

            var allTargets = subRoot.GetComponentsInChildren<AutosortTarget>().ToList();
            foreach (var target in allTargets)
            {
                if (target.isActiveAndEnabled && target.CanAddItems())
                {
                    if (target.CanTakeAnyItem())
                    {
                        anyTargets.Add(target);
                    }
                    else
                    {
                        if (target.HasItemFilters())
                        {
                            singleItemTargets.Add(target);
                        }
                        if (target.HasCategoryFilters())
                        {
                            categoryTargets.Add(target);
                        }
                    }
                }
            }
        }
        private IEnumerator Sort()
        {
            sortedItem = false;
            unsortableItems = container.container.count;

            if (!initialized || container.IsEmpty())
            {
                isSorting = false;
                yield break;
            }

            if (sortToggle == false)
            {
                yield break;
            }

            AccumulateTargets();
            if (NoTargets())
            {
                isSorting = false;
                yield break;
            }

            isSorting = true;
            yield return SortFilteredTargets(false);
            if (sortedItem)
            {
                yield break;
            }

            yield return SortFilteredTargets(true);
            if (sortedItem)
            {
                yield break;
            }

            yield return SortAnyTargets();
            if (sortedItem)
            {
                yield break;
            }

            isSorting = false;
        }

        private bool NoTargets()
        {
            return singleItemTargets.Count <= 0 && categoryTargets.Count <= 0 && anyTargets.Count <= 0;
        }

        private IEnumerator SortFilteredTargets(bool byCategory)
        {
            int callsToCanAddItem = 0;
            const int CanAddItemCallThreshold = 10;

            foreach (AutosortTarget target in byCategory ? categoryTargets : singleItemTargets)
            {
                foreach (AutosorterFilter filter in target.GetCurrentFilters())
                {
                    if (filter.IsCategory() == byCategory)
                    {
                        foreach (var techType in filter.Types)
                        {
                            callsToCanAddItem++;
                            if(!TechTypeExtensions.FromString(techType, out TechType tt, true))
                            {
                                continue;
                            }
                            var items = container.container.GetItems(tt);
                            if (items != null && items.Count > 0 && target.CanAddItem(items[0].item))
                            {
                                unsortableItems -= items.Count;
                                SortItem(items[0].item, target);
                                sortedItem = true;
                                yield break;
                            }
                            else if (callsToCanAddItem > CanAddItemCallThreshold)
                            {
                                callsToCanAddItem = 0;
                                yield return null;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator SortAnyTargets()
        {
            int callsToCanAddItem = 0;
            const int CanAddItemCallThreshold = 10;
            foreach (var item in container.container.ToList())
            {
                foreach (AutosortTarget target in anyTargets)
                {
                    callsToCanAddItem++;
                    if (target.CanAddItem(item.item))
                    {
                        SortItem(item.item, target);
                        unsortableItems--;
                        sortedItem = true;
                        yield break;
                    }
                    else if (callsToCanAddItem > CanAddItemCallThreshold)
                    {
                        callsToCanAddItem = 0;
                        yield return null;
                    }
                }
            }
        }

        private void SortItem(Pickupable pickup, AutosortTarget target)
        {
            container.container.RemoveItem(pickup, true);
            target.AddItem(pickup);

            StartCoroutine(PulseIcon());
        }

        public IEnumerator PulseIcon()
        {
            float t = 0;
            float rate = 0.5f;
            while (t < 1.0)
            {
                t += Time.deltaTime * rate;
                icon.color = Color.Lerp(PulseColor, MainColor, t);
                yield return null;
            }
        }

        internal static class AutosortLockerBuildable
        {
            public static PrefabInfo Info { get; private set; }

            public static void Patch()
            {
                Info = Utilities.CreatePrefabInfo(
                    "Autosorter",
                    "Autosort Distributor",
                    "Small, wall-mounted smart-locker that automatically transfers items into linked Autosort Receptacles.",
                    Utilities.GetSprite("AutosortLocker")
                    );

                var customPrefab = new CustomPrefab(Info);
                var clonePrefab = new CloneTemplate(Info, TechType.SmallLocker);

                clonePrefab.ModifyPrefab += obj =>
                {
                    var triggerCull = obj.GetComponentInChildren<TriggerCull>();
                    DestroyImmediate(triggerCull);

                    var label = obj.FindChild("Label");
                    DestroyImmediate(label);

                    var container = obj.GetComponent<StorageContainer>();
                    container.width = AutosortConfig.AutosorterWidth.Value;
                    container.height = AutosortConfig.AutosorterHeight.Value;
                    container.container.Resize(AutosortConfig.AutosorterWidth.Value, AutosortConfig.AutosorterHeight.Value);

                    var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
                    foreach (var meshRenderer in meshRenderers)
                    {
                        meshRenderer.material.color = new Color(1, 0, 0);
                    }

                    var autoSorter = obj.AddComponent<AutosortLocker>();

                    var autosorterui = GameObject.Instantiate(Mod.autosorterBundle.LoadAsset<GameObject>("AutosorterUI"));

                    autosorterui.transform.transform.SetParent(obj.transform, false);
                    autosorterui.transform.localPosition = new Vector3(0f, -0.48f, 0.345f);
                    autosorterui.transform.localEulerAngles = new Vector3(0, 180f, 0);
                    autosorterui.GetComponentInChildren<Canvas>().sortingOrder = 2;

                    autoSorter.background = autosorterui.transform.Find("AutosorterCanvas/Background").GetComponent<Image>();
                    autoSorter.icon = autosorterui.transform.Find("AutosorterCanvas/Background/AutosorterIcon").GetComponent<Image>();
                    autoSorter.text = autosorterui.transform.Find("AutosorterCanvas/Background/AutosorterTitle").GetComponent<TextMeshProUGUI>();
                    autoSorter.sortingText = autosorterui.transform.Find("AutosorterCanvas/Background/SortingText").GetComponent<TextMeshProUGUI>();
                    autoSorter.sortToggleButton = autosorterui.transform.Find("AutosorterCanvas/Background/SortToggle").gameObject;

                    autoSorter.sortToggleButton.EnsureComponent<ToggleButton>();

                    autoSorter.background.gameObject.SetActive(false);
                    autoSorter.icon.gameObject.SetActive(false);
                    autoSorter.text.gameObject.SetActive(false);
                    autoSorter.sortingText.gameObject.SetActive(false);
                    autoSorter.sortToggleButton.gameObject.SetActive(false);
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
                        new Ingredient(TechType.ComputerChip, 1),
                        new Ingredient(TechType.AluminumOxide, 2)
                    }
                };

                customPrefab.SetGameObject(clonePrefab);
                customPrefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);
                customPrefab.SetRecipe(recipe);
                customPrefab.Register();
            }
        }

    }
}
