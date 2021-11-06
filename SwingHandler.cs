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

			bool RealAutoReuseValue = false;
			bool FakeAutoReuse = false;

			public override bool InstancePerEntity => true;

            public override GlobalItem Clone(Item item, Item clonedItem)
			{
				SwingGlobalItem clone = (SwingGlobalItem)base.Clone(item, clonedItem);
				clone.RealAutoReuseValue = false;
				clone.FakeAutoReuse = false;
				return clone;
			}

			public override bool CanUseItem(Item item, Player player)
			{
				if(ShouldForceAutoSwing(item))
				{
					if(!FakeAutoReuse)
					{
						RealAutoReuseValue = item.autoReuse;
						FakeAutoReuse = true;
					}
					item.autoReuse = true;
				}
				else
				{
					if(FakeAutoReuse)
					{
						item.autoReuse = RealAutoReuseValue;
						FakeAutoReuse = false;
					}
				}
				return base.CanUseItem(item, player);
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
