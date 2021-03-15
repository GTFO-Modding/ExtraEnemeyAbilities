using Enemies;
using ExtraEnemyAbilities.Components;
using HarmonyLib;

namespace ExtraEnemyAbilities.Patches
{
    [HarmonyPatch(typeof(ES_Hibernate), "UpdateDetectionAnim")]
    class Patch_ES_Hibernate
    {
        //This is fucking awful
        static void Prefix(ES_Hibernate __instance)
        {
            var customAbility = __instance.m_enemyAgent.GetComponent<CustomAbility>();
            if (customAbility != null)
            {
                __instance.m_detectingColorVec = customAbility.GlowColor;
                __instance.m_heartbeatColorVec = customAbility.GlowColor;
            }
        }
    }
}
