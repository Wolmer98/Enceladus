using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_Action : ScriptableObject
{
    public bool isFixedUpdate;
    public abstract void PreformAction(AI ai);

    public void Move(AI ai, float speed)
    {

        //if (ai.Stats == null)
        //{
        //    return;
        //}

        ////Move Position
        //Vector3 target = ai.Agent.nextPosition;
        //Vector3 source = ai.transform.position;
        //float dist = Vector3.Distance(target, source);
        //if (dist < 0.1)
        //{
        //    return;
        //}
        //else
        //{
        //    if(dist > 0.2)
        //    {
        //        ai.Agent.speed = speed /2;
        //        if (dist > 0.8f)
        //        {
        //            ai.Agent.nextPosition = ai.Rigidbody.position;
        //        }
        //    }
        //    else
        //    {
        //        ai.Agent.speed = speed + float.Epsilon;
        //    }
            
        //    Vector3 moveVector = (target - source).normalized;
        //    if(ai.Agent.isOnOffMeshLink)
        //    {
        //        ai.Agent.speed = speed / 5;
        //        ai.Rigidbody.MovePosition(ai.transform.position + moveVector * speed * 1.5f * Time.fixedDeltaTime);
        //        ai.Rigidbody.AddForce(Vector3.up * 4f);
        //    }
        //    else
        //    {
        //        target.y = 0; source.y = 0;
        //        ai.Rigidbody.MovePosition(ai.transform.position + moveVector * speed * Time.fixedDeltaTime);
        //    }
        //}
    }

    public void Rotate(Vector3 lookPosition, AI ai)
    {
        //Torque Works somewhat, gravity needs to be false for best effect.
        //Vector3 target = lookPosition;
        //Vector3 source = ai.transform.position;
        //target.y = 0; source.y = 0;
        //Vector3 targetDir = (target - source).normalized;
        //float dotProd = Vector3.Dot(targetDir, ai.transform.forward);
        //Debug.Log(dotProd);
        //if (dotProd < 0.99)
        //{
        //    //float torque = Time.fixedDeltaTime * 40f;

        //    float angle = Mathf.Clamp(Vector3.SignedAngle(ai.transform.forward, targetDir, Vector3.up), -1, 1);
        //    Debug.Log(angle);

        //    ai.Rigidbody.AddTorque(ai.transform.up * 4f * angle, ForceMode.VelocityChange);
        //    Debug.Log(ai.Rigidbody.angularVelocity);

        //}
        //else
        //{
        //    ai.Rigidbody.angularVelocity = new Vector3(0, 0, 0);
        //}

        //Test5, rigidbody must be frozen in rotation, WORKS
        Vector3 targetDir = lookPosition - ai.transform.position;
        targetDir.y = 0;
        float step = ai.Stats.rotationSpeed * Time.fixedDeltaTime;

        Vector3 newDir = Vector3.RotateTowards(ai.transform.forward, targetDir, step, 0.0f);
        Debug.DrawRay(ai.transform.position, newDir, Color.red);
        ai.transform.rotation = Quaternion.LookRotation(newDir);
    }


    private void TwitchyMovement(AI ai)
    {
        
       // ai.Rigidbody.AddForce
    }

}
