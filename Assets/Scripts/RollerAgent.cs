using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{
    public Transform target;
    public float speed = 10f;

    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    public override void AgentReset()
    {
        if (transform.position.y < 0)
        {
            //If the Agent fell, zero its momentum
            body.angularVelocity = Vector3.zero;
            body.velocity = Vector3.zero;
            transform.position = new Vector3(0f, 0.5f, 0f);
        }

        //Move the target to a new spot
        target.position = new Vector3(Random.value * 8f - 4f, 0.5f, Random.value * 8f - 4f);
    }

    public override void CollectObservations()
    {
        //Position of the Agent itself
        AddVectorObs(transform.position);

        //Position of the Target
        AddVectorObs(target.position);

        //Agent velocity
        AddVectorObs(body.velocity.x);
        AddVectorObs(body.velocity.z);
    }

    public override void AgentAction(float[] action)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = action[0];
        controlSignal.z = action[1];
        body.AddForce(controlSignal * speed);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        //Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            Done();
        }

        //Fell off platform
        if (transform.position.y < 0)
            Done();
    }

    public override float[] Heuristic()
    {
        float[] action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
}
