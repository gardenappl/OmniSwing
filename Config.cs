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
	public class Config : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[DefaultValue(true)]
		public bool EnableWeapons;

		public List<ItemDefinition> Whitelist = new List<ItemDefinition>();

		public List<ItemDefinition> Blacklist = new List<ItemDefinition>
				{
					new ItemDefinition(ItemID.MagicDagger),
					new ItemDefinition(ItemID.PhoenixBlaster)
				};
	}
}
