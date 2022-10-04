using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;
//using UnityEditor.SceneManagement;

namespace SungJae
{
    public class BasicDoller : MemoryPoolingFlag
    {
        private float objtimer;
        // Start is called before the first frame update

        public bool isClick;

        public int money;

        Vector3 endPos = new Vector3(7.5f, -4.5f, 0.0f);

        private void OnEnable()
        {
            objtimer = 7.5f;
        }

        //void Start()
        //{

        //}

        // Update is called once per frame
        void Update()
        {
            if (isClick)
            {
                Vector3 vec = (endPos - this.transform.position).normalized;

                transform.position += vec * Time.deltaTime * 15.0f;

                if ((endPos - transform.position).magnitude < 0.1f)
                {
                    Stage_Mgr.instance.money += money;
                    Stage_Mgr.instance.RefreshMoney();
                    isClick = false;
                    MoneyReturn();
                }
            }
            else
            {
                objtimer -= Time.deltaTime;
                if (objtimer <= 0.0f)
                    MoneyReturn();
            }
        }


        public void MoneyReturn()
        {
            ObjectReturn();
        }
    }
}
