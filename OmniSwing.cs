
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
					Config.WhitelistedItemIDs.Remove(itemID);
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
	}
}