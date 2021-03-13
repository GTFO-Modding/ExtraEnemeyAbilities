using AK;
using Enemies;
using ExtraEnemyAbilities.Utilities;
using LevelGeneration;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtraEnemyAbilities.Components
{
    public class EMPComponent : MonoBehaviour
    {
        public EMPComponent(IntPtr intPtr) : base(intPtr)
        {
        }

        public EnemyAgent EnemyAgent;
        public EMPConfig EMPConfig;
        private float Timer = 0;
        private bool Triggered = false;
        LG_LightCollection lightCollection;
        private FogSphereAllocator fogSphereAdd;
        private FogSphereAllocator fogSphereSub;
        private float stateTimer = 0;
        private EMPState state = EMPState.BuildUp;
        private Vector3 fogPos;
        public static readonly Color EMPColor = new Color(0.525f, 0.956f, 0.886f, 1.0f);
        public void Trigger()
        {
            lightCollection = LG_LightCollection.Create(EnemyAgent.CourseNode, EnemyAgent.EyePosition, LG_LightCollectionSorting.Distance, EMPConfig.Radius);

            ExtraEnemyAbilities.log.LogDebug($"Collected {lightCollection.collectedLights.Count} lights");


            foreach (var collectedLight in lightCollection.collectedLights)
            {
                ExtraEnemyAbilities.log.LogDebug($"Evaling light with distance of {collectedLight.distance}");
                if (collectedLight.distance > EMPConfig.Radius)
                {
                    break;
                }
                LightEMPManager prevLightEMPMan = collectedLight.light.gameObject.GetComponent<LightEMPManager>();
                if (prevLightEMPMan != null)
                {
                    prevLightEMPMan.ResetDuration(EMPConfig);
                } else
                {
                    LightEMPManager lightEMPMan = collectedLight.light.gameObject.AddComponent<LightEMPManager>();
                    lightEMPMan.ResetDuration(EMPConfig);
                    lightEMPMan.CollectedLight = collectedLight;
                }
            }

            var targets = Physics.OverlapSphere(EnemyAgent.EyePosition, EMPConfig.Radius, LayerManager.MASK_EXPLOSION_TARGETS);

            int playerCount = 0;
            foreach(var target in targets)
            {
                PlayerAgent playerAgent = target.GetComponent<PlayerAgent>();
                if (playerAgent != null)
                {
                    if (playerAgent.IsLocallyOwned)
                    {
                        var existingPEMPM = playerAgent.GetComponent<PlayerEMPManager>();
                        if (existingPEMPM != null)
                        {
                            //TODO Duration
                            existingPEMPM.ResetDuration(EMPConfig.Duration);
                        } else
                        {
                            PlayerEMPManager playerEMPManager = playerAgent.gameObject.AddComponent<PlayerEMPManager>();
                            if (playerEMPManager != null)
                            {
                                playerEMPManager.ResetDuration(EMPConfig.Duration);
                                playerEMPManager.triggered = true;
                            }
                            playerCount++;
                        }
                    }
                } 
            }

            ExtraEnemyAbilities.log.LogDebug($"Added PlayerEMPManager to {playerCount} players.");



            Triggered = true;
        }

        public void Update()
        {
            if (!Triggered) return;
            Timer += Time.deltaTime;

            switch(state)
            {
                case EMPState.BuildUp:
                    state = EMPState.WaveStart;
                    break;
                case EMPState.WaveStart:
                    if (stateTimer < Clock.Time)
                    {
                        LightWave();
                        stateTimer = Clock.Time + 1.5f;
                        state = EMPState.LightOffSFX;
                    }
                    break;

                case EMPState.LightOffSFX:
                    EnemyAgent.Voice.PlayVoiceEvent(EVENTS.LIGHTS_OFF_GLOBAL);
                    state = EMPState.WaveExpand;
                    break;
                case EMPState.WaveExpand:
                    if (stateTimer < Clock.Time)
                    {
                        state = EMPState.WaveEnd;
                    }
                    if (fogSphereAdd.IsAllocated)
                    {
                        float num = 1f - (stateTimer - Clock.Time) / 1.5f;
                        fogSphereAdd.SetPositionRange(fogPos, Mathf.Lerp(2f, 130f, num));
                        float t = Mathf.Pow(num, 32f);
                        fogSphereAdd.SetDensity(Mathf.Lerp(1f, 0f, t));
                        if (fogSphereSub.IsAllocated)
                        {
                            fogSphereSub.SetPositionRange(fogPos, Mathf.Lerp(0f, 90f, num));
                            fogSphereSub.SetDensity(Mathf.Lerp(-10f, 0f, t));
                            return;
                        }
                    }
                    break;

                case EMPState.WaveEnd:
                    if (stateTimer < Clock.Time)
                    {
                        fogSphereAdd.Deallocate();
                        fogSphereSub.Deallocate();
                        state = EMPState.Done;
                    }
                    break;
                case EMPState.Done:

                    break;
            }

            if (Timer >= EMPConfig.Duration)
            {
                Destroy(gameObject);
            }
        }

        private enum EMPState
        {
            BuildUp,
            WaveStart,
            LightOffSFX,
            WaveExpand,
            WaveEnd,
            Done
        }

        private void LightWave()
        {
            fogSphereAdd = new FogSphereAllocator();
            fogSphereSub = new FogSphereAllocator();
            fogPos = EnemyAgent.EyePosition;
            if (fogSphereAdd.TryAllocate())
            {
                fogSphereAdd.SetPositionRange(fogPos, 2f);
                fogSphereAdd.SetDensity(1f);
                fogSphereAdd.SetRadiance(EMPColor, 1f);
            }
            if (fogSphereSub.TryAllocate())
            {
                fogSphereSub.SetPositionRange(fogPos, 1f);
                fogSphereSub.SetDensity(-10f);
                fogSphereSub.SetRadiance(Color.black, 0f);
            }
        }
    }
}
