using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OmniSwing
{
	public class BlacklistCommand : ModCommand
	{
		public override string Command { get { return "omniSwing"; } }

		public override CommandType Type { get { return CommandType.World; } }

		public override string Description { get { return Language.GetTextValue("Mods.OmniSwing.CommandDescription"); } }

		public override string Usage { get { return "/omniSwing"; } }

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if(Main.netMode == NetmodeID.MultiplayerClient || caller.Player == null)
			{
				caller.Reply(Language.GetTextValue("Mods.OmniSwing.CommandServerOnly"));
			}
			if(caller.Player.HeldItem != null && caller.Player.HeldItem.stack > 0 && caller.Player.HeldItem.type > 0)
			{
				var item = caller.Player.HeldItem;
				if(SwingHandler.ShouldAutoSwingDefault(item))
				{
					if(Config.BlacklistedItemIDs.Contains(item.type))
					{
						Config.BlacklistedItemIDs.Remove(item.type);
						caller.Reply(Language.GetTextValue("Mods.OmniSwing.CommandBlacklistRemoved", item.Name));
						if(Main.netMode != NetmodeID.SinglePlayer)
						{
							var message = mod.GetPacket();
							message.Write((byte)OmniSwing.MessageType.BlacklistItemRemove);
							message.Write(item.type);
							message.Send();
						}
					}
					else
					{
						Config.BlacklistedItemIDs.Add(item.type);
						caller.Reply(Language.GetTextValue("Mods.OmniSwing.CommandBlacklistAdded", item.Name));
						if(Main.netMode != NetmodeID.SinglePlayer)
						{
							var message = mod.GetPacket();
							message.Write((byte)OmniSwing.MessageType.BlacklistItemAdd);
							message.Write(item.type);
							message.Send();
						}
					}
				}
				else
				{
					if(Config.WhitelistedItemIDs.Contains(item.type))
					{
						Config.WhitelistedItemIDs.Remove(item.type);
						caller.Reply(Language.GetTextValue("Mods.OmniSwing.CommandWhitelistRemoved", item.Name));
						if(Main.netMode != NetmodeID.SinglePlayer)
						{
							var message = mod.GetPacket();
							message.Write((byte)OmniSwing.MessageType.WhitelistItemRemove);
							message.Write(item.type);
							message.Send();
						}
					}
					else
					{
						Config.WhitelistedItemIDs.Add(item.type);
						caller.Reply(Language.GetTextValue("Mods.OmniSwing.CommandWhitelistAdded", item.Name));
						if(Main.netMode != NetmodeID.SinglePlayer)
						{
							var message = mod.GetPacket();
							message.Write((byte)OmniSwing.MessageType.WhitelistItemAdd);
							message.Write(item.type);
							message.Send();
						}
					}
				}
				Config.SaveConfig();
			}
			else
			{
				caller.Reply(Language.GetTextValue("Mods.OmniSwing.CommandUsage"), Colors.RarityRed);
			}
		}
	}
}
