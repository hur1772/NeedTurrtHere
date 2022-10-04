using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace SungJae
{
    public class Stage1_Mgr : Stage_Mgr
    {
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

        //반장 추가
        protected override void InitData()
        {
            base.InitData();
            beforeWave_num = 50;
        }

        //int baseEnemy = 0;
        //protected override void EnemyInstantiate()
        //{
        //    baseEnemy++;
        //    int pos = Random.Range(0, spawn);
        //    int ran = 0;
        //    if(baseEnemy < 10)
        //        ran = 0;
        //    else if (baseEnemy >= 10 && baseEnemy < 20)
        //        ran = Random.Range(0, 3);
        //    else if (baseEnemy >= 20 && baseEnemy < 35)
        //        ran = Random.Range(0, 7);
        //    else
        //        ran = Random.Range(0, 9);


        //    GameObject go = null;
        //    switch (ran)
        //    {
        //        case 0:
        //            go = MemoryPoolManager.instance.GetObject("Tracks1", spawnPoint[pos].position);
        //            break;
        //        case 1:
        //            go = MemoryPoolManager.instance.GetObject("DoubleBarrel", spawnPoint[pos].position);
        //            break;
        //        case 2:
        //            go = MemoryPoolManager.instance.GetObject("DoubleTank", spawnPoint[pos].position);
        //            break;
        //        case 3:
        //            go = MemoryPoolManager.instance.GetObject("HeavyShieldGLauncher", spawnPoint[pos].position);
        //            break;
        //        case 4:
        //            go = MemoryPoolManager.instance.GetObject("JumpingTank", spawnPoint[pos].position);
        //            break;
        //        case 5:
        //            go = MemoryPoolManager.instance.GetObject("QuadRupedSniper", spawnPoint[pos].position);
        //            break;
        //        case 6:
        //            go = MemoryPoolManager.instance.GetObject("SelfDestroy", spawnPoint[pos].position);
        //            break;
        //        case 7:
        //            go = MemoryPoolManager.instance.GetObject("Tank1", spawnPoint[pos].position);
        //            break;
        //        case 8:
        //            go = MemoryPoolManager.instance.GetObject("Walker", spawnPoint[pos].position);
        //            break;
        //        default:
        //            break;
        //    }
        //    go.GetComponent<LeeSpace.MonsterCtrl>().MonsterSetting();
        //    enemyNum[pos].Add(go);
        //}
    }

}