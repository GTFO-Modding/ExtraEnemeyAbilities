using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtraEnemyAbilities.Utilities
{
    public static class Util
    {
        public static Color GetUnityColor(ColorData cd)
        {
            if (cd.r == 0 && cd.b == 0 && cd.g == 0 && cd.a == 0)
            {
                return new Color(1, 0, 0, 1);
            }

            return new Color(cd.r, cd.g, cd.b, cd.a);
        }
    }
}
