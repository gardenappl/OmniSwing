using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using Terraria.ID;

namespace OmniSwing
{
	[Label("$Mods.OmniSwing.Config")]
	public class Config : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Label("$Mods.OmniSwing.Config.EnableWeapons")]
		[Tooltip("$Mods.OmniSwing.Config.EnableWeapons.Description")]
		[DefaultValue(true)]
		public bool EnableWeapons;

		[Label("$Mods.OmniSwing.Config.Whitelist")]
		[Tooltip("$Mods.OmniSwing.Config.Whitelist.Description")]
		public List<ItemDefinition> Whitelist = new List<ItemDefinition>();

		[Label("$Mods.OmniSwing.Config.Blacklist")]
		[Tooltip("$Mods.OmniSwing.Config.Blacklist.Description")]
		public List<ItemDefinition> Blacklist = new List<ItemDefinition>
				{
					new ItemDefinition(ItemID.MagicDagger),
					new ItemDefinition(ItemID.PhoenixBlaster)
				};
	}
}
