using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;
using Choi;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class FortressTurretCtrl : Turret_Ctrl //MonoBehaviour
    {
        float CheckTime = 0.0f;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void SetType(int ii)
        {
            base.SetType(ii);
        }

        protected override void turretAtt()
        {
            base.turretAtt();
        }

        public override void OnDamage(int dam)
        {
            base.OnDamage(dam);
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (GlobalData.turretDataJson != null)
            {
                GlobalData.choi_InitData();
            }

            if (GlobalData.choi_m_TrList != null && turretIdx == -1)
            {
                SetType(27);
                CheckTime = turretAttWait;
                //Debug.Log(turretHp);
            }

            if (turretEnum == turretAction.deploy)
            {
                //�����Լ�ȣ��
                turretEnum = turretAction.idle;
            }
            else if (turretEnum == turretAction.idle)
            {
                //��ġ�� 1�� ���
                DelayAcitve();
            }
            else if (turretEnum == turretAction.attack)
            {
                //�������� ����
            }
            else if (turretEnum == turretAction.Destroy)
            {
                //�ͷ� hp = 0 �ı��ϱ�
                //Destroy(this.gameObject);
                ObjectReturn();
            }
        }

        //��ġ�� Ȱ��ȭ ���
        public void DelayAcitve()
        {
            //Debug.Log(CheckTime.ToString());
            CheckTime -= Time.deltaTime;

            if (CheckTime <= 0.0f)
                turretEnum = turretAction.attack;
        }
    }
}
