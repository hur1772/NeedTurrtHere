using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Altair_Memory_Pool_Pro;
using Altair;
using SungJae;

namespace KJH
{
    public class Button_Item : MonoBehaviour
    {
        private Stage_Mgr stageMgr;

        public Image characterImage;
        public Text tempText;

        public Image delayImage;
        public Image nameImage;
        public Text nameText;

        Vector3[] v = new Vector3[4];
        Vector3 mousePos;

        bool isPick = false;
        bool isUp = false;

        int cost = 0;
        int costCheck = 0;

        float check = 0.0f;
        [SerializeField] float delay = 0.0f;

        // JSON 데이터에 있는 터렛 인덱스
        public int number = -1;
        string prefabName = "";

        public int dragNumber = -1;
        string dragName = "";

        [SerializeField] private bool DeleteBtn = false;

        [SerializeField] private Text costText;

        void Start()
        {
            stageMgr = FindObjectOfType<Stage_Mgr>();
            GetComponent<RectTransform>().GetWorldCorners(v);
        }

        void Update()
        {
            Setting();
            ButtonUpCheck();
            TimeCheck();
        }

        void Setting()
        {
            if (DeleteBtn == true)
            {
                dragName = "Drag_" + 10;
                tempText.text = "삭제";
                return;
            }

            if (GlobalData.choi_m_TrList.Count <= 0)
                GlobalData.choi_InitData();

            if (string.IsNullOrEmpty(nameText.text))
            {
                string str = "icon/" + GlobalData.choi_TurretNameList[number];
                characterImage.sprite = Resources.Load<Sprite>(str);
                dragName = "Drag_" + dragNumber;
                prefabName = GlobalData.choi_TurretNameList[number]; // choi_m_TrList[number].m_name;
                tempText.text = ""; //number.ToString("00");
                cost = GlobalData.choi_m_TrList[number].m_cost;
                delay = GlobalData.choi_m_TrList[number].m_prepTime;
                nameText.text = GlobalData.choi_m_TrList[number].m_name + "\n" + cost;

                costText.text = cost.ToString();

                Debug.Log(GlobalData.choi_TurretNameList[number]);
            }
        }

        void ButtonUpCheck()
        {
            isUp = ButtonInside();

            if (isUp)
                nameImage.gameObject.SetActive(true);
            else
                nameImage.gameObject.SetActive(false);
        }

        void TimeCheck()
        {
            if (0.0f < check)
            {
                check -= Time.deltaTime;

                if (delayImage != null)
                    delayImage.fillAmount = check / delay;

                if (check <= 0.0f)
                    check = 0.0f;
            }
            else
            {
                MouseCheck();
            }
        }

        bool ButtonInside()
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (v[0].x <= mousePos.x && mousePos.x <= v[2].x &&
                v[0].y <= mousePos.y && mousePos.y <= v[2].y)
            {
                return true;
            }

            return false;
        }

        void MouseCheck()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPick = ButtonInside();

                if (isPick) 
                { 
                    costCheck = Stage_Mgr.instance.money;

                    if (cost <= costCheck)
                    {
                        Create();
                    }
                }
            }
        }

        public void MinusMoney()
        {
            costCheck = Stage_Mgr.instance.money;
            costCheck -= cost;
            Stage_Mgr.instance.money = costCheck;
            Stage_Mgr.instance.RefreshMoney();
        }

        void Create()
        {
            Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vec.z = 0.0f;

            //Debug.Log(dragName);

            GameObject go = MemoryPoolManager.instance.GetObject(dragName, vec);

            Drag_Item d = go.GetComponent<Drag_Item>();

            if (d != null)
            {
                if (DeleteBtn == false)
                {
                    d.buttonItem = this;
                }

                d.prefabName = prefabName;

                if (d.characterSprite != null)
                    d.characterSprite.sprite = characterImage.sprite;
                //d.prefabNumber = prefabNumber;
            }
        }

        public void SetDelay()
        {
            check = delay;
        }
    }
}
