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
using System.Collections;
using Nautilus.Utility;
using System.Text.RegularExpressions;

namespace AutosortLockers
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Mod : BaseUnityPlugin
	{
        // Plugin Setup
        private const string myGUID = "com.chadlymasterson.autosortlockers";
        private const string pluginName = "Autosort Lockers";
        private const string versionString = "1.0.4";

		public static List<Color> colors = new List<Color>();

		public static SaveData saveData;
        private static string modDirectory;

		public static readonly Harmony harmony = new Harmony(myGUID);
        public static AssetBundle pickerMenuBundle;

        private void Awake()
		{
			AutosortLogger.Log("Patching started...");

            pickerMenuBundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), "pickermenu");

            modDirectory = "AutosortLockers";

            AutosortConfig.LoadConfig(Config);

            saveData = SaveDataHandler.RegisterSaveDataCache<SaveData>();

            RegisterColors();

			AddBuildables();

			harmony.PatchAll(Assembly.GetExecutingAssembly());

            //UWE.CoroutineHost.StartCoroutine(CheckPrefabs());

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

        public static IEnumerator CheckPrefabs()
        {
            ErrorMessage.AddMessage($"Starting to check for prefabs!");
            var pickupableTypes = new Dictionary<string, string>();
            var startTime = DateTime.Now;
            var lastCalledTime = startTime;
            foreach (TechType type in Enum.GetValues(typeof(TechType)))
            {
                if (type == TechType.None) continue;

                var request = CraftData.GetPrefabForTechTypeAsync(type);
                yield return request;
                var result = request.GetResult();
                if (!result) continue;
                var intermediaryTime = DateTime.Now - lastCalledTime;
                var words = Regex.Matches(type.ToString(), @"([A-Z][a-z]+)").Cast<Match>().Select(m => m.Value);
                if (result.TryGetComponent<Pickupable>(out _)) pickupableTypes.Add(type.AsString(), string.Join(" ", words)) ;

            }

            var json = JsonConvert.SerializeObject(pickupableTypes, Formatting.Indented);
            File.WriteAllText(IOUtilities.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets","pickupableTechTypes.json"), json);

            var endTime = DateTime.Now;
            var timeDifference = endTime - startTime;

            var techType = TechTypeExtensions.FromString("Titanium", out TechType o, true);


            ErrorMessage.AddMessage($"Got all prefabs, it took :{timeDifference.TotalSeconds} seconds");
            ErrorMessage.AddMessage($"Titanium Techtype result: {o}");
        }

    }
}