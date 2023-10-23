
using HarmonyLib;
using Nautilus;

namespace AutosortLockers.Patches
{
	class AutosortLocker_Initializer_Patches
	{
		private static bool initialized;

		[HarmonyPatch(typeof(uGUI_PowerIndicator))]
		[HarmonyPatch("Initialize")]
		class uGUI_PowerIndicator_Initialize_Patch
		{
			private static void Postfix(uGUI_PowerIndicator __instance)
			{
				if (initialized)
				{
					if (Player.main == null)
					{
						Mod.logger.LogInfo("Deinitialize from no player");
						initialized = false;
					}
					return;
				}

				if (Inventory.main == null)
				{
					return;
				}
				initialized = true;
			}
		}

		[HarmonyPatch(typeof(uGUI_PowerIndicator))]
		[HarmonyPatch("Deinitialize")]
		class uGUI_PowerIndicator_Deinitialize_Patch
		{
			private static void PostFix()
			{
                Mod.logger.LogInfo("Deinitialize");
				initialized = false;
			}
		}
	}
}
