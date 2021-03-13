using ExtraEnemyAbilities.Utilities;
using LevelGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtraEnemyAbilities.Components
{
    public class LightEMPManager : MonoBehaviour
    {
        public LightEMPManager(IntPtr intPtr) : base(intPtr)
        {
        }
        public float Duration;
        public CollectedLight CollectedLight;
        public float StartFlickerAt;
        private float buildIntensity;
        private float stateTimer;
        private LightState state;
        private IEnumerator coroutine;
        private enum LightState
        {
            Overload,
            Off,
            Flicker,
            On
        }

        public void ResetDuration(EMPConfig eMPConfig)
        {
            stateTimer = 0;
            if (state != LightState.Overload)
            {
                state = LightState.Off;
            }
            EEA_MelonCoroutines.Stop(coroutine);
            StartFlickerAt = UnityEngine.Random.RandomRange((float)(eMPConfig.Duration * 0.5), eMPConfig.Duration);
            Duration = eMPConfig.Duration;
        }

        void Awake()
        {
            state = LightState.Overload;
            stateTimer = 0;
        }

        void Update()
        {
            switch (state)
            {
                case LightState.Overload:
                    CollectedLight.light.ChangeIntensity(CollectedLight.originalIntensity + buildIntensity);
                    buildIntensity += 1f;
                    if (CollectedLight.originalIntensity + buildIntensity > 5f)
                    {
                        state = LightState.Off;
                    }
                    break;

                case LightState.Off:
                    CollectedLight.light.ChangeIntensity(0);
                    state = LightState.Flicker;
                    stateTimer = Clock.Time + StartFlickerAt;
                    break;

                case LightState.Flicker:
                    if (stateTimer < Clock.Time)
                    {
                        coroutine = EEA_MelonCoroutines.Start(FlickerLight(CollectedLight)) as IEnumerator;
                        state = LightState.On;
                        stateTimer = Clock.Time + Duration - StartFlickerAt - UnityEngine.Random.Range(0, 5f);
                    }
                    break;

                case LightState.On:
                    if (stateTimer < Clock.Time)
                    {
                        EEA_MelonCoroutines.Stop(coroutine);
                        CollectedLight.light.ChangeIntensity(CollectedLight.originalIntensity);
                        Destroy(this);
                    }
                    break;
            }
        }

        private IEnumerator FlickerLight(CollectedLight cLight)
        {
            cLight.light.ChangeIntensity(cLight.originalIntensity / UnityEngine.Random.Range(1, 3));
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));

            cLight.light.ChangeIntensity(cLight.originalIntensity / UnityEngine.Random.Range(1, 3));
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));

            cLight.light.ChangeIntensity(cLight.originalIntensity / UnityEngine.Random.Range(1, 3));
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));

            cLight.light.ChangeIntensity(cLight.originalIntensity / UnityEngine.Random.Range(1, 3));
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));

            coroutine = EEA_MelonCoroutines.Start(FlickerLight(cLight)) as IEnumerator;
            yield break;
        }
    }
}
