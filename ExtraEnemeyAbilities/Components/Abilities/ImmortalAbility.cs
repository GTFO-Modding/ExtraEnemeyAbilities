using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraEnemyAbilities.Components.Abilities
{
    public class ImmortalAbility : CustomAbility
    {
        public ImmortalAbility(IntPtr intPtr) : base(intPtr)
        {
        }

        public void Awake()
        {
            Agent.Damage.IsImortal = true;
        }
    }
}
