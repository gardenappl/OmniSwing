
using Terraria.ModLoader;

namespace OmniSwing
{
	public class OmniSwing : Mod
	{
		public static OmniSwing Instance;

		public override void Load()
		{
			Instance = this;	
		}

		public override void PostSetupContent()
		{
			LegacyConfig.Load();
		}
	}
}