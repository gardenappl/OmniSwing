using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace OmniSwing
{
	public static class SwingHandler
	{
		public static bool ShouldAutoSwing(Item item)
		{
			return Config.WhitelistedItemIDs.Contains(item.type) ||
				(ShouldAutoSwingDefault(item) && !Config.BlacklistedItemIDs.Contains(item.type));
		}

		public static bool ShouldAutoSwingDefault(Item item)
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

			//public override void SetDefaults(Item item)
			//{
			//	if(ShouldAutoSwing(item))
			//		item.autoReuse = true;
			//}

			bool RealAutoReuseValue = false;
			bool FakeAutoReuse = false;

			public override bool CanUseItem(Item item, Player player)
			{
				if(ShouldAutoSwing(item))
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

			public override GlobalItem NewInstance(Item item)
			{
				return new SwingGlobalItem();
			}

			public override GlobalItem Clone(Item item, Item itemClone)
			{
				return new SwingGlobalItem();
			}
		}

		//spear fix by CrimsHallowHero
		class SwingGlobalProjectile : GlobalProjectile
		{
			public override void AI(Projectile projectile)
			{
				if((projectile.aiStyle == 19 || projectile.aiStyle == 699) &&
					ShouldAutoSwing(Main.player[projectile.owner].HeldItem) &&
					projectile.timeLeft > Main.player[projectile.owner].itemAnimation)
				{
					projectile.timeLeft = Main.player[projectile.owner].itemAnimation;
					projectile.netUpdate = true;
				}
			}
		}
	}
}
