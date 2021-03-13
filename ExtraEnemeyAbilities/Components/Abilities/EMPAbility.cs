using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtraEnemyAbilities.Components
{
    public class EMPAbility : CustomAbility
    {
        public EMPAbility(IntPtr intPtr) : base(intPtr)
        {
        }

        private EMPConfig EMPConfig;

        public void Awake()
        {
            GlowColor = EMPComponent.EMPColor;
            Agent = GetComponent<EnemyAgent>();
            EMPConfig = ConfigManager.EMPConfigDictionary[Agent.EnemyDataID];
        }

        public void Update()
        {
            if (Agent.Alive)
            {
                Agent.Appearance.InterpolateGlow(GlowColor, new Vector4(0f, 1.25f, 0f, 1.5f), 1f);
            } else
            {
                Agent.Appearance.InterpolateGlow(Color.black, new Vector4(0f, 1.25f, 0f, 1.5f), 2f);
                Destroy(this);
            }
        }

        public override bool Trigger()
        {
            GameObject EMPObject = new GameObject();

            EMPObject.AddComponent<EMPComponent>();
            EMPComponent emp = EMPObject.GetComponent<EMPComponent>();
            emp.EnemyAgent = Agent;
            emp.EMPConfig = EMPConfig;
            emp.Trigger();

            return false;
        }
    }
}
