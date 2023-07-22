
using Terraria.ModLoader;

namespace OmniSwing
{
	public class OmniSwing : Mod
	{

		public override void PostSetupContent()
		{
			LegacyConfig.Load();
		}

        public override void Unload()
        {
			LegacyConfig.Unload();
        }
    }
}