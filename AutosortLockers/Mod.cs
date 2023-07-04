using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Common.Mod;
using Newtonsoft.Json;
using BepInEx;
using HarmonyLib;
using Nautilus.Utility;

namespace AutosortLockers
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Mod : BaseUnityPlugin
	{
        // Plugin Setup
        private const string myGUID = "com.chadlymasterson.autosortlockers";
        private const string pluginName = "Autosort Lockers";
        private const string versionString = "1.0.0";

        public const string SaveDataFilename = "AutosortLockerNautilusSaveData.json";
		public static Config config;
		public static SaveData saveData;
		public static List<Color> colors = new List<Color>();
		private static string modDirectory;
		private static ModSaver saveObject;

        public static event Action<SaveData> OnDataLoaded;

		public static readonly Harmony harmony = new Harmony(myGUID);

        private void Awake()
		{

			AutosortLogger.Log("Patching started...");

			Mod.modDirectory = "AutosortLockers";

            LoadConfig();

			AddBuildables();

			harmony.PatchAll(Assembly.GetExecutingAssembly());
            AutosortLogger.Log("Patched Successfully!");
		}

		public void AddBuildables()
		{
			AutosortLocker.AutosortLockerBuildable.Patch();
			AutosortTarget.AutosortTargetBuildable.Patch();
			AutosortTarget.AutosortStandingTargetBuildable.Patch();
		}

		public static string GetModPath()
		{
			return Environment.CurrentDirectory + "/BepInEx/plugins/" + modDirectory;
		}

		public static string GetAssetPath(string filename)
		{
			return GetModPath() + "/Assets/" + filename;
		}

		private static void LoadConfig()
		{
			config = ModUtils.LoadConfig<Config>(GetModPath() + "/config.json");
			ValidateConfig();

			var serializedColors = JsonConvert.DeserializeObject<List<SerializableColor>>(File.ReadAllText(GetAssetPath("colors.json")));
			foreach (var sColor in serializedColors)
			{
				colors.Add(sColor.ToColor());
			}
		}

		private static void ValidateConfig()
		{
			Config defaultConfig = new Config();

			ModUtils.ValidateConfigValue("SortInterval", 0.1f, 10.0f, ref config, ref defaultConfig);
			ModUtils.ValidateConfigValue("AutosorterWidth", 1, 8, ref config, ref defaultConfig);
			ModUtils.ValidateConfigValue("AutosorterHeight", 1, 10, ref config, ref defaultConfig);
			ModUtils.ValidateConfigValue("ReceptacleWidth", 1, 8, ref config, ref defaultConfig);
			ModUtils.ValidateConfigValue("ReceptacleHeight", 1, 10, ref config, ref defaultConfig);
		}

		public static SaveData GetSaveData()
		{
			return saveData ?? new SaveData();
		}

		public static SaveDataEntry GetSaveData(string id)
		{
			var saveData = GetSaveData();
			foreach (var entry in saveData.Entries)
			{
				if (entry.Id == id)
				{
					return entry;
				}
			}
			return new SaveDataEntry() { Id = id };
		}

		public static void Save()
		{
			if (!IsSaving())
			{
				saveObject = new GameObject().AddComponent<ModSaver>();

				SaveData newSaveData = new SaveData();
				var targets = GameObject.FindObjectsOfType<AutosortTarget>();
				foreach (var target in targets)
				{
					target.Save(newSaveData);
				}
				saveData = newSaveData;
				ModUtils.Save<SaveData>(saveData, SaveDataFilename, OnSaveComplete);
			}
		}

		public static void OnSaveComplete()
		{
			saveObject.StartCoroutine(SaveCoroutine());
		}

		private static IEnumerator SaveCoroutine()
		{
			while (SaveLoadManager.main != null && SaveLoadManager.main.isSaving)
			{
				yield return null;
			}
			GameObject.DestroyImmediate(saveObject.gameObject);
			saveObject = null;
		}

		public static bool IsSaving()
		{
			return saveObject != null;
		}

		public static void LoadSaveData()
		{
            AutosortLogger.Log("Loading Save Data...");
			ModUtils.LoadSaveData<SaveData>(SaveDataFilename, (data) =>
			{
				saveData = data;
                AutosortLogger.Log("Save Data Loaded");
				OnDataLoaded?.Invoke(saveData);
			});
		}
	}
}