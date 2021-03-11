using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Enemies;

namespace ExtraEnemyAbilities.Components
{
    public class CustomAbility : MonoBehaviour
    {
        public CustomAbility(IntPtr intPtr) : base(intPtr)
        {
        }

        public virtual void Trigger() { }
    }
}
