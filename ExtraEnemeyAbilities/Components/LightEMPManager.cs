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
        private float stateTimer;
        private LightState state;
        private IEnumerator coroutine;
        private enum LightState
        {
            Off,
            Flicker,
            On
        }

        void Awake()
        {
            state = LightState.Off;
            stateTimer = 0;
        }

        void Update()
        {
            MelonCoroutines.Process();
            switch(state)
            {
                case LightState.Off:
                    state = LightState.Flicker;
                    stateTimer = Clock.Time + StartFlickerAt;
                    break;

                case LightState.Flicker:
                    if (stateTimer < Clock.Time)
                    {
                        coroutine = MelonCoroutines.Start(FlickerLight(CollectedLight)) as IEnumerator;
                        state = LightState.On;
                        stateTimer = Clock.Time + Duration - StartFlickerAt;
                    }
                    break;

                case LightState.On:
                    MelonCoroutines.Stop(coroutine);
                    CollectedLight.light.SetEnabled(true);
                    CollectedLight.light.ChangeIntensity(CollectedLight.originalIntensity);
                    Destroy(this);
                    break;
            }
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
            cLight.light.SetEnabled(false);
            coroutine = MelonCoroutines.Start(FlickerLight(CollectedLight)) as IEnumerator;
            yield break;
        }
    }
}
