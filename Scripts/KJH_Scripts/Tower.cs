using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace KJH
{
    public class Tower : MemoryPoolingFlag
    {
        private void Start() => StartFunc();

        private void StartFunc()
        {
         
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
        
        }

        public void Shovel()
        {
            ObjectReturn();
        }
    }
}
