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
            return new Color(cd.r, cd.g, cd.b, cd.a);
        }
    }
}
