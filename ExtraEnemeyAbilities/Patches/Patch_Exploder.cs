using AIGraph;
using AK;
using Enemies;
using ExtraEnemyAbilities;
using ExtraEnemyAbilities.Components;
using ExtraEnemyAbilities.Utilities;
using HarmonyLib;
using SNetwork;
using UnityEngine;

namespace Offshoot.Components
{
    [HarmonyPatch(typeof(EAB_FogSphere), "DoTrigger")]
    public class Patch_FogSphere
    {
        public static bool Prefix(EAB_FogSphere __instance)
        {

            if (!ConfigManager.ExploderConfigDictionary.ContainsKey(__instance.m_owner.EnemyDataID)) return true;            
            if (SNet.IsMaster)
            {
                ExploderBase exploderBase = __instance.m_owner.GetComponent<ExploderBase>();
                exploderBase.Explode();
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(ES_TriggerFogSphere), "CommonUpdate")]
    public class Patch_TriggerFogShphere
    {
        public static bool Prefix(ES_TriggerFogSphere __instance)
        {
            if (!ConfigManager.ExploderConfigDictionary.ContainsKey(__instance.m_enemyAgent.EnemyDataID)) return true;
            __instance.m_fogSphereAbility.DoTrigger();
            __instance.m_machine.ChangeState((int)ES_StateEnum.Dead);
            return false;
        }
    }
}
