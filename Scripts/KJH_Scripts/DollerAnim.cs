using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;
using SungJae;

namespace KJH
{
    public class DollerAnim : MemoryPoolingFlag
    {
        [SerializeField] private Animator anim;

        float check = 0.0f;
        float delay = 0.3f;

        void Update()
        {
            if (check < delay)
            {
                check += Time.deltaTime;

                if (delay < check)
                {
                    check = 0.0f;

                    Stage_Mgr.instance.money += 25;
                    Stage_Mgr.instance.RefreshMoney();

                    ObjectReturn();
                }
            }
        }
    }
}
