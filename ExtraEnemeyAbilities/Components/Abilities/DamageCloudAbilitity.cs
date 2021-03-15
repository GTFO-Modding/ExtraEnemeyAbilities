using Enemies;
using Player;
using StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtraEnemyAbilities.Components.Abilities
{
    public class DamageCloudAbilitity : CustomAbility
    {
        public DamageCloudAbilitity(IntPtr intPtr) : base(intPtr)
        {
        }

        public override bool Trigger()
        {
            Activated = true;
            return true;
        }

        public void Awake()
        {
            Agent = GetComponent<EnemyAgent>();
        }

        public void Update()
        {
            if (Activated == true && PlayerManager.TryGetLocalPlayerAgent(out PlayerAgent playerAgent))
            {
                if (Vector3.Distance(playerAgent.Position, Agent.Position) < 10)
                {
                    playerAgent.Damage.ParasiteDamage(0.1f);
                }
            }
        }
    }
}
