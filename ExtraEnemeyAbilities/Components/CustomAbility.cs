using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Enemies;
using StateMachines;

namespace ExtraEnemyAbilities.Components
{
    public class CustomAbility : MonoBehaviour
    {
        public CustomAbility(IntPtr intPtr) : base(intPtr)
        {
            Activated = false;
        }

        public Color GlowColor;
        public EnemyAgent Agent;
        public bool Activated { get; protected set; }
        public virtual bool Trigger() { throw new NotImplementedException(); }
    }
}
