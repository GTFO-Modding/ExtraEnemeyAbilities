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
using Player;
using Agents;

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

                    Vector3 targetPos = target.transform.position;

                    Agent agent = target.GetComponent<Agent>();
                    if (agent != null)
                    {
                        targetPos = agent.EyePosition;
                    }
                    Vector3 direction = (targetPos - position).normalized;
                    bool hit = false;

                    if (!Physics.Raycast(pos, direction.normalized, out RaycastHit raycastHit, range, LayerManager.MASK_EXPLOSION_BLOCKERS))
                    {
                        hit = true;
                        var comp = target.GetComponent<IDamageable>();

                        if (comp != null)
                        {
                            comp.ExplosionDamage(damage, pos, Vector3.up * 1000);
                        }
                    } else
                    {
                        GameObject mySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        mySphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        mySphere.transform.position = raycastHit.point;
                        var col = mySphere.GetComponent<Collider>();
                        var mesh = mySphere.GetComponent<MeshRenderer>();
                        mesh.material.color = Color.red;
                        GameObject.Destroy(col);
                    }


#if DEBUG
                    GameObject line = new GameObject();
                    line.AddComponent<LineRenderer>();
                    LineRenderer lineRender = line.GetComponent<LineRenderer>();
                    lineRender.material = new Material(Shader.Find("Sprites/Default"));
                    lineRender.widthMultiplier = 0.05f;
                    lineRender.SetColors(Color.green, Color.green);
                    lineRender.SetPositions(new Vector3[] { pos, targetPos });

                    if (hit == false)
                    {
                        lineRender.SetColors(Color.red, Color.red);
                        lineRender.SetPositions(new Vector3[] { pos, raycastHit.point });
                    }
#endif
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
