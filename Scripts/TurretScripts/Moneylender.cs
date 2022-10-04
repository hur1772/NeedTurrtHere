using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using Altair;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class Moneylender : Turret_Ctrl//MonoBehaviour
    {
        float CheckTime = 0.0f; //할성화 대기시간
        //--- Money 관련 변수
        public GameObject m_DollorObj = null;
        GameObject a_NewDObj = null;
        float RandomMaxTime = 10.0f;
        float RandomMinTime = 5.0f;
        float RandomTime = 0.0f;
        //--- Money 관련 변수

        Animator _animator;
        int giveMoneyHash = Animator.StringToHash("Givemoney");
        int dieHash = Animator.StringToHash("Dieing");

        public GameObject m_suitcase = null;
        int m_count = 0;

        Vector3 m_SpawnPos = Vector3.zero;
        float dieTimer = 2.0f;

        protected override void OnEnable()
        {
            base.OnEnable();
            RandomMaxTime = 10.0f;
            RandomMinTime = 5.0f;
            dieTimer = 2.0f;
            this.gameObject.layer = 7;
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
            if (turretHp <= 0.0f)
            {
                this.gameObject.layer = 0;
                _animator.SetTrigger(dieHash);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            RandomTime = Random.Range(RandomMinTime, RandomMaxTime);
            m_count = 0;
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
                SetType(22);
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
                {
                    _animator.SetTrigger(giveMoneyHash);
                    StartCoroutine(CreateDollor());

                }
            }
            else if (turretEnum == turretAction.Destroy)
            {
                //터렛 hp = 0 파괴하기
                //MemoryPoolManager.instance.GetObject("SmallExplosive", this.transform.position);

                if (dieTimer >= 0.0f)
                    dieTimer -= Time.deltaTime;

                if (dieTimer <= 0.0f)
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
            m_SpawnPos = this.transform.position;
            m_SpawnPos.y = this.transform.position.y + 0.5f;
            m_SpawnPos.z -= 1.0f;

            a_NewDObj = MemoryPoolManager.instance.GetObject("BasicDoller", m_SpawnPos);

            if (m_count <= 4)
            {
                if (a_NewDObj != null && a_NewDObj.TryGetComponent(out BasicDoller doller))
                {
                    doller.money = 15;
                }
            }
            else if (m_count > 4)
            {
                m_suitcase.gameObject.SetActive(true);
                if (a_NewDObj != null && a_NewDObj.TryGetComponent(out BasicDoller doller))
                {
                    doller.money = 25;
                }
            }
            m_count++;


            RandomMaxTime = 30.0f;
            RandomMinTime = 20.0f;
            RandomTime = Random.Range(RandomMinTime, RandomMaxTime);

            yield return new WaitForSeconds(RandomTime);

            _animator.SetTrigger(giveMoneyHash);
        }
    }
}
