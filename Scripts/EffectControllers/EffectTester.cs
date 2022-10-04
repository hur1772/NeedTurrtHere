using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Altair
{
    public class EffectTester : MonoBehaviour
    {
        [SerializeField] private Vector3 spawnPos;

        //private void Start() => StartFunc();

        private void StartFunc()
        {
        
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (SwitchManager.GetSwitch("Switch1")) EffectSpawn();
        }

        private void EffectSpawn()
        {
            MemoryPoolManager.instance.BringObject("Effect1", spawnPos);
            SwitchManager.SetSwitch("Switch1", false);
        }
    }
}