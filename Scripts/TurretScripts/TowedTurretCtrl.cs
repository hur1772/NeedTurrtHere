using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;
using Choi;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class TowedTurretCtrl : Turret_Ctrl //MonoBehaviour
    {
        //---------- 총알 발사 관련 변수 선언
        public GameObject m_BulletObj = null;
        float m_CacAtTick = -.1f;   //기관총 발사 틱 만들기....
        GameObject a_NewObj = null;
        GameObject findObj = null;
        //---------- 총알 발사 관련 변수 선언
        float CheckTime = 0.0f;

        public Animation anim;

        int randomshot = 0;

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

            randomshot = Random.Range(0, 4);
            if (m_CacAtTick <= 0.0f)
            {
                if (randomshot == 3)
                {
                    a_NewObj = MemoryPoolManager.instance.GetObject("LightingBullet", ShotPoint);

                    if (a_NewObj != null && a_NewObj.TryGetComponent(out BulletCtrl bull))
                    {
                        bull.p1 = ShotPoint; //this.transform.position;
                        bull.r1 = ShotPoint; //this.transform.position;
                        bull.r1.x += 2.5f;
                        bull.r1.y += 5.0f;

                        bull.value = 0.0f;
                        bull.ishit = false;
                        bull.hitObj = findObj;
                        bull.bulletSpeed = 0.089f;
                        bull.Damage = turretAttDamage + 4;
                        bull.attackType = Altair.AttackType.Balistic;
                        bull.shotType = BulletCtrl.ShotType.Front;
                        bull.splashType = BulletCtrl.SplashType.NonSplash;
                        bull.isStun = true;
                        GunShot();
                    }
                }
                else
                {
                    a_NewObj = MemoryPoolManager.instance.GetObject("BalisticBullet", ShotPoint);

                    if (a_NewObj != null && a_NewObj.TryGetComponent(out BulletCtrl bull))
                    {
                        bull.p1 = ShotPoint; //this.transform.position;
                        bull.r1 = ShotPoint; //this.transform.position;
                        bull.r1.x += 2.5f;
                        bull.r1.y += 5.0f;

                        bull.value = 0.0f;
                        bull.ishit = false;
                        bull.hitObj = findObj;
                        bull.bulletSpeed = 0.089f;
                        bull.Damage = turretAttDamage;
                        bull.attackType = Altair.AttackType.Balistic;
                        bull.shotType = BulletCtrl.ShotType.Front;
                        bull.splashType = BulletCtrl.SplashType.NonSplash;
                        bull.isStun = false;
                        GunShot();
                    }
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
            anim = GetComponentInChildren<Animation>();
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
                SetType(18);
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
            rayVec = this.transform.position;
            float value = dist * turretSensor;//8.0f;
            if (endPos < (transform.position.x + value))
                value = endPos - transform.position.x;

            hit = Physics2D.Raycast(rayVec, Vector2.right, value, enemylayer);
            Debug.DrawRay(rayVec, Vector2.right * value, Color.red);
            if (hit)
            {
                //Debug.Log(hit.collider.gameObject);
                findObj = hit.collider.gameObject;
                turretAtt();
            }

        }

        void GunShot()
        {
            anim.Play("Shoot");
        }
    }
}
