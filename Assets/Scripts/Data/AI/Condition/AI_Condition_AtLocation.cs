using UnityEngine;

[CreateAssetMenu(fileName = "Condition_AtLocation", menuName = "AI/Condition/AtLocation")]
public class AI_Condition_AtLocation : AI_Condition
{
    public override bool CheckCondition(AI ai)
    {
        return AtSoundLocation(ai);
    }

    private bool AtSoundLocation(AI ai)
    {
        if (Vector3.Distance(ai.SoundLastPosition, ai.Rigidbody.position) < 0.5f)
        {
            ai.Agent.stoppingDistance = ai.BaseStoppingDist;
            return true;
        }

        return false;
    }
}
