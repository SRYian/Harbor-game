using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.Collections;
using System.Collections.Generic;

public class Monster : Agent
{
    public float Movespeed = 20f;

    public float PercentFloorHoles = 0.15f;
    public float vision_distance = 10f;

    private Rigidbody2D rb = null;

    private float episodeTime;
    public float maxTime;

    public void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void OnEpisodeBegin()
    {

        //rb.transform.localPosition = new Vector3(UnityEngine.Random.Range(-5, 5), 1, UnityEngine.Random.Range(-5, 5));
        //rb.transform.rotation = Quaternion.Euler(0, (float)(360 * UnityEngine.Random.value), 0);
        episodeTime = 0;
        base.OnEpisodeBegin();
    }
    public override void OnActionReceived(ActionBuffers actions)
    {

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 p = this.transform.position;

        List<bool> vision_observations = new List<bool>();
        Transform tf = rb.transform;


    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
    private void FixedUpdate()
    {
        episodeTime += Time.deltaTime;
        if (episodeTime > maxTime)
        {
            AddReward(-0.5f);
            EndEpisode();
        }


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ship") == true)
        {
            // om nom nom
            AddReward(1.0f);
            return;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Target") == true)
        {
            AddReward(1.0f + (maxTime - episodeTime) / maxTime);
            EndEpisode();
        }
    }
    private void OnCollisionExit(Collision collision)
    {

    }
    private void OnDrawGizmos()
    {
        Vector3 p = this.transform.position;

        //floor sensors (left, right, back, forward)
        Gizmos.color = Color.magenta;

        // down
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(p, Vector2.down);
    }
}
