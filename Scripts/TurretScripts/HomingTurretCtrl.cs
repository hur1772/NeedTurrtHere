using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;
using Choi;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class HomingTurretCtrl : Turret_Ctrl //MonoBehaviour
    {//---------- �Ѿ� �߻� ���� ���� ����
        public GameObject m_BulletObj = null;
        float m_CacAtTick = 0.0f;   //����� �߻� ƽ �����....
        public Stage_Mgr m_StageMgr;
        GameObject a_NewObj = null;
        GameObject findObj = null;
        //---------- �Ѿ� �߻� ���� ���� ����
        float CheckTime = 0.0f;

        //�Ÿ� üũ�� ����
        public LayerMask enemylayer;
        //�Ÿ� üũ�� ����

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
            //base.turretAtt();

            if (0.0f < m_CacAtTick)
                m_CacAtTick = m_CacAtTick - Time.deltaTime;

            if (m_CacAtTick <= 0.0f)
            {
                a_NewObj = MemoryPoolManager.instance.GetObject("HomingBullet", ShotPoint);

                if (a_NewObj != null && a_NewObj.TryGetComponent(out BulletCtrl bull))
                {
                    bull.p1 = this.transform.position;
                    bull.r1 = this.transform.position;
                    bull.r1.x += 2.5f;
                    bull.r1.y += 5.0f;

                    bull.value = 0.0f;
                    bull.ishit = false;
                    bull.hitObj = findObj;
                    bull.bulletSpeed = 0.089f;
                    bull.Damage = turretAttDamage;
                    bull.attackType = Altair.AttackType.Homing;
                    bull.shotType = BulletCtrl.ShotType.Front;
                    bull.splashType = BulletCtrl.SplashType.NonSplash;
                }

                m_CacAtTick = turretAttSpeed;
            }
        }

        public override void OnDamage(int dam)
        {
            base.OnDamage(dam);
        }

        // Start is called before the first frame update
        void Start()
        {
            m_StageMgr = FindObjectOfType<Stage_Mgr>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Altair.GlobalData.turretDataJson != null)
            {
                Altair.GlobalData.choi_InitData();
            }

            if (Altair.GlobalData.choi_m_TrList != null && turretIdx == -1)
            {
                SetType(37);
                CheckTime = turretAttWait;
                //Debug.Log(CheckTime.ToString());
                //Debug.Log(turretHp);
            }

            if (turretEnum == turretAction.deploy)
            {
                //�����Լ�ȣ��
                ShotPointSet(1.0f, 1.0f);
                turretEnum = turretAction.idle;
            }
            else if (turretEnum == turretAction.idle)
            {
                //��ġ�� 1�� ���
                DelayAcitve();
            }
            else if (turretEnum == turretAction.attack)
            {
                //���� ��Ÿ� üũ
                //Debug.Log("��������");
                CheckAttSensor();
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

        //���� ��Ÿ� üũ �Լ�
        public void CheckAttSensor()
        {
            //Debug.Log(m_StageMgr);
            //Debug.Log(m_StageMgr.firstObj);

            findObj = m_StageMgr.firstObj;
            if (findObj != null)
            {
                turretAtt();
            }
            
        }
    }
}
