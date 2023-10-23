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
using Nautilus.Utility;
using BepInEx.Logging;

namespace AutosortLockers
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Mod : BaseUnityPlugin
	{
        // Plugin Setup
        private const string myGUID = "com.chadlymasterson.autosortlockers";
        private const string pluginName = "Autosort Lockers";
        private const string versionString = "1.0.5";

		public static List<Color> colors = new List<Color>();

		public static SaveData saveData = SaveDataHandler.RegisterSaveDataCache<SaveData>();
        private static string modDirectory = "AutosortLockers";

		public static readonly Harmony harmony = new Harmony(myGUID);
        public static AssetBundle pickerMenuBundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), "pickermenu");
        public static ManualLogSource logger;

        private void Awake()
		{
            // Save the logger
            logger = Logger;

            logger.LogMessage("Autosort lockers patching started...");

            // Register modoptions
            AutosortConfig.LoadConfig(Config);

            // Write default filters if it doesn't already exist
            AutosortConfig.WriteDefaultFilters();

            // Register hardcoded colours
            RegisterColors();

            // Register locker buildables
			AddBuildables();

            // Patch all harmony patches
			harmony.PatchAll(Assembly.GetExecutingAssembly());

            logger.LogMessage("Autosort lockers patching finished...");
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
			return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		public static string GetAssetPath(string filename = "")
		{
            return IOUtilities.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", filename);

        }

        public static SaveDataEntry GetSaveData(string id)
        {
            if (saveData.Entries.Count() == 0) saveData.Load();
            return saveData.Entries.GetOrDefault(id, new SaveDataEntry());
        }

        public void AutosortLockersAddEntry(object[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidOperationException("You are needing to supply two arguments, both are strings.");
            }

            AutosorterList.AddEntry(
                args[0] switch
                {
                    string s => s,
                    _ => throw new InvalidOperationException("The typing is incorrect for the first argument, it must be a string.")
                },
                args[1] switch
                {
                    string t => t,
                    _ => throw new InvalidOperationException("The typing is incorrect for the second argument, it must be a string.")
                }
            );
        }
        public void AutosortLockersAddCategory(object[] args)
        {
            if (args.Length < 1)
            {
                throw new InvalidOperationException("A string must be supplied for the Category Name.");
            }

            AutosorterList.AddCategory(
                args[0] switch
                {
                    string s => s,
                    _ => throw new InvalidOperationException("The typing is incorrect, this requires a string.")
                }
            );
        }
        public void AutosortLockersAddIndividualEntry(object[] args)
        {
            if (args.Length < 1)
            {
                throw new InvalidOperationException("There must be only one argument, it must be a string.");
            }

            AutosorterList.AddIndividualEntry(
                args[0] switch
                {
                    string t => t,
                    _ => throw new InvalidOperationException("The typing is incorrect for the first argument, it must be a string.")
                }
            );
        }

        private static void RegisterColors()
		{
			var serializedColors = JsonConvert.DeserializeObject<List<SerializableColor>>(File.ReadAllText(GetAssetPath("colors.json")));

			foreach( var sColor in serializedColors )
            {
                colors.Add(sColor.ToColor());
            }
        }

    }
}