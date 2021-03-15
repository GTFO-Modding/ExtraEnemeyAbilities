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
    //[HarmonyPatch(typeof(EAB_FogSphere), "DoTrigger")]
    //public class Patch_FogSphere
    //{
    //    public static bool Prefix(EAB_FogSphere __instance)
    //    {
    //        CustomAbility customAbility = __instance.m_owner.GetComponent<CustomAbility>();
    //        if (customAbility == null)
    //        {
    //            return true;
    //        }
    //        var runOrig = customAbility.Trigger(__instance.m_machine);
    //        return runOrig;
    //    }
    //}

    [HarmonyPatch(typeof(ES_TriggerFogSphere), "CommonUpdate")]
    public class Patch_TriggerFogShphere
    {
        public static bool Prefix(ES_TriggerFogSphere __instance)
        {
            
            CustomAbility customAbility = __instance.m_enemyAgent.GetComponent<CustomAbility>();

            if (customAbility == null)
            {
                return true;
            }

            if (customAbility.Activated == false)
            {
                return customAbility.Trigger();
            }

            //This needs to be changed somehow
            //if (!ConfigManager.CustomIDs.Contains(__instance.m_enemyAgent.EnemyDataID)) return true;
            //__instance.m_fogSphereAbility.DoTrigger();
            //
            //ES_StateEnum endState = ConfigManager.GetEndState(__instance.m_enemyAgent.EnemyDataID);
            //__instance.m_machine.ChangeState((int)endState);
            return false;
        }
    }
}
