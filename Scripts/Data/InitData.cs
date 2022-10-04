using UnityEngine;

namespace Altair
{
    public partial class InitData : MonoBehaviour
    {
        private bool isActivate;

        public bool MyProperty
        {
            get { return isActivate; }
            set
            {
                isActivate = value;
                if (isActivate)
                    gameObject.SetActive(true);
            }
        }

        //private void Start() => StartFunc();

        private void Awake()
        {
            if (!LoadJsonData()) return;
            if (!ParsingJsonData()) return;
        }

        private void StartFunc()
        {
        
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (!LoadJsonData()) return;
            if (!ParsingJsonData()) return;

            gameObject.SetActive(false);
        }

        private bool LoadJsonData()
        {
            if (GlobalData.turretDataJson == null)
            {
                GlobalData.turretDataJson = Resources.Load<TextAsset>("JSON/PlayerTurretJSON");
                return false;
            }
            if (GlobalData.enemyDataJson == null)
            {
                GlobalData.enemyDataJson = Resources.Load<TextAsset>("JSON/EnemyJSON");
                return false;
            }

            return true;
        }

        private bool ParsingJsonData()
        {
            if (GlobalData.turretData == null)
                if (!JSONParser.DataValidation(GlobalData.turretDataJson, out GlobalData.turretData))
                    return false;

            if (GlobalData.enemyData == null)
                if (!JSONParser.DataValidation(GlobalData.enemyDataJson, out GlobalData.enemyData))
                    return false;

            return true;
        }
    }
}