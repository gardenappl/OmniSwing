using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace OmniSwing
{
	public static class SwingHandler
	{
		public static bool ShouldForceAutoSwing(Item item)
		{
			if (Config.Instance.Whitelist.Contains(new ItemDefinition(item.type)))
				return true;
			else if (Config.Instance.Blacklist.Contains(new ItemDefinition(item.type)))
				return false;

			if (Config.Instance.EnableWeapons)
				return ShouldForceAutoSwingDefault(item);
			else
				return false;
		}

		static bool ShouldForceAutoSwingDefault(Item item)
		{
			if(item.damage <= 0 || item.summon || item.sentry)
				return false;

			if(item.shoot > 0)
			{
				var projectile = new Projectile();
				projectile.SetDefaults(item.shoot);
				return projectile.aiStyle != 9;
			}

			return true;
		}

		class SwingGlobalItem : GlobalItem
		{
			public override bool InstancePerEntity { get { return true; } }

			public override bool CloneNewInstances => true;

			bool RealAutoReuseValue = false;
			bool FakeAutoReuse = false;

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
