using ExtraEnemyAbilities.Utilities;
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
    public class PlayerEMPManager : MonoBehaviour
    {
        public PlayerEMPManager(IntPtr intPtr) : base(intPtr)
        {
        }
        public float Duration;
        public float Timer = 0;
        public bool triggered = false;
        public bool FlashlightEnabled = false;
        public bool HUDEnabled = false;

        private float FlashDuration
        {
            get
            {
                if (Duration - 2 <= 0)
                {
                    return Duration * 0.8f;
                }
                return Duration - 2;
            }
        }
        private PlayerInventoryBase PlayerInventoryBase;
        private PlayerAgent playerAgent;
        private enum HUDState
        {
            FlickerOff,
            ForceDelay,
            ForceOff,
            FlickerOn,
            End
        }
        private HUDState state = HUDState.FlickerOff;
        private float stateTimer;
        private List<object> coroutines;

        public void ResetDuration(float newDuration)
        {
            Duration = newDuration;
            foreach (var item in coroutines)
            {
                EEA_MelonCoroutines.Stop(item as IEnumerator);
            }
            coroutines.Clear();

            if (state == HUDState.FlickerOn)
            {
                stateTimer = Clock.Time + FlashDuration;
            }
            if (state == HUDState.End)
            {
                state = HUDState.FlickerOff;
            }
        }

        public void Awake()
        {
            playerAgent = gameObject.GetComponent<PlayerAgent>();
            PlayerInventoryBase = playerAgent.Inventory;
            coroutines = new List<object>();
        }

        private IEnumerator FlickerFlashlight(int iterations, bool finalState)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
            for (int i = 0; i < iterations; i++)
            {
                FlashlightEnabled = !FlashlightEnabled;
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
            }
            FlashlightEnabled = finalState;
            yield break;
        }

        private IEnumerator FlickerHudElement(RectTransformComp guiLayer, int iterations, bool finalState, float startDelayMin, float startDelayMax)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(startDelayMin, startDelayMax));
            for (int i = 0; i < iterations; i++)
            {
                guiLayer.SetVisible(!guiLayer.IsVisible);
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.1f));
            }
            guiLayer.SetVisible(finalState);
            yield break;
        }


        private void Flicker(int iterations, bool finalState, float startDelayMin = 0.8f, float startDelayMax = 2.5f)
        {
            var inventory = EEA_MelonCoroutines.Start(FlickerHudElement(GuiManager.PlayerLayer.Inventory, iterations, finalState, startDelayMin, startDelayMax));
            var compass = EEA_MelonCoroutines.Start(FlickerHudElement(GuiManager.PlayerLayer.m_compass, iterations, finalState, startDelayMin, startDelayMax));
            var warden = EEA_MelonCoroutines.Start(FlickerHudElement(GuiManager.PlayerLayer.m_wardenObjective, iterations, finalState, startDelayMin, startDelayMax));
            var status = EEA_MelonCoroutines.Start(FlickerHudElement(GuiManager.PlayerLayer.m_playerStatus, iterations, finalState, startDelayMin, startDelayMax));
            var chat = EEA_MelonCoroutines.Start(FlickerHudElement(GuiManager.PlayerLayer.m_gameEventLog, iterations, finalState, startDelayMin, startDelayMax));
            var flashLight = EEA_MelonCoroutines.Start(FlickerFlashlight(iterations, finalState));
            coroutines.Add(inventory);
            coroutines.Add(compass);
            coroutines.Add(warden);
            coroutines.Add(status);
            coroutines.Add(flashLight);
            coroutines.Add(chat);
        }

        public void Update()
        {
            if (!triggered) return;

            PlayerInventoryBase.SetFlashlightEnabled(FlashlightEnabled);

            switch (state)
            {
                case HUDState.FlickerOff:
                    Flicker(10, false, 0.5f, 0.8f);
                    EEAGlobalState.IsPlayerEMP = true;
                    state = HUDState.FlickerOn;
                    stateTimer = Clock.Time + FlashDuration;
                    break;

                case HUDState.FlickerOn:
                    if (stateTimer < Clock.Time)
                    {
                        GuiManager.PlayerLayer.SetVisible(true);
                        GuiManager.PlayerLayer.Inventory.SetVisible(false);
                        GuiManager.PlayerLayer.m_compass.SetVisible(false);
                        GuiManager.PlayerLayer.m_wardenObjective.SetVisible(false);
                        GuiManager.PlayerLayer.m_playerStatus.SetVisible(false);
                        GuiManager.PlayerLayer.m_gameEventLog.SetVisible(false);

                        Flicker(20, true);
                        state = HUDState.End;
                        stateTimer = Clock.Time + Duration - FlashDuration;
                    }
                    break;

                case HUDState.End:
                    if (stateTimer < Clock.Time)
                    {
                        Destroy(this);
                        EEAGlobalState.IsPlayerEMP = false;
                    }
                    break;
            }
        }
    }
}
