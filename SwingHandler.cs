using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace OmniSwing
{
	public static class SwingHandler
	{
		public static bool ShouldForceAutoSwing(Item item)
		{
			if (ModContent.GetInstance<Config>().Whitelist.Contains(new ItemDefinition(item.type)))
				return true;
			else if (ModContent.GetInstance<Config>().Blacklist.Contains(new ItemDefinition(item.type)))
				return false;

			if (ModContent.GetInstance<Config>().EnableWeapons)
				return ShouldForceAutoSwingDefault(item);
			else
				return false;
		}

		static bool ShouldForceAutoSwingDefault(Item item)
		{
			Projectile ShotProj = new Projectile();
			ShotProj.SetDefaults(item.shoot);
			if (item.damage <= 0 || (item.DamageType == DamageClass.Summon && ShotProj.aiStyle != 165) || item.sentry || item.channel)
				return false;

			return ShotProj.aiStyle != 9; // return true if not magic missile
		}

		class SwingGlobalItem : GlobalItem
		{
            public override bool? CanAutoReuseItem(Item item, Player player)
            {
				if (ShouldForceAutoSwing(item))
					return true;

                return null;
            }
        }

		//spear fix by CrimsHallowHero
		class SwingGlobalProjectile : GlobalProjectile
		{
			public override void AI(Projectile projectile)
			{
				if((projectile.aiStyle == 19 || projectile.aiStyle == 699) &&
					ShouldForceAutoSwing(Main.player[projectile.owner].HeldItem) &&
					projectile.timeLeft > Main.player[projectile.owner].itemAnimation)
				{
					projectile.timeLeft = Main.player[projectile.owner].itemAnimation;
					projectile.netUpdate = true;
				}
			}
		}
	}
}
