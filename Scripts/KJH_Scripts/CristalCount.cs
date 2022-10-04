using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Altair_Memory_Pool_Pro;
using Altair;

namespace KJH
{
    public class CristalCount : MemoryPoolingFlag
    {
        public Text cristalTxt;
        public Image delayImage;

        Color color;

        float delay = 2.0f;
        float check = 2.0f;

        void Update()
        {
            if (0.0f < check)
            {
                check -= Time.deltaTime;

                color = delayImage.color;
                color.a = check;
                delayImage.color = color;

                //delayImage.fillAmount = check / delay;

                if (check <= 0.0f)
                {
                    ResetCristal();
                }
            }
        }

        void ResetCristal()
        {
            check = delay;
            ObjectReturn();
        }

        public void ViewCristalCount(int cristal)
        {
            cristalTxt.text = GlobalData.choi_userDia + " + " + cristal;
        }
    }
}
