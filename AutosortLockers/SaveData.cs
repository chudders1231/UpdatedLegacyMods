using System;
using System.Collections.Generic;
using UnityEngine;
using Common.Mod;
using Nautilus.Json;

namespace AutosortLockers
{
	[Serializable]
	public class SaveDataEntry
	{
		public List<AutosorterFilter> FilterData = new List<AutosorterFilter>();
		public string Label = "Locker";
		public SerializableColor LabelColor = Color.white;
		public SerializableColor IconColor = Color.white;
		public SerializableColor OtherTextColor = Color.white;
		public SerializableColor ButtonsColor = Color.white;
		public SerializableColor LockerColor = new Color(0.3f, 0.3f, 0.3f);
	}

	[Serializable]
	public class SaveData : SaveDataCache
	{
		public Dictionary<string, SaveDataEntry> Entries = new Dictionary<string, SaveDataEntry>();
	}
}
