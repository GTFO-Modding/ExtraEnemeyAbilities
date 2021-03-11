﻿using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Enemies;

namespace ExtraEnemyAbilities
{
    public static class ConfigManager
    {
		public static bool HasLocalPath;
		public static string LocalPath;
		public static Dictionary<uint, ExploderConfig> ExploderConfigDictionary { get { return ConfigHolder.ExploderConfigs; } }
		public static Dictionary<uint, EMPConfig> EMPConfigDictionary { get { return ConfigHolder.EMPConfigs; } }
		public static List<uint> CustomIDs;

		private static ConfigHolder ConfigHolder;

		static ConfigManager()
        {
			if (!MTFO.Managers.ConfigManager.HasCustomContent) return;
			CustomIDs = new List<uint>();

			var exploderAbilities = new Dictionary<uint, ExploderConfig>
			{
				{ 0, new ExploderConfig() }
            };
			var empAbilities = new Dictionary<uint, EMPConfig>
			{
				{ 0, new EMPConfig() }
			};
            ConfigHolder = new ConfigHolder() { ExploderConfigs = exploderAbilities, EMPConfigs = empAbilities };
			string customContentPath = MTFO.Managers.ConfigManager.CustomPath;

			//Setup Exploder Config
			MigrateOldConfig(customContentPath);

			string customAbilityConfig = Path.Combine(customContentPath, "AbilityConfig.json");

			if (File.Exists(customAbilityConfig))
            {
				Log.Debug("Loading from disk");
				ConfigHolder = JsonConvert.DeserializeObject<ConfigHolder>(File.ReadAllText(customAbilityConfig));
				Log.Debug(File.ReadAllText(customAbilityConfig));
            } else
            {
				Log.Debug("Writing to disk");
				File.WriteAllText(customAbilityConfig, JsonConvert.SerializeObject(ConfigHolder));
            }

			if (ExploderConfigDictionary != null)
            {
				CustomIDs.AddRange(ExploderConfigDictionary.Keys);
            }
			if (EMPConfigDictionary != null)
            {
				CustomIDs.AddRange(EMPConfigDictionary.Keys);
            }
        }

		public static ES_StateEnum GetEndState(uint ID)
        {
			if (ExploderConfigDictionary != null)
            {
				if (ExploderConfigDictionary.ContainsKey(ID))
                {
					return ES_StateEnum.Dead;
                }
            }


			return ES_StateEnum.PathMove;
        }

		private static void MigrateOldConfig(string customContentPath)
		{
			string newPath = Path.Combine(customContentPath, "AbilityConfig.json");

			string oldPath = Path.Combine(customContentPath, "ExploderConfig.json");

			if (File.Exists(oldPath))
			{
				Log.Message("Updating old config");
				File.Move(oldPath, newPath);
			}
		}
	}

	public struct ConfigHolder
	{
		public Dictionary<uint, ExploderConfig> ExploderConfigs;
		public Dictionary<uint, EMPConfig> EMPConfigs;
	}

	public struct EMPConfig
    {
		public float Radius;
		public float Duration;
    }

	public struct ExploderConfig
    {
		public float Radius;
		public float Damage;
		public float NoiseMin;
		public float NoiseMax;
		public float InfectionAmount;
		public bool NoExplosionOnDeath;
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
