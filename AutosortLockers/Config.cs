using BepInEx;
using Nautilus.Options;
using BepInEx.Configuration;
using Nautilus.Handlers;
using System.Runtime.CompilerServices;

namespace AutosortLockers
{

    public class AutosortConfig
    {
        public static ConfigEntry<bool> EasyBuild;
        public static ConfigEntry<float> SortInterval;
        public static ConfigEntry<bool> ShowAllItems;

        public static ConfigEntry<int> AutosorterWidth;
        public static ConfigEntry<int> AutosorterHeight;

        public static ConfigEntry<int> ReceptacleWidth;
        public static ConfigEntry<int> ReceptacleHeight;

        public static ConfigEntry<int> StandingReceptacleWidth;
        public static ConfigEntry<int> StandingReceptacleHeight;


        public static void LoadConfig(ConfigFile config)
        {
            // Recipe config regsitration

            EasyBuild = config.Bind("Autosorter Recipe",
                "Easy Build Recipe",
                false,
                "Toggle whether to use the easy recipe or not"
                );

            SortInterval = config.Bind("Autosorter Recipe",
                "Sort Interval",
                1.0f,
                "How long to wait in-between sorting each item"
                );

            ShowAllItems = config.Bind("Autosorter Recipe",
                "Show All Items",
                false,
                "Whether to show all items or not"
                );

            // Autosorter config options

            AutosorterWidth = config.Bind("Autosorter Config",
                "Storage width size",
                5,
                "The width of the inventory for the autosorter"
                );

            AutosorterHeight = config.Bind("Autosorter Config",
                "Storage height size",
                6,
                "The height of the inventory for the autosorter"
                );

            // Autosort receptacle config options

            ReceptacleWidth = config.Bind("Receptacle Config",
                "Storage width size",
                6,
                "The width of the inventory for the autosort receptacle"
                );
            ReceptacleHeight = config.Bind("Receptacle Config",
                "Storage height size",
                8,
                "The height of the inventory for the autosort receptacle"
                );
            // Autosort standing receptacle config options

            StandingReceptacleWidth = config.Bind("Standing Receptacle Config",
                "Storage width size",
                6,
                "The width of the inventory for the standing autosort receptacle"
                );
            StandingReceptacleHeight = config.Bind("Standing Receptacle Config",
                "Storage height size",
                8,
                "The height of the inventory for the standing autosort receptacle"
                );
        }

        public class AutoSortModOptions : ModOptions 
        {
            AutoSortModOptions() : base("Autosort Lockers")
            {
                OptionsPanelHandler.RegisterModOptions(this);

                AddItem(EasyBuild.ToModToggleOption());
                AddItem(SortInterval.ToModSliderOption( 0.1f, 5.0f, 0.1f, "{0:F1}x"));
                AddItem(ShowAllItems.ToModToggleOption());

                AddItem(AutosorterWidth.ToModSliderOption(1, 10, 1));
                AddItem(AutosorterHeight.ToModSliderOption(1, 10, 1));

                AddItem(ReceptacleWidth.ToModSliderOption(1, 10, 1));
                AddItem(ReceptacleHeight.ToModSliderOption(1, 10, 1));

                AddItem(StandingReceptacleWidth.ToModSliderOption(1, 10, 1));
                AddItem(StandingReceptacleHeight.ToModSliderOption(1, 10, 1));
            }
        }
    }

}
