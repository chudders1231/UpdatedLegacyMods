﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
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

	public static class AutosorterCategoryData
	{
		public static Dictionary<string, List<TechType>> defaultCategories = new Dictionary<string, List<TechType>>() 
		{
			{"Fish", Fish},
            {"Alterra Artifacts", AlterraArtifacts},
            {"Mysterious Tablets", MysteriousTablets},
            {"CreatureEggs", CreatureEggs},
            {"Food", Food},
            {"Water", Water},
            {"Scanner Room Upgrades", ScannerRoomUpgrades},
            {"Cyclops Upgrades", CyclopsUpgrades},
            {"Prawn Suit Upgrades", PrawnSuitUpgrades},
            {"Seamoth Upgrades", SeamothUpgrades},
            {"Equipment", Equipment},
            {"Tools", Tools},
            {"Torpedos", Torpedoes},
        };

		public static List<TechType> Fish = new List<TechType> {
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
		};

		public static List<TechType> AlterraArtifacts = new List<TechType> {
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
		};

		public static List<TechType> MysteriousTablets = new List<TechType> {
			TechType.PrecursorKey_Blue,
			TechType.PrecursorKey_Orange,
			TechType.PrecursorKey_Purple,
		};

		public static List<TechType> CreatureEggs = new List<TechType> {
			TechType.BonesharkEgg,
			TechType.CrabsnakeEgg,
			TechType.CrabsquidEgg,
			TechType.CrashEgg,
			TechType.CutefishEgg,
			TechType.GasopodEgg,
			TechType.JellyrayEgg,
			TechType.JumperEgg,
			TechType.LavaLizardEgg,
			TechType.MesmerEgg,
			TechType.RabbitrayEgg,
			TechType.ReefbackEgg,
			TechType.SandsharkEgg,
			TechType.ShockerEgg,
			TechType.SpadefishEgg,
			TechType.StalkerEgg,
			TechType.GrandReefsEgg,
			TechType.GrassyPlateausEgg,
			TechType.KelpForestEgg,
			TechType.KooshZoneEgg,
			TechType.LavaZoneEgg,
			TechType.MushroomForestEgg,
			TechType.SafeShallowsEgg,
			TechType.TwistyBridgesEgg,
			TechType.RabbitrayEggUndiscovered,
			TechType.JellyrayEggUndiscovered,
			TechType.StalkerEggUndiscovered,
			TechType.ReefbackEggUndiscovered,
			TechType.JumperEggUndiscovered,
			TechType.BonesharkEggUndiscovered,
			TechType.GasopodEggUndiscovered,
			TechType.MesmerEggUndiscovered,
			TechType.SandsharkEggUndiscovered,
			TechType.ShockerEggUndiscovered,
			TechType.GenericEgg,
			TechType.CrashEgg,
			TechType.CrashEggUndiscovered,
			TechType.CrabsquidEgg,
			TechType.CrabsquidEggUndiscovered,
			TechType.CutefishEgg,
			TechType.CutefishEggUndiscovered,
			TechType.LavaLizardEgg,
			TechType.LavaLizardEggUndiscovered,
			TechType.CrabsnakeEggUndiscovered,
			TechType.SpadefishEggUndiscovered
		};

		public static List<TechType> Food = new List<TechType> {
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

		public static List<TechType> Water = new List<TechType> {
			TechType.BigFilteredWater,
			TechType.Coffee,
			TechType.DisinfectedWater,
			TechType.FilteredWater,
			TechType.WaterFiltrationSuit,
		};

		public static List<TechType> ScannerRoomUpgrades = new List<TechType> {
			TechType.MapRoomUpgradeScanRange,
			TechType.MapRoomUpgradeScanSpeed,
			TechType.MapRoomCamera,
		};

		public static List<TechType> CyclopsUpgrades = new List<TechType> {
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

		public static List<TechType> PrawnSuitUpgrades = new List<TechType> {
			TechType.ExoHullModule1,
			TechType.ExoHullModule2,
			TechType.ExosuitDrillArmModule,
			TechType.ExosuitGrapplingArmModule,
			TechType.ExosuitJetUpgradeModule,
			TechType.ExosuitPropulsionArmModule,
			TechType.ExosuitThermalReactorModule,
			TechType.ExosuitTorpedoArmModule,
		};

		public static List<TechType> SeamothUpgrades = new List<TechType> {
			TechType.SeamothElectricalDefense,
			TechType.SeamothReinforcementModule,
			TechType.SeamothSolarCharge,
			TechType.SeamothSonarModule,
			TechType.SeamothTorpedoModule,
		};

		public static List<TechType> GeneralUpgrades = new List<TechType> {
			TechType.HullReinforcementModule,
			TechType.PowerUpgradeModule,
			TechType.VehicleArmorPlating,
			TechType.VehicleHullModule1,
			TechType.VehicleHullModule2,
			TechType.VehicleHullModule3,
			TechType.VehiclePowerUpgradeModule,
			TechType.VehicleStorageModule,
		};

		public static List<TechType> Equipment = new List<TechType> {
			TechType.MapRoomHUDChip,
			TechType.Rebreather,
			TechType.Compass,
			TechType.Fins,
			TechType.HighCapacityTank,
			TechType.PlasteelTank,
			TechType.RadiationGloves,
			TechType.RadiationHelmet,
			TechType.RadiationSuit,
			TechType.ReinforcedDiveSuit,
			TechType.ReinforcedGloves,
			TechType.WaterFiltrationSuit,
			TechType.SwimChargeFins,
			TechType.Tank,
			TechType.UltraGlideFins,
		};

		public static List<TechType> Tools = new List<TechType> {
			TechType.AirBladder,
			TechType.Beacon,
			TechType.Builder,
			TechType.CyclopsDecoy,
			TechType.DiamondBlade,
			TechType.DiveReel,
			TechType.DoubleTank,
			TechType.FireExtinguisher,
			TechType.Flare,
			TechType.Flashlight,
			TechType.Gravsphere,
			TechType.HeatBlade,
			TechType.Knife,
			TechType.LaserCutter,
			TechType.LEDLight,
			TechType.Pipe,
			TechType.PipeSurfaceFloater,
			TechType.PropulsionCannon,
			TechType.RepulsionCannon,
			TechType.Scanner,
			TechType.Seaglide,
			TechType.SmallStorage,
			TechType.StasisRifle,
			TechType.Welder,
			TechType.LuggageBag,
		};

		public static List<TechType> Torpedoes = new List<TechType> {
			TechType.GasTorpedo,
			TechType.WhirlpoolTorpedo
		};

		public static List<TechType> PlantsAndSeeds = new List<TechType> {
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
			TechType.MembrainTreeSeed,
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
		};

		public static List<TechType> Metals = new List<TechType> {
			TechType.Copper,
			TechType.Gold,
			TechType.Lead,
			TechType.Lithium,
			TechType.Magnetite,
			TechType.ScrapMetal,
			TechType.Nickel,
			TechType.PlasteelIngot,
			TechType.Silver,
			TechType.Titanium,
			TechType.TitaniumIngot,
		};

		public static List<TechType> NaturalMaterials = new List<TechType> {
			TechType.GasPod,
			TechType.CoralChunk,
			TechType.WhiteMushroom,
			TechType.AcidMushroom,
			TechType.JeweledDiskPiece,
			TechType.BloodOil,
			TechType.CrashPowder,
			TechType.Salt,
			TechType.SeaTreaderPoop,
			TechType.StalkerTooth,
			TechType.JellyPlant,
		};

		public static List<TechType> Electronics = new List<TechType> {
			TechType.AdvancedWiringKit,
			TechType.ComputerChip,
			TechType.CopperWire,
			TechType.DepletedReactorRod,
			TechType.ReactorRod,
			TechType.WiringKit,
		};

		public static List<TechType> SyntheticMaterials = new List<TechType> {
			TechType.Aerogel,
			TechType.AramidFibers,
			TechType.Benzene,
			TechType.Bleach,
			TechType.EnameledGlass,
			TechType.FiberMesh,
			TechType.Glass,
			TechType.HatchingEnzymes,
			TechType.HydrochloricAcid,
			TechType.Lubricant,
			TechType.Polyaniline,
			TechType.PrecursorIonCrystal,
			TechType.Silicone,
		};

		public static List<TechType> CrystalMaterials = new List<TechType> {
			TechType.AluminumOxide,
			TechType.Diamond,
			TechType.Kyanite,
			TechType.Quartz,
			TechType.Sulphur,
			TechType.UraniniteCrystal,
		};

		public static List<TechType> Batteries = new List<TechType> {
			TechType.Battery,
			TechType.PowerCell,
			TechType.PrecursorIonBattery,
			TechType.PrecursorIonPowerCell,
		};

		public static List<TechType> IndividualItems = new List<TechType> {
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
	}

	[Serializable]
	public class AutosorterFilter
	{
		public string Category;
		public List<int> Types = new List<int>();

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
				return textInfo.ToTitleCase(Language.main.Get((TechType)Types[0]));
			}
		}

		public bool IsTechTypeAllowed(TechType techType)
		{
			return Types.Contains((int)techType);
		}

		public bool IsSame(AutosorterFilter other)
		{
			return Category == other.Category && Types.Count > 0 && Types.Count == other.Types.Count && Types[0] == other.Types[0];
		}
	}

	[Serializable]
	public static class AutosorterList
	{
		public static List<AutosorterFilter> Filters;

		public static List<AutosorterFilter> GetFilters()
		{
			if (Filters == null)
			{
				InitializeFilters();
			}
			return Filters;
		}

		public static List<int> GetOldFilter(string oldCategory, out bool success, out string newCategory)
		{
			var category = AutoSorterCategory.None;
			if (!Int32.TryParse(oldCategory, out int oldCategoryInt))
			{
				newCategory = "";
				success = false;
				return new List<int>();
			}
			category = (AutoSorterCategory)oldCategoryInt;
			newCategory = category.ToString();

			success = true;
			switch (category)
			{
				default:
				case AutoSorterCategory.None: return AutosorterCategoryData.IndividualItems.Cast<int>().ToList();

				case AutoSorterCategory.Food: return AutosorterCategoryData.Food.Cast<int>().ToList();
				case AutoSorterCategory.Water: return AutosorterCategoryData.Water.Cast<int>().ToList();
				case AutoSorterCategory.PlantsAndSeeds: return AutosorterCategoryData.PlantsAndSeeds.Cast<int>().ToList();
				case AutoSorterCategory.Metals: return AutosorterCategoryData.Metals.Cast<int>().ToList();
				case AutoSorterCategory.NaturalMaterials: return AutosorterCategoryData.NaturalMaterials.Cast<int>().ToList();
				case AutoSorterCategory.SyntheticMaterials: return AutosorterCategoryData.SyntheticMaterials.Cast<int>().ToList();
				case AutoSorterCategory.Electronics: return AutosorterCategoryData.Electronics.Cast<int>().ToList();
				case AutoSorterCategory.CrystalMaterials: return AutosorterCategoryData.CrystalMaterials.Cast<int>().ToList();
				case AutoSorterCategory.Batteries: return AutosorterCategoryData.Batteries.Cast<int>().ToList();
				case AutoSorterCategory.Fish: return AutosorterCategoryData.Fish.Cast<int>().ToList();
				case AutoSorterCategory.Eggs: return AutosorterCategoryData.CreatureEggs.Cast<int>().ToList();
				case AutoSorterCategory.Tools: return AutosorterCategoryData.Tools.Cast<int>().ToList();
				case AutoSorterCategory.Equipment: return AutosorterCategoryData.Equipment.Cast<int>().ToList();
				case AutoSorterCategory.MysteriousTablets: return AutosorterCategoryData.MysteriousTablets.Cast<int>().ToList();
				case AutoSorterCategory.ScannerRoomUpgrades: return AutosorterCategoryData.ScannerRoomUpgrades.Cast<int>().ToList();
				case AutoSorterCategory.GeneralUpgrades: return AutosorterCategoryData.GeneralUpgrades.Cast<int>().ToList();
				case AutoSorterCategory.SeamothUpgrades: return AutosorterCategoryData.SeamothUpgrades.Cast<int>().ToList();
				case AutoSorterCategory.PrawnSuitUpgrades: return AutosorterCategoryData.PrawnSuitUpgrades.Cast<int>().ToList();
				case AutoSorterCategory.CyclopsUpgrades: return AutosorterCategoryData.CyclopsUpgrades.Cast<int>().ToList();
				case AutoSorterCategory.Torpedoes: return AutosorterCategoryData.Torpedoes.Cast<int>().ToList();
				case AutoSorterCategory.AlterraStuff: return AutosorterCategoryData.AlterraArtifacts.Cast<int>().ToList();
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
			Filters = file.Where((f) => f.IsCategory()).ToList();
			
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
					Filters.Add(new AutosorterFilter() {Category = "", Types = new List<int> {(int)v}});
				}
				return;
			}
			var sorted = file.Where(f => !f.IsCategory()).ToList();
			sorted.Sort((x, y) =>
			{
				string xName = Language.main.Get((TechType)x.Types.First());
				string yName = Language.main.Get((TechType)y.Types.First());
				return string.Compare(xName.ToLowerInvariant(), yName.ToLowerInvariant(), StringComparison.Ordinal);
			});
			foreach (var filter in sorted)
			{
				Filters.Add(filter);
			}
		}

		private static void AddEntry(string category, List<int> types)
		{
			Filters.Add(new AutosorterFilter {
				Category = category,
				Types = types
			});
		}

		private static void AddEntry(int type)
		{
			Filters.Add(new AutosorterFilter {
				Category = "",
				Types = new List<int> { type }
			});
		}
	}
}
