using Enemies;
using ExtraEnemyAbilities.Components;
using HarmonyLib;

namespace ExtraEnemyAbilities.Patches
{
    [HarmonyPatch(typeof(ES_Hibernate), "UpdateDetectionAnim")]
    class Patch_ES_Hibernate
    {
        static void Prefix(ES_Hibernate __instance)
        {
            var exploderBase = __instance.m_enemyAgent.GetComponent<ExploderAbility>();
            if (exploderBase != null)
            {
                __instance.m_detectingColorVec = exploderBase.glowColor;
                __instance.m_heartbeatColorVec = exploderBase.glowColor;
            }
        }
    }
}
