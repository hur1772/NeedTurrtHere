using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using Altair;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class MilitiaTurretCtrl : Turret_Ctrl
    {
        public Transform firepos;
        public GameObject m_BulletObj = null;
        float m_CacAtTick = 0.0f;  
        GameObject a_NewObj = null;
        GameObject findObj = null;
        //---------- �Ѿ� �߻� ���� ���� ����
        float CheckTime = 0.0f;

        float dist = 1.67f;
        //�Ÿ� üũ�� ����
        Vector2 rayVec;
        RaycastHit2D hit;
        public LayerMask enemylayer;
        //�Ÿ� üũ�� ����
        public Animation[] anim;

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
                //a_NewObj = (GameObject)Instantiate(m_BulletObj);

                a_NewObj = MemoryPoolManager.instance.GetObject("bullet", ShotPoint);
                SoundPlay(ref m_fireclip);

                if (a_NewObj != null && a_NewObj.TryGetComponent(out BulletCtrl bull))
                { 
                    bull.ishit = false;
                    bull.hitObj = findObj;
                    bull.bulletSpeed = 0.089f;
                    bull.Damage = turretAttDamage;
                    bull.attackType = Altair.AttackType.Directional;
                    bull.shotType = BulletCtrl.ShotType.Front;
                    bull.splashType = BulletCtrl.SplashType.NonSplash;
                    
                }
                GunShot();
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
            anim = GetComponentsInChildren<Animation>();
            ShotPoint = firepos.transform.position;
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
                SetType(5);
                //Debug.Log(turretHp);

                CheckTime = turretAttWait;
                Debug.Log(turretAttWait);
            }

            if (turretEnum == turretAction.deploy)
            {
                //�����Լ�ȣ��
                SoundPlay(ref clip);
                turretEnum = turretAction.idle;
                
            }
            else if (turretEnum == turretAction.idle)
            {
                //��ġ�� 1�� ���
                DelayAcitve();
            }
            else if (turretEnum == turretAction.attack)
            {              
                CheckAttSensor();
            }
            else if (turretEnum == turretAction.Destroy)
            {
                if (isDeathPlay == true)
                    SoundPlay(ref m_Dethclip);

                if (a_Time > 0.0f)
                    a_Time -= Time.deltaTime;

                if (a_Time <= 0.0f)
                {
                    ObjectReturn();
                }
            }
        }

        //��ġ�� Ȱ��ȭ ���
        public void DelayAcitve()
        {
            CheckTime -= Time.deltaTime;

            if (CheckTime <= 0.0f)
            {
                turretEnum = turretAction.attack;
            }

        }

        //���� ��Ÿ� üũ �Լ�
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
                findObj = hit.collider.gameObject;               
                turretAtt();
            }


        }

        void GunShot()
        {
            anim[4].Play("Take 001");
            
        }
    }
}

