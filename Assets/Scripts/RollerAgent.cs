using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RollerAgent : Agent
{
    public Transform target;
    Rigidbody rigidbody;

    //初期化
    public override void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    //エピソード開始時
    public override void OnEpisodeBegin()
    {
        if (this.transform.position.y < 0)
        {
            this.rigidbody.angularVelocity = Vector3.zero;
            this.rigidbody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0);
        }
        target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    //観察所得時
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.position);
        sensor.AddObservation(this.transform.position);
        sensor.AddObservation(rigidbody.velocity.x);
        sensor.AddObservation(rigidbody.velocity.y);
    }

    //行動実行時に呼ばれる
    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rigidbody.AddForce(controlSignal * 10);

        //到着
        float distanceToTarget = Vector3.Distance(this.transform.position, target.position);
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        //床から落下した時
        if(this.transform.position.y < 0)
        {
            EndEpisode();
        }
    }

    //ヒューリスティック
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
}
