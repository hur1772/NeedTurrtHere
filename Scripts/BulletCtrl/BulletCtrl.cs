using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;
using Altair;
using LeeSpace;

namespace Choi
{
    public class BulletCtrl : MemoryPoolingFlag
    {
        public enum SplashType
        {
            NonSplash = 0,
            Splash
        }

        public enum BulletType
        {
            Turret,
            Enemy
        }

        public enum ShotType
        {
            Up,
            Front,
            Down,
            Back,
        }

        internal Altair.AttackType attackType = Altair.AttackType.Null;
        public ShotType shotType = ShotType.Front;
        public SplashType splashType = SplashType.NonSplash;
        public BulletType bulletType = BulletType.Turret;
        public float bulletY;
        public int Damage;
        public bool isSlow = false;
        public bool isStun = false;

        public bool ishit = false;
        public int isBack = 1;

        public Vector2 SplashVec = Vector2.zero;
        public GameObject hitObj;
        public float bulletSpeed = 0.089f;
        Vector2 rayVec;

        //유도2(Chasing Bullet) 변수
        bool m_SelRandom = false;
        float r2yRandVal = 0.0f;
        float r1yRandVal = 0.0f;

        RaycastHit2D hit;
        public LayerMask enemylayer;
        public LayerMask turretlayer;

        //관통용 변수
        public List<GameObject> AttackList;

        //곡사용 변수
        [SerializeField] internal Vector3 p1;
        [SerializeField] internal Vector3 p2;
        [SerializeField] internal Vector3 r1;
        [SerializeField] internal Vector3 r2;
        public float value = 0;
        Vector3 BlasticVec = Vector3.zero;

        //사거리 변수
        public float maxXPos = 6;

        //트레일 변수
        TrailRenderer trail;

        //Accelerator(강화장치) 전용 변수
        public LayerMask acceleratorMask;
        RaycastHit2D acceleratorHit;
        public bool isUpgraded = false;



        private void OnDrawGizmos()
        {
            if(this.splashType == SplashType.Splash)
            Gizmos.DrawCube(this.transform.position, SplashVec);
        }

        private void Start() => StartFunc();

        private void StartFunc()
        {

            acceleratorMask = 1 << LayerMask.NameToLayer("Accelerator");

            if (shotType == ShotType.Back)
                isBack = -1;


            trail = GetComponentInChildren<TrailRenderer>();
        }

		private void OnEnable()
		{
            if (trail != null && trail.time <= 0.1f)
                trail.time = 0.0f;
            isUpgraded = false;
        }

		private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
            if (pos.x >= 1.0f || pos.x <= -1.0f)
			{
                ObjectReturn();
                if (trail != null)
                    trail.time = -1.0f;
			}

            if (trail != null && trail.time <= 0.3f)
                trail.time += Time.deltaTime * 1.0f;
        }

        private void FixedUpdate()
        {
            switch (attackType)
            {
                case Altair.AttackType.Directional:
                    DirectionalBullet();
                    break;

                case Altair.AttackType.Balistic:
                    BalisticBullet();
                    break;

                case Altair.AttackType.Homing:
                    HomingBullet();
                    break;

                case Altair.AttackType.Piercing:
                    PiercingBullet();
                    break;

                case Altair.AttackType.Chasing:
                    ChasingBullet();
                    break;

            }
        }

        public void DirectionalBullet()
        {
            //강화장치 체크하는 함수
            CheckAccelerator();

            if(shotType == ShotType.Up)
            {
                if (this.transform.position.y <= bulletY)
                    this.transform.Translate(Vector2.up * bulletSpeed);
            }
            else if(shotType == ShotType.Down)
            {
                if (this.transform.position.y >= bulletY)
                    this.transform.Translate(Vector2.down  * bulletSpeed);
            }

            this.transform.Translate((Vector2.right * isBack)  * bulletSpeed);
            rayVec = new Vector2(this.transform.position.x - 0.5f, this.transform.position.y);

            if (ishit)
                return;

            if(bulletType == BulletType.Turret)
            {
                hit = Physics2D.Raycast(rayVec, Vector2.right, 1.0f, enemylayer);
                Debug.DrawRay(rayVec, Vector2.right * 1.0f, Color.red);
                if (hit)
                {
                    hitObj = hit.collider.gameObject;
                    AttackEnemy(splashType);
                    ishit = true;
                    //this.gameObject.SetActive(false);

                }
            }
            else
            {
                hit = Physics2D.Raycast(rayVec, Vector2.right, 1.0f, turretlayer);
                Debug.DrawRay(rayVec, Vector2.right * 1.0f, Color.red);
                if (hit)
                {
                    hitObj = hit.collider.gameObject;
                    AttackTurret();
                    ishit = true;
                    //this.gameObject.SetActive(false);

                }
            }

        }

        public void BalisticBullet()
        {
            if (hitObj != null)
            {
                p2 = hitObj.transform.position;
                r2 = hitObj.transform.position; 
                r2.y += 5.0f;

                if (hitObj.TryGetComponent(out MonsterCtrl enemy))
                {
                    if (enemy.mon_Hp <= 0)
                        hitObj = null;
                }

                if (value > 0.95f)
                {
                    if (bulletType == BulletType.Turret)
                        AttackEnemy(splashType);
                    else
                        AttackTurret(splashType);
                }
            }            

            if (value >= 0.99f)
			{
                ObjectReturn();
                if (trail != null)
                    trail.time = -1.0f;
			}
                

            if (value < 0.4f)
                value += Time.deltaTime;
            else if (value <= 0.4f && 0.6f < value)
                value += Time.deltaTime * 0.4f;
            else
                value += Time.deltaTime;

            BlasticVec = BezierTest(p1, p2, r1, r2, value);

            Vector3 rotvec = BlasticVec - this.transform.position;
            float angle = Mathf.Atan2(rotvec.y, rotvec.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.eulerAngles = angleAxis.eulerAngles;

            this.transform.position = BlasticVec;
        }

        public void HomingBullet()
        {
            if (hitObj != null)
            {
                p2 = hitObj.transform.position;
                r2 = hitObj.transform.position;
                if (hitObj.TryGetComponent(out MonsterCtrl enemy))
                {
                    if (enemy.mon_Hp <= 0)
                        hitObj = null;
                }

                if (value > 0.95f)
                    AttackEnemy(splashType);
            }

            if (value < 0.4f)
                value += Time.deltaTime * bulletSpeed * 15.0f;
            else if (value <= 0.4f && 0.6f < value)
                value += Time.deltaTime * 0.4f;
            else
                value += Time.deltaTime * bulletSpeed * 15.0f;

            BlasticVec = BezierTest(p1, p2, r1, r2, value);

            if (value >= 0.85f)
            {
                this.transform.Translate(Vector2.right * bulletSpeed * 2);
            }
            else
            {
                Vector3 rotvec = BlasticVec - this.transform.position;
                float angle = Mathf.Atan2(rotvec.y, rotvec.x) * Mathf.Rad2Deg;
                Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                //Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5 * Time.deltaTime);
                transform.eulerAngles = angleAxis.eulerAngles;

                this.transform.position = BlasticVec;
            }
        }

        public void ChasingBullet()
		{
            if (hitObj != null)
            {
                p2 = hitObj.transform.position;
                r2 = hitObj.transform.position;
                if (m_SelRandom == false)
                {
                    //r2xRandVal = Random.Range(-0.25f, 0.25f);
                    r2yRandVal = Random.Range(-0.25f, 0.25f);
                    r1yRandVal = Random.Range(-0.5f, 0.25f);
                    m_SelRandom = true;
                }
                //r2.x += r2xRandVal;
                r2.y += r2yRandVal;
                r1.y += r1yRandVal;

                if (hitObj.TryGetComponent(out MonsterCtrl enemy))
                {
                    if (enemy.mon_Hp <= 0)
                        hitObj = null;
                }

                if (value > 0.95f)
                {
                    AttackEnemy(splashType);
                    m_SelRandom = false;
                }
            }
            if (value < 0.4f)
                value += Time.deltaTime;
            else if (value <= 0.4f && 0.6f < value)
                value += Time.deltaTime * 0.4f;
            else
                value += Time.deltaTime;

            BlasticVec = BezierTest(p1, p2, r1, r2, value);

            if (value >= 0.85f)
            {
                this.transform.Translate(Vector2.right * bulletSpeed * 2);
            }
            else
            {
                Vector3 rotvec = BlasticVec - this.transform.position;
                float angle = Mathf.Atan2(rotvec.y, rotvec.x) * Mathf.Rad2Deg;
                Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                //Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5 * Time.deltaTime);
                transform.eulerAngles = angleAxis.eulerAngles;

                this.transform.position = BlasticVec;
            }
        }

        public void PiercingBullet()
        {
            if (this.transform.position.x <= maxXPos)
                this.transform.Translate(Vector2.right * bulletSpeed);
            else
			{
                ObjectReturn();
                if (trail != null)
                    trail.time = -1.0f;
			}

            for(int i = 0; i < AttackList.Count; i++)
            {
                if(this.transform.position.x >= AttackList[i].transform.position.x)
                {
                    hitObj = AttackList[i];
                    AttackEnemy();
                    AttackList.RemoveAt(i);
                    i--;
                }
                    
            }
        }

        public void AttackEnemy(SplashType isSplashType = SplashType.NonSplash)
        {
            if (ishit)
                return;

            if (hitObj != null)
            {
                if(isSplashType == SplashType.NonSplash)
                {
                    if (hitObj.TryGetComponent(out MonsterCtrl enemy))
                    {
                        if (hitObj.TryGetComponent(out IDamageable target))
                        {
                            target.OnDamage(Damage);
                        }
                        if (attackType != Altair.AttackType.Piercing)
                        ishit = true;

                        if(isSlow)
                        {
                            enemy.slowTimer = 2.0f;
                        }

                        if (isStun)
                        {
                            enemy.stunTimer = 4.0f;
                        }
                    }
                }
                else
                {
                    Collider2D[] colls = Physics2D.OverlapBoxAll(hitObj.transform.position, SplashVec, 0, enemylayer);
                    if(colls != null)
                    {
                        Debug.Log("범위 피해");

                        if(attackType == AttackType.Balistic)
                            for(int i = 0; i < colls.Length; i++)
                            {
                                if(colls[i].TryGetComponent(out MonsterCtrl enemy))
                                {
                                    if (colls[i].TryGetComponent(out IDamageable target))
                                    {
                                        if(colls[i] == hitObj.GetComponent<Collider2D>())
                                        {
                                            target.OnDamage(Damage);
                                            Debug.Log("직격데미지" + Damage);
                                            if (isSlow)
                                                enemy.slowTimer = 2.0f;

                                            continue;
                                        }

                                        target.OnDamage((int)(Damage * 0.3f));
                                        Debug.Log("스플데미지" + ((int)(Damage * 0.3f)).ToString());
                                        if (trail != null)
                                            trail.time = -1.0f;
                                    }
                                    ishit = true;

                                    if (isSlow)
                                    {
                                        Debug.Log("스플 슬로우");
                                        enemy.slowTimer = 2.0f;
                                        //enemy.speed = 0.5f;
                                    }

                                    if (isStun)
                                    {
                                        enemy.stunTimer = 4.0f;
                                    }
                                }
                            }
                        else
                            for (int i = 0; i < colls.Length; i++)
                            {
                                if (colls[i].TryGetComponent(out MonsterCtrl enemy))
                                {
                                    if (colls[i].TryGetComponent(out IDamageable target))
                                    {
                                        target.OnDamage((int)(Damage));
                                        Debug.Log("스플데미지" + ((int)(Damage)).ToString());
                                        if (trail != null)
                                            trail.time = -1.0f;
                                    }
                                    ishit = true;

                                    if (isSlow)
                                    {
                                        Debug.Log("스플 슬로우");
                                        enemy.slowTimer = 2.0f;
                                        //enemy.speed = 0.5f;
                                    }

                                    if (isStun)
                                    {
                                        enemy.stunTimer = 4.0f;
                                    }
                                }
                            }
                    }

                }


                if(ishit)
                {
                    ObjectReturn();
                    if (trail != null)
                        trail.time = -1.0f;
                }

            }
        }

        public void AttackTurret(SplashType isSplashType = SplashType.NonSplash)
        {
            if (ishit)
                return;

            if (hitObj != null)
            {
                if (isSplashType == SplashType.NonSplash)
                {
                    if (hitObj.TryGetComponent(out SungJae.Turret_Ctrl turret))
                    {
                        if (hitObj.TryGetComponent(out IDamageable target))
                        {
                            target.OnDamage(Damage);
                        }
                        if (attackType != Altair.AttackType.Piercing)
                            ishit = true;

                        //if (isStun)
                        //{
                        //    turret.stunTimer = 4.0f;
                        //}
                    }
                }
                else
                {
                    Collider2D[] colls = Physics2D.OverlapBoxAll(hitObj.transform.position, SplashVec, 0, turretlayer);
                    if (colls != null)
                    {


                        for (int i = 0; i < colls.Length; i++)
                        {
                            if (colls[i].TryGetComponent(out SungJae.Turret_Ctrl turret))
                            {
                                if (colls[i].TryGetComponent(out IDamageable target))
                                {
                                    if (colls[i] == hitObj.GetComponent<Collider2D>())
                                    {
                                        target.OnDamage(Damage);
                                        Debug.Log("직격데미지" + Damage);
                                        continue;
                                    }

                                    target.OnDamage((int)(Damage * 0.3f));
                                    Debug.Log("스플데미지" + ((int)(Damage * 0.3f)).ToString());
                                    if (trail != null)
                                        trail.time = -1.0f;
                                }
                                ishit = true;

                                //if (isStun)
                                //{
                                //    turret.stunTimer = 4.0f;
                                //}
                            }
                        }
                    }

                }

                ObjectReturn();
                if (trail != null)
                    trail.time = -1.0f;
            }
        }

        internal Vector2 BezierTest(Vector2 p1, Vector2 p2, Vector2 r1, Vector2 r2, float value)
        {
            Vector2 v1 = Vector2.Lerp(p1, r1, value);
            Vector2 v2 = Vector2.Lerp(r1, r2, value);
            Vector2 v3 = Vector2.Lerp(r2, p2, value);

            Vector2 v4 = Vector2.Lerp(v1, v2, value);
            Vector2 v5 = Vector2.Lerp(v2, v3, value);

            Vector2 v6 = Vector2.Lerp(v4, v5, value);
            return v6;
        }

        public void CheckAccelerator()
        {
            acceleratorHit = Physics2D.Raycast(rayVec, Vector2.right, 0.5f, acceleratorMask);

            if(acceleratorHit)
            {
                acceleratorHit.collider.GetComponent<SungJae.TorchWood>().UpgradeBullet(this);
            }
        }
    }
}