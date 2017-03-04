
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
		
		public OmniSwing()
		{
			Properties = new ModProperties();
		}
		
		public override void Load()
		{
			AddGlobalItem("SwingGlobalItem", new SwingGlobalItem());
		}
		
		//please don't kill me for this
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
		
		class SwingGlobalItem : GlobalItem
		{
			public override void SetDefaults(Item item)
			{
				if(item.damage > 0 && ShouldAutoSwing(item))
				{
					item.autoReuse = true;
				}
			}
			
			static bool ShouldAutoSwing(Item item)
			{
				if(item.channel)
				{
					return false;
				}
				if(item.shoot > 0)
				{
					var projectile = new Projectile();
					projectile.SetDefaults(item.shoot);
					//Spears and Magic Missile-type projectiles get buggy with auto-swing
					return projectile.aiStyle != 9 && projectile.aiStyle != 19;
				}
				return true;
			}
		}
	}
}
