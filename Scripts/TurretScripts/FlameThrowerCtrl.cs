using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using Altair;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class FlameThrowerCtrl : Turret_Ctrl
    {
        public Transform firepos;
        public GameObject m_FlameObj = null;
        float m_CacAtTick = 0.0f;  
        GameObject a_NewObj = null;
        GameObject findObj = null;
        //---------- 총알 발사 관련 변수 선언
        float CheckTime = 0.0f;

        //float valuepos = 4.0f;
        //float maxval = 0.0068f;
        //float curpos = 0.0f;

        //Vector3 pos = Vector3.zero;
        float dist = 1.67f;
        //float endPos = 9.0f;
        //거리 체크용 변수
        Vector2 rayVec;
        RaycastHit2D hit;
        public LayerMask enemylayer;
        //거리 체크용 변수
        public MeshRenderer Mtrl;
        public Material[] Mtrls;

        int mtrlstate = 0;

        //화염방사기(관통) 관련 변수
        ParticleSystem FlameParticle;
        //RaycastHit2D[] hits;
        //List<GameObject> EnemyList;


        protected override void SetType(int ii)
        {
            base.SetType(ii);
        }

        protected override void turretAtt()
        {
            //base.turretAtt();

            if (0.0f < m_CacAtTick)
                m_CacAtTick = m_CacAtTick - Time.deltaTime;

            CheckTime -= Time.deltaTime;
            if (CheckTime <= 0)
            {
                Mtrl.material = Mtrls[mtrlstate];
                if (mtrlstate == 0)
                {
                    mtrlstate = 1;
                }
                else
                {
                    mtrlstate = 0;
                }
                CheckTime = 0.5f;

            }

            if (m_CacAtTick <= 0.0f)
            {

                List<GameObject> EnemyList = new List<GameObject>();

                RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, Vector2.right, 6, enemylayer);
                if (hits.Length > 0)
                {
                    //EnemyList.Clear();
                    for (int i = 0; i < hits.Length; i++)
                    {
                        EnemyList.Add(hits[i].collider.gameObject);
                    }
                }

                a_NewObj = MemoryPoolManager.instance.GetObject("piercingbullet", ShotPoint);

                if (a_NewObj != null && a_NewObj.TryGetComponent(out BulletCtrl bull))
                {
                    bull.ishit = false;

                    bull.bulletSpeed = 0.089f;
                    bull.Damage = turretAttDamage;
                    bull.attackType = Altair.AttackType.Piercing;
                    bull.shotType = BulletCtrl.ShotType.Front;
                    bull.splashType = BulletCtrl.SplashType.NonSplash;
                    bull.AttackList = EnemyList;
                    bull.maxXPos = this.transform.position.x + 6;

                }
                m_CacAtTick = turretAttSpeed;
                FlameParticle.Play();
            }
        }

        public override void OnDamage(int dam)
        {
            base.OnDamage(dam);
        }

        // Start is called before the first frame update
        void Start()
        {
            //pos = Camera.main.ScreenToViewportPoint(transform.position);
            ShotPoint = firepos.transform.position;
            CheckTime = turretAttWait;

            FlameParticle = GetComponentInChildren<ParticleSystem>();
            //EnemyList = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()            
        {
            if (Altair.GlobalData.turretDataJson != null)
            {
                GlobalData.choi_InitData();
            }

            if (GlobalData.choi_m_TrList != null && turretIdx == -1)
            {
                SetType(6);
                //Debug.Log(turretHp);
            }

            if (turretEnum == turretAction.deploy)
            {
                //생성함수호출
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
                ObjectReturn();
            }
        }

        //설치후 활성화 대기
        public void DelayAcitve()
        {
            CheckTime -= Time.deltaTime;
            //Debug.Log(CheckTime.ToString());

            if (CheckTime <= 0.0f)
            {
                CheckTime = 0.5f;
                turretEnum = turretAction.attack;
            }
        }

        //공격 사거리 체크 함수
        public void CheckAttSensor()
        {

            rayVec = this.transform.position;
            //Vector2 a_EndPos;

            //curpos = maxval - pos.x;

            //if (curpos >= 0.0032)
            //{
            //    //Debug.Log(curpos);
            //    valuepos = 4.0f;
            //}

            //else
            //{
            //    //Debug.Log(curpos);
            //    valuepos = curpos * 1400;
            //}
            //a_EndPos = new Vector2(transform.position.x + valuepos, rayVec.y);
            //float value = (a_EndPos - rayVec).magnitude;

            float value = dist * turretSensor;//8.0f;
            if (endPos < (transform.position.x + value))
                value = endPos - transform.position.x;

            hit = Physics2D.Raycast(rayVec, Vector2.right, value, enemylayer);
            Debug.DrawRay(rayVec, Vector2.right * value, Color.red);
            if (hit)
            {
                findObj = hit.collider.gameObject;
                turretAtt();
            }


        }
    }
}

