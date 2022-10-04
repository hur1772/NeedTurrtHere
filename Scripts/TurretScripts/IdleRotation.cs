using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SungJae
{
    public enum turrectState
    {
        Milltia,
        Pyromaniac
    }

    public class IdleRotation : MonoBehaviour
    {
        public turrectState m_turrect = turrectState.Milltia;
        float m_MoveSpeed = 100.0f;
        float m_delay = 2.0f;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (m_turrect == turrectState.Milltia)
            {
                m_delay -= Time.deltaTime;
                if (m_delay <= 0.0f)
                {
                    m_delay = 0.0f;
                    this.transform.Rotate(new Vector3(0.0f, 0.0f, m_MoveSpeed * Time.deltaTime));
                }
            }
            else if(m_turrect == turrectState.Pyromaniac)
            {
                m_delay -= Time.deltaTime;
                if (m_delay <= 0.0f)
                {
                    m_delay = 0.0f;
                    this.transform.Rotate(new Vector3(0.0f, m_MoveSpeed * Time.deltaTime,0.0f));
                }
            }
        }
    }
}
