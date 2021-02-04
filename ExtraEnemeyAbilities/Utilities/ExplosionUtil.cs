using AIGraph;
using AK;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FX_EffectSystem;
using Il2CppSystem.Collections;

namespace ExtraEnemyAbilities.Utilities
{
    public static class ExplosionUtil
    {
        public static void TriggerExplodion(Vector3 position, float damage, float range, NM_NoiseData noiseData)
        {
            CellSound.Post(EVENTS.STICKYMINEEXPLODE, position);
            _ = LightFlash(position);
            if (SNet.IsMaster)
            {
                var pos = position;
                var targets = Physics.OverlapSphere(pos, range, LayerManager.MASK_EXPLOSION_TARGETS);

                foreach (var target in targets)
                {
                    var comp = target.GetComponent<IDamageable>();

                    if (comp != null)
                    {
                        comp.ExplosionDamage(damage, pos, Vector3.up * 1000);
                    }
                }
                NoiseManager.MakeNoise(noiseData);
            }
        }

        private static async Task LightFlash(Vector3 pos)
        {
            FX_Manager.TryAllocateFXLight(out FX_PointLight light);
            light.SetColor(new Color(1, 0.2f, 0, 1));
            light.SetRange(50);
            light.m_intensity = 5;
            light.m_position = pos;
            light.m_isOn = true;
            light.UpdateData();
            light.UpdateTransform();
            await Task.Delay(50);
            FX_Manager.DeallocateFXLight(light);
        }
    }
}
