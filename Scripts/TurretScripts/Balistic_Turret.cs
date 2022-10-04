using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;
using Choi;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class Balistic_Turret : Turret_Ctrl //MonoBehaviour
    {
        //---------- �Ѿ� �߻� ���� ���� ����
        public GameObject m_BulletObj = null;
        float m_CacAtTick = -.1f;   //����� �߻� ƽ �����....
        GameObject a_NewObj = null;
        GameObject findObj = null;
        //---------- �Ѿ� �߻� ���� ���� ����
        float CheckTime = 0.0f;

        public Animation anim;

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
                a_NewObj = MemoryPoolManager.instance.GetObject("BalisticBullet", ShotPoint);

                if (a_NewObj != null && a_NewObj.TryGetComponent(out BulletCtrl bull))
                {
                    bull.p1 = ShotPoint;//this.transform.position;
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
                    //bull.SplashVec = new Vector2(1.5f, 4.0f);
                    bull.isSlow = false;
                    GunShot();
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
            //ShotPoint = this.transform.position;
            //Debug.Log(anim);
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
                SetType(17);
                CheckTime = turretAttWait;
                //Debug.Log(CheckTime.ToString());
                //Debug.Log(turretHp);
            }

            if (turretEnum == turretAction.deploy)
            {
                //�����Լ�ȣ��
                ShotPointSet(1.0f, 1.0f);
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
                //�ͷ� hp = 0 �ı��ϱ�
                //Destroy(this.gameObject);
                //MemoryPoolManager.instance.GetObject("SmallExplosive", this.transform.position);
                ObjectReturn();
            }
        }

        //��ġ�� Ȱ��ȭ ���
        public void DelayAcitve()
        {
            //Debug.Log(CheckTime.ToString());
            CheckTime -= Time.deltaTime;

            if (CheckTime <= 0.0f)
                turretEnum = turretAction.attack;
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
