﻿using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Enemies;
using ExtraEnemyAbilities.Components;
using System;
using UnhollowerRuntimeLib;
using ExtraEnemyAbilities.Components.Abilities;

namespace ExtraEnemyAbilities
{
	//This is such a mess
    public static class ConfigManager
    {
		public static bool HasLocalPath;
		public static string LocalPath;
		public static Dictionary<uint, ExploderConfig> ExploderConfigDictionary { get { return ConfigHolder.ExploderConfigs; } }
		public static Dictionary<uint, EMPConfig> EMPConfigDictionary { get { return ConfigHolder.EMPConfigs; } }
		public static Dictionary<uint, ImmortalConfig> ImmortalConfigDictionary { get { return ConfigHolder.ImmortalConfigs; } }

		private static ConfigHolder ConfigHolder;

		static ConfigManager()
        {
			if (!MTFO.Managers.ConfigManager.HasCustomContent) return;

			var exploderAbilities = new Dictionary<uint, ExploderConfig>
			{
				{ 0, new ExploderConfig() }
            };

			var empAbilities = new Dictionary<uint, EMPConfig>
			{
				{ 0, new EMPConfig() }
			};

			var imortalConfigs = new Dictionary<uint, ImmortalConfig>
			{
				{ 0, new ImmortalConfig() }
			};

			ConfigHolder = new ConfigHolder() { ExploderConfigs = exploderAbilities, EMPConfigs = empAbilities, ImmortalConfigs = imortalConfigs };
			string customContentPath = MTFO.Managers.ConfigManager.CustomPath;

			//Migrate old config
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
        }

		public static bool GetAbility(uint id, out Il2CppSystem.Type ability)
        {
			try
            {
				if (ExploderConfigDictionary.ContainsKey(id))
				{
					ability = Il2CppType.Of<ExploderAbility>();
					return true;
				}

				if (EMPConfigDictionary.ContainsKey(id))
				{
					ability = Il2CppType.Of<EMPAbility>();
					return true;
				}

				if (ImmortalConfigDictionary.ContainsKey(id))
				{
					ability = Il2CppType.Of<ImmortalAbility>();
					return true;
				}
			}
			catch
            {
				ability = null;
				return false;
			}

			ability = null;
			return false;
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
		public Dictionary<uint, ImmortalConfig> ImmortalConfigs;
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

	public struct ImmortalConfig
    {
		public bool Immortal;
    }

	public struct CloakConfig
    {
		public float Duration;
		public uint SFXID;
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
