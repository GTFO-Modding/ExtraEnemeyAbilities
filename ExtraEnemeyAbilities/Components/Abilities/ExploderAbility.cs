﻿using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ExtraEnemyAbilities.Utilities;
using Player;
using AK;
using FX_EffectSystem;
using LevelGeneration;
using StateMachines;

namespace ExtraEnemyAbilities.Components.Abilities
{
    public class ExploderAbility : CustomAbility
    {
        public ExploderAbility(IntPtr intPtr) : base(intPtr)
        {
        }

        public bool exploded = false;
        public float detonatedTime = 0;

        private ExploderConfig ExploderConfig;

        private bool fade = false;
        private float glowAmount = 0;
        private float splatterTimer = 0;
        private readonly float splatterMax = 0.2f;
        private bool splattered = false;
        private static FX_Pool s_explodeFXPool;
        private static bool staticValuesLoaded = false;


        public void Awake()
        {
            Agent = GetComponent<EnemyAgent>();


            ExploderConfig = ConfigManager.ExploderConfigDictionary[Agent.EnemyDataID];
            GlowColor = Util.GetUnityColor(ExploderConfig.ColorData);

            Agent.MaterialHandler.m_defaultGlowColor = GlowColor;

            if (staticValuesLoaded == false)
            {
                s_explodeFXPool = FX_Manager.GetPreloadedEffectPool("FX_InfectionSpit");
                staticValuesLoaded = true;
            }

            Agent.add_OnDeadCallback((Action)(() => {
                if (ExploderConfig.NoExplosionOnDeath) return;
                if (Activated == true) return;
                Trigger();
            }));
        }

        public void Update()
        {
            if (exploded == false)
            {
                GlowBounce();
            } else
            {
                splatterTimer += Time.deltaTime;
                detonatedTime += Time.deltaTime;
                //if (splatterTimer >= splatterMax)
                //{
                //    PlaySplatter();
                //}


                if (detonatedTime > 1 && !fade)
                {
                    fade = true;
                    Agent.Appearance.InterpolateGlow(Color.black, 4f);
                }
            }
        }

        public override bool Trigger()
        {
            Activated = true;
            Agent.Locomotion.ChangeState((int)ES_StateEnum.Dead);

            Agent.Appearance.InterpolateGlow(GlowColor, 0.1f);
            var fx = s_explodeFXPool.AquireEffect();
            fx.Play(null, Agent.Position, Quaternion.LookRotation(Agent.TargetLookDir));

            var noise = new NM_NoiseData()
            {
                noiseMaker = null,
                position = Agent.Position,
                radiusMin = ExploderConfig.NoiseMin,
                radiusMax = ExploderConfig.NoiseMax,
                yScale = 1,
                node = Agent.CourseNode,
                type = NM_NoiseType.InstaDetect,
                includeToNeightbourAreas = true,
                raycastFirstNode = false
            };

            ExplosionUtil.TriggerExplodion(Agent.EyePosition, ExploderConfig.Damage, ExploderConfig.Radius, noise);
            PlaySplatter();
            return false;
        }

        private void PlaySplatter()
        {
            if (splattered == true) return;
            splattered = true;
            if (PlayerManager.TryGetLocalPlayerAgent(out PlayerAgent playerAgent))
            {
                if (ScreenLiquidManager.TryApply(ScreenLiquidSettingName.enemyBlood_BigBloodBomb, Agent.Position, ExploderConfig.Radius * 2, true))
                {
                    playerAgent.Sound.Post(EVENTS.VISOR_SPLATTER_GORE);
                }

                if (ExploderConfig.InfectionAmount <= 0) return;
                if (ScreenLiquidManager.TryApply(ScreenLiquidSettingName.spitterJizz, Agent.Position, ExploderConfig.Radius, true))
                {
                    playerAgent.Damage.ModifyInfection(new pInfection
                    {
                        amount = ExploderConfig.InfectionAmount,
                        mode = pInfectionMode.Add
                    }, true, true);
                    playerAgent.Sound.Post(EVENTS.VISOR_SPLATTER_INFECTION);
                }
            }
        }

        private void GlowBounce()
        {
            glowAmount += Time.deltaTime;
            int speed = 4;

            if (Agent.ScannerData.m_soundIndex > -1)
            {
                speed = 10;
            }

            double curvePos = Math.Sin(glowAmount * speed) * 1.1;

            if (curvePos >= 1)
            {
                Agent.Appearance.InterpolateGlow(GlowColor, Agent.MaterialHandler.m_defaultHeartbeatLocation, 1f/speed);
            }

            if (curvePos <= -1)
            {
                Agent.Appearance.InterpolateGlow(GlowColor * 0.5f, Agent.MaterialHandler.m_defaultHeartbeatLocation, 1f/speed);
            }
        }
    }
}
