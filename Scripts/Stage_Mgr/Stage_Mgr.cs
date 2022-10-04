
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Altair_Memory_Pool_Pro;
using static System.Net.WebRequestMethods;
using LeeSpace;
//using static UnityEditor.PlayerSettings;
//using static UnityEditor.PlayerSettings;
using KJH;
using Altair;

namespace SungJae
{
    public enum StageState
    {
        getmoney,   
        normal1,    
        normal2,    
        normal3,    
        wave,       
        waveStart,  
        afterWave,  
        end         
    }

    public class Stage_Mgr : MonoBehaviour
    {
        protected float instantTime;

        [SerializeField ]protected int getMoney = 100000;

        public int money
        {
            get { return getMoney; }
            set { getMoney = value; }
        }

        Ray ray;
        RaycastHit hit;

        RaycastHit2D vecHit;

        [Header("====스테이지 진행사항 체크====")]
        [SerializeField] protected StageState s_State;
        [SerializeField] protected int killCheck;
        [SerializeField] protected int killCount;
        [SerializeField] protected int getMoney_num;
        [Header("===getMoney -> normal1 killCount")]
        [SerializeField] protected int normal1_num;
        [Header("===normal1 -> normal2 killCount")]
        [SerializeField] protected int normal2_num;
        [Header("===normal2 -> normal3 killCount")]
        [SerializeField] protected int normal3_num;
        [Header("===normal3 -> normal4 killCount")]
        [SerializeField] protected int normal4_num;
        [SerializeField] protected int beforeWave_num;
        [SerializeField] protected int waveCount;
        [SerializeField] protected int afterWave_num;
        [SerializeField] protected float warningTimer = 4.0f;
        [SerializeField] protected bool isGameOver;

        [Header("====Enemy Layer 체크====")]
        public LayerMask enemeyLayer;
        //public List<GameObject> enemyList;

        protected int spawn;
        [Header("====몬스터 생성 위치====")]
        public Transform[] spawnPoint;

        [SerializeField] protected List<List<GameObject>> enemyNum = new List<List<GameObject>>();

        [Header("====몬스터 가까운 거리 체크====")]
        public Transform[] checkVecPoint;


        [Header("===기타 UI===")]
        public static Stage_Mgr instance;
        public Text money_Txt;
        bool moneyClick;
        GameObject saveObj;
        public GameObject firstObj;
        public Text waveWarning;
        protected float timeCheck = 0.0f;


        [Header("====게임 엔드 판넬====")]
        public Image GameEnd_Panel;
        public Button NextStage_Btn;
        public Button Lobby_Btn;

        [Header("=== 게임 오버 판넬====")]
        public Image GameOver_Panel;
        public Button Retry_Btn;
        public Button OverLobby_Btn;


        private void Awake()
        {
            if (!instance) 
                instance = this;
            else Destroy(gameObject);
        }

        //Awake에서 InitData하기
        protected virtual void InitData()
        {
            Time.timeScale = 1.0f;

            //stage 상태 받아놓기
            s_State = StageState.getmoney;

            //몬스터 생성 위치 5개
            spawn = 5;  //몬스터 생성 위치

            //killCount개수
            killCount = 0;

            getMoney_num = 10;
            //wave 시작 전 몬스터 생성 개수
            normal1_num = 20;
            normal2_num = 30;
            normal3_num = 40;

            beforeWave_num = 50;

            //첫번 째 몬스터 생성 시간
            instantTime = 20.0f;

            for(int i = 0; i < spawn; i++)
            {
                List<GameObject> tempList = new List<GameObject>();
                enemyNum.Add(tempList);
            }

            killCheck = 5;
            waveCount = 0;

            money_Txt.text = getMoney.ToString();

            moneyClick = false;

            timeCheck = Random.Range(6.0f, 7.0f);

            GameEnd_Panel.gameObject.SetActive(false);
            GameOver_Panel.gameObject.SetActive(false);


            if (Lobby_Btn != null)
                Lobby_Btn.onClick.AddListener(LobbyBtnFunc);

            if (NextStage_Btn != null)
                NextStage_Btn.onClick.AddListener(NextStageFunc);

            if (Retry_Btn != null)
                Retry_Btn.onClick.AddListener(RetryFunc);

            if (OverLobby_Btn != null)
                OverLobby_Btn.onClick.AddListener(LobbyBtnFunc);

            Time.timeScale = 1.0f;
        }

        //private void Update()
        //{
        //}

        protected virtual void StateChanger(StageState state)
        {
            switch (state)
            {
                case StageState.getmoney:
                    {
                        if (instantTime <= 0.0f)
                        {
                            WaveKillChecker();

                            if (killCheck == 5)
                            {
                                instantTime = Random.Range(3.0f, 5.0f);
                                EnemyInstantiate();
                            }
                        }
                    }
                    break;
                case StageState.normal1:
                    {
                        WaveKillChecker();

                        if (instantTime <= 0.0f)
                        {
                            instantTime = Random.Range(2.0f, 4.0f);
                            EnemyInstantiate();
                        }
                    }
                    break;
                case StageState.normal2:
                    {
                        WaveKillChecker();

                        if (instantTime <= 0.0f)
                        {
                            instantTime = Random.Range(2.0f, 3.5f);
                            EnemyInstantiate();
                        }
                    }
                    break;
                case StageState.normal3:
                    {
                        WaveKillChecker();

                        if (instantTime <= 0.0f)
                        {
                            instantTime = Random.Range(2.0f, 3.0f);
                            EnemyInstantiate();
                        }
                    }
                    break;
                case StageState.wave:
                    {
                        WaveKillChecker();

                        if (killCheck == 5)
                        {
                            warningTimer -= Time.deltaTime;
                            waveWarning.gameObject.SetActive(true);
                            if (warningTimer <= 0.0f)
                            {
                                waveWarning.gameObject.SetActive(false);
                                WaveRefreshChecker();
                            }
                        }
                    }
                    break;
                case StageState.waveStart:
                    {
                        if (instantTime <= 0.0f)
                        {
                            instantTime = 5.0f;
                            //Debug.Log("Check");
                            WaveCreate();

                            if (30 <= waveCount)
                            {
                                s_State = StageState.afterWave;
                            }
                        }
                    }
                    break;
                case StageState.afterWave:
                    {
                        WaveKillChecker();

                        if (killCheck == 5)
                        {
                            s_State = StageState.end;
                        }
                    }
                    break;
                case StageState.end:
                    {
                        if (isGameOver == false)
                        {
                            GameEndFunc();
                        }
                        else
                        {
                            GameOverFunc();
                        }
                    }
                    break;
            }
        }

        protected virtual void EnemyInstantiate()
        {
            int pos = Random.Range(0, spawn);
            GameObject go = MemoryPoolManager.instance.GetObject("Tracks1", spawnPoint[pos].position);
            go.GetComponent<LeeSpace.MonsterCtrl>().MonsterSetting();
            enemyNum[pos].Add(go);
        }

        public void DeleteEnemyList()
        {
            for( int i = 0; i< enemyNum.Count; i++ )
            {
                for (int j = 0; j < enemyNum[i].Count; j++)
                {
                    if (enemyNum[i][j].GetComponent<MonsterCtrl>().mon_Hp <= 0.0f)
                    {
                        if (s_State != StageState.wave && s_State != StageState.waveStart && s_State != StageState.afterWave)
                        {
                            killCount++;
                        }
                        enemyNum[i].RemoveAt(j);

                        if (getMoney_num <= killCount && killCount < normal1_num)
                        {
                            s_State = StageState.normal1;
                        }
                        else if(normal1_num <= killCount && killCount < normal2_num)
                        {
                            s_State = StageState.normal2;
                        }
                        else if(normal2_num <= killCount && killCount < normal3_num)
                        {
                            s_State = StageState.normal3;
                        }
                        else if(killCount == beforeWave_num)
                        {
                            s_State = StageState.wave;
                        }

                        //if (firstObj == enemyNum[i][j] && enemyNum[i][j].GetComponent<MonsterCtrl>().isDie)
                        //    firstObj = null;
                    }
                }
            }
        }
        protected virtual void WaveCreate()
        {
            for(int i = 0; i < enemyNum.Count; i++)
            {
                GameObject go = MemoryPoolManager.instance.GetObject("Tracks1", spawnPoint[i].position);
                go.GetComponent<LeeSpace.MonsterCtrl>().MonsterSetting();
                enemyNum[i].Add(go);
                waveCount++;
            }
        }
        protected virtual void MoneyClick(int money)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "Dollar")
                {
                    Transform objectHit = hit.transform;
                    Debug.Log(objectHit);
                    moneyClick = true;
                    bool b = objectHit.gameObject.TryGetComponent(out BasicDoller doller);

                    if (b)
                    {
                        doller.isClick = true;
                    }
                }
            }
        }

        protected virtual void CristalClick()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "Cristal")
                {
                    Transform objectHit = hit.transform;
                    Debug.Log(objectHit);
                    moneyClick = true;
                    bool b = objectHit.gameObject.TryGetComponent(out CristalAnim cristal);

                    if (b)
                    {
                        cristal.CristalUp();
                    }
                }
            }
        }
        protected void WaveRefreshChecker()
        {
            killCheck = 0;
            instantTime = 1.5f;
            killCount = 0;
            s_State = StageState.waveStart;
        }
        protected void WaveKillChecker()
        {
            killCheck = 0;
            for (int i = 0; i < enemyNum.Count; i++)
            {
                for (int j = 0; j <= enemyNum[i].Count; j++)
                {
                    if (enemyNum[i].Count <= 0)
                    {
                        killCheck++;
                    }
                }
            }
        }
        protected virtual void AutomaticMoney()
        {
            if (0.0f < timeCheck)
            {
                timeCheck -= Time.deltaTime;

                if (timeCheck <= 0.0f)
                {
                    timeCheck = Random.Range(6.0f, 7.0f);
                    MemoryPoolManager.instance.BringObject("DollerAnim", transform.position);
                }
            }
        }
        protected virtual void NearEnemyCheck()
        {
            int firstidx = -1;
            //GameObject obj = null;
            firstObj = null;
            saveObj = null;
            for (int i =0; i < checkVecPoint.Length; i ++)
            {
                vecHit = Physics2D.Raycast(checkVecPoint[i].transform.position, Vector2.right, 17.0f, enemeyLayer);
                Debug.DrawRay(checkVecPoint[i].transform.position, Vector2.right * 17.0f, Color.red);
                if (vecHit)
                {
                    //if (firstObj != null)
                    //{
                    //    if (firstObj.GetComponent<LeeSpace.MonsterCtrl>().mon_Hp <= 0.0f)
                    //        firstObj = null;
                    //}

                    if (saveObj == null)
                    {
                        saveObj = vecHit.collider.gameObject;
                        firstidx = i;
                        //Debug.Log(firstidx);
                    }
                    else
                    {
                        if (vecHit.collider.gameObject.transform.position.x < saveObj.transform.position.x)
                        {
                            saveObj = vecHit.collider.gameObject;
                            firstidx = i;
                            //Debug.Log(firstidx);
                        }
                    }
                    //obj = vecHit.collider.gameObject;
                    firstObj = saveObj;
                }
            }
        }
        public void RefreshMoney()
        {
            if (money_Txt != null)
                money_Txt.text = getMoney.ToString();
        }

        protected void GameEndFunc()
        {
            //게임 엔드 판넬 켜주기
            GameEnd_Panel.gameObject.SetActive(true);

            //게임 일시정지
            Time.timeScale = 0.0f;
        }

        protected void GameOverFunc()
        {
            //게임 오버 판넬 켜주기
            GameOver_Panel.gameObject.SetActive(true);

            //게임 일시정지
            Time.timeScale = 0.0f;
        }

        protected void LobbyBtnFunc()
        {
            Debug.Log("Lobby");
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("TestLobby_Pre");
        }

        protected virtual void NextStageFunc()
        {
            GlobalData.choi_Stage++;

            if ((GlobalData.choi_Stage % 100) >= 5)
            {
                GlobalData.choi_Stage--;
                SceneManager.LoadScene("TestLobby_Pre");
                return;
            }

            Debug.Log(GlobalData.choi_Stage);

            string scene = "";
            scene = "stage" + (GlobalData.choi_Stage % 100).ToString();
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(scene);
        }

        protected virtual void RetryFunc()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Stage1");
            GameOver_Panel.gameObject.SetActive(false);
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            bool b = collision.gameObject.TryGetComponent(out MonsterCtrl ctrl);
            if (b)
            {
                s_State = StageState.end;
                isGameOver = true;
            }
        }
    }
}