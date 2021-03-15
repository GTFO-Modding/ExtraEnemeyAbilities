using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtraEnemyAbilities.Components
{
    public class EMPAbility : CustomAbility
    {
        public EMPAbility(IntPtr intPtr) : base(intPtr)
        {
        }

        private EMPConfig EMPConfig;
        private int m_lastAnimIndex;

        public void Awake()
        {
            GlowColor = EMPComponent.EMPColor;
            Agent = GetComponent<EnemyAgent>();
            EMPConfig = ConfigManager.EMPConfigDictionary[Agent.EnemyDataID];
        }

        public void Update()
        {
            if (Agent.Alive)
            {
                Agent.Appearance.InterpolateGlow(GlowColor, new Vector4(0f, 1.25f, 0f, 1.5f), 1f);
            } else
            {
                Agent.Appearance.InterpolateGlow(Color.black, new Vector4(0f, 1.25f, 0f, 1.5f), 2f);
                Destroy(this);
            }
        }

        public override bool Trigger()
        {
            //GameObject EMPObject = new GameObject();
            //
            //EMPObject.AddComponent<EMPComponent>();
            //EMPComponent emp = EMPObject.GetComponent<EMPComponent>();
            //emp.EnemyAgent = Agent;
            //emp.EMPConfig = EMPConfig;
            //emp.Trigger();
            //m_locomotion.GetUniqueAnimIndex(EnemyLocomotion.s_hashAbilityUse, ref m_lastAnimIndex);
            //Agent.Locomotion.GetUniqueAnimIndex(EnemyLocomotion.s_hashAbilityUse, ref m_lastAnimIndex);

            Agent.AI.m_navMeshAgent.velocity = Vector3.zero;
            if (Agent.AI.m_navMeshAgent.isOnNavMesh)
            {
                Agent.AI.m_navMeshAgent.isStopped = true;
            }
            Agent.AI.m_abilities.CanTriggerAbilities = false;
            ExtraEnemyAbilities.log.LogDebug(Agent.AI.m_abilities.CanTriggerAbilities);

            //Agent.Locomotion.m_animator.CrossFadeInFixedTime(EnemyLocomotion.s_hashAbilityUse[0], 1f);
            return false;
        }
    }
}
