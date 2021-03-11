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
        private Queue flashingLights;
        private readonly int lightCap = 5;
        private FogSphereAllocator fogSphereAdd;
        private FogSphereAllocator fogSphereSub;
        private float stateTimer = 0;
        private EMPState state = EMPState.BuildUp;
        private Vector3 fogPos;
        public void Trigger()
        {
            lightCollection = LG_LightCollection.Create(EnemyAgent.CourseNode, EnemyAgent.EyePosition, LG_LightCollectionSorting.Distance, EMPConfig.Radius);

            ExtraEnemyAbilities.log.LogDebug($"Collected {lightCollection.collectedLights.Count} lights");

            foreach (var light in lightCollection.collectedLights)
            {
                ExtraEnemyAbilities.log.LogDebug($"Evaling light with distance of {light.distance}");
                if (light.distance > EMPConfig.Radius)
                {
                    break;
                }
                light.light.SetEnabled(false);
                light.intensityProgression = 0;
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
                        var prevEMPManager = playerAgent.GetComponent<PlayerEMPManager>();
                        if (prevEMPManager != null)
                        {
                            Destroy(prevEMPManager);
                        }
                        playerAgent.gameObject.AddComponent<PlayerEMPManager>();
                        PlayerEMPManager playerEMPManager = playerAgent.gameObject.GetComponent<PlayerEMPManager>();
                        if (playerEMPManager != null)
                        {
                            playerEMPManager.FlashlightEnabled = false;
                            playerEMPManager.HUDEnabled = false;
                            playerEMPManager.Duration = EMPConfig.Duration;
                            playerEMPManager.triggered = true;
                        }
                        playerCount++;
                    }
                } 
            }

            ExtraEnemyAbilities.log.LogDebug($"Added PlayerEMPManager to {playerCount} players.");



            Triggered = true;
        }

        public void Awake()
        {
            flashingLights = new Queue();
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
                        state = EMPState.WaveExpand;
                        EnemyAgent.Voice.PlayVoiceEvent(EVENTS.LIGHTS_OFF_GLOBAL);
                    }
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

            if (Timer > EMPConfig.Duration * 0.5)
            {
                FlickerRandomLight();
            }

            if (Timer >= EMPConfig.Duration)
            {
                lightCollection.ResetUpdateValues(true);
                ExtraEnemyAbilities.log.LogDebug("Turned lights back on");
                lightCollection.SetMode(true);
                Destroy(gameObject);
            }
        }

        private enum EMPState
        {
            BuildUp,
            WaveStart,
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
                fogSphereAdd.SetRadiance(Color.blue, 1f);
            }
            if (fogSphereSub.TryAllocate())
            {
                fogSphereSub.SetPositionRange(fogPos, 1f);
                fogSphereSub.SetDensity(-10f);
                fogSphereSub.SetRadiance(Color.black, 0f);
            }
        }

        private void FlickerRandomLight()
        {
            MelonCoroutines.Process();

            var num = UnityEngine.Random.Range(0, lightCollection.collectedLights.Count);
            var light = lightCollection.collectedLights[num];
            IEnumerator lightEnumerator = (IEnumerator)MelonCoroutines.Start(FlickerLight(light));
        }

        private IEnumerator FlickerLight(CollectedLight cLight)
        {
            cLight.light.SetEnabled(true);
            cLight.light.ChangeIntensity(cLight.originalIntensity / UnityEngine.Random.Range(1, 3));
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
            cLight.light.ChangeIntensity(cLight.originalIntensity / UnityEngine.Random.Range(1, 3));
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
            cLight.light.ChangeIntensity(cLight.originalIntensity / UnityEngine.Random.Range(1, 3));
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
            cLight.light.ChangeIntensity(cLight.originalIntensity / UnityEngine.Random.Range(1, 3));
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
            cLight.light.ChangeIntensity(cLight.originalIntensity);
            cLight.light.SetEnabled(true);
            yield break;
        }
    }
}
