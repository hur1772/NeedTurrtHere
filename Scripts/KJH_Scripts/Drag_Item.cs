using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Altair_Memory_Pool_Pro;
using SungJae;

namespace KJH
{
    public enum DragType
    {
        Create,
        Delete
    }

    public class Drag_Item : MemoryPoolingFlag
    {
        public DragType type;
        [HideInInspector] public GameObject tower;
        [HideInInspector] public Button_Item buttonItem;

        GameObject ground;
        bool isCreate = false;
        Vector3 vec;

        public SpriteRenderer characterSprite;

        [HideInInspector] public string prefabName = "";

        void Start()
        {

        }

        void Update()
        {
            MouseButtonDown();
            MouseDrag();
            MouseButtonUp();
        }

        void MouseButtonDown()
        {
            if (Input.GetMouseButtonDown(1))
            {
                ObjectReturn();
            }
        }

        void MouseDrag()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                vec.z = 0.0f;

                this.transform.position = vec;
            }
        }

        void MouseButtonUp()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (isCreate == true)
                {
                    if (type == DragType.Delete)
                        Delete();
                    else
                        Create();
                }

                ObjectReturn();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name.Contains("In_Node"))
            {
                isCreate = true;
                vec = collision.transform.position;
                ground = collision.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.name.Contains("In_Node"))
            {
                isCreate = false;
                vec = Vector3.zero;
                ground = null;
            }
        }

        void Create()
        {
            if (ground != null)
            {
                In_Node node = ground.GetComponent<In_Node>();

                if (node != null)
                {
                    if (node.tower != null)
                        return;
                    else
                    {
                        if (buttonItem != null)
                        {
                            buttonItem.MinusMoney();
                            buttonItem.SetDelay();
                        }

                        vec.z = 0.0f;

                        GameObject go = MemoryPoolManager.instance.GetObject(prefabName, vec);

                        if (go.GetComponent<Turret_Ctrl>() != null)
                            go.GetComponent<Turret_Ctrl>().ShotPoint = vec;

                        node.tower = go;
                    }
                }
            }
        }

        void Delete()
        {
            if (ground != null)
            {
                In_Node node = ground.GetComponent<In_Node>();

                if (node != null)
                {
                    if (node.tower != null)
                    {
                        node.tower.GetComponent<Turret_Ctrl>().turretObjectReturn();
                        node.tower = null;
                    }
                    else
                        return;
                }
            }
        }
    }
}
