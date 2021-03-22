using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enemies;
using ExtraEnemyAbilities.Components;
using ExtraEnemyAbilities.Components.Abilities;
using GameData;
using HarmonyLib;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace ExtraEnemyAbilities.Patches
{
    [HarmonyPatch(typeof(EnemyPrefabManager), "GenerateEnemy")]
    public class Patch_BuildEnemyPrefab_Generic
    {
        public static void Postfix(EnemyDataBlock data)
        {
            if (ConfigManager.GetAbility(data.persistentID, out Il2CppSystem.Type ability))
            {
                Log.Debug($"Creating '{ability.Name}' type enemy with name of '{data.name}'");

                GameObject enemyPrefab = EnemyPrefabManager.Current.m_enemyPrefabs[data.persistentID];
                enemyPrefab.AddComponent(ability);
            }
        }
    }
}
