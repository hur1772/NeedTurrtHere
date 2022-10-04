using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using Altair;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class MultipleTurretCtrl : Turret_Ctrl //MonoBehaviour
    {
        //---------- 총알 발사 관련 변수 선언
        public GameObject m_BulletObj = null;
        float m_CacAtTick = 0.0f;   //기관총 발사 틱 만들기....
        GameObject a_NewObj = null;
        GameObject findObj = null;
        //---------- 총알 발사 관련 변수 선언
        float CheckTime = 0.0f;

        //거리 체크용 변수
        float dist = 1.67f;  //한칸거리
        //float endPos = 9.0f; //끝지점
        Vector2 rayVec;
        RaycastHit2D hit;
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

                StartCoroutine(ShotBulletCo());

                m_CacAtTick = turretAttSpeed;
            }
        }

        IEnumerator ShotBulletCo()
        {
            for (int ii = 0; ii < 4; ii++)
            {
                a_NewObj = MemoryPoolManager.instance.GetObject("bullet", ShotPoint);

                if (a_NewObj != null && a_NewObj.TryGetComponent(out BulletCtrl bull))
                {
                    bull.p1 = this.transform.position;
                    bull.r1 = this.transform.position;
                    bull.r1.x += 2.5f;
                    bull.r1.y += 5.0f;

                    bull.ishit = false;
                    bull.hitObj = findObj;
                    bull.bulletSpeed = 0.089f;
                    bull.Damage = turretAttDamage;
                    bull.attackType = Altair.AttackType.Directional;
                    bull.shotType = BulletCtrl.ShotType.Front;
                    bull.splashType = BulletCtrl.SplashType.NonSplash;
                }

                yield return new WaitForSeconds(0.1f);
            }
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
                SetType(34);
                CheckTime = turretAttWait;
                //Debug.Log(turretHp);
            }

            if (turretEnum == turretAction.deploy)
            {
                //생성함수호출
                ShotPointSet(1.5f, 0.4f);
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
            CheckTime -= Time.deltaTime;
            //Debug.Log(CheckTime.ToString());

            if (CheckTime <= 0.0f)
                turretEnum = turretAction.attack;
        }

        //공격 사거리 체크 함수
        public void CheckAttSensor()
        {
            rayVec = this.transform.position;
            //Vector2 a_EndPos = new Vector2(9.0f, rayVec.y);
            //float value = (a_EndPos - rayVec).magnitude;

            float value = dist * turretSensor;//8.0f;
            if (endPos < (transform.position.x + value))
                value = endPos - transform.position.x;

            hit = Physics2D.Raycast(rayVec, Vector2.right, value, enemylayer);
            Debug.DrawRay(rayVec, Vector2.right * value, Color.red);
            if (hit)
            {
                turretAtt();
            }
        }
    }
}
