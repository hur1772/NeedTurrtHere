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
            deploy,         //설치상태(설치딜레이주는상황)
            idle,           //설치후대기(1초)
            attack,         //공격거리 탐지
            Destroy         //공격받아서 파괴
        }

        //설치(함수)
        //공격(함수)
        //파괴(데미지)(함수)
        protected turretAction turretEnum = turretAction.deploy;    //터렛 상태값
        protected int turretIdx = -1;                    //이름(인덱스)
        protected float turretCreateWait;           //설치 대기시간(변수)
        protected float turretAttWait;              //공격 대기(변수)
        protected int turretAttDamage;              //공격 력(변수)
        protected int turretAttType;                //공격 타입(변수)
        protected float turretSensor;               //공격 감지거리(변수)
        protected float turretAttRange;             //공격 가능거리(변수)
        protected float turretAttSpeed;             //공격속도(변수)
        protected int turretSplashAttType;               //공격 범위 타입(변수)
        protected bool isTurret;                    //공격 가능 건물(변수)
        protected bool isRangeAtt;                  //단일타겟(변수)
        protected float endPos = 8.8f;

        [SerializeField] protected int turretHp;                     //CurHp(변수)
        [SerializeField] protected int MaxturretHp;                     //MaxHp(변수)

        Choi.BulletCtrl bulletCtrl;

        public Vector2 ShotPoint = Vector2.zero;
        public Vector2 effpoint = Vector2.zero;

        protected new AudioSource audio; //AudioSource Component를 불러온다.
        [SerializeField] protected AudioClip clip; //AudioClip을 가져온다. //설치시
        [SerializeField] protected AudioClip m_fireclip; //AudioClip을 가져온다. //총알 발사
        [SerializeField] protected AudioClip m_Dethclip; //AudioClip을 가져온다. //터렛 파괴시
        protected bool isFirstPlay = true; //true로 초기화 한다.
        protected bool isDeathPlay = false;
        protected float a_Time = 0.3f;

        protected virtual void OnEnable()
        {
            turretEnum = turretAction.deploy;
            turretHp = MaxturretHp;
            isDeathPlay = false;
            a_Time = 0.3f;
        }

        private void Awake() //반드시 Awake에서 처리한다.
        {
            audio = GetComponent<AudioSource>(); //AudioSource를 GetComponent
            if (clip == null) clip = Resources.Load<AudioClip>("Sounds/TF2/Weapons/cbar_hit1"); //혹시 에디터에서 미리 연결해놓지 않은 경우를 위한 예외 처리
            if (m_fireclip == null) m_fireclip = Resources.Load<AudioClip>("리소스 폴더 내 경로");
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
            //상태값 체크하기
            //if (turretEnum == turretAction.deploy)
            //{
            //    //생성함수호출
            //}
            //else if (turretEnum == turretAction.idle)
            //{
            //    //설치후 1초 대기함수
            //}
            //else if (turretEnum == turretAction.attackSensor)
            //{
            //    //공격 사거리 체크함수
            //}
            //else if (turretEnum == turretAction.attackAble)
            //{
            //    //공격하기함수
            //}
            //else if (turretEnum == turretAction.Destroy)
            //{
            //    //터렛 hp = 0 파괴하기함수
            //}

        }

        //!!!!!!!!!!!!!!!!!!!!!!!!!SetType 최우선 구현!!!!!!!!!!!!!!!!!!!!!!!!!
        protected virtual void SetType(int ii)
        {
            turretIdx = ii;                                                         //이름(인덱스)
            turretCreateWait = GlobalData.choi_m_TrList[ii].m_prepTime;             //설치 대기시간(변수)
            turretAttWait = GlobalData.choi_m_TrList[ii].m_activateTime;              //공격 대기(변수)
            turretAttType = GlobalData.choi_m_TrList[ii].m_attackType;              //공격 타입(변수)
            turretSensor = GlobalData.choi_m_TrList[ii].m_sensor;               //공격 가능거리(변수)
            turretAttRange = GlobalData.choi_m_TrList[ii].m_range;             //공격 감지거리(변수)
            turretAttSpeed = GlobalData.choi_m_TrList[ii].m_atkDelay;             //공격속도(변수)
            turretAttDamage = GlobalData.choi_m_TrList[ii].m_dam;                   //공격력 (변수)
            turretAttType = GlobalData.choi_m_TrList[ii].m_isSingleUse;               //공격 범위 타입(변수)
            turretHp = GlobalData.choi_m_TrList[ii].m_hp;                     //CurHp(변수)
            MaxturretHp = GlobalData.choi_m_TrList[ii].m_hp;
            //Debug.Log(GlobalData.choi_TurretNameList[ii]);

            //공격 가능 건물(변수)
            if (GlobalData.choi_m_TrList[ii].m_isAtk == 0)
            {
                isTurret = false; //공격 안하는 건물
            }
            else
            {
                isTurret = true; //공격 하는 건물
            }
            //공격 가능 건물(변수)

            //단일타겟(변수)
            if (GlobalData.choi_m_TrList[ii].m_isSingleTargeting == 0)
            {
                isRangeAtt = false; //범위 공격
            }
            else
            {
                isRangeAtt = true;//단일타겟
            }
            //단일타겟(변수)
        }
        //instantiate 할 때 SetType함수로 정보들 다 세팅하기
        //int로 이름을 받아와서
        //거기 있는 값들을 다 추가해버리면

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
            //공격 공격!!!삐용삐용 탕탕탕 콰ㅏㅇ쾅쾅
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
                //터렛 이펙트 나오기
                //이펙트 호출
                //ObjectReturn();
            }

        }

        public void SoundPlay(ref AudioClip clip)
        {
            if (audio == null) return; //AudioSource를 가져오지 않았을 경우를 대비한 예외 처리

            audio.Stop(); //혹시 아직 재생 중인 효과음이 있을 경우 정지
            if(GlobalData.volumeisOn)
                audio.PlayOneShot(clip, GlobalData.masterVolume); //소리를 재생한다.
            isDeathPlay = false;
        }
    }
}