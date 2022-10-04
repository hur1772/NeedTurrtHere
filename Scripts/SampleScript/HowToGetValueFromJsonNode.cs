using System;
using UnityEngine;

namespace Altair
{
    [ExecuteInEditMode]
    public class HowToGetValueFromJsonNode : MonoBehaviour
    {
        private string str = "";

        //private void Start() => StartFunc();

        private void StartFunc()
        {
        
        }

        //private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
        
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(36, 7, 2000, 1500), "<color=#ffff00><size=30>" +
                        str + "</size></color>");

            if (GUI.Button(new Rect(990, 33, 200, 45),
                "<size=28>" + "JSonFileLoad" + "</size>") == true)
            {
                CheckJson();
            }
        }

        //GlobalData의 JSON Node에서 값을 가져오는 방법
        private void CheckJson()
        {
            string name = "";
            int cost = 0;
            int dam = 0;
            if (!JSONParser.DataValidation(GlobalData.turretData["Rocket Turret"]["name"], out name)) return;
            if (!JSONParser.DataValidation(GlobalData.turretData["Rocket Turret"]["cost"], out cost)) return;
            if (!JSONParser.DataValidation(GlobalData.turretData["Rocket Turret"]["dam"], out dam)) return;

            str = $"이름 {name}, 가격 {cost}, 공격력 {dam}";
        }
    }
}