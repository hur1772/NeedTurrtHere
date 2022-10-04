using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using Altair;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class DoubleRocket_TrrCtrl : Turret_Ctrl //MonoBehaviour
    {
        //---------- �Ѿ� �߻� ���� ���� ����
        public GameObject m_BulletObj = null;
        float m_CacAtTick = 0.0f;   //����� �߻� ƽ �����....
        GameObject a_NewObj = null;
        GameObject findObj = null;
        //---------- �Ѿ� �߻� ���� ���� ����
        float CheckTime = 0.0f;

        //�Ÿ� üũ�� ����
        float dist = 1.67f;  //��ĭ�Ÿ�
        //float endPos = 9.0f; //������
        Vector2 rayVec;
        RaycastHit2D hit;
        public LayerMask enemylayer;
        //�Ÿ� üũ�� ����

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
            for (int ii = 0; ii < 2; ii++)
            {
                a_NewObj = MemoryPoolManager.instance.GetObject("bullet", ShotPoint);
                SoundPlay(ref m_fireclip);

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

                yield return new WaitForSeconds(0.3f);
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
                SetType(4);
                CheckTime = turretAttWait;
                //Debug.Log(turretHp);
            }

            if (turretEnum == turretAction.deploy)
            {
                //�����Լ�ȣ��
                ShotPointSet(1.5f, 0.4f);
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
                //���� ��Ÿ� üũ
                //Debug.Log("��������");
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
            //Debug.Log(CheckTime.ToString());

            if (CheckTime <= 0.0f)
                turretEnum = turretAction.attack;
        }

        //���� ��Ÿ� üũ �Լ�
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
