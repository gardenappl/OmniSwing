
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OmniSwing
{
	public class OmniSwing : Mod
	{
		public override void PostSetupContent()
		{
			Config.Load();
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			if(Main.netMode == NetmodeID.Server)
				return;

			var messageType = (MessageType)reader.ReadByte();
			int itemID = reader.ReadInt32();
			switch(messageType)
			{
				case MessageType.BlacklistItemAdd:
					Config.BlacklistedItemIDs.Add(itemID);
					break;
				case MessageType.WhitelistItemAdd:
					Config.WhitelistedItemIDs.Add(itemID);
					break;
				case MessageType.BlacklistItemRemove:
					Config.BlacklistedItemIDs.Remove(itemID);
					break;
				case MessageType.WhitelistItemRemove:
					Config.WhitelistedItemIDs.Add(itemID);
					break;
			}
		}

		public enum MessageType : byte
		{
			BlacklistItemAdd = 0,
			WhitelistItemAdd = 1,
			BlacklistItemRemove = 2,
			WhitelistItemRemove = 3
		}

		#region Hamstar's Mod Helpers integration

		public static string GithubUserName { get { return "goldenapple3"; } }
		public static string GithubProjectName { get { return "OmniSwing"; } }

		public static string ConfigFileRelativePath { get { return "Mod Configs/OmniSwing.txt"; } }

		public static void ReloadConfigFromFile()
		{
			Config.Load();
			Config.PrintErrors();
		}

		public static void ResetConfigFromDefaults()
		{
			Config.SetDefaults();
			Config.SaveConfig();
		}

		#endregion

		public static void Log(object message)
		{
			ErrorLogger.Log(String.Format("[OmniSwing][{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), message));
		}

		public static void Log(string message, params object[] formatData)
		{
			ErrorLogger.Log(String.Format("[OmniSwing][{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), String.Format(message, formatData)));
		}
	}
}