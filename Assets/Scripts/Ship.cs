using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.Collections;
using System.Collections.Generic;

public class Ship : Agent
{
    public float maxMoveSpeed = 20f;
    public float moveSpeed = 0f;
    public float maxSpeedChange = 1f;
    public float decelerationMultiplier = 0.3f;
    public bool randomTargetPosition = false;

    public float maxTurnSpeed = 15; //degrees
    public float turnSpeed = 0f;

    public float vision_distance = 10f;

    private Rigidbody2D rb = null;

    public List<float> visionAngles;

    private Transform targetTransform;

    public float areaSize;

    private float episodeTime;
    public float maxTime;

    public void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }
    public override void Initialize()
    {
        targetTransform = transform.parent.Find("Target");
        base.Initialize();
    }
    public override void OnEpisodeBegin()
    {
        episodeTime = 0;
        randomizePosition();
        base.OnEpisodeBegin();
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<float> continuousActions = actions.ContinuousActions;
        AddVel(continuousActions[1]);
        turnShip(continuousActions[0]);

        base.OnActionReceived(actions);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 p = this.transform.position;

        // position
        sensor.AddObservation(transform.localPosition.x / 30f); // 2
        sensor.AddObservation(transform.localPosition.y / 30f);
        sensor.AddObservation(transform.rotation.z / Mathf.PI); // 1

        sensor.AddObservation((targetTransform.localPosition.x - transform.localPosition.x) / 30f); // 2
        sensor.AddObservation((targetTransform.localPosition.y - transform.localPosition.y) / 30f);
        // ship speed
        sensor.AddObservation(moveSpeed / maxMoveSpeed);

        // vision
        List<bool> vision_observations = new List<bool>();
        Transform tf = rb.transform;
        foreach (float visionAngle in visionAngles) // 3 * 8 = 24
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * visionAngle);
            float y = Mathf.Sin(Mathf.Deg2Rad * visionAngle);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), vision_distance);
            bool sees_monster = (hit && hit.collider.gameObject.CompareTag("Monster"));
            bool sees_obstacle = (hit && hit.collider.gameObject.CompareTag("Obstacle"));
            bool sees_goal = (hit && hit.collider.gameObject.CompareTag("Target"));
            sensor.AddObservation(sees_monster ? hit.distance / vision_distance : 1.0f);
            sensor.AddObservation(sees_obstacle ? hit.distance / vision_distance : 1.0f);
            sensor.AddObservation(sees_goal ? hit.distance / vision_distance : 1.0f);
            //if (hit) print("keliatan");
        }
        base.CollectObservations(sensor);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //Heuristic: is a good way to test you actions to see if they work as you would expect
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = 0;  //movement x axis   
        continuousActionsOut[1] = 0;  //movement z axis

        if (Input.GetKey(KeyCode.LeftArrow) == true)
            continuousActionsOut[0] = -1;
        if (Input.GetKey(KeyCode.RightArrow) == true)
            continuousActionsOut[0] = 1;

        if (Input.GetKey(KeyCode.UpArrow) == true)
            continuousActionsOut[1] = 1;
        if (Input.GetKey(KeyCode.DownArrow) == true)
            continuousActionsOut[1] = -1;

        if (Input.GetKey(KeyCode.UpArrow) == true)
            print("up pressed");

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") == true)
        {
            // crash
            AddReward(-1f);
            EndEpisode();
            return;
        }

        if (collision.gameObject.CompareTag("Monster") == true)
        {
            // eaten, om nom
            AddReward(-1.0f);
            EndEpisode();
            return;
        }

        if (collision.gameObject.CompareTag("Target") == true)
        {
            AddReward(1.0f + (maxTime - episodeTime) / maxTime);
            EndEpisode();
        }

        print("collided");
    }

    private void OnDrawGizmos()
    {
        Vector3 p = transform.position;
        Transform tf = transform;
        foreach (float visionAngle in visionAngles)
        {

            float x = Mathf.Cos(Mathf.Deg2Rad * visionAngle);
            float y = Mathf.Sin(Mathf.Deg2Rad * visionAngle);

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), vision_distance);
            bool sees_monster = (hit && hit.collider.gameObject.CompareTag("Monster"));
            bool sees_obstacle = (hit && hit.collider.gameObject.CompareTag("Obstacle"));
            bool sees_goal = (hit && hit.collider.gameObject.CompareTag("Target"));
            bool saw_something = sees_goal || sees_monster || sees_obstacle;

            Gizmos.color = Color.white;
            if (sees_monster)
            {
                Gizmos.color = Color.red;
            }
            if (sees_obstacle)
            {
                Gizmos.color = Color.black;
            }
            if (sees_goal)
            {
                Gizmos.color = Color.green;
            }
            if (saw_something)
            {
                Vector2 dp = hit.point - new Vector2(p.x, p.y);
                Gizmos.DrawRay(new Vector2(p.x, p.y), dp);
                
            } 
            else 
                Gizmos.DrawRay(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)) * vision_distance);
        }
        return;
    }

    private void FixedUpdate()
    {
        episodeTime += Time.deltaTime;
        if (episodeTime > maxTime)
        {
            AddReward(-0.9f);
            EndEpisode();
        }

        this.transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        this.transform.rotation *= Quaternion.Euler(0, 0, turnSpeed);
    }

    private void Update()
    {
    }

    private void AddVel(float scale)
    {
        float newVal = scale * maxSpeedChange;
        //float newVal = scale * maxMoveSpeed;
        if (newVal < 0) newVal *= decelerationMultiplier;
        newVal = Mathf.Clamp(moveSpeed + newVal, 0, maxMoveSpeed);
        moveSpeed = newVal;
    }

    private void turnShip(float scale)
    {
        turnSpeed = -maxTurnSpeed * scale;
    }

    private void randomizePosition()
    {
        rb.transform.localPosition = new Vector3(UnityEngine.Random.Range(-areaSize, areaSize), UnityEngine.Random.Range(-areaSize, areaSize), 0);
        rb.transform.rotation = Quaternion.Euler(0, 0, (float)(360 * UnityEngine.Random.value));

        if(randomTargetPosition)
            targetTransform.gameObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(-areaSize, areaSize), UnityEngine.Random.Range(-areaSize, areaSize), 0);
    }
}
