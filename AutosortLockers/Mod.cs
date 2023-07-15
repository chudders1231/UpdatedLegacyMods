using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Common.Mod;
using Newtonsoft.Json;
using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using System.Linq;

namespace AutosortLockers
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Mod : BaseUnityPlugin
	{
        // Plugin Setup
        private const string myGUID = "com.chadlymasterson.autosortlockers";
        private const string pluginName = "Autosort Lockers";
        private const string versionString = "1.0.2";

		public static List<Color> colors = new List<Color>();

		public static SaveData saveData;
        private static string modDirectory;

		public static readonly Harmony harmony = new Harmony(myGUID);

        private void Awake()
		{

			AutosortLogger.Log("Patching started...");

			modDirectory = "AutosortLockers";

            AutosortConfig.LoadConfig(Config);
            saveData = SaveDataHandler.RegisterSaveDataCache<SaveData>();

            RegisterColors();

			AddBuildables();

			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		public void AddBuildables()
		{
			AutosortLocker.AutosortLockerBuildable.Patch();
			AutosortUnloader.AutosortUnloaderLockerBuildable.Patch();

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

        public static SaveDataEntry GetSaveData(string id)
        {
            if (saveData.Entries.Count() == 0) saveData.Load();
            return saveData.Entries.GetOrDefault(id, new SaveDataEntry());
        }

        public static void RegisterColors()
		{
			var serializedColors = JsonConvert.DeserializeObject<List<SerializableColor>>(File.ReadAllText(GetAssetPath("colors.json")));

			foreach( var sColor in serializedColors )
			{
				colors.Add(sColor.ToColor());
			}
		}

	}
}