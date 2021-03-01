using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ExtraEnemyAbilities
{
    public static class ConfigManager
    {
		public static bool HasLocalPath;
		public static string LocalPath;
		public static Dictionary<uint, ExploderConfig> ExploderConfigDictionary { get { return ExploderConfigHolder.ExploderConfigs; } }

		private static ExploderConfigHolder ExploderConfigHolder;



		static ConfigManager()
        {
			var Dictionary = new Dictionary<uint, ExploderConfig>
			{
				{ 0, new ExploderConfig() { Radius = 0, Damage = 0, NoiseMin = 0, NoiseMax = 0, InfectionAmount = 0, NoExplosionOnDeath = false, EMPEnabled = false, EMPRange = 0, EMPDuration = 0, ColorData = new ColorData() { a = 0, r = 0, g = 0, b = 0 } } }
            };
            ExploderConfigHolder = new ExploderConfigHolder() { ExploderConfigs = Dictionary };
			string customContentPath = MTFO.Managers.ConfigManager.CustomPath;

			//Setup Exploder Config
			string exploderPath = Path.Combine(customContentPath, "ExploderConfig.json");
			if (File.Exists(exploderPath))
            {
				Log.Debug("Loading from disk");
				ExploderConfigHolder = JsonConvert.DeserializeObject<ExploderConfigHolder>(File.ReadAllText(exploderPath));
				Log.Debug(File.ReadAllText(exploderPath));
            } else
            {
				Log.Debug("Writing to disk");
				File.WriteAllText(exploderPath, JsonConvert.SerializeObject(ExploderConfigHolder));
            }
        }
	}

	public struct ExploderConfigHolder
	{
		public Dictionary<uint, ExploderConfig> ExploderConfigs;
	}

	public struct ExploderConfig
    {
		public float Radius;
		public float Damage;
		public float NoiseMin;
		public float NoiseMax;
		public float InfectionAmount;
		public bool NoExplosionOnDeath;
        public bool EMPEnabled;
        public float EMPRange;
        public float EMPDuration;   
		public ColorData ColorData;
    }

    public struct ColorData
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }

	public struct FogSphereConfigHolder
    {
		public Dictionary<uint, FogSphereConfig> FogSphereConfigs;
    }

	public struct FogSphereConfig
    {
		public float Radius;
    }
}
