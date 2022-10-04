using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class Turret_Ctrl : MemoryPoolingFlag, IDamageable, ISoundPlay //MonoBehaviour
    {
        protected enum turretAction
        {
            deploy,         //��ġ����(��ġ�������ִ»�Ȳ)
            idle,           //��ġ�Ĵ��(1��)
            attack,         //���ݰŸ� Ž��
            Destroy         //���ݹ޾Ƽ� �ı�
        }

        //��ġ(�Լ�)
        //����(�Լ�)
        //�ı�(������)(�Լ�)
        protected turretAction turretEnum = turretAction.deploy;    //�ͷ� ���°�
        protected int turretIdx = -1;                    //�̸�(�ε���)
        protected float turretCreateWait;           //��ġ ���ð�(����)
        protected float turretAttWait;              //���� ���(����)
        protected int turretAttDamage;              //���� ��(����)
        protected int turretAttType;                //���� Ÿ��(����)
        protected float turretSensor;               //���� �����Ÿ�(����)
        protected float turretAttRange;             //���� ���ɰŸ�(����)
        protected float turretAttSpeed;             //���ݼӵ�(����)
        protected int turretSplashAttType;               //���� ���� Ÿ��(����)
        protected bool isTurret;                    //���� ���� �ǹ�(����)
        protected bool isRangeAtt;                  //����Ÿ��(����)
        protected float endPos = 8.8f;

        [SerializeField] protected int turretHp;                     //CurHp(����)
        [SerializeField] protected int MaxturretHp;                     //MaxHp(����)

        Choi.BulletCtrl bulletCtrl;

        public Vector2 ShotPoint = Vector2.zero;
        public Vector2 effpoint = Vector2.zero;

        protected new AudioSource audio; //AudioSource Component�� �ҷ��´�.
        [SerializeField] protected AudioClip clip; //AudioClip�� �����´�. //��ġ��
        [SerializeField] protected AudioClip m_fireclip; //AudioClip�� �����´�. //�Ѿ� �߻�
        [SerializeField] protected AudioClip m_Dethclip; //AudioClip�� �����´�. //�ͷ� �ı���
        protected bool isFirstPlay = true; //true�� �ʱ�ȭ �Ѵ�.
        protected bool isDeathPlay = false;
        protected float a_Time = 0.3f;

        protected virtual void OnEnable()
        {
            turretEnum = turretAction.deploy;
            turretHp = MaxturretHp;
            isDeathPlay = false;
            a_Time = 0.3f;
        }

        private void Awake() //�ݵ�� Awake���� ó���Ѵ�.
        {
            audio = GetComponent<AudioSource>(); //AudioSource�� GetComponent
            if (clip == null) clip = Resources.Load<AudioClip>("Sounds/TF2/Weapons/cbar_hit1"); //Ȥ�� �����Ϳ��� �̸� �����س��� ���� ��츦 ���� ���� ó��
            if (m_fireclip == null) m_fireclip = Resources.Load<AudioClip>("���ҽ� ���� �� ���");
            if (m_Dethclip == null) m_Dethclip = Resources.Load<AudioClip>("Sounds/TF2/Weapons/tomislav_wind_down");
        }

        private void Start() => StartFunc();

        private void StartFunc()
        {
            ShotPoint = this.transform.position;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            //���°� üũ�ϱ�
            //if (turretEnum == turretAction.deploy)
            //{
            //    //�����Լ�ȣ��
            //}
            //else if (turretEnum == turretAction.idle)
            //{
            //    //��ġ�� 1�� ����Լ�
            //}
            //else if (turretEnum == turretAction.attackSensor)
            //{
            //    //���� ��Ÿ� üũ�Լ�
            //}
            //else if (turretEnum == turretAction.attackAble)
            //{
            //    //�����ϱ��Լ�
            //}
            //else if (turretEnum == turretAction.Destroy)
            //{
            //    //�ͷ� hp = 0 �ı��ϱ��Լ�
            //}

        }

        //!!!!!!!!!!!!!!!!!!!!!!!!!SetType �ֿ켱 ����!!!!!!!!!!!!!!!!!!!!!!!!!
        protected virtual void SetType(int ii)
        {
            turretIdx = ii;                                                         //�̸�(�ε���)
            turretCreateWait = GlobalData.choi_m_TrList[ii].m_prepTime;             //��ġ ���ð�(����)
            turretAttWait = GlobalData.choi_m_TrList[ii].m_activateTime;              //���� ���(����)
            turretAttType = GlobalData.choi_m_TrList[ii].m_attackType;              //���� Ÿ��(����)
            turretSensor = GlobalData.choi_m_TrList[ii].m_sensor;               //���� ���ɰŸ�(����)
            turretAttRange = GlobalData.choi_m_TrList[ii].m_range;             //���� �����Ÿ�(����)
            turretAttSpeed = GlobalData.choi_m_TrList[ii].m_atkDelay;             //���ݼӵ�(����)
            turretAttDamage = GlobalData.choi_m_TrList[ii].m_dam;                   //���ݷ� (����)
            turretAttType = GlobalData.choi_m_TrList[ii].m_isSingleUse;               //���� ���� Ÿ��(����)
            turretHp = GlobalData.choi_m_TrList[ii].m_hp;                     //CurHp(����)
            MaxturretHp = GlobalData.choi_m_TrList[ii].m_hp;
            //Debug.Log(GlobalData.choi_TurretNameList[ii]);

            //���� ���� �ǹ�(����)
            if (GlobalData.choi_m_TrList[ii].m_isAtk == 0)
            {
                isTurret = false; //���� ���ϴ� �ǹ�
            }
            else
            {
                isTurret = true; //���� �ϴ� �ǹ�
            }
            //���� ���� �ǹ�(����)

            //����Ÿ��(����)
            if (GlobalData.choi_m_TrList[ii].m_isSingleTargeting == 0)
            {
                isRangeAtt = false; //���� ����
            }
            else
            {
                isRangeAtt = true;//����Ÿ��
            }
            //����Ÿ��(����)
        }
        //instantiate �� �� SetType�Լ��� ������ �� �����ϱ�
        //int�� �̸��� �޾ƿͼ�
        //�ű� �ִ� ������ �� �߰��ع�����

        protected void ShotPointSet(float Sx, float Sy)
        {
            //Debug.Log("A" + ShotPoint);
            ShotPoint.x += Sx;
            ShotPoint.y += Sy;
            //Debug.Log("B" + ShotPoint);
            //Debug.Log("Set");
        }

        protected virtual void turretAtt()
        {
            //���� ����!!!�߿�߿� ������ �⤿������
        }

        public void turretObjectReturn()
        {
            ObjectReturn();
        }

        public virtual void OnDamage(int dam)
        {
            turretHp -= dam;
            if (turretHp <= 0)
            {
                Debug.Log(turretHp);
                effpoint = this.transform.position;
                effpoint.y -= 0.3f;
                MemoryPoolManager.instance.GetObject("SmallExplosive", effpoint);
                isDeathPlay = true;
                turretEnum = turretAction.Destroy;
                //�ͷ� ����Ʈ ������
                //����Ʈ ȣ��
                //ObjectReturn();
            }

        }

        public void SoundPlay(ref AudioClip clip)
        {
            if (audio == null) return; //AudioSource�� �������� �ʾ��� ��츦 ����� ���� ó��

            audio.Stop(); //Ȥ�� ���� ��� ���� ȿ������ ���� ��� ����
            if(GlobalData.volumeisOn)
                audio.PlayOneShot(clip, GlobalData.masterVolume); //�Ҹ��� ����Ѵ�.
            isDeathPlay = false;
        }
    }
}