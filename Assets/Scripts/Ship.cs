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
    public float areaSizeMin;

    private float episodeTime;
    public float maxTime;

    public bool training;
    private SetController setController;
    public bool logDebug = false;

    public void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }
    public override void Initialize()
    {
        targetTransform = transform.parent.Find("Target");
        setController = GetComponentInParent<SetController>();
        base.Initialize();
    }
    public override void OnEpisodeBegin()
    {
        episodeTime = 0;
        moveSpeed = 0;
        turnSpeed = 0;
        //randomizePosition();
        if (training)
            setController.OnEpisodeBegin();
        else
            crashShip();
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
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), vision_distance, LayerMask.GetMask("Obstacles", "Monster"));
            bool sees_monster = (hit && hit.collider.gameObject.CompareTag("Monster"));
            bool sees_obstacle = (hit && hit.collider.gameObject.CompareTag("Obstacle"));
            bool sees_goal = (hit && hit.collider.gameObject.CompareTag("Target"));
            sensor.AddObservation(sees_monster ? hit.distance / vision_distance : 1.0f);
            sensor.AddObservation(sees_obstacle ? hit.distance / vision_distance : 1.0f);
            sensor.AddObservation(sees_goal ? hit.distance / vision_distance : 1.0f);
            // todo : raycast to detect a light object 1 if true 0 if false
            bool sees_something = sees_goal || sees_monster || sees_obstacle;
            bool sees_light = false;
            if (sees_something)
                sees_light |= true;
            RaycastHit2D hit_light = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), vision_distance, LayerMask.GetMask("Light"));
            sees_light |= (hit_light && hit_light.collider.gameObject.CompareTag("Light"));
            sensor.AddObservation(sees_light);
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

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") == true)
        {
            // crash
            AddReward(-1f);
            setController.EndMonster(-GetCumulativeReward());
            EndEpisode();
            return;
        }

        if (collision.gameObject.CompareTag("Monster") == true)
        {
            // eaten, om nom
            AddReward(-1.5f);
            setController.EndMonster(1f);
            EndEpisode();
            return;
        }

        if (collision.gameObject.CompareTag("Target") == true)
        {
            AddReward(1.0f + (maxTime - episodeTime) / maxTime);
            setController.EndMonster(-GetCumulativeReward());
            EndEpisode();
            Debug.Log("poggersssssssss");
        }
        if(logDebug)
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

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), vision_distance, LayerMask.GetMask("Obstacles", "Monster"));
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
            {
                RaycastHit2D hit_light = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), vision_distance, LayerMask.GetMask("Light"));
                if (hit_light)
                {
                    Gizmos.color = Color.blue;
                }
                //else
                    Gizmos.DrawRay(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)) * vision_distance);
            }
            
        }
        return;
    }

    private void FixedUpdate()
    {
        episodeTime += Time.deltaTime;
        if (episodeTime > maxTime)
        {
            AddReward(-0.6f);
            setController.EndMonster(0.0f);
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

    public void randomizePosition()
    {
        Vector3 newPosition = new Vector3(UnityEngine.Random.Range(areaSizeMin, areaSize), UnityEngine.Random.Range(-areaSize, areaSize), 0);
        float randomSign1 = Mathf.Sign(UnityEngine.Random.value - 0.5f);
        float randomSign2 = Mathf.Sign(UnityEngine.Random.value - 0.5f);
        float randomSign3 = Mathf.Sign(UnityEngine.Random.value - 0.5f);
        if (randomSign3 < 0)
        {
            var tempx = newPosition.x;
            var tempy = newPosition.y;
            newPosition.x = tempy;
            newPosition.y = tempx;
        }
        newPosition.x *= randomSign1;
        newPosition.y *= randomSign2;
        rb.transform.localPosition = newPosition;

        float directionNoise = 45 - UnityEngine.Random.value * 90;
        Vector2 deltaDirection = targetTransform.localPosition - transform.localPosition;
        float facingTargetAngle = Mathf.Atan2(deltaDirection.y, deltaDirection.x) * Mathf.Rad2Deg;
        if(logDebug)
            print("angle to goal:" + facingTargetAngle);
        facingTargetAngle -= 90; //default facing top
        rb.transform.rotation = Quaternion.Euler(0,0, facingTargetAngle + directionNoise);

        if(randomTargetPosition)
            targetTransform.gameObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(-areaSize, areaSize), UnityEngine.Random.Range(-areaSize, areaSize), 0);
    }

    public void crashShip()
    {
        //Debug.Log("I crashed!!");
    }
}
