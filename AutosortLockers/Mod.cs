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
using Nautilus.Utility.ModMessages;

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
        public static ModInbox inbox = new ModInbox(myGUID);

        private void Awake()
		{
			AutosortLogger.Log("Patching started...");

            AutosortConfig.LoadConfig(Config);
            AutosortConfig.WriteDefaultFilters();
            RegisterModMessageReaders();

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

        private static void RegisterColors()
		{
			var serializedColors = JsonConvert.DeserializeObject<List<SerializableColor>>(File.ReadAllText(GetAssetPath("colors.json")));

			foreach( var sColor in serializedColors )
            {
                colors.Add(sColor.ToColor());
            }
        }

        private static void RegisterModMessageReaders()
        {
            var AddEntry = new BasicModMessageReader(nameof(AutosorterList.AddEntry), (args) => {
                if (args.Length < 2)
                {
                    throw new InvalidOperationException("ffs you are supposed to send two arguments, two strings");
                }

                AutosorterList.AddEntry(
                    args[0] switch
                    {
                        string s => s,
                        _ => throw new InvalidOperationException("bro wtf please send me a string as the first parameter, it's supposed to be a category name")
                    },
                    args[1] switch
                    {
                        string t => t,
                        _ => throw new InvalidOperationException("bro wtf come on now, the second parameter is supposed to be a string...")
                    }
                );
            });

            var AddCategory = new BasicModMessageReader(nameof(AutosorterList.AddCategory), (args) => {
                if (args.Length < 1)
                {
                    throw new InvalidOperationException("ffs you are supposed to send an argument, a string");
                }

                AutosorterList.AddCategory(
                    args[0] switch
                    {
                        string s => s,
                        _ => throw new InvalidOperationException("bro wtf please send me a string as the first parameter, it's supposed to be a category name")
                    }
                );
            });

            var AddIndividiualItem = new BasicModMessageReader(nameof(AutosorterList.AddIndividualEntry), (args) => {
                if (args.Length < 1)
                {
                    throw new InvalidOperationException("ffs you are supposed to send a string as the argument, the TechType name of the item");
                }

                AutosorterList.AddIndividualEntry(
                    args[0] switch
                    {
                        string t => t,
                        _ => throw new InvalidOperationException("bro wtf please send me a string as the first parameter, it's supposed to be the item")
                    }
                );
            });
            inbox.AddMessageReader(AddEntry);
            inbox.AddMessageReader(AddCategory);
            inbox.AddMessageReader(AddIndividiualItem);
            ModMessageSystem.RegisterInbox(inbox);

            inbox.ReadAnyHeldMessages();

        }

    }
}