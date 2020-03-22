
using Terraria.ModLoader;

namespace OmniSwing
{
	public class OmniSwing : Mod
	{

		public override void Load()
		{

		}

		public override void PostSetupContent()
		{
			LegacyConfig.Load();
		}
	}
}