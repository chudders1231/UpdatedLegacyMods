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
using Nautilus.Utility;

namespace AutosortLockers
{
    internal class AutosortUnloader : MonoBehaviour
    {
        private static readonly Color MainColor = new Color(0.2f, 0.2f, 1.0f);
        private static readonly Color PulseColor = Color.white;

        private bool initialized;
        private Constructable constructable;
        private StorageContainer container;
        private List<AutosortTarget> singleItemTargets = new List<AutosortTarget>();
        private List<AutosortTarget> categoryTargets = new List<AutosortTarget>();
        private List<AutosortTarget> anyTargets = new List<AutosortTarget>();

        private int unsortableItems = 0;
        private int unloadableItems = 0;

        [SerializeField]
        private Image background;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Image unloadIcon;
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private TextMeshProUGUI sortingText;
        [SerializeField]
        private TextMeshProUGUI unloadingText;
        [SerializeField]
        private TextMeshProUGUI unloadingTitle;

        [SerializeField]
        private bool isSorting;
        [SerializeField]
        private bool isUnloading;
        [SerializeField]
        private bool sortedItem;
        [SerializeField]
        private List<ItemsContainer> containerTargets;


        public bool IsSorting => isSorting;

        private void Awake()
        {
            constructable = GetComponent<Constructable>();
            container = GetComponent<StorageContainer>();
            container.hoverText = "Open autosorter";
            container.storageLabel = "Autosorter";

            containerTargets = new List<ItemsContainer>();
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
            UpdateSortingText();
            UpdateUnloadingText();
        }

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Mathf.Max(0, AutosortConfig.SortInterval.Value - (unsortableItems / 60.0f)));

                yield return Sort();

                yield return Unload();
            }
        }

        private void UpdateUnloadingText()
        {
            if(isUnloading)
            {
                unloadingText.text = "Unloading...";

            } else if (unloadableItems > 0)
            {
                unloadingText.text = $"Unhandled Items: {unloadableItems}" ;
            } else {
                unloadingText.text = "Ready to Unload";
            }
        }
        private void UpdateSortingText()
        {
            if (isSorting)
            {
                sortingText.text = "Sorting...";

            } else if (unsortableItems > 0)
            {
                sortingText.text = "Unsorted Items: " + unsortableItems;

            }
            else
            {
                sortingText.text = "Ready to Sort";
            }
        }

        private void Initialize()
        {
            background.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            text.gameObject.SetActive(true);
            sortingText.gameObject.SetActive(true);

            background.sprite = Common.Utility.ImageUtils.TextureToSprite(Utilities.GetTexture("LockerScreen"));
            icon.sprite = Common.Utility.ImageUtils.TextureToSprite(Utilities.GetTexture("Sorter"));

            unloadIcon.sprite = Common.Utility.ImageUtils.TextureToSprite(Utilities.GetTexture("Unloading"));

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

            AccumulateTargets();
            if (NoTargets())
            {
                isSorting = false;
                yield break;
            }

            yield return SortFilteredTargets(false);
            if (sortedItem)
            {
                isSorting = true;
                yield break;
            }

            yield return SortFilteredTargets(true);
            if (sortedItem)
            {
                isSorting = true;
                yield break;
            }

            yield return SortAnyTargets();
            if (sortedItem)
            {
                isSorting = true;
                yield break;
            }

            isSorting = false;
        }

        private void AccumulateUnloadTargets()
        {
            unloadableItems = 0;
            SubRoot subRoot = GetComponentInParent<SubRoot>();
            if (subRoot == null)
                return;

            containerTargets.Clear();
            SeamothStorageContainer[] seamothStorageContainers = subRoot.gameObject.GetComponentsInChildren<SeamothStorageContainer>(true);

            // This can be particularly confusing but storage containers in prawn suit also use the seamoth storage component...

            foreach (var storageContainer in seamothStorageContainers)
            {
                ItemsContainer itemContainer = storageContainer.container;
                if (itemContainer.count <= 0 || storageContainer.GetComponent<TechTag>().type == TechType.SeamothTorpedoModule || storageContainer.GetComponent<TechTag>().type == TechType.ExosuitTorpedoArmModule)
                    continue;
                unloadableItems += itemContainer.count;
                containerTargets.Add(itemContainer);
            }
        }
        private IEnumerator Unload()
        {
            isUnloading = false;

            AccumulateUnloadTargets();

            if (container.container.IsFull())
                yield break;

            if (containerTargets.Count() <= 0)
                yield break;

            foreach( var containerTarget in containerTargets)
            {
                foreach( var item in containerTarget.ToList())
                {
                    isUnloading = true;
                    if (container.container.HasRoomFor(item.techType))
                    {
                        container.container.AddItem(item.item);
                        StartCoroutine(PulseUnloadIcon());

                        yield break;
                    } else
                    {
                        isUnloading = false;
                        yield return null;
                    }
                }
            }
            isUnloading = false;
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
                            if (!TechTypeExtensions.FromString(techType, out TechType tt, true))
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
        public IEnumerator PulseUnloadIcon()
        {
            float t = 0;
            float rate = 0.5f;
            while (t < 1.0)
            {
                t += Time.deltaTime * rate;
                unloadIcon.color = Color.Lerp(PulseColor, MainColor, t);
                yield return null;
            }
        }

        internal class AutosortUnloaderLockerBuildable
        {
            public static PrefabInfo Info { get; private set; }
            public static void Patch()
            {
                Info = Utilities.CreatePrefabInfo(
                    "AutosortUnloader",
                    "Autosort Vehicle Unloader",
                    "Works like an Autosort Receptacle, while also unloading items from docked vehicles!",
                    Utilities.GetSprite("AutosortUnloader")
                    );

                var customPrefab = new CustomPrefab(Info);
                var clonePrefab = new CloneTemplate(Info, TechType.SmallLocker);

                clonePrefab.ModifyPrefab += obj =>
                {
                    var triggerCull = obj.GetComponentInChildren<TriggerCull>();
                    var container = obj.GetComponent<StorageContainer>();
                    container.width = AutosortConfig.AutosorterWidth.Value;
                    container.height = AutosortConfig.AutosorterHeight.Value;
                    container.container.Resize(AutosortConfig.AutosorterWidth.Value, AutosortConfig.AutosorterHeight.Value);

                    var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
                    foreach (var meshRenderer in meshRenderers)
                    {
                        meshRenderer.material.color = new Color(0.2f, 0.4f, 1.0f);
                    }

                    var prefabText = obj.GetComponentInChildren<TextMeshProUGUI>();

                    var label = obj.FindChild("Label");
                    DestroyImmediate(label);

                    var autoSorter = obj.AddComponent<AutosortUnloader>();

                    var canvas = LockerPrefabShared.CreateCanvas(obj.transform);
                    triggerCull.objectToCull = canvas.gameObject;

                    autoSorter.background = LockerPrefabShared.CreateBackground(canvas.transform);
                    autoSorter.icon = LockerPrefabShared.CreateIcon(autoSorter.background.transform, MainColor, 80); // Originally 40
                    autoSorter.text = LockerPrefabShared.CreateText(autoSorter.background.transform, prefabText, MainColor, 44, 14, "Autosorter");

                    autoSorter.sortingText = LockerPrefabShared.CreateText(autoSorter.background.transform, prefabText, MainColor, -70, 8, "Sorting...");
                    autoSorter.sortingText.alignment = TextAlignmentOptions.Top;

                    autoSorter.unloadIcon = LockerPrefabShared.CreateIcon(autoSorter.background.transform, MainColor, -30); // Originally 40
                    autoSorter.unloadingTitle = LockerPrefabShared.CreateText(autoSorter.background.transform, prefabText, MainColor, -64, 14, "Unloader");

                    autoSorter.unloadingText = LockerPrefabShared.CreateText(autoSorter.background.transform, prefabText, MainColor, -180, 8, "Unloading...");
                    autoSorter.unloadingText.alignment = TextAlignmentOptions.Top;

                    autoSorter.background.gameObject.SetActive(false);
                    autoSorter.icon.gameObject.SetActive(false);
                    autoSorter.text.gameObject.SetActive(false);
                    autoSorter.sortingText.gameObject.SetActive(false);
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
                        new Ingredient(TechType.Kyanite, 2),
                        new Ingredient(TechType.AdvancedWiringKit, 1),
                        new Ingredient(TechType.PrecursorIonCrystal, 1)

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
