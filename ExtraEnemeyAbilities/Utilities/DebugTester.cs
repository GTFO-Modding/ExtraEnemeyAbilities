using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Player;
using UnhollowerRuntimeLib;
using HarmonyLib;

namespace ExtraEnemyAbilities.Utilities
{
#if DEBUG
    public class DebugTester : MonoBehaviour
    {
        public DebugTester(IntPtr intPtr) : base(intPtr)
        {
        }
	
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F11))
            {
				SpawnTendrils(PlayerManager.GetLocalPlayerAgent().Position, Vector3.forward);
            }
        }
	
		private void SpawnTendrils(Vector3 pos, Vector3 dir)
		{
			ScoutAntennaDetection scout = new ScoutAntennaDetection();
			try
            {
				scout.SpawnTendrils(Vector3.zero, Vector3.zero);
			} catch
            {

            }

			var m_myAntennaPrefab = UnityEngine.Object.Instantiate<GameObject>(ScoutAntennaDetection.s_antennaePrefab);
			MeshRenderer component = m_myAntennaPrefab.GetComponent<MeshRenderer>();
			var m_antennaMaterial = component.material;
		}
	}
#endif
}
