using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeeSpace
{
    public class MonEventCtrl : MonoBehaviour
    {
        MonsterCtrl ref_Monster;

        // Start is called before the first frame update
        void Start()
        {
            ref_Monster = transform.parent.GetComponent<MonsterCtrl>();
        }

        void Skill_EventSend(string type)
        {
            ref_Monster.MonAttack(type);
        }

        void Jump_EventSend(string type)
        {
            ref_Monster.JumpEvent(type);
        }

        void Pause_EventSend(int type)
        {
            if (type == 1)
                ref_Monster.PauseOnOff(true);
            else
                ref_Monster.PauseOnOff(false);
        }
    }
}
