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
            var dic = new Dictionary<uint, ExploderConfig>
            {
                { 0, new ExploderConfig() { Damage = 0, NoiseMax = 0, NoiseMin = 0, Radius = 0 } }
            };
            ExploderConfigHolder = new ExploderConfigHolder() { ExploderConfigs = dic };
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
		public bool ExplodeOnDeath;
		public ColorData ColorData;

        public ExploderConfig(float radius = 0, float damage = 0, float noiseMin = 0, float noiseMax = 0, float infectionAmount = 0, bool explodeOnDeath = true, ColorData colorData = new ColorData())
        {
            Radius = radius;
            Damage = damage;
            NoiseMin = noiseMin;
            NoiseMax = noiseMax;
            InfectionAmount = infectionAmount;
            ExplodeOnDeath = explodeOnDeath;
            ColorData = colorData;
        }
    }

    public struct ColorData
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public ColorData(float r = 1, float g = 0, float b = 0, float a = 1)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
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
