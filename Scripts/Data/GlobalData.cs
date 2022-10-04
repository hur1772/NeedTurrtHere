using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;

namespace Altair
{
    internal enum AttackType
    {
        Null        = 0,    //기타
        Directional = 1,    //직선
        Balistic    = 2,    //곡사
        Homing      = 3,    //유도
        Piercing    = 4,    //관통
        Chasing     = 5,    //유도2
    }
    internal static partial class GlobalData
    {

        internal static TextAsset turretDataJson = Resources.Load<TextAsset>("JSON/PlayerTurretJSON");
        internal static TextAsset enemyDataJson = Resources.Load<TextAsset>("JSON/EnemyJSON");

        internal static JSONNode turretData;
        internal static JSONNode enemyData;

        internal static int curStage = 0;
        public static int choi_userDia;
        public static string choi_userNick;
        public static string choi_UniqueID;
        public static int choi_Stage;
        public static bool Check = false;
        public static float masterVolume = 1.0f;
        public static bool volumeisOn = true;

        public static int[] choi_TurretPickNums = new int[10];
        public static int choi_TurretMaxCount = 6;

        public static int choi_IsPick = -1;

        public static string[] choi_TurretNameList = { "Rocket Turret" , "Air Strike" , "Claymore" , "Electric Turret" , "Double Rocket Turret" , "Militia" , "Flame Thrower" , "Sniper"
                                  ,"EMP Wave","Little Boy","Kamikaze","Contra Turret","IED","Carpet Bombing","Booby Trap","Recycle Turret","Rich Turret","Balistic Turret","Towed Turret",
                                  "Galaxy Turret","ATM","Outpost","Moneylender","Negotiator","Engineer","Constractor","Accelerator","Fortress","Barricade","Alien Collector","Terraforming",
                                  "Protector","AA Turret","Black Market","Multiple Turret","Bank","Pyromaniac","Homing Turret","Arcrite Turret","Debt Collector","Metal Trap","ICBM"};

        public static List<TurretList> choi_m_TrList = new List<TurretList>();

        public static int[] choi_DiamondList = new int[4];
        public static int[] choi_StageList = new int[4];

        public static int[,] choi_m_SaveTrList = new int[4, choi_TurretNameList.Length];

        public static void SaveInitData()
        {
            for (int ii = 0; ii < 4; ii++)
            {
                choi_DiamondList[ii] = 0;
                choi_StageList[ii] = 0;
                Debug.Log(choi_m_SaveTrList.Length);
                for (int aa = 0; aa < choi_TurretNameList.Length; aa++)
                {
                    choi_m_SaveTrList[ii, aa] = 0;
                }
            }
        }

        public static void choi_InitData()
        {
            if (0 < choi_m_TrList.Count)
                return;

            for (int ii = 0; ii < choi_TurretNameList.Length; ii++)
            {
                TurretList trlist = new TurretList();
                trlist.SetType(ii);
                choi_m_TrList.Add(trlist);
            }
            Check = true;
        }


    }
}