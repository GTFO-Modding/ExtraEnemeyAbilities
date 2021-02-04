using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using ExtraEnemyAbilities.Components;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;

namespace ExtraEnemyAbilities
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency(DATADUMPER_GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class ExtraEnemyAbilities : BasePlugin
    {
        public const string DATADUMPER_GUID = "com.dak.MTFO";

        public const string
            MODNAME = "ExtraEnemyAbilities",
            AUTHOR = "Dak",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "0.0.3";

        public static ManualLogSource log;


        public override void Load()
        {
            //Inject
            ClassInjector.RegisterTypeInIl2Cpp<ExploderBase>();

            log = Log;
            var harmony = new Harmony(GUID);
            harmony.PatchAll();

        }
    }
}
