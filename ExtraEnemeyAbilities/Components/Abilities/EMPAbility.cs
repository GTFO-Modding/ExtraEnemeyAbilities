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

        private EnemyAgent EnemyAgent;
        private EMPConfig EMPConfig;

        public void Awake()
        {
            EnemyAgent = GetComponent<EnemyAgent>();
            EMPConfig = ConfigManager.EMPConfigDictionary[EnemyAgent.EnemyDataID];
        }

        public override void Trigger()
        {
            GameObject EMPObject = new GameObject();

            EMPObject.AddComponent<EMPComponent>();
            EMPComponent emp = EMPObject.GetComponent<EMPComponent>();
            emp.EnemyAgent = EnemyAgent;
            emp.EMPConfig = EMPConfig;
            emp.Trigger();
        }
    }
}
