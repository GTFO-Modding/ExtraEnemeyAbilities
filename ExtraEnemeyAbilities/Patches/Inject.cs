using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CellMenu;
using ExtraEnemyAbilities.Utilities;
using HarmonyLib;
using UnityEngine;

namespace ExtraEnemyAbilities.Patches
{
#if DEBUG
    [HarmonyPatch(typeof(CM_PageIntro), "EXT_PressInject")]
    public class Inject
    {
        static void Prefix()
        {
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<DebugTester>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }
    }
#endif
}
