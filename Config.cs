using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OmniSwing
{
	//so I spent like half a day coding this
	//tML says they'll add ModConfig soon, what do I do?
	static class Config
	{
		public static List<int> BlacklistedItemIDs = new List<int>();
		public static List<int> WhitelistedItemIDs = new List<int>();

		static List<string> ErrorLines = new List<string>();

		static string ConfigFolderPath = Path.Combine(Main.SavePath, "Mod Configs");
		static string ConfigPath = Path.Combine(ConfigFolderPath, "OmniSwing.txt");

		static ILog Logger = LogManager.GetLogger("OmniSwingConfig");

		public static void Load()
		{
			if(!Directory.Exists(ConfigFolderPath))
			{
				Logger.Warn("Mod Config directory not found, creating...");
				Directory.CreateDirectory(ConfigFolderPath);
			}

			if(File.Exists(ConfigPath))
			{
				Reset();
				ReadConfig();
			}
			else
			{
				Logger.Warn("Config file not found, creating default...");
				SetDefaults();
				SaveConfig();
			}
		}

		static void ReadConfig()
		{
			int lineNumber = 0;
			using(var reader = new StreamReader(ConfigPath))
			{
				while(!reader.EndOfStream)
				{
					lineNumber++;
					string line = reader.ReadLine().Split('#')[0].Trim();
					if(line.Length == 0)
						continue;

					if(line[0] == '!')
					{
						line = line.Remove(0, 1).Trim();
						ParseConfigLine(line, lineNumber, WhitelistedItemIDs);
					}
					else
					{
						ParseConfigLine(line, lineNumber, BlacklistedItemIDs);
					}
				}
			}
		}

		static void ParseConfigLine(string line, int lineNumber, List<int> idList)
		{
			int id = 0;
			if(Int32.TryParse(line, out id))
			{
				if(id <= 0 || id >= ItemID.Count)
					throw new Exception(id + " is not a valid Vanilla Item ID!");
				idList.Add(id);
			}
			else
			{
				string[] splitLine = line.Split(':');
				if(splitLine.Length < 2)
					throw new Exception("Wrong config file format at line " + lineNumber);

				var mod = ModLoader.GetMod(splitLine[0]);
				if(mod == null)
				{
					ErrorLines.Add(line);
					return;
				}
				id = mod.ItemType(splitLine[1]);
				if(id == 0)
				{
					ErrorLines.Add(line);
					return;
				}
				idList.Add(id);
			}
		}

		public static void SetDefaults()
		{
			Reset();
			BlacklistedItemIDs = new List<int>(new int[]
			{
				ItemID.MagicDagger,
				ItemID.PhoenixBlaster
			});
		}

		static void Reset()
		{
			ErrorLines.Clear();
			BlacklistedItemIDs.Clear();
			WhitelistedItemIDs.Clear();
		}

		public static void SaveConfig()
		{
			File.WriteAllLines(ConfigPath, new string[]
			{
				"# This is the OmniSwing configuration file. Please use the /omniSwing command to modify this.",
				"# (or edit this manually if you know what you're doing)",
				"# Each line is a new blacklisted item.",
				"# If it's a Vanilla item, add the item ID directly.",
				"# If it's a modded item, specify its name in this format:",
				"# InternalModName:InternalItemName",
				"# (use WMITF's Internal Name button to see the internal names)",
				"# Any item that starts with an exclamation mark ! will auto-swing even if it's not a weapon (whitelist)",
				"# Anything that comes after a # sign will be ignored (like these comments)",
			});
			foreach(int id in BlacklistedItemIDs)
			{
				if(id < ItemID.Count)
				{
					File.AppendAllLines(ConfigPath, new string[] { id + " # " + Lang.GetItemNameValue(id) });
				}
				else if(id < ItemLoader.ItemCount)
				{
					var item = new Item();
					item.SetDefaults();
					if(item.modItem != null)
					{
						string modName = item.modItem.mod.Name;
						string itemName = item.modItem.Name;
						File.AppendAllLines(ConfigPath, new string[] { modName + ':' + itemName + " # " + Lang.GetItemNameValue(id) });
					}
				}
			}
			foreach(int id in WhitelistedItemIDs)
			{
				if(id < ItemID.Count)
				{
					File.AppendAllLines(ConfigPath, new string[] { "! " + id + " # " + Lang.GetItemNameValue(id) });
				}
				else if(id < ItemLoader.ItemCount)
				{
					var item = new Item();
					item.SetDefaults();
					if(item.modItem != null)
					{
						string modName = item.modItem.mod.Name;
						string itemName = item.modItem.Name;
						File.AppendAllLines(ConfigPath, new string[] { "! " + modName + ':' + itemName + " # " + Lang.GetItemNameValue(id) });
					}
				}
			}
		}

		public static void PrintErrors()
		{
			if(ErrorLines.Count == 0)
				return;
			Main.NewText(Language.GetTextValue("Mods.OmniSwing.ConfigLineError1"), Colors.RarityRed);
			Main.NewText(Language.GetTextValue("Mods.OmniSwing.ConfigLineError2"), Colors.RarityRed);
			Main.NewText(Language.GetTextValue("Mods.OmniSwing.ConfigLineError3"), Colors.RarityRed);
			foreach(var line in ErrorLines)
				Main.NewText(line, Colors.RarityRed);
			Main.NewText(Language.GetTextValue("Mods.OmniSwing.ConfigLineError4"), Colors.RarityRed);
		}

		public class MultiplayerSyncWorld : ModWorld
		{
			public override void NetSend(BinaryWriter writer)
			{
				writer.Write(BlacklistedItemIDs.Count);
				foreach(int id in BlacklistedItemIDs)
					writer.Write(id);

				writer.Write(WhitelistedItemIDs.Count);
				foreach(int id in WhitelistedItemIDs)
					writer.Write(id);
			}

			public override void NetReceive(BinaryReader reader)
			{
				ErrorLines.Clear();

				int count = reader.ReadInt32();
				BlacklistedItemIDs = new List<int>(count);
				for(int i = 0; i < count; i++)
					BlacklistedItemIDs.Add(reader.ReadInt32());

				count = reader.ReadInt32();
				WhitelistedItemIDs = new List<int>(count);
				for(int i = 0; i < count; i++)
					WhitelistedItemIDs.Add(reader.ReadInt32());
			}
		}

		public class MultiplayerSyncPlayer : ModPlayer
		{
			public override void OnEnterWorld(Player player)
			{
				if(ErrorLines.Count > 0)
				{
					PrintErrors();
				}
			}

			public override void PlayerDisconnect(Player player)
			{
				Config.Load();
			}
		}
	}
}
