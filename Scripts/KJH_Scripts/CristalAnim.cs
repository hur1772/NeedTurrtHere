using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;
using Altair;

namespace KJH
{
    public class CristalAnim : MemoryPoolingFlag
    {
        bool isUp = false;
        float speed = 0.5f;
        int cristal = 10;

        void FixedUpdate()
        {
            CristalCheck();

            if (isUp == true)
            {
                transform.Translate(Vector2.up * speed);
            }
        }

        public void CristalUp()
        {
            isUp = true;
        }

        void CristalCheck()
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

            if (1.0f < pos.y)
                AddCristal();
        }

        void AddCristal()
        {
            isUp = false;

            // 크리스탈 추가 부분
            GlobalData.choi_userDia += cristal;

            Vector3 vec = transform.position;
            vec.y -= 1.0f;

            GameObject go = MemoryPoolManager.instance.GetObject("CristalCount", vec);

            go.GetComponent<CristalCount>().ViewCristalCount(cristal);

            ObjectReturn();
        }
    }
}
