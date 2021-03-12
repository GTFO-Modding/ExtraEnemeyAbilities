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
        private float FlashDuration { get
            {
                if (Duration - 2 <= 0)
                {
                    return Duration * 0.8f;
                }
                return Duration - 2;
            } 
        }
        public float Timer = 0;
        public bool triggered = false;
        PlayerAgent playerAgent;
        private PlayerInventoryBase PlayerInventoryBase;
        private IEnumerator coroutine;
        public bool FlashlightEnabled = false;
        public bool HUDEnabled = false;

        public void Awake()
        {
            playerAgent = gameObject.GetComponent<PlayerAgent>();
            PlayerInventoryBase = playerAgent.Inventory;
        }

        private void UpdateFlashlightAndHUD()
        {
            GuiManager.PlayerLayer.SetVisible(HUDEnabled);
            GuiManager.NavMarkerLayer.SetVisible(HUDEnabled);
            PlayerInventoryBase.SetFlashlightEnabled(FlashlightEnabled);
        }

        private void TriggerUIFlash()
        {
            if (coroutine == null)
            {
                coroutine = (IEnumerator)MelonCoroutines.Start(FlashUI());
            }
            MelonCoroutines.Process();
        }

        private IEnumerator FlashUI()
        {
            //ExtraEnemyAbilities.log.LogDebug("Flickering HUD");
            for (int i = 0; i < 200; i++)
            {
                FlashlightEnabled = UnityEngine.Random.RandomRange(0, 2) == 1;
                HUDEnabled = UnityEngine.Random.RandomRange(0, 2) == 1;
                //ExtraEnemyAbilities.log.LogDebug($"\nFlashlight: {FlashlightEnabled}\nHUD: {HUDEnabled}");
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
            }

            yield break;
        }




        public void Update()
        {
            if (!triggered) return;

            UpdateFlashlightAndHUD();

            if (Timer >= FlashDuration)
            {
                TriggerUIFlash();
            }

            if (Timer >= Duration)
            {
                MelonCoroutines.Stop(coroutine);
                FlashlightEnabled = true;
                HUDEnabled = true;
                UpdateFlashlightAndHUD();
                Destroy(this);
            }

            Timer += Time.deltaTime;
        }
    }
}
