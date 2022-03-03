using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;
using Terraria.ModLoader;

namespace OmniSwing
{
	static class LegacyConfig
	{
		static List<ItemDefinition> BlacklistedItemIDs = new List<ItemDefinition>();
		static List<ItemDefinition> WhitelistedItemIDs = new List<ItemDefinition>();

		static string ConfigFolderPath = Path.Combine(Main.SavePath, "Mod Configs");
		static string ConfigPath = Path.Combine(ConfigFolderPath, "OmniSwing.txt");

		public static void Load()
		{
			if(File.Exists(ConfigPath))
			{
                ModContent.GetInstance<OmniSwing>().Logger.Warn("Found legacy config file. Reading...");
				ReadLegacyConfig();

                ModContent.GetInstance<OmniSwing>().Logger.Warn("Migrating to new version");
				MigrateToNewFormat();

                ModContent.GetInstance<OmniSwing>().Logger.Warn("Migrated, deleting old config file.");
				File.Delete(ConfigPath);
			}
		}

		static void ReadLegacyConfig()
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

		static void ParseConfigLine(string line, int lineNumber, List<ItemDefinition> idList)
		{
			int id = 0;
			if(Int32.TryParse(line, out id))
			{
				if(id <= 0 || id >= ItemID.Count)
					throw new Exception(id + " is not a valid Vanilla Item ID!");
				idList.Add(new ItemDefinition(id));
			}
			else
			{
				string[] splitLine = line.Split(':');
				if(splitLine.Length < 2)
					throw new Exception("Wrong OmniSwing.txt config format at line " + lineNumber);

				string modName = splitLine[0];
				string itemName = splitLine[1];
				idList.Add(new ItemDefinition(modName, itemName));
			}
		}

		static void MigrateToNewFormat()
		{
			string newConfigPath = Path.Combine(ConfigManager.ModConfigPath,
					nameof(OmniSwing) + '_' + nameof(Config) + ".json");

			var newConfig = new
			{
				Whitelist = WhitelistedItemIDs,
				Blacklist = BlacklistedItemIDs
			};

			string json = JsonConvert.SerializeObject(newConfig, ConfigManager.serializerSettings);
			File.WriteAllText(newConfigPath, json);
		}

		public static void Unload()
        {
			BlacklistedItemIDs.Clear();
			BlacklistedItemIDs = null;
			WhitelistedItemIDs.Clear();
			WhitelistedItemIDs = null;
			ConfigFolderPath = null;
			ConfigPath = null;
        }
	}
}
