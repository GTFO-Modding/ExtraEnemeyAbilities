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
using UnityEngine;

namespace ExtraEnemyAbilities.Patches
{
    [HarmonyPatch(typeof(EnemyPrefabManager), "GenerateEnemy")]
    public class Patch_BuildEnemyPrefab
    {
        public static void Postfix(EnemyDataBlock data)
        {
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
            if (data.persistentID == 303)
            {
                Log.Debug($"Created brap type enemy with name of '{data.name}'");
                GameObject EMPPrefab = EnemyPrefabManager.Current.m_enemyPrefabs[data.persistentID];
                EMPPrefab.AddComponent<DamageCloudAbilitity>();
                return;
            }
#if TAB
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
#endif
        }
    }
}
