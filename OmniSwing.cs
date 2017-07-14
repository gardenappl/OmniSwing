
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OmniSwing
{
	public class OmniSwing : Mod
	{
//		public static List<int> Spears = new List<int>();
		
//		public override void PostSetupContent()
//		{
//			var bindFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
//			var field = typeof(ItemLoader).GetField("nextItem", bindFlags);
//			int itemCount = ItemID.Count;
//			if(field != null)
//			{
//				object o = field.GetValue(null);
//				if(o is int)
//				{
//					itemCount = (int)o;
//				}
//			}
//			else
//			{
//				ErrorLogger.Log("[OmniSwing] ItemLoader.nextItem not found! Report this to the mod author!");
//				ErrorLogger.Log("[OmniSwing] (as a result, modded spears will not work)");
//			}
//			for(int i = 0; i < itemCount; i++)
//			{
//				var item = new Item();
//				item.SetDefaults(i);
//				if(item.shoot > 0)
//				{
//					var projectile = new Projectile();
//					projectile.SetDefaults(item.shoot);
//					if(projectile.aiStyle == 19)
//					{
//						Spears.Add(item.type);
//					}
//				}
//			}
//		}
//
//		public override void Unload()
//		{
//			Spears.Clear();
//		}
	}
	
	class SwingGlobalItem : GlobalItem
	{
		public override void SetDefaults(Item item)
		{
			if(ShouldAutoSwing(item))
				item.autoReuse = true;
		}
		
		public override bool CanUseItem(Item item, Player player)
		{
			if(ShouldAutoSwing(item))
				item.autoReuse = true;
			return base.CanUseItem(item, player);
		}
		
		static bool ShouldAutoSwing(Item item)
		{
			try
			{
				if(item.damage <= 0 || item.summon || item.sentry)
					return false;
				
				if(item.shoot > 0)
				{
					var projectile = new Projectile();
					projectile.SetDefaults(item.shoot);
					//Magic Missile-type projectiles get buggy with auto-swing
					return projectile.aiStyle != 9;
				}
				return true;
			}
			catch(NullReferenceException e)
			{
				return false;
			}
		}
	}
	
	//spear fix by CrimsHallowHero
	class SwingGlobalProjectile : GlobalProjectile
	{
		public override void AI(Projectile projectile)
		{
			if((projectile.aiStyle == 19 || projectile.aiStyle == 699) && projectile.timeLeft > Main.player[projectile.owner].itemAnimation)
			{
				projectile.timeLeft = Main.player[projectile.owner].itemAnimation;
				projectile.netUpdate = true;
			}
		}
	}
}
