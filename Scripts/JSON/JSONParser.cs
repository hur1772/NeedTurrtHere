using UnityEngine;
using SimpleJSON;
using System;

namespace Altair
{
    public static class JSONParser
    {
        #region DataValidation
        /// <summary>
        /// JSON Node로 파싱하는 bool형 함수
        /// </summary>
        /// <param name="str">TextAsset.text string값</param>
        /// <param name="node">파싱할 Node</param>
        /// <returns></returns>
        internal static bool DataValidation(string str, out JSONNode node)
        {
            node = JSON.Parse(str);
            if (node != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// JSON Node로 파싱하는 bool형 함수
        /// </summary>
        /// <param name="asset">TextAsset 변수</param>
        /// <param name="node">파싱할 Node</param>
        /// <returns></returns>
        internal static bool DataValidation(TextAsset asset, out JSONNode node)
        {
            string str = asset.text;
            node = JSON.Parse(str);
            if (node != null)
                return true;
            else
                return false;
        }


        /// <summary>
        /// JSON Node의 Key에 해당하는 Value를 string으로 꺼내오는 bool형 함수
        /// </summary>
        /// <param name="node">JSON Node의 Key</param>
        /// <param name="str">string value</param>
        /// <returns></returns>
        internal static bool DataValidation(JSONNode node, out string str)
        {
            if (node != null)
            {
                str = node;
                return true;
            }
            else
            {
                str = "";
                return false;
            }
        }

        /// <summary>
        /// JSON Node의 Key에 해당하는 Value를 bool형으로 꺼내오는 bool형 함수
        /// </summary>
        /// <param name="node">JSON Node의 Key</param>
        /// <param name="b">bool value</param>
        /// <returns></returns>
        internal static bool DataValidation(JSONNode node, out bool b)
        {
            if (node != null)
            {
                if (node == 1)
                    b = true;
                else
                    b = false;
                return true;
            }
            else
            {
                b = false;
                return false;
            }
        }

        /// <summary>
        /// JSON Node의 Key에 해당하는 Value를 int형으로 꺼내오는 bool형 함수
        /// </summary>
        /// <param name="node">JSON Node의 Key</param>
        /// <param name="i">int value</param>
        /// <returns></returns>
        internal static bool DataValidation(JSONNode node, out int i)
        {
            if (node != null)
            {
                i = node.AsInt;
                return true;
            }
            else
            {
                i = 0;
                return false;
            }
        }

        /// <summary>
        /// JSON Node의 Key에 해당하는 Value를 float형으로 꺼내오는 bool형 함수
        /// </summary>
        /// <param name="node">JSON Node의 Key</param>
        /// <param name="i">float value</param>
        /// <returns></returns>
        internal static bool DataValidation(JSONNode node, out float i)
        {
            if (node != null)
            {
                i = node.AsFloat;
                return true;
            }
            else
            {
                i = .0f;
                return false;
            }
        }
        #endregion
    }
}