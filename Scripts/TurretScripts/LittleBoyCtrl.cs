using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Altair;
using Altair_Memory_Pool_Pro;
using LeeSpace;

namespace SungJae
{
    public class LittleBoyCtrl : Turret_Ctrl//MonoBehaviour
    {
        float dist = 1.67f;
        //float endPos = 9.0f;
        //거리 체크용 변수
        Vector2 rayVec;
        Vector3 Startpos = Vector3.zero;
        Vector3 SpEndpos = Vector3.zero;
        Vector3 CurPos = Vector3.zero;
        public Transform spawnpos;
        public LayerMask enemylayer;
        public GameObject RockOnMark;
        public SpriteRenderer Rockon;
        public GameObject airstrike;
        float CheckTime = 0.0f;
        float MoveSpeed = 20.0f;
        float m_MoveSpeed = 100.0f;
        public GameObject AirStrikeExplosive;

        protected override void SetType(int ii)
        {
            base.SetType(ii);
        }

        public override void OnDamage(int dam)
        {
            //base.OnDamage(dam);
        }

        protected override void turretAtt()
        {
            base.turretAtt();
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Rockon.transform.Rotate(new Vector3(0.0f, 0.0f, m_MoveSpeed * Time.deltaTime));

            if (Altair.GlobalData.turretDataJson != null)
            {
                GlobalData.choi_InitData();

            }

            if (GlobalData.choi_m_TrList != null && turretIdx == -1)
            {
                SetType(9);
                CheckTime = turretAttWait;

            }
            if (turretEnum == turretAction.deploy)
            {
                //생성함수호출
                SpEndpos = RockOnMark.transform.position;
                airstrike.transform.position = spawnpos.position;
                Startpos = airstrike.transform.position;
                turretEnum = turretAction.idle;
            }
            else if (turretEnum == turretAction.idle)
            {
                DelayAcitve();
                //Debug.Log("idle");

            }
            else if (turretEnum == turretAction.attack)
            {
                CheckAttSensor();
                //Debug.Log("attack");
            }
            else if (turretEnum == turretAction.Destroy)
            {

                ObjectReturn();
                //Debug.Log("dest");
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector2(10.0f, 7.0f));
        }

        public void DelayAcitve()
        {
            CheckTime -= Time.deltaTime;
            //Debug.Log(CheckTime.ToString());

            if (CheckTime <= 0.0f)
                turretEnum = turretAction.attack;
            airstrike.gameObject.SetActive(true);
        }

        public void CheckAttSensor()
        {
            CurPos = airstrike.transform.position;
            CurPos.y -= MoveSpeed * Time.deltaTime;
            airstrike.transform.position = CurPos;


            if (airstrike.transform.position.y <= SpEndpos.y)
            {
                if (!MemoryPoolManager.instance) return;

                //GameObject a_newObj = MemoryPoolManager.instance.GetObject(5, this.transform.position);
                Vector2 a_ExpPoint = Vector2.zero;
                a_ExpPoint = this.transform.position;
                a_ExpPoint.y -= 1.0f;

                GameObject a_explosiveObj = MemoryPoolManager.instance.GetObject("AirStrikeExplosive", a_ExpPoint);

                a_explosiveObj.GetComponent<ParticleSystem>().Play();

                Rockon.gameObject.SetActive(false);
                rayVec = airstrike.transform.position;
                float value = dist * 3.0f;
                if (endPos < (transform.position.x + value))
                    value = endPos - transform.position.x;
                
                airstrike.SetActive(false);
                Collider2D[] colls = Physics2D.OverlapBoxAll(airstrike.transform.position, new Vector2(10.0f, 7.0f), 0, enemylayer);

                for (int i = 0; i < colls.Length; i++)
                {

                    if (colls[i].TryGetComponent(out MonsterCtrl enemy))
                    {
                        if (colls[i].TryGetComponent(out IDamageable target))
                        {
                            target.OnDamage(1000);
                        }


                    }
                }

                airstrike.gameObject.SetActive(false);
                turretIdx = -1;
                CurPos = Vector3.zero;
                SpEndpos = Vector3.zero;
                Rockon.gameObject.SetActive(true);
                turretEnum = turretAction.Destroy;
            }


        }
    }
}
