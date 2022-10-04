using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using Altair;
using Altair_Memory_Pool_Pro;
using LeeSpace;

namespace SungJae
{
    public class Claymore_Ctrl : Turret_Ctrl
    {
        float m_Checktime = 0.5f;
        float dist = 1.67f;
        //float endPos = 9.0f;
        //거리 체크용 변수
        Vector2 rayVec;
        RaycastHit2D hit;
        RaycastHit2D[] hits;
        public LayerMask enemylayer;
        public List<GameObject> findObjs;
        float sensor = 1.0f;
        float CheckTime = 0.0f;

        protected override void SetType(int ii)
        {
            base.SetType(ii);
        }

        public override void OnDamage(int dam)
        {
            base.OnDamage(dam);
        }

        // Start is called before the first frame update
        void Start()
        {
            findObjs = new List<GameObject>();
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
                SetType(2);
                CheckTime = turretAttWait;
            }
            if (turretEnum == turretAction.deploy)
            {
                //생성함수호출
                turretEnum = turretAction.idle;
            }
            else if (turretEnum == turretAction.idle)
            {
                DelayAcitve();
            }
            else if (turretEnum == turretAction.attack)
            { 
                CheckAttSensor();
            }
            else if (turretEnum == turretAction.Destroy)
            {
                if (findObjs.Count > 0)
                {
                    Debug.Log(findObjs.Count);
                    for(int i =0; i < findObjs.Count; i++)
                    {
                        if (findObjs[i].TryGetComponent(out MonsterCtrl enemy))
                        {
                            enemy.OnDamage(1000);

                            //if (enemy.hp <= 0)
                            //    Destroy(findObjs[i]);
                        }

                    }

                    //Debug.Log("찾음");
                }

                //Destroy(gameObject);
                ObjectReturn();
            }

        }

        //설치후 활성화 대기
        public void DelayAcitve()
        {
            CheckTime -= Time.deltaTime;
            Debug.Log(CheckTime.ToString());

            if (CheckTime <= 0.0f)
                turretEnum = turretAction.attack;
        }

        public void CheckAttSensor()
        {
            rayVec = this.transform.position;

            float value = dist * sensor;
            if (endPos < (transform.position.x + value))
                value = endPos - transform.position.x;

            hit = Physics2D.Raycast(rayVec, Vector2.right, value, enemylayer);
            Debug.DrawRay(rayVec, Vector2.right * value, Color.red);
            if (hit)
            {
                m_Checktime -= Time.deltaTime;
                Debug.Log(m_Checktime);

                if (m_Checktime <= 0.0f)
                {
                    hits = Physics2D.RaycastAll(rayVec, Vector2.right, value, enemylayer);
                    if (hits.Length > 0)
                    {
                        for (int i = 0; i < hits.Length; i++)
                        {
                            if (hits[i].collider.TryGetComponent(out MonsterCtrl enemy))
                            {
                                findObjs.Add(hits[i].collider.gameObject);
                                rayVec.y -= 0.5f;
                                GameObject a_Expobj = MemoryPoolManager.instance.GetObject("SquashExplosive", rayVec);
                                a_Expobj.GetComponent<ParticleSystem>().Play();
                            }
                        }
                    }

                    rayVec = Vector2.zero;
                    m_Checktime = 0.5f;
                    CheckTime = turretAttWait;
                    turretEnum = turretAction.Destroy;
                }

            }






        }
    }
}
