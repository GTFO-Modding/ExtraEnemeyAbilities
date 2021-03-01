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

namespace ExtraEnemyAbilities.Components
{
    public class ExploderBase : MonoBehaviour
    {
        public ExploderBase(IntPtr intPtr) : base(intPtr)
        {
        }

        private EnemyAgent enemyAgent;
        private ExploderConfig exploderConfig;
        public Color glowColor;
        public bool dead = false;
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
            enemyAgent = GetComponent<EnemyAgent>();
            exploderConfig = ConfigManager.ExploderConfigDictionary[enemyAgent.EnemyDataID];

            glowColor = Util.GetUnityColor(exploderConfig.ColorData);

            enemyAgent.MaterialHandler.m_defaultGlowColor = glowColor;
            if (staticValuesLoaded == false)
            {
                s_explodeFXPool = FX_Manager.GetPreloadedEffectPool("FX_InfectionSpit");
                staticValuesLoaded = true;
            }

            enemyAgent.add_OnDeadCallback((Action)(() => {
                Explode();
            }));
        }

        public void Update()
        {
            if (dead == false)
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
                    enemyAgent.Appearance.InterpolateGlow(Color.black, 4f);
                }
            }
        }

        public void Explode()
        {
            if (dead == true) return;
            dead = true;

            if (exploderConfig.NoExplosionOnDeath) return;

            enemyAgent.Appearance.InterpolateGlow(glowColor, 0.1f);
            var fx = s_explodeFXPool.AquireEffect();
            fx.Play(null, enemyAgent.Position, Quaternion.LookRotation(enemyAgent.TargetLookDir));

            var noise = new NM_NoiseData()
            {
                noiseMaker = null,
                position = enemyAgent.Position,
                radiusMin = exploderConfig.NoiseMin,
                radiusMax = exploderConfig.NoiseMax,
                yScale = 1,
                node = enemyAgent.CourseNode,
                type = NM_NoiseType.InstaDetect,
                includeToNeightbourAreas = true,
                raycastFirstNode = false
            };
            ExplosionUtil.TriggerExplodion(enemyAgent.EyePosition, exploderConfig.Damage, exploderConfig.Radius, noise);
        }

        private void PlaySplatter()
        {
            if (splattered == true) return;
            splattered = true;
            if (PlayerManager.TryGetLocalPlayerAgent(out PlayerAgent playerAgent))
            {
                if (ScreenLiquidManager.TryApply(ScreenLiquidSettingName.enemyBlood_BigBloodBomb, enemyAgent.Position, exploderConfig.Radius * 2, true))
                {
                    playerAgent.Sound.Post(EVENTS.VISOR_SPLATTER_GORE);
                }

                if (exploderConfig.InfectionAmount == 0) return;
                if (ScreenLiquidManager.TryApply(ScreenLiquidSettingName.spitterJizz, enemyAgent.Position, exploderConfig.Radius, true))
                {
                    playerAgent.Damage.ModifyInfection(new pInfection
                    {
                        amount = exploderConfig.InfectionAmount,
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

            if (enemyAgent.ScannerData.m_soundIndex > -1)
            {
                speed = 10;
            }

            double curvePos = Math.Sin(glowAmount * speed) * 1.1;

            if (curvePos >= 1)
            {
                enemyAgent.Appearance.InterpolateGlow(glowColor, enemyAgent.MaterialHandler.m_defaultHeartbeatLocation, 1f/speed);
            }

            if (curvePos <= -1)
            {
                enemyAgent.Appearance.InterpolateGlow(glowColor * 0.5f, enemyAgent.MaterialHandler.m_defaultHeartbeatLocation, 1f/speed);
            }
        }
    }
}
