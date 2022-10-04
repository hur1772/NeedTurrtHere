using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Altair;
using Altair_Memory_Pool_Pro;

namespace LeeSpace
{
    enum MonsterState
    {
        Idle,
        Run,
        Attack,
        Jump,
    }

    public class MonsterCtrl : MemoryPoolingFlag, IDamageable, ISoundPlay
    {
        MonsterState MonState = MonsterState.Idle;      // ���� ���� state
        MonsterState CurState = MonsterState.Idle;      // �ִϸ��̼ǿ� state
        private Animator animator;

        float MoveSpeed = 0.0066f;                  // �̵��ӵ�
        public float slowTimer = 0.0f;              // �̵��ӵ� ���� Ÿ�̸�
        public float stunTimer = 0.0f;              // ���� Ÿ�̸�
        float AttackDist = 0.0f;                    // ���� �����Ÿ�
        public int mon_Hp = 96;                     // ���� ü��
        int mon_MaxHp = 96;                         // ���� �ִ� ü��
        int mon_AttPoint = 10;                      // ���� ���ݷ�
        float DieTime = 1.5f;                       // ���� ��� �ִϸ��̼� ����
        float AttackCool = 0.0f;                    // ���� ���� ��Ÿ��
        float AttackCurCool = 0.0f;                 // ���� ���� ���� ��Ÿ��
        int isShield = 0;                           // ���� �ǵ� ����
        int mon_ShieldHp = 0;                       // ���� �ǵ� ü��
        public bool isDie = false;                  // ���� ��� ����
        bool pause = false;                         // ��� ���⶧ ����� ����
        Vector3 Effectpos;                          // ����Ʈ �� �Ѿ� ȣ�� ��ġ�� ����
        private RaycastHit2D hit;                   // ���ݻ�Ÿ� üũ�� Raycast

        Vector2 size = new Vector2(4.0f, 4.0f);     // Physics2D.OverlapBoxAll�� ����� ���� ������ ���� Vecter ����
        Vector2 a_StepVec;                          // ���� �̵� ���� �� �Ÿ�

        // --- ������ ����
        Vector3 p1, p2;                             // ������ ��� ���� ���� �������� ��Ÿ�� Vecter ����
        Vector3 r1, r2;                             // ������ ��� ���̸� ���� ���� �������� ��Ÿ�� Vecter ����
        float Jump_Power = 0.0f;                    // ������ ��� �̵��� ���� Value�� ����
        int Jumping = 0;                            // ���� �ִϸ��̼� ��Ʈ���� ���� ����
        bool isJump = false;                        // �ش� ������ ������ ������ �������� �ƴ��� üũ�ϱ� ���� ����
        // --- ������ ����

        public GameObject ArmorObj;                 // ������ ���� �����ϴ� ����
        public MeshRenderer[] mon_Mesh;             // ������ ���� ������ ���� MeshRenderer �迭
        public SkinnedMeshRenderer[] mon_Mesh2;     // ������ ���� ������ ���� SkinnedMeshRenderer �迭
        Color color_Mesh = Color.white;             // ������ ���� ������ ���� Color ����
        float color_Time = 255.0f;                  // ������ ���� ������ ���� float ����
        bool isDestruction = false;                 // ���� ������ ���ݽ� �ǰ� ����Ʈ�� �ȳ�Ÿ���� �ϱ� ���� ����        

        private new AudioSource audio;              // AudioSource Component�� �ҷ��´�.
        [SerializeField] private AudioClip[] clip;  // AudioClip�� �����´�. 
        private bool isFirstPlay = true;            // true�� �ʱ�ȭ �Ѵ�.
        float sound_col = 0.0f;                     // �̵� ���� ��Ÿ��

        // Clip �迭
        // 0�� -> ���ݻ���, (���������� ��� �������嵵 ����)
        // 1�� -> ����� ����
        // 2�� -> �̵��� ����

        void OnEnable()
        {
            MonsterSetting();
            if (isFirstPlay)
                isFirstPlay = false; //ù ��° ������ ��� �Ҹ��� ������� �ʰ� boolen�� true�� ��ȯ�Ѵ�.
            else
                SoundPlay(ref clip[2]);     // ��� �� �̵� ���� ���
        }

        // Start is called before the first frame update
        void Start()
        {
            animator = this.gameObject.GetComponentInChildren<Animator>();
            audio = GetComponent<AudioSource>();
            if (clip[0] == null)
                clip = Resources.LoadAll<AudioClip>("EnemySound");
            MonsterSetting();
        }

        // Update is called once per frame
        void Update()
        {
            if (pause == true)
                return;

            Color_Change();
            StunChack();
            ShieldChack();
            MonActionUpdate();
            AttackCheck();
            DieCheck();
        }

        private void FixedUpdate()
        {
            if (isDie == true || Jumping != 0 || stunTimer >= 0)
                return;

            if (pause == true)
                return;

            if (MonState == MonsterState.Run)
            {
                Monster_Move();
                MoveSound();
            }
        }

        void MoveSound()            // ���� �̵����� ȣ��
        {
            if (this.transform.position.x > 8.5f)
                return;

            sound_col -= Time.deltaTime;

            if (sound_col <= 0)
            {
                if (Random.Range(0, 3) == 0)        // ���� Ȯ���θ� ����� Range() ���� ���ڸ� ������ �Ҹ��� ���� Ȯ���� ������
                    SoundPlay(ref clip[2]);

                sound_col = 4.0f;                   // ���� ��Ÿ�� ����
            }
        }

        void DieCheck()
        {
            if (isDie == true)
            {
                DieTime -= Time.deltaTime;

                if (DieTime <= 0.02f)
                {
                    animator.SetTrigger("Idle");
                }

                if (DieTime <= 0)
                {
                    int random_drop = Random.Range(0, 3);

                    if (random_drop == 0)
                    {
                        GameObject Diamondobj = MemoryPoolManager.instance.GetObject("CristalAnim", Effectpos);
                    }

                    ObjectReturn();
                }
            }
        }

        void ShieldChack()
        {
            if (isShield == 0 && ArmorObj != null)
            {
                if (ArmorObj.activeSelf == true)
                    ArmorObj.SetActive(false);
            }
        }


        void StunChack()
        {
            if (stunTimer >= 0)
            {
                stunTimer -= Time.deltaTime;
                return;
            }
        }

        public void MonsterSetting()
        {
            if (this.gameObject.name.Contains("Tracks1"))
            {
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie"]["hp"], out mon_MaxHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie"]["moveSpeed"], out MoveSpeed)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie"]["atkDelay"], out AttackCool)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie"]["atkRange"], out AttackDist)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie"]["haveShield"], out isShield)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie"]["shieldHp"], out mon_ShieldHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie"]["dam"], out mon_AttPoint)) return;
                AttackDist = 3.0f;
            }
            else if (this.gameObject.name.Contains("Tank1"))
            {
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Gargantuar"]["hp"], out mon_MaxHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Gargantuar"]["moveSpeed"], out MoveSpeed)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Gargantuar"]["atkDelay"], out AttackCool)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Gargantuar"]["atkRange"], out AttackDist)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Gargantuar"]["haveShield"], out isShield)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Gargantuar"]["shieldHp"], out mon_ShieldHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Gargantuar"]["dam"], out mon_AttPoint)) return;
                AttackDist = 5.0f;
            }
            else if (this.gameObject.name.Contains("DoubleBarrel"))
            {
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie2"]["hp"], out mon_MaxHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie2"]["moveSpeed"], out MoveSpeed)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie2"]["atkDelay"], out AttackCool)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie2"]["atkRange"], out AttackDist)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie2"]["haveShield"], out isShield)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie2"]["shieldHp"], out mon_ShieldHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["zombie2"]["dam"], out mon_AttPoint)) return;
                AttackDist = 3.0f;
            }
            else if (this.gameObject.name.Contains("Walker"))
            {
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pole Vaultimg Zombie"]["hp"], out mon_MaxHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pole Vaultimg Zombie"]["moveSpeed"], out MoveSpeed)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pole Vaultimg Zombie"]["atkDelay"], out AttackCool)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pole Vaultimg Zombie"]["atkRange"], out AttackDist)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pole Vaultimg Zombie"]["haveShield"], out isShield)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pole Vaultimg Zombie"]["shieldHp"], out mon_ShieldHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pole Vaultimg Zombie"]["dam"], out mon_AttPoint)) return;
                isJump = false;
                AttackDist = 0.5f;
            }
            else if (this.gameObject.name.Contains("Grenader"))
            {
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Conehead Zombie"]["hp"], out mon_MaxHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Conehead Zombie"]["moveSpeed"], out MoveSpeed)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Conehead Zombie"]["atkDelay"], out AttackCool)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Conehead Zombie"]["atkRange"], out AttackDist)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Conehead Zombie"]["haveShield"], out isShield)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Conehead Zombie"]["shieldHp"], out mon_ShieldHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Conehead Zombie"]["dam"], out mon_AttPoint)) return;
                AttackDist = 5.0f;
            }
            else if (this.gameObject.name.Contains("DoubleTank"))
            {
                // DoubleTank ����
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Football Zombie"]["hp"], out mon_MaxHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Football Zombie"]["moveSpeed"], out MoveSpeed)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Football Zombie"]["atkDelay"], out AttackCool)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Football Zombie"]["atkRange"], out AttackDist)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Football Zombie"]["haveShield"], out isShield)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Football Zombie"]["shieldHp"], out mon_ShieldHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Football Zombie"]["dam"], out mon_AttPoint)) return;
                AttackDist = 3.0f;
            }
            else if (this.gameObject.name.Contains("QuadRupedSniper"))
            {
                // QuadRupedSniper ����
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Newspaper Zombie"]["hp"], out mon_MaxHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Newspaper Zombie"]["moveSpeed"], out MoveSpeed)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Newspaper Zombie"]["atkDelay"], out AttackCool)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Newspaper Zombie"]["atkRange"], out AttackDist)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Newspaper Zombie"]["haveShield"], out isShield)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Newspaper Zombie"]["shieldHp"], out mon_ShieldHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Newspaper Zombie"]["dam"], out mon_AttPoint)) return;
                AttackDist = 3.0f;
            }
            else if (this.gameObject.name.Contains("HeavyShieldGLauncher"))
            {
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Screen Door Zombie"]["hp"], out mon_MaxHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Screen Door Zombie"]["moveSpeed"], out MoveSpeed)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Screen Door Zombie"]["atkDelay"], out AttackCool)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Screen Door Zombie"]["atkRange"], out AttackDist)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Screen Door Zombie"]["haveShield"], out isShield)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Screen Door Zombie"]["shieldHp"], out mon_ShieldHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Screen Door Zombie"]["dam"], out mon_AttPoint)) return;
                AttackDist = 3.0f;
            }
            else if (this.gameObject.name.Contains("JumpingTank"))
            {
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pogo Zombie"]["hp"], out mon_MaxHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pogo Zombie"]["moveSpeed"], out MoveSpeed)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pogo Zombie"]["atkDelay"], out AttackCool)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pogo Zombie"]["atkRange"], out AttackDist)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pogo Zombie"]["haveShield"], out isShield)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pogo Zombie"]["shieldHp"], out mon_ShieldHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Pogo Zombie"]["dam"], out mon_AttPoint)) return;
                isJump = false;
                AttackDist = 0.5f;
            }
            else if (this.gameObject.name.Contains("SelfDestroy"))
            {
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Jack-in-the-box Zombie"]["hp"], out mon_MaxHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Jack-in-the-box Zombie"]["moveSpeed"], out MoveSpeed)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Jack-in-the-box Zombie"]["atkDelay"], out AttackCool)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Jack-in-the-box Zombie"]["atkRange"], out AttackDist)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Jack-in-the-box Zombie"]["haveShield"], out isShield)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Jack-in-the-box Zombie"]["shieldHp"], out mon_ShieldHp)) return;
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyData["Jack-in-the-box Zombie"]["dam"], out mon_AttPoint)) return;
                isDestruction = false;
                AttackDist = 0.5f;
            }

            if (ArmorObj != null)
            {
                ArmorObj.SetActive(true);
            }

            mon_Mesh = this.gameObject.GetComponentsInChildren<MeshRenderer>();
            mon_Mesh2 = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            MonState = MonsterState.Idle;
            CurState = MonsterState.Idle;
            mon_Hp = mon_MaxHp;
            isDie = false;
            this.gameObject.layer = 6;
            DieTime = 1.5f;
        }

        void Monster_Move()
        {
            if (slowTimer >= 0.0f)
            {
                a_StepVec = (Vector2.left * MoveSpeed) / 2;

                slowTimer -= Time.deltaTime;
                if (slowTimer <= 0.0f)
                {
                    a_StepVec = (Vector2.left * MoveSpeed);
                }
            }

            transform.Translate(a_StepVec, Space.World);
        }

        void MonActionUpdate()
        {
            if (isDie == true)
                return;

            switch (MonState)
            {
                case MonsterState.Idle:
                    {
                        if (CurState != MonState)
                        {
                            animator.SetTrigger("Idle");
                        }
                    }
                    break;

                case MonsterState.Run:
                    {
                        if (CurState != MonState)
                        {
                            animator.SetTrigger("Move");
                            CurState = MonState;
                        }
                    }
                    break;

                case MonsterState.Attack:
                    {
                        if (CurState != MonState || AttackCurCool >= AttackCool)
                        {
                            animator.SetTrigger("Attack");
                            CurState = MonState;
                            AttackCurCool = 0.0f;
                        }
                    }
                    break;

                case MonsterState.Jump:
                    {
                        if (CurState != MonState)
                        {
                            animator.SetTrigger("Jump");
                            CurState = MonState;
                        }

                        if (Jumping >= 1)
                        {
                            MonsterJumping();
                            Jump_Power += Time.deltaTime;
                        }
                    }
                    break;
            }
        }

        #region --- ���� ����

        public void JumpEvent(string Send_Event)
        {
            if (Send_Event == "Jump")
            {
                Debug.Log(Send_Event);
                p1 = this.transform.position;
                p2 = this.transform.position + new Vector3(-2.2f, 0, 0);

                r1 = this.transform.position + new Vector3(0, 2.5f, 0);
                r2 = this.transform.position + new Vector3(-2.2f, 2.5f, 0);

                Jumping = 1;
                Jump_Power = 0.0f;

                SoundPlay(ref clip[1]);

                if (this.gameObject.name.Contains("JumpingTank"))
                {
                    animator.SetTrigger("Jumping");
                }

                this.gameObject.layer = 0;
            }
            else if (Send_Event == "Randing")
            {
                if (this.gameObject.name.Contains("Walker"))
                    isJump = true;

                this.gameObject.layer = 6;
                MonState = MonsterState.Run;
            }
        }

        GameObject Flameobj;

        void MonsterJumping()
        {
            Vector2 v1 = Vector2.Lerp(p1, r1, Jump_Power);
            Vector2 v2 = Vector2.Lerp(r1, r2, Jump_Power);
            Vector2 v3 = Vector2.Lerp(r2, p2, Jump_Power);

            Vector2 v4 = Vector2.Lerp(v1, v2, Jump_Power);
            Vector2 v5 = Vector2.Lerp(v2, v3, Jump_Power);

            Vector2 v6 = Vector2.Lerp(v4, v5, Jump_Power);

            this.transform.position = v6;

            if (Jump_Power == 0 && Jumping >= 1)
            {
                Effectpos = this.transform.position;
                Effectpos.y += -0.2f;
                Effectpos.z -= 3f;

                if (this.gameObject.name.Contains("Walker"))
                    Flameobj = MemoryPoolManager.instance.GetObject("Flame", Effectpos, new Vector3(90f, -90f, 0f));

            }

            if (Jump_Power >= 0.5f && Jumping == 1)
            {
                if (this.gameObject.name.Contains("JumpingTank"))
                {
                    Effectpos = this.transform.position;

                    GameObject Bulletobj = MemoryPoolManager.instance.GetObject("Grenade1", Effectpos);
                    if (Bulletobj != null && Bulletobj.TryGetComponent(out Choi.BulletCtrl bullet))
                    {
                        bullet.hitObj = hit.collider.gameObject;
                        bullet.p1 = this.gameObject.transform.position;
                        bullet.r1 = this.gameObject.transform.position;
                        bullet.value = 0.0f;

                        bullet.Damage = mon_AttPoint;
                        bullet.bulletType = Choi.BulletCtrl.BulletType.Enemy;
                        bullet.shotType = Choi.BulletCtrl.ShotType.Back;
                        bullet.attackType = AttackType.Balistic;
                        bullet.ishit = false;
                    }
                }

                Jumping = 2;
            }

            if (Jump_Power >= 1.0f)
            {
                animator.SetTrigger("Randing");
                Jumping = 0;
            }

            if (this.gameObject.name.Contains("Walker"))
            {
                Effectpos = this.transform.position;
                Effectpos.y += -0.2f;
                Effectpos.z -= 3f;
                Flameobj.transform.position = Effectpos;
            }
        }

        #endregion

        #region --- ���� ����

        void AttackCheck()
        {
            if (MonState == MonsterState.Jump)
                return;

            Debug.DrawRay(transform.position - new Vector3(0.5f, -0.3f, 0),
                    new Vector3(-1f, 0, 0) * AttackDist, new Color(0, 1, 0));
            hit = Physics2D.Raycast(transform.position - new Vector3(0.5f, -0.3f, 0),
                    new Vector3(-1f, 0, 0), AttackDist, 1 << 7);

            if (hit.collider != null)
            {
                if (this.transform.position.x > 8.5f)
                {
                    MonState = MonsterState.Run;
                    return;
                }

                if (MonState != MonsterState.Attack && hit.collider.gameObject.layer == 7)
                {
                    MonState = MonsterState.Attack;
                }

                if (slowTimer >= 0.0f)
                    AttackCurCool += Time.deltaTime / 2;
                else
                    AttackCurCool += Time.deltaTime;
            }
            else
            {
                MonState = MonsterState.Run;
            }
        }

        public void MonAttack(string type)
        {
            if (!MemoryPoolManager.instance)
                return;

            if (type == "Tracks1")
            {
                Effectpos = this.transform.position;
                Effectpos.y += 0.8f;
                Effectpos.x += -0.5f;

                GameObject Bulletobj = MemoryPoolManager.instance.GetObject("Bullet1", Effectpos);
                if (Bulletobj != null && Bulletobj.TryGetComponent(out Choi.BulletCtrl bullet))
                {
                    bullet.Damage = mon_AttPoint;
                    bullet.bulletType = Choi.BulletCtrl.BulletType.Enemy;
                    bullet.shotType = Choi.BulletCtrl.ShotType.Back;
                    bullet.attackType = AttackType.Directional;
                    bullet.ishit = false;
                }

                Effectpos.z -= 3f;

                GameObject Effectobj = MemoryPoolManager.instance.GetObject("MuzzleFlashEffect", Effectpos, Quaternion.Euler(0, 0, 180f));
            }
            else if (type == "Tank1")
            {
                Effectpos = this.transform.position;
                Effectpos.y += 0.9f;
                Effectpos.x += -0.8f;

                GameObject Bulletobj = MemoryPoolManager.instance.GetObject("Bullet1", Effectpos);
                if (Bulletobj != null && Bulletobj.TryGetComponent(out Choi.BulletCtrl bullet))
                {
                    bullet.Damage = mon_AttPoint;
                    bullet.bulletType = Choi.BulletCtrl.BulletType.Enemy;
                    bullet.shotType = Choi.BulletCtrl.ShotType.Back;
                    bullet.attackType = AttackType.Directional;
                    bullet.ishit = false;
                }

                Effectpos.z -= 3f;
                Effectpos.x += -0.5f;

                GameObject Effectobj = MemoryPoolManager.instance.GetObject("RocketMuzzle", Effectpos, Quaternion.Euler(0, 0, 180f));
            }
            else if (type == "DoubleBarrel")
            {
                Effectpos = this.transform.position;
                Effectpos.y += 0.6f;
                Effectpos.x += -0.6f;

                GameObject Bulletobj = MemoryPoolManager.instance.GetObject("Bullet1", Effectpos);
                if (Bulletobj != null && Bulletobj.TryGetComponent(out Choi.BulletCtrl bullet))
                {
                    bullet.Damage = mon_AttPoint;
                    bullet.bulletType = Choi.BulletCtrl.BulletType.Enemy;
                    bullet.shotType = Choi.BulletCtrl.ShotType.Back;
                    bullet.attackType = AttackType.Directional;
                    bullet.ishit = false;
                }

                Effectpos.z -= 3f;
                
                GameObject Effectobj = MemoryPoolManager.instance.GetObject("MuzzleFlashEffect", Effectpos, Quaternion.Euler(0, 0, 180f));
            }
            else if (type == "Walker")
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == 7)
                    {
                        if (hit.collider.gameObject.tag != "NotJump")
                        {
                            if (isJump == false && MonState != MonsterState.Jump)
                            {
                                MonState = MonsterState.Jump;
                                Jump_Power = 0.0f;
                                return;
                            }
                        }

                        hit.collider.gameObject.GetComponent<SungJae.Turret_Ctrl>().OnDamage(mon_AttPoint);
                    }

                    Effectpos = this.transform.position;
                    Effectpos.x += -0.46f;
                    Effectpos.y += 1.1f;
                    Effectpos.z -= 3f;

                    GameObject Flametobj = MemoryPoolManager.instance.GetObject("Flame", Effectpos, new Vector3(30f, -90f, 0f));
                }
            }
            else if (type == "Grenader")
            {
                Effectpos = this.transform.position;
                Effectpos.y += 1.2f;
                Effectpos.x += -0.45f;

                GameObject Bulletobj = MemoryPoolManager.instance.GetObject("Grenade1", Effectpos);
                if (Bulletobj != null && Bulletobj.TryGetComponent(out Choi.BulletCtrl bullet))
                {
                    bullet.hitObj = hit.collider.gameObject;
                    bullet.p1 = Effectpos;
                    bullet.r1 = Effectpos;
                    bullet.r1.y += 0.5f;
                    bullet.value = 0.0f;

                    bullet.Damage = mon_AttPoint;
                    bullet.bulletType = Choi.BulletCtrl.BulletType.Enemy;
                    bullet.shotType = Choi.BulletCtrl.ShotType.Back;
                    bullet.attackType = AttackType.Balistic;
                    bullet.ishit = false;
                }

                Effectpos.z -= 3f;

                GameObject Effectobj = MemoryPoolManager.instance.GetObject("RocketMuzzle", Effectpos, Quaternion.Euler(0, 0, 150f));
            }
            else if (type == "DoubleTank")
            {
                // DoubleTank ����
                Effectpos = this.transform.position;
                Effectpos.x += -0.5f;
                Effectpos.y += 0.3f;

                GameObject Bulletobj = MemoryPoolManager.instance.GetObject("Bullet1", Effectpos);
                if (Bulletobj != null && Bulletobj.TryGetComponent(out Choi.BulletCtrl bullet))
                {
                    bullet.Damage = mon_AttPoint;
                    bullet.bulletType = Choi.BulletCtrl.BulletType.Enemy;
                    bullet.shotType = Choi.BulletCtrl.ShotType.Back;
                    bullet.attackType = AttackType.Directional;
                    bullet.ishit = false;
                }

                Effectpos.z -= 3f;

                GameObject Effectobj = MemoryPoolManager.instance.GetObject("RocketMuzzle", Effectpos, Quaternion.Euler(0, 0, 180f));
            }
            else if (type == "QuadRupedSniper")
            {
                // QuadRupedSniper ����
                Effectpos = this.transform.position;
                Effectpos.x += -0.6f;
                Effectpos.y += 0.75f;

                GameObject Bulletobj = MemoryPoolManager.instance.GetObject("Bullet1", Effectpos);
                if (Bulletobj != null && Bulletobj.TryGetComponent(out Choi.BulletCtrl bullet))
                {
                    bullet.Damage = mon_AttPoint;
                    bullet.bulletType = Choi.BulletCtrl.BulletType.Enemy;
                    bullet.shotType = Choi.BulletCtrl.ShotType.Back;
                    bullet.attackType = AttackType.Directional;
                    bullet.ishit = false;
                }

                Effectpos.z -= 3f;

                GameObject Effectobj = MemoryPoolManager.instance.GetObject("MuzzleFlashEffect", Effectpos, Quaternion.Euler(0, 0, 180f));

            }
            else if (type == "HeavyShieldGLauncher")
            {
                Effectpos = this.transform.position;
                Effectpos.x += -0.5f;
                Effectpos.y += 0.8f;

                GameObject Bulletobj = MemoryPoolManager.instance.GetObject("Bullet1", Effectpos);
                if (Bulletobj != null && Bulletobj.TryGetComponent(out Choi.BulletCtrl bullet))
                {
                    bullet.Damage = mon_AttPoint;
                    bullet.bulletType = Choi.BulletCtrl.BulletType.Enemy;
                    bullet.shotType = Choi.BulletCtrl.ShotType.Back;
                    bullet.attackType = AttackType.Directional;
                    bullet.ishit = false;
                }

                Effectpos.z -= 3f;

                GameObject Effectobj = MemoryPoolManager.instance.GetObject("MuzzleFlashEffect", Effectpos, Quaternion.Euler(0, 0, 180f));
            }
            else if (type == "JumpingTank")
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == 7)
                    {
                        if (hit.collider.gameObject.tag != "NotJump")
                        {
                            if (isJump == false && MonState != MonsterState.Jump)
                            {
                                MonState = MonsterState.Jump;
                                Jump_Power = 0.0f;
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            else if (type == "SelfDestroy")
            {
                isDestruction = true;
                color_Time = 255.0f;
            }

            if (type != "SelfDestroy")
                SoundPlay(ref clip[0]);         // ���� ���ݽ� ���� ȣ�� �����κ��� Destruction_Damage()���� ���� ȣ��
            AttackCurCool = 0.0f;
        }

        #endregion

        #region --- ����� ����

        void Destruction_Damage()
        {
            Collider2D[] hitColl = Physics2D.OverlapBoxAll(this.transform.position, size, 0, 1 << 7);

            for (int i = 0; hitColl.Length > i; i++)
            {
                hitColl[i].gameObject.GetComponent<SungJae.Turret_Ctrl>().OnDamage(mon_AttPoint);
            }
            Effectpos = this.transform.position;
            Effectpos.z -= 3f;

            SoundPlay(ref clip[0]);         // ���� ���� ���� ȣ��
            GameObject Dieobj = MemoryPoolManager.instance.GetObject("SquashExplosive", Effectpos, Quaternion.Euler(0, 0, 0));
            ObjectReturn();
        }

        public void PauseOnOff(bool pause_state)
        {
            pause = pause_state;

            if (pause == false)
            {                
                MonState = MonsterState.Idle;
                CurState = MonsterState.Idle;
            }
        }

        public void StealShield()
        {
            isShield = 0;
            isJump = true;
        }

        void ShieldDamage(int dam)
        {
            mon_ShieldHp -= dam;

            Debug.Log("mon_ShieldHp : " + mon_ShieldHp);

            if (mon_ShieldHp <= 0)
            {
                isShield = 0;

                if (ArmorObj != null)
                {
                    animator.SetTrigger("ArmorDestroy");
                    PauseOnOff(true);
                }
            }

            color_Mesh = Color.red;
            for (int i = 0; mon_Mesh.Length > i; i++)
            {
                mon_Mesh[i].material.color = color_Mesh;
            }
            for (int i = 0; mon_Mesh2.Length > i; i++)
            {
                mon_Mesh2[i].material.color = color_Mesh;
            }

            if(isDestruction == false)
                color_Time = 0.0f;
        }

        void Color_Change()
        {
            if (isDestruction == true)
            {
                if (color_Time > 0)
                {
                    color_Time -= Time.deltaTime * 255;
                    color_Mesh = new Color(1f, color_Time / 255.0f, color_Time / 255.0f);
                    if (color_Time <= 0)
                    {
                        color_Time = 0.0f;
                        Destruction_Damage();
                    }
                }
            }
            else
            {
                if (color_Time < 255)
                {
                    color_Time += Time.deltaTime * 255 * 2;
                    color_Mesh = new Color(1f, color_Time / 255.0f, color_Time / 255.0f);
                    if (color_Time >= 255)
                        color_Mesh = Color.white;
                }
            }

            for (int i = 0; mon_Mesh.Length > i; i++)
            {
                mon_Mesh[i].material.color = color_Mesh;
            }
            for (int i = 0; mon_Mesh2.Length > i; i++)
            {
                mon_Mesh2[i].material.color = color_Mesh;
            }
        }

        public void OnDamage(int dam)
        {
            if (isDie == true)
                return;

            if (this.transform.position.x > 8.5f)
                return;

                if (isShield >= 1)
            {
                ShieldDamage(dam);
                return;
            }

            mon_Hp -= dam;

            Debug.Log("mon_Hp : " + mon_Hp);

            if (mon_Hp <= 0)
            {
                this.gameObject.layer = 0;
                isDie = true;
                animator.SetTrigger("Die");

                Effectpos = this.transform.position;
                Effectpos.z -= 3f;

                SoundPlay(ref clip[1]);         // ���� ����� ���� ȣ��
                GameObject Dieobj = MemoryPoolManager.instance.GetObject("SmallExplosive", Effectpos, Quaternion.Euler(0, 0, 0));
            }

            color_Mesh = Color.red;
            for (int i = 0; mon_Mesh.Length > i; i++)
            {
                mon_Mesh[i].material.color = color_Mesh;
            }
            for (int i = 0; mon_Mesh2.Length > i; i++)
            {
                mon_Mesh2[i].material.color = color_Mesh;
            }

            if (isDestruction == false)
                color_Time = 0.0f;
        }

        protected override void InitOnDisable()
        {
            SungJae.Stage_Mgr.instance.DeleteEnemyList();
        }

        #endregion

        public void SoundPlay(ref AudioClip clip)
        {
            if (audio == null) return; //AudioSource�� �������� �ʾ��� ��츦 ����� ���� ó��

            audio.Stop(); //Ȥ�� ���� ��� ���� ȿ������ ���� ��� ����

            if (GlobalData.volumeisOn)
                audio.PlayOneShot(clip, GlobalData.masterVolume); //�Ҹ��� ����Ѵ�.
        }
    }
}
