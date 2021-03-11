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

            //This should probably be some sort of generic

            if (ConfigManager.ExploderConfigDictionary != null)
            {
                if (ConfigManager.ExploderConfigDictionary.ContainsKey(data.persistentID))
                {
                    Log.Debug($"Created exploder type enemy with name of '{data.name}'");
                    GameObject ExploderPrefab = EnemyPrefabManager.Current.m_enemyPrefabs[data.persistentID];
                    ExploderPrefab.AddComponent<ExploderAbility>();
                    return;
                }
            }

            if (ConfigManager.EMPConfigDictionary != null)
            {
                if (ConfigManager.EMPConfigDictionary.ContainsKey(data.persistentID))
                {
                    GameObject EMPPrefab = EnemyPrefabManager.Current.m_enemyPrefabs[data.persistentID];
                    EMPPrefab.AddComponent<EMPAbility>();
                    Log.Debug($"Created EMP type enemy with name of '{data.name}'");
                    return;
                }
            }
        }
    }
}
