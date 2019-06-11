using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Idle", menuName = "AI/Action/Idle")]
public class AI_Action_Idle : AI_Action
{
    public override void PreformAction(AI ai)
    {
        Idle(ai);
    }

    public void Idle(AI ai)
    {
        return;
    }
}
