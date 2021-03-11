using Enemies;
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

namespace ExtraEnemyAbilities.Components
{
    public class ExploderAbility : CustomAbility
    {
        public ExploderAbility(IntPtr intPtr) : base(intPtr)
        {
        }

        private EnemyAgent EnemyAgent;
        private ExploderConfig ExploderConfig;
        public Color glowColor;
        public bool exploded = false;
        public float detonatedTime = 0;
        private bool fade = false;
        private float glowAmount = 0;
        private float splatterTimer = 0;
        private readonly float splatterMax = 0.2f;
        private bool splattered = false;
        private static FX_Pool s_explodeFXPool;
        private static bool staticValuesLoaded = false;


        public void Awake()
        {
            EnemyAgent = GetComponent<EnemyAgent>();
            ExploderConfig = ConfigManager.ExploderConfigDictionary[EnemyAgent.EnemyDataID];

            glowColor = Util.GetUnityColor(ExploderConfig.ColorData);

            EnemyAgent.MaterialHandler.m_defaultGlowColor = glowColor;
            if (staticValuesLoaded == false)
            {
                s_explodeFXPool = FX_Manager.GetPreloadedEffectPool("FX_InfectionSpit");
                staticValuesLoaded = true;
            }

            EnemyAgent.add_OnDeadCallback((Action)(() => {
                if (ExploderConfig.NoExplosionOnDeath) return;
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
                if (splatterTimer >= splatterMax)
                {
                    PlaySplatter();
                }


                if (detonatedTime > 1 && !fade)
                {
                    fade = true;
                    EnemyAgent.Appearance.InterpolateGlow(Color.black, 4f);
                }
            }
        }

        public override void Trigger()
        {
            if (exploded == true) return;
            exploded = true;

            EnemyAgent.Appearance.InterpolateGlow(glowColor, 0.1f);
            var fx = s_explodeFXPool.AquireEffect();
            fx.Play(null, EnemyAgent.Position, Quaternion.LookRotation(EnemyAgent.TargetLookDir));

            var noise = new NM_NoiseData()
            {
                noiseMaker = null,
                position = EnemyAgent.Position,
                radiusMin = ExploderConfig.NoiseMin,
                radiusMax = ExploderConfig.NoiseMax,
                yScale = 1,
                node = EnemyAgent.CourseNode,
                type = NM_NoiseType.InstaDetect,
                includeToNeightbourAreas = true,
                raycastFirstNode = false
            };

            ExplosionUtil.TriggerExplodion(EnemyAgent.EyePosition, ExploderConfig.Damage, ExploderConfig.Radius, noise);
        }

        private void PlaySplatter()
        {
            if (splattered == true) return;
            splattered = true;
            if (PlayerManager.TryGetLocalPlayerAgent(out PlayerAgent playerAgent))
            {
                if (ScreenLiquidManager.TryApply(ScreenLiquidSettingName.enemyBlood_BigBloodBomb, EnemyAgent.Position, ExploderConfig.Radius * 2, true))
                {
                    playerAgent.Sound.Post(EVENTS.VISOR_SPLATTER_GORE);
                }

                if (ExploderConfig.InfectionAmount <= 0) return;
                if (ScreenLiquidManager.TryApply(ScreenLiquidSettingName.spitterJizz, EnemyAgent.Position, ExploderConfig.Radius, true))
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

            if (EnemyAgent.ScannerData.m_soundIndex > -1)
            {
                speed = 10;
            }

            double curvePos = Math.Sin(glowAmount * speed) * 1.1;

            if (curvePos >= 1)
            {
                EnemyAgent.Appearance.InterpolateGlow(glowColor, EnemyAgent.MaterialHandler.m_defaultHeartbeatLocation, 1f/speed);
            }

            if (curvePos <= -1)
            {
                EnemyAgent.Appearance.InterpolateGlow(glowColor * 0.5f, EnemyAgent.MaterialHandler.m_defaultHeartbeatLocation, 1f/speed);
            }
        }
    }
}
