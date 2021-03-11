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
            CustomAbility customAbility = __instance.m_owner.GetComponent<CustomAbility>();
            if (customAbility == null)
            {
                Log.Debug("Triggered vanilla ability");
                return true;
            }
            customAbility.Trigger();
            Log.Debug("Triggered custom ability");
            return false;
        }
    }

    [HarmonyPatch(typeof(ES_TriggerFogSphere), "CommonUpdate")]
    public class Patch_TriggerFogShphere
    {
        public static bool Prefix(ES_TriggerFogSphere __instance)
        {
            //This needs to be changed somehow
            if (!ConfigManager.CustomIDs.Contains(__instance.m_enemyAgent.EnemyDataID)) return true;
            __instance.m_fogSphereAbility.DoTrigger();

            ES_StateEnum endState = ConfigManager.GetEndState(__instance.m_enemyAgent.EnemyDataID);
            __instance.m_machine.ChangeState((int)endState);
            return false;
        }
    }
}
