using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;
using Choi;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class HomingTurretCtrl : Turret_Ctrl //MonoBehaviour
    {//---------- 총알 발사 관련 변수 선언
        public GameObject m_BulletObj = null;
        float m_CacAtTick = 0.0f;   //기관총 발사 틱 만들기....
        public Stage_Mgr m_StageMgr;
        GameObject a_NewObj = null;
        GameObject findObj = null;
        //---------- 총알 발사 관련 변수 선언
        float CheckTime = 0.0f;

        //거리 체크용 변수
        public LayerMask enemylayer;
        //거리 체크용 변수

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
                //생성함수호출
                ShotPointSet(1.0f, 1.0f);
                turretEnum = turretAction.idle;
            }
            else if (turretEnum == turretAction.idle)
            {
                //설치후 1초 대기
                DelayAcitve();
            }
            else if (turretEnum == turretAction.attack)
            {
                //공격 사거리 체크
                //Debug.Log("센서들어옴");
                CheckAttSensor();
            }
            else if (turretEnum == turretAction.Destroy)
            {
                //터렛 hp = 0 파괴하기
                //Destroy(this.gameObject);
                ObjectReturn();
            }
        }

        //설치후 활성화 대기
        public void DelayAcitve()
        {
            //Debug.Log(CheckTime.ToString());
            CheckTime -= Time.deltaTime;

            if (CheckTime <= 0.0f)
                turretEnum = turretAction.attack;
        }

        //공격 사거리 체크 함수
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
