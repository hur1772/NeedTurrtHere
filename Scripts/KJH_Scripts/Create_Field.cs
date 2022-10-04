using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;

namespace KJH
{
    public class Create_Field : MonoBehaviour
    {
        [SerializeField] private Transform inNodeParent;
        [SerializeField] private Transform buttonNodeParent;
        [SerializeField] private GameObject inNode;
        [SerializeField] private GameObject buttonNode;

        Vector2 nodeStartVec = new Vector2((int)-375.0f, (int)175.0f);
        Vector2 nodeValue = new Vector2((int)110, (int)108);

        Vector2 buttonStartVec = new Vector2((int)-580, (int)300);
        int buttonXValue = 100;

        int[] testArray;

        int[] buttonArray;
        public int buttonListCount = 8;

        Vector2 vec = Vector2.zero;

        void Start()
        {
            //testArray = new int[buttonListCount];

            //testArray[0] = 0;
            //testArray[1] = 4;
            //testArray[2] = 3;
            //testArray[3] = 17;
            //testArray[4] = 20;
            //testArray[5] = 35;
            //testArray[6] = -1;
            //testArray[7] = -1;

            SettingList(GlobalData.choi_TurretPickNums, GlobalData.choi_TurretPickNums.Length);

            Create_InNode();
            Create_Button();
        }

        public void SettingList(int[] array, int count)
        {
            if (array.Length != count)
                return;

            buttonArray = new int[count];

            for (int i = 0; i < count; i++)
            {
                buttonArray[i] = array[i];
            }
        }

        void Create_InNode()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    vec.x = nodeStartVec.x + (nodeValue.x * i);
                    vec.y = nodeStartVec.y - (nodeValue.y * j);

                    GameObject go = Instantiate(inNode, vec, Quaternion.identity);

                    go.transform.SetParent(inNodeParent, false);
                }
            }
        }

        void Create_Button()
        {
            for (int i = 0; i < buttonListCount; i++)
            {
                vec.x = buttonStartVec.x + (buttonXValue * i);
                vec.y = buttonStartVec.y;

                if (buttonArray[i] < 0)
                    continue;

                GameObject go = Instantiate(buttonNode, vec, Quaternion.identity);

                bool b = go.TryGetComponent(out Button_Item item);

                if (b)
                {
                    item.number = buttonArray[i];
                    item.dragNumber = i;
                }

                go.transform.SetParent(buttonNodeParent, false);
            }
        }
    }
}
