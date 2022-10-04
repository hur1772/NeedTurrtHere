using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using Altair;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class BankTurretCtrl : Turret_Ctrl//MonoBehaviour
    {
        float CheckTime = 0.0f; //할성화 대기시간
        //--- Money 관련 변수
        public GameObject m_DollorObj = null;
        GameObject a_NewDObj = null;
        float RandomMaxTime = 10.0f;
        float RandomMinTime = 5.0f;
        float RandomTime = 0.0f;
        //--- Money 관련 변수

        Vector3 m_SpawnPos = Vector3.zero;

        protected override void OnEnable()
        {
            base.OnEnable();
            RandomMaxTime = 10.0f;
            RandomMinTime = 5.0f;
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
            RandomTime = Random.Range(RandomMinTime, RandomMaxTime);
            //Debug.Log(RandomTime);
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
                SetType(20);
                CheckTime = turretAttWait;
                //Debug.Log(CheckTime.ToString());
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
                if (0.0f < RandomTime)
                    RandomTime = RandomTime - Time.deltaTime;

                //Debug.Log(RandomTime);

                if (RandomTime <= 0.0f)
                    StartCoroutine(CreateDollor());
            }
            else if (turretEnum == turretAction.Destroy)
            {
                //터렛 hp = 0 파괴하기
                ObjectReturn();
                //Destroy(this.gameObject);
            }
        }

        public void DelayAcitve()
        {
            //Debug.Log(CheckTime.ToString());
            CheckTime -= Time.deltaTime;

            if (CheckTime <= 0.0f)
                turretEnum = turretAction.attack;
        }

        IEnumerator CreateDollor()
        {
            //a_NewDObj = (GameObject)Instantiate(m_DollorObj);

            Debug.Log("On");

            m_SpawnPos = this.transform.position;
            m_SpawnPos.y = this.transform.position.y + 0.5f;
            m_SpawnPos.z -= 1.0f;
            //a_NewDObj.transform.position = m_SpawnPos;

            //MemoryPoolManager.instance.BringObject("BasicDoller", m_SpawnPos);
            a_NewDObj = MemoryPoolManager.instance.GetObject("BasicDoller", m_SpawnPos);
            if (a_NewDObj != null && a_NewDObj.TryGetComponent(out BasicDoller doller))
            {
                doller.money = 25;
            }


            RandomMaxTime = 30.0f;
            RandomMinTime = 20.0f;
            RandomTime = Random.Range(RandomMinTime, RandomMaxTime);
            //Debug.Log(RandomTime);

            Debug.Log("Off");


            yield return new WaitForSeconds(RandomTime);
        }
    }
}
