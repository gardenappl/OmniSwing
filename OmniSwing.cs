
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
		//Hamstar's Mod Helpers integration
		public static string GithubUserName { get { return "goldenapple3"; } }
		public static string GithubProjectName { get { return "OmniSwing"; } }
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
