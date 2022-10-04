using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using Altair;
using Altair_Memory_Pool_Pro;
using LeeSpace;

namespace SungJae
{
    public class SquashCtrl : Turret_Ctrl
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
        float CheckTime = 0.0f;
        [SerializeField]float m_MoveSpeed = 2.0f;
        //SpriteRenderer rend;
        public GameObject squash;
        public GameObject SquashExplosive;


        protected override void SetType(int ii)
        {
            base.SetType(ii);
        }

        public override void OnDamage(int dam)
        {
            base.OnDamage(dam);
        }

        protected override void turretAtt()
        {

        }
        // Start is called before the first frame update
        void Start()
        {
            
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
                SetType(10);
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
                    for (int i = 0; i < findObjs.Count; i++)
                    {
                        if (findObjs[i].TryGetComponent(out MonsterCtrl enemy))
                        {
                            //enemy.hp -= 1000;
                            enemy.OnDamage(1000);

                            //if (enemy.hp <= 0)
                            //   Destroy(findObjs[i]);
                        }

                    }

                }

                //Destroy(gameObject);
                ObjectReturn();
            }

        }

        public void DelayAcitve()
        {
            CheckTime -= Time.deltaTime;
            //Debug.Log(CheckTime.ToString());

            if (CheckTime <= 0.0f)
                turretEnum = turretAction.attack;
        }

        public void CheckAttSensor()
        {
            rayVec = this.transform.position;

            float value = dist * turretSensor;
            if (endPos < (transform.position.x + value))
                value = endPos - transform.position.x;

            hitsensor(rayVec, Vector2.right, value);            
            hitsensor(rayVec, Vector2.left, value);
        }
        
        void hitsensor(Vector3 pos, Vector2 dir, float value)
        {
            hit = Physics2D.Raycast(pos, dir, value, enemylayer);
            Debug.DrawRay(pos, dir * value, Color.red);
            if (hit)
            {
                m_Checktime -= Time.deltaTime;
                //Debug.Log(m_Checktime);

                this.transform.Translate((dir.x * m_MoveSpeed) * Time.deltaTime, 0.0f, 0.0f);

                if (dir.x < 0.0f)
                {
                    squash.transform.localScale = new Vector3(0.2f, 0.2f, -0.2f);

                }
                else
                {
                    squash.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }    

                if (m_Checktime <= 0.0f)
                {
                    hits = Physics2D.RaycastAll(pos, dir, value, enemylayer);
                    if (hits.Length > 0)
                    {
                        for (int i = 0; i < hits.Length; i++)
                        {
                            if (hits[i].collider.TryGetComponent(out MonsterCtrl enemy))
                            {
                                findObjs.Add(hits[i].collider.gameObject);
                                pos.y -= 0.5f;

                                GameObject a_Expobj = MemoryPoolManager.instance.GetObject("SquashExplosive", pos);//(GameObject)Instantiate(SquashExplosive, pos, Quaternion.identity);
                                //Debug.Log(a_Expobj);
                                //MemoryPoolManager.instance.GetObject("SmallExplosive", effpoint);
                                a_Expobj.GetComponent<ParticleSystem>().Play();
                                //Destroy(a_Expobj, 2.0f);
                            }
                        }
                    }


                    rayVec = Vector2.zero;
                    m_Checktime = 0.5f;
                    CheckTime = turretAttWait;
                    squash.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    turretEnum = turretAction.Destroy;
                }

            }
        }
        
    }
}
