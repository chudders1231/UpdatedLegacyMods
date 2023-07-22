using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AutosortLockers
{
	[Serializable]
	public enum AutoSorterCategory
	{
		None,
		Food,
		Water,
		PlantsAndSeeds,
		Metals,
		Electronics,
		Batteries,
		NaturalMaterials,
		SyntheticMaterials,
		CrystalMaterials,
		Fish,
		Eggs,
		Tools,
		Equipment,
		MysteriousTablets,
		ScannerRoomUpgrades,
		GeneralUpgrades,
		SeamothUpgrades,
		PrawnSuitUpgrades,
		CyclopsUpgrades,
		Torpedoes,
		AlterraStuff,
	}

	public class AutosorterCategoryData
	{
		public static readonly TechType[] Fish =
		{
            TechType.Bladderfish,
            TechType.Boomerang,
            TechType.LavaBoomerang,
            TechType.Eyeye,
            TechType.LavaEyeye,
            TechType.GarryFish,
            TechType.HoleFish,
            TechType.Hoopfish,
            TechType.Spinefish,
            TechType.Hoverfish,
            TechType.Oculus,
            TechType.Peeper,
            TechType.Reginald,
            TechType.Spadefish,
			TechType.Floater,
        };
		public static readonly TechType[] Metals =
		{
			TechType.ScrapMetal,
			TechType.Lead,
			TechType.Titanium,
			TechType.Gold,
			TechType.Silver,
			TechType.TitaniumIngot,
			TechType.PlasteelIngot,
			TechType.Magnetite,
			TechType.Nickel,
		};
		public static readonly TechType[] NaturalMaterials =
		{
			TechType.Salt,
			TechType.AluminumOxide,
			TechType.Sulphur,
			TechType.CrashPowder,
			TechType.GasPod,
            TechType.JellyPlant,
            TechType.CoralChunk,
            TechType.WhiteMushroom,
            TechType.AcidMushroom,
            TechType.JeweledDiskPiece,
            TechType.BloodOil,
            TechType.SeaTreaderPoop,
            TechType.StalkerTooth,
        };
		public static readonly TechType[] SyntheticMaterials =
		{
			TechType.FiberMesh,
			TechType.Glass,
			TechType.Silicone,
			TechType.Lithium,
			TechType.EnameledGlass,
			TechType.AramidFibers,
            TechType.FirstAidKit,
        };
		public static readonly TechType[] Chemicals =
		{
			TechType.Bleach,
			TechType.HydrochloricAcid,
			TechType.Polyaniline,
			TechType.Benzene,
			TechType.Lubricant,
			TechType.HatchingEnzymes,
		};
		public static readonly TechType[] Electronics =
		{
			TechType.CopperWire,
			TechType.WiringKit,
			TechType.AdvancedWiringKit,
			TechType.ComputerChip,
			TechType.ReactorRod,
			TechType.DepletedReactorRod,
			TechType.PrecursorIonCrystal,
		};
		public static readonly TechType[] CrystalMaterials =
		{
            TechType.Quartz,
            TechType.Diamond,
			TechType.UraniniteCrystal,
			TechType.Kyanite,
		};
		public static readonly TechType[] PlantsAndSeeds =
		{
			TechType.Aerogel,
            TechType.AcidMushroomSpore,
            TechType.BluePalmSeed,
            TechType.BulboTreePiece,
            TechType.CreepvinePiece,
            TechType.CreepvineSeedCluster,
            TechType.EyesPlantSeed,
            TechType.FernPalmSeed,
            TechType.GabeSFeatherSeed,
            TechType.HangingFruit,
            TechType.JellyPlantSeed,
            TechType.KooshChunk,
            TechType.Melon,
            TechType.MelonSeed,
			TechType.PinkMushroom,
			TechType.PurpleRattle,
            TechType.MembrainTreeSeed,
			TechType.TreeMushroomPiece,
            TechType.OrangeMushroomSpore,
            TechType.OrangePetalsPlantSeed,
            TechType.PinkFlowerSeed,
            TechType.PinkMushroomSpore,
            TechType.PurpleBrainCoralPiece,
            TechType.PurpleBranchesSeed,
            TechType.PurpleFanSeed,
            TechType.PurpleRattleSpore,
            TechType.PurpleStalkSeed,
            TechType.PurpleTentacleSeed,
            TechType.PurpleVasePlantSeed,
            TechType.PurpleVegetable,
            TechType.RedBasketPlantSeed,
            TechType.RedBushSeed,
            TechType.RedConePlantSeed,
            TechType.RedGreenTentacleSeed,
            TechType.RedRollPlantSeed,
            TechType.SeaCrownSeed,
            TechType.ShellGrassSeed,
            TechType.SmallFanSeed,
            TechType.SmallMelon,
            TechType.SnakeMushroomSpore,
            TechType.SpikePlantSeed,
            TechType.SpottedLeavesPlantSeed,
            TechType.WhiteMushroomSpore,
			TechType.BulboTreePiece,

        };
		public static readonly TechType[] Equipment =
		{
			TechType.Fins,
			TechType.SwimChargeFins,
			TechType.UltraGlideFins,
			TechType.Tank,
			TechType.DoubleTank,
			TechType.HighCapacityTank,
			TechType.PlasteelTank,
			TechType.Compass,
			TechType.Rebreather,
			TechType.RadiationGloves,
			TechType.RadiationSuit,
			TechType.RadiationHelmet,
			TechType.MapRoomHUDChip,
			TechType.ReinforcedGloves,
			TechType.ReinforcedDiveSuit,
			TechType.WaterFiltrationSuit,
		};
		public static readonly TechType[] Tools =
		{
			TechType.Knife,
			TechType.HeatBlade,
			TechType.Flashlight,
			TechType.Builder,
			TechType.AirBladder,
			TechType.DiveReel,
			TechType.Scanner,
			TechType.FireExtinguisher,
			TechType.Welder,
			TechType.Seaglide,
			TechType.Flare,
			TechType.StasisRifle,
			TechType.PropulsionCannon,
			TechType.RepulsionCannon,
			TechType.Gravsphere,
			TechType.LaserCutter,
		};
		public static readonly TechType[] Deployables =
		{
			TechType.Beacon,
			TechType.CyclopsDecoy,
			TechType.Pipe,
			TechType.PipeSurfaceFloater,
			TechType.Constructor,
			TechType.SmallStorage,
			TechType.LEDLight,
			TechType.DiamondBlade,
		};
		public static readonly TechType[] Batteries =
		{
			TechType.Battery,
			TechType.PowerCell,
			TechType.LithiumIonBattery,
			TechType.PrecursorIonBattery,
			TechType.PrecursorIonPowerCell,
		};
		public static readonly TechType[] CreatureEggs =
		{
			TechType.StalkerEgg,
			TechType.ReefbackEgg,
			TechType.SpadefishEgg,
			TechType.RabbitrayEgg,
			TechType.MesmerEgg,
			TechType.JumperEgg,
			TechType.SandsharkEgg,
			TechType.JellyrayEgg,
			TechType.BonesharkEgg,
			TechType.CrabsnakeEgg,
			TechType.ShockerEgg,
			TechType.GasopodEgg,
			TechType.CrashEgg,
			TechType.CrabsquidEgg,
			TechType.CutefishEgg,
			TechType.LavaLizardEgg,
		};
		public static readonly TechType[] AlterraArtifacts = {
            TechType.LabContainer,
            TechType.LabContainer2,
            TechType.LabContainer3,
            TechType.ArcadeGorgetoy,
            TechType.Cap1,
            TechType.Cap2,
            TechType.LabEquipment1,
            TechType.LabEquipment2,
            TechType.LabEquipment3,
            TechType.LEDLightFragment,
            TechType.StarshipSouvenir,
            TechType.Poster,
            TechType.PosterAurora,
            TechType.PosterExoSuit1,
            TechType.PosterExoSuit2,
            TechType.PosterKitty,
			TechType.LuggageBag,
        };
        public static readonly TechType[] CyclopsUpgrades =
        {
            TechType.CyclopsDecoyModule,
            TechType.CyclopsFireSuppressionModule,
            TechType.CyclopsHullModule1,
            TechType.CyclopsHullModule2,
            TechType.CyclopsHullModule3,
            TechType.CyclopsSeamothRepairModule,
            TechType.CyclopsShieldModule,
            TechType.CyclopsSonarModule,
            TechType.CyclopsThermalReactorModule,
        };
        public static readonly TechType[] SeamothUpgrades =  {
            TechType.SeamothElectricalDefense,
            TechType.SeamothReinforcementModule,
            TechType.SeamothSolarCharge,
            TechType.SeamothSonarModule,
            TechType.SeamothTorpedoModule,
        };
        public static readonly TechType[] GeneralUpgrades = {
            TechType.HullReinforcementModule,
            TechType.PowerUpgradeModule,
            TechType.VehicleArmorPlating,
            TechType.VehicleHullModule1,
            TechType.VehicleHullModule2,
            TechType.VehicleHullModule3,
            TechType.VehiclePowerUpgradeModule,
            TechType.VehicleStorageModule,
        };
        public static readonly TechType[] Torpedoes = {
            TechType.GasTorpedo,
            TechType.WhirlpoolTorpedo
        };
        public static readonly TechType[] PrawnSuitUpgrades = {
            TechType.ExoHullModule1,
            TechType.ExoHullModule2,
            TechType.ExosuitDrillArmModule,
            TechType.ExosuitGrapplingArmModule,
            TechType.ExosuitJetUpgradeModule,
            TechType.ExosuitPropulsionArmModule,
            TechType.ExosuitThermalReactorModule,
            TechType.ExosuitTorpedoArmModule,
        };
        public static readonly TechType[] ScannerRoomUpgrades = {
            TechType.MapRoomUpgradeScanRange,
            TechType.MapRoomUpgradeScanSpeed,
            TechType.MapRoomCamera,
        };
		public static readonly TechType[] MysteriousTablets =
		{
			TechType.PrecursorKey_Blue,
			TechType.PrecursorKey_Orange,
			TechType.PrecursorKey_Purple,
		};
        public static readonly TechType[] Water = {
            TechType.BigFilteredWater,
            TechType.Coffee,
            TechType.DisinfectedWater,
            TechType.FilteredWater,
            TechType.WaterFiltrationSuitWater,
        };
        public static readonly TechType[] Food = {
            TechType.CookedBladderfish,
            TechType.CookedBoomerang,
            TechType.CookedEyeye,
            TechType.CookedGarryFish,
            TechType.CookedHoleFish,
            TechType.CookedHoopfish,
            TechType.CookedHoverfish,
            TechType.CookedLavaBoomerang,
            TechType.CookedLavaEyeye,
            TechType.CookedOculus,
            TechType.CookedPeeper,
            TechType.CookedReginald,
            TechType.CookedSpadefish,
            TechType.CookedSpinefish,
            TechType.CuredBladderfish,
            TechType.CuredBoomerang,
            TechType.CuredEyeye,
            TechType.CuredGarryFish,
            TechType.CuredHoleFish,
            TechType.CuredHoopfish,
            TechType.CuredHoverfish,
            TechType.CuredLavaBoomerang,
            TechType.CuredLavaEyeye,
            TechType.CuredOculus,
            TechType.CuredPeeper,
            TechType.CuredReginald,
            TechType.CuredSpadefish,
            TechType.CuredSpinefish,
            TechType.NutrientBlock,
            TechType.Snack1,
            TechType.Snack2,
            TechType.Snack3,
            TechType.BulboTreePiece,
            TechType.HangingFruit,
            TechType.Melon,
            TechType.PurpleVegetable,
        };
        public static readonly TechType[] IndividualItems =  {
			TechType.GasPod,
			TechType.CoralChunk,
			TechType.WhiteMushroom,
			TechType.AcidMushroom,
			TechType.JeweledDiskPiece,
			TechType.AdvancedWiringKit,
			TechType.Aerogel,
			TechType.AluminumOxide,
			TechType.AramidFibers,
			TechType.Benzene,
			TechType.Bleach,
			TechType.BloodOil,
			TechType.ComputerChip,
			TechType.Copper,
			TechType.CopperWire,
			TechType.CrashPowder,
			TechType.DepletedReactorRod,
			TechType.Diamond,
			TechType.EnameledGlass,
			TechType.FiberMesh,
			TechType.FirstAidKit,
			TechType.Glass,
			TechType.Gold,
			TechType.HatchingEnzymes,
			TechType.HydrochloricAcid,
			TechType.JellyPlant,
			TechType.Kyanite,
			TechType.Lead,
			TechType.Lithium,
			TechType.Lubricant,
			TechType.Magnetite,
			TechType.ScrapMetal,
			TechType.Nickel,
			TechType.PlasteelIngot,
			TechType.Polyaniline,
			TechType.PrecursorIonCrystal,
			TechType.Quartz,
			TechType.ReactorRod,
			TechType.Salt,
			TechType.SeaTreaderPoop,
			TechType.Silicone,
			TechType.Silver,
			TechType.StalkerTooth,
			TechType.Sulphur,
			TechType.Titanium,
			TechType.TitaniumIngot,
			TechType.UraniniteCrystal,
			TechType.WiringKit,
			TechType.Battery,
			TechType.PowerCell,
			TechType.PrecursorIonBattery,
			TechType.PrecursorIonPowerCell,
		};

        public static readonly Dictionary<string, TechType[]> defaultCategories = new()
        {
			{ "Fish", Fish},
            { "Metals", Metals},
            { "Synthetic Materials", SyntheticMaterials},
            { "Chemicals", Chemicals},
            { "Electronics", Electronics},
            { "Crystal Materials", CrystalMaterials},
            { "Plants and Seeds", PlantsAndSeeds},
            { "Equipment", Equipment},
            { "Tools", Tools},
            { "Deployables", Deployables},
            { "Batteries", Batteries},
            { "Creature Eggs", CreatureEggs},
            { "Alterra Artifacts", AlterraArtifacts},
            { "Cyclops Upgrades", CyclopsUpgrades},
            { "Seamoth Upgrades", SeamothUpgrades},
            { "General Upgrades", GeneralUpgrades},
            { "Torpedoes", Torpedoes},
            { "Prawn Suit Upgrades", PrawnSuitUpgrades},
            { "Scanner Room Upgrades", ScannerRoomUpgrades},
            { "Mysterious Tablets", MysteriousTablets},
            { "Water", Water},
            { "Food", Food},
        };
    }

	[Serializable]
	public class AutosorterFilter
	{
		public string Category;
		public List<string> Types = new List<string>();

		public bool IsCategory() => !string.IsNullOrEmpty(Category);

		public string GetString()
		{
			if (IsCategory())
			{
				return Category;
			}
			else
			{
				var textInfo = (new CultureInfo("en-US", false)).TextInfo;

				return textInfo.ToTitleCase(Language.main.Get(Types[0]));
			}
		}

		public bool IsTechTypeAllowed(TechType techType)
		{
			return Types.Contains(techType.ToString());
		}

		public bool IsSame(AutosorterFilter other)
		{
			return Category == other.Category && Types.Count > 0 && Types.Count == other.Types.Count && Types[0] == other.Types[0];
		}
	}
	[Serializable]
	public class FilterEntry
	{
		public string Category { get; set; }
        public List<string> Types { get; set; }

		public FilterEntry(string category, List<string> types)
        {
            Category = category;
            Types = types;
        }
    }
	[Serializable]
	public static class AutosorterList
	{

		public static List<AutosorterFilter> Filters = new List<AutosorterFilter>();
		public static bool isInitialized = false;

		public static List<AutosorterFilter> GetFilters()
		{
			if (isInitialized == false)
			{
				InitializeFilters();
			}
			return Filters;
		}

		public static List<string> GetOldFilter(string oldCategory, out bool success, out string newCategory)
		{
			if (!Int32.TryParse(oldCategory, out int oldCategoryInt))
			{
				newCategory = "";
				success = false;
				return new List<string>();
			}
			AutoSorterCategory category = (AutoSorterCategory)oldCategoryInt;
			newCategory = category.ToString();

			success = true;
			switch (category)
			{
				default:
				case AutoSorterCategory.None: return AutosorterCategoryData.IndividualItems.Cast<string>().ToList();

				case AutoSorterCategory.Food: return AutosorterCategoryData.Food.Cast<string>().ToList();
				case AutoSorterCategory.Water: return AutosorterCategoryData.Water.Cast<string>().ToList();
				case AutoSorterCategory.PlantsAndSeeds: return AutosorterCategoryData.PlantsAndSeeds.Cast<string>().ToList();
				case AutoSorterCategory.Metals: return AutosorterCategoryData.Metals.Cast<string>().ToList();
				case AutoSorterCategory.NaturalMaterials: return AutosorterCategoryData.NaturalMaterials.Cast<string>().ToList();
				case AutoSorterCategory.SyntheticMaterials: return AutosorterCategoryData.SyntheticMaterials.Cast<string>().ToList();
				case AutoSorterCategory.Electronics: return AutosorterCategoryData.Electronics.Cast<string>().ToList();
				case AutoSorterCategory.CrystalMaterials: return AutosorterCategoryData.CrystalMaterials.Cast<string>().ToList();
				case AutoSorterCategory.Batteries: return AutosorterCategoryData.Batteries.Cast<string>().ToList();
				case AutoSorterCategory.Fish: return AutosorterCategoryData.Fish.Cast<string>().ToList();
				case AutoSorterCategory.Eggs: return AutosorterCategoryData.CreatureEggs.Cast<string>().ToList();
				case AutoSorterCategory.Tools: return AutosorterCategoryData.Tools.Cast<string>().ToList();
				case AutoSorterCategory.Equipment: return AutosorterCategoryData.Equipment.Cast<string>().ToList();
				case AutoSorterCategory.MysteriousTablets: return AutosorterCategoryData.MysteriousTablets.Cast<string>().ToList();
				case AutoSorterCategory.ScannerRoomUpgrades: return AutosorterCategoryData.ScannerRoomUpgrades.Cast<string>().ToList();
				case AutoSorterCategory.GeneralUpgrades: return AutosorterCategoryData.GeneralUpgrades.Cast<string>().ToList();
				case AutoSorterCategory.SeamothUpgrades: return AutosorterCategoryData.SeamothUpgrades.Cast<string>().ToList();
				case AutoSorterCategory.PrawnSuitUpgrades: return AutosorterCategoryData.PrawnSuitUpgrades.Cast<string>().ToList();
				case AutoSorterCategory.CyclopsUpgrades: return AutosorterCategoryData.CyclopsUpgrades.Cast<string>().ToList();
				case AutoSorterCategory.Torpedoes: return AutosorterCategoryData.Torpedoes.Cast<string>().ToList();
				case AutoSorterCategory.AlterraStuff: return AutosorterCategoryData.AlterraArtifacts.Cast<string>().ToList();
			}
		}

		[Serializable]
		private class TypeReference
		{
			public string Name = "";
			public TechType Value = TechType.None;
		}

		private static void InitializeFilters()
		{
			var path = Mod.GetAssetPath("filters.json");
			var file = JsonConvert.DeserializeObject<List<AutosorterFilter>>(File.ReadAllText(path));

			Filters.AddRange(file.Where((f) => f.IsCategory()).ToList());

            if (AutosortConfig.ShowAllItems.Value)
			{
				var typeRefPath = Mod.GetAssetPath("type_reference.json");
				List<TypeReference> typeReferences =
					JsonConvert.DeserializeObject<List<TypeReference>>(File.ReadAllText(typeRefPath));
				typeReferences.Sort((TypeReference a, TypeReference b) =>
				{
					string aName = Language.main.Get(a.Value);
					string bName = Language.main.Get(b.Value);
					return String.Compare(aName.ToLowerInvariant(), bName.ToLowerInvariant(), StringComparison.Ordinal);
				});

				foreach (var typeRef in typeReferences)
				{
					var v = typeRef.Value;
					Filters.Add(new AutosorterFilter() { Category = "", Types = new List<string> { v.ToString() } });
				}
				return;
			}
			
			var sorted = file.Where(f => !f.IsCategory()).ToList();
			sorted.Sort((x, y) =>
			{
				string xName = Language.main.Get(x.Types[0]);
				string yName = Language.main.Get(y.Types[0]);
				return string.Compare(xName.ToLowerInvariant(), yName.ToLowerInvariant(), StringComparison.Ordinal);
			});

			foreach (var item in AutosorterCategoryData.IndividualItems)
			{
				Filters.Add(new AutosorterFilter() { Category = "", Types = new List<string> { item.ToString() } });
			}

			isInitialized = true;
		}

		public static void AddCategory(string categoryName)
		{
            var category = Filters.Find(filter => filter.Category == categoryName);

            if (category is null)
            {
				Filters.Add(new AutosorterFilter() { Category = categoryName, Types = new List<string> { } });
            }
        }

		public static void AddEntry(string categoryName, string item)
		{
            var filter = Filters.Find(filter => filter.Category == categoryName);

            if (filter is not null)
            {
                filter.Types.Add(item);
            }
		}

        public static void AddIndividualEntry(string item)
        {
            Filters.Add(new AutosorterFilter() { Category = "", Types = new List<string> { item } });
        }
    }
}
