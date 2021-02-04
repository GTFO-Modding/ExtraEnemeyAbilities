using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enemies;
using ExtraEnemyAbilities.Components;
using GameData;
using HarmonyLib;
using UnityEngine;

namespace ExtraEnemyAbilities.Patches
{
    [HarmonyPatch(typeof(EnemyPrefabManager), "GenerateEnemy")]
    public class Patch_BuildEnemyPrefab
    {
        public static void Postfix(EnemyDataBlock data)
        {
            //if (data.persistentID == 303)
            //{
            //    Log.Debug($"Created bleeder type enemy with name of {data.name}");
            //    GameObject BleederPrefab = EnemyPrefabManager.Current.m_enemyPrefabs[data.persistentID];
            //    BleederPrefab.AddComponent<BleederBase>();
            //}

            if (ConfigManager.ExploderConfigDictionary.ContainsKey(data.persistentID))
            {
                Log.Debug($"Created exploder type enemy with name of '{data.name}'");
                GameObject ExploderPrefab = EnemyPrefabManager.Current.m_enemyPrefabs[data.persistentID];
                ExploderPrefab.AddComponent<ExploderBase>();
            }
        }
    }
}
