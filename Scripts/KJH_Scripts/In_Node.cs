using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KJH
{
    public class In_Node : MonoBehaviour
    {
        [HideInInspector] public GameObject tower;
        Image image;

        void Start()
        {
            image = GetComponent<Image>();
            image.enabled = false;
        }

        private void Update()
        {
            if (tower != null)
            {
                if (tower.activeSelf == false)
                    tower = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name.Contains("Drag"))
            {
                image.enabled = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.name.Contains("Drag"))
            {
                image.enabled = false;
            }
        }
    }
}