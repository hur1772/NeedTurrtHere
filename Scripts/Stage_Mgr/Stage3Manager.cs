using UnityEngine;
using UnityEngine.SceneManagement;
using SungJae;
using Altair_Memory_Pool_Pro;

namespace Altair
{
    public class Stage3Manager : Stage_Mgr
    {
        int baseEnemy = 0;
        private void Start() => StartFunc();

        private void StartFunc()
        {
            InitData();
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (Input.GetMouseButtonDown(0))
            {
                MoneyClick(10);
                CristalClick();
            }

            NearEnemyCheck();

            AutomaticMoney();

            StateChanger(s_State);


            instantTime -= Time.deltaTime;

            //Debug.Log(s_State);
        }

        protected override void InitData()
        {
            base.InitData();
            beforeWave_num = 45;
        }

        protected override void EnemyInstantiate()
        {
            baseEnemy++;
            int pos = Random.Range(0, spawn);
            int ran = 0;

            if (baseEnemy > 9)
                ran = Random.Range(0, 5);

            GameObject go = null;
            switch (ran)
            {
                case 0:
                    go = MemoryPoolManager.instance.GetObject("Tracks1", spawnPoint[pos].position);
                    break;
                case 1:
                    go = MemoryPoolManager.instance.GetObject("DoubleBarrel", spawnPoint[pos].position);
                    break;
                case 2:
                    go = MemoryPoolManager.instance.GetObject("DoubleTank", spawnPoint[pos].position);
                    break;
                case 3:
                    go = MemoryPoolManager.instance.GetObject("HeavyShieldGLauncher", spawnPoint[pos].position);
                    break;
                case 4:
                    go = MemoryPoolManager.instance.GetObject("JumpingTank", spawnPoint[pos].position);
                    break;
                default:
                    break;
            }
            go.GetComponent<LeeSpace.MonsterCtrl>().MonsterSetting();
            enemyNum[pos].Add(go);
        }

        //protected override void NextStageFunc()
        //{
        //    Time.timeScale = 1.0f;
        //    SceneManager.LoadScene("Stage5");
        //}
    }
}