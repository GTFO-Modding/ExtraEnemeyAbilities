using AK;
using Enemies;
using StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtraEnemyAbilities.Components.Abilities
{
    public class EMPAbility : CustomAbility
    {
        public EMPAbility(IntPtr intPtr) : base(intPtr)
        {
        }

        private EMPConfig EMPConfig;
        private EMPState state;
        private float stateTimer;
        private enum EMPState
        {
            Inactive,
            Build,
            Activate,
            Cooldown
        }

        private void ResetState()
        {
            state = EMPState.Inactive;
            Activated = false;
            Agent.AI.m_navMeshAgent.isStopped = false;
            Agent.AI.m_abilities.CanTriggerAbilities = true;
        }

        private void TriggerEMP()
        {
            GameObject EMPObject = new GameObject();
            EMPObject.AddComponent<EMPComponent>();
            EMPComponent emp = EMPObject.GetComponent<EMPComponent>();
            emp.EnemyAgent = Agent;
            emp.EMPConfig = EMPConfig;
            emp.Trigger();
        }

        public void Awake()
        {
            GlowColor = EMPComponent.EMPColor;
            Agent = GetComponent<EnemyAgent>();
            EMPConfig = ConfigManager.EMPConfigDictionary[Agent.EnemyDataID];
        }

        public void Update()
        {
            if (Agent.Locomotion.CurrentStateEnum != ES_StateEnum.TriggerFogSphere && Activated == true)
            {
                ResetState();
            }


            switch(state)
            {
                case EMPState.Inactive:
                    Agent.Appearance.InterpolateGlow(GlowColor, new Vector4(0f, 1.25f, 0f, 1.5f), 1f);
                    break;

                case EMPState.Build:
                    Agent.AI.m_navMeshAgent.velocity = Vector3.zero;
                    if (Agent.AI.m_navMeshAgent.isOnNavMesh)
                    {
                        Agent.AI.m_navMeshAgent.isStopped = true;
                    }
                    Agent.AI.m_abilities.CanTriggerAbilities = false;

                    Agent.Appearance.InterpolateGlow(GlowColor * 5, new Vector4(0f, 1.25f, 0f, 1.5f), 1f);

                    Agent.Locomotion.m_animator.CrossFadeInFixedTime(EnemyLocomotion.s_hashAbilityUse[0], 2f);
                    Agent.Voice.PlayVoiceEvent(EVENTS.INFECTION_SPITTER_PRIMED);

                    state = EMPState.Activate;
                    stateTimer = Clock.Time + 2f;
                    break;

                case EMPState.Activate:
                    if (stateTimer < Clock.Time)
                    {
                        Agent.Voice.PlayVoiceEvent(EVENTS.INFECTION_SPITTER_SPIT);
                        TriggerEMP();

                        Agent.Locomotion.m_animator.CrossFadeInFixedTime(EnemyLocomotion.s_hashAbilityUseOut[0], 0.15f);
                        Agent.Appearance.InterpolateGlow(GlowColor, new Vector4(0f, 1.25f, 0f, 1.5f), 0.15f);

                        state = EMPState.Cooldown;
                        stateTimer = Clock.Time + 3f;
                    }
                    break;

                case EMPState.Cooldown:
                    if (stateTimer < Clock.Time)
                    {
                        ResetState();
                        Agent.Locomotion.ChangeState((int)ES_StateEnum.PathMove);
                    }
                    break;
            }


            if (!Agent.Alive)
            {
                Agent.Appearance.InterpolateGlow(Color.black, new Vector4(0f, 1.25f, 0f, 1.5f), 2f);
                Destroy(this);
            }
        }

        public override bool Trigger()
        {
            Activated = true;
            state = EMPState.Build;
            return false;
        }
    }
}
