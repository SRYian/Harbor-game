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
    public float light_vision_distance = 10f;

    public float moveSpeed;
    public float maxMoveSpeed = 10;

    public float turnSpeed;
    public float maxTurnSpeed = 30;

    private Rigidbody2D rb = null;

    public List<float> visionAngles;

    public float areaSize;
    public float areaSizeMin;

    private float episodeTime;
    public float maxTime;

    private SetController setController;

    public void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }
    public override void Initialize()
    {
        setController = GetComponentInParent<SetController>();
        base.Initialize();
    }
    public override void OnEpisodeBegin()
    {
        //rb.transform.localPosition = new Vector3(UnityEngine.Random.Range(-5, 5), 1, UnityEngine.Random.Range(-5, 5));
        //rb.transform.rotation = Quaternion.Euler(0, (float)(360 * UnityEngine.Random.value), 0);
        episodeTime = 0;
        //setController.OnEpisodeBegin();
        base.OnEpisodeBegin();
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<float> continuousActions = actions.ContinuousActions;
        setVel(continuousActions[1]);
        turn(continuousActions[0]);

        base.OnActionReceived(actions);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 p = this.transform.position;

        // position
        sensor.AddObservation(transform.localPosition.x / 30f); // 2
        sensor.AddObservation(transform.localPosition.y / 30f);
        sensor.AddObservation(transform.rotation.z / Mathf.PI); // 1

        // vision
        List<bool> vision_observations = new List<bool>();
        Transform tf = rb.transform;
        foreach (float visionAngle in visionAngles) // 3 * 8 = 24
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * visionAngle);
            float y = Mathf.Sin(Mathf.Deg2Rad * visionAngle);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), vision_distance, LayerMask.GetMask("Obstacles", "Ship"));
            bool sees_obstacle = (hit && hit.collider.gameObject.CompareTag("Obstacle"));
            bool sees_ship = (hit && hit.collider.gameObject.CompareTag("Ship"));
            bool saw_something = sees_ship || sees_obstacle;

            if (!saw_something)
            {
                hit = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), 5.0f, LayerMask.GetMask("Darken"));
            }

            sees_ship = (hit && hit.collider.gameObject.CompareTag("Ship"));
            sees_obstacle = (hit && (hit.collider.gameObject.CompareTag("Obstacle") || hit.collider.gameObject.CompareTag("Target")));

            sensor.AddObservation(sees_ship ? hit.distance / vision_distance : 1.0f);
            sensor.AddObservation(sees_obstacle ? hit.distance / vision_distance : 1.0f);
            // todo : raycast to detect a light object 1 if true 0 if false
            bool sees_light = false;
            RaycastHit2D hit_light = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), light_vision_distance, LayerMask.GetMask("Light"));
            sees_light |= (hit_light && hit_light.collider.gameObject.CompareTag("Light"));
            if (sees_ship)
                AddReward(0.01f);
            sensor.AddObservation(sees_light);
        }
        base.CollectObservations(sensor);

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
    private void FixedUpdate()
    {
        episodeTime += Time.deltaTime;
        if (episodeTime > maxTime)
        {
            //AddReward(-0.6f);
            //EndEpisode();
        }

        this.transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        this.transform.rotation *= Quaternion.Euler(0, 0, turnSpeed);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ship") == true)
        {
            // om nom nom
            //AddReward(1.0f);
            //EndEpisode();
            return;
        }
        if (collision.gameObject.CompareTag("Obstacle") | collision.gameObject.CompareTag("Target") == true)
        {
            // navrak
            AddReward(-0.2f);
            return;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //if (collision.gameObject.CompareTag("Target") == true)
        //{
        //    AddReward(1.0f + (maxTime - episodeTime) / maxTime);
        //    EndEpisode();
        //}
    }
    private void OnCollisionExit(Collision collision)
    {

    }
    private void OnDrawGizmos()
    {
        Vector3 p = transform.position;
        Transform tf = transform;
        foreach (float visionAngle in visionAngles)
        {

            float x = Mathf.Cos(Mathf.Deg2Rad * visionAngle);
            float y = Mathf.Sin(Mathf.Deg2Rad * visionAngle);

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), vision_distance, LayerMask.GetMask("Ship", "Obstacles"));
            bool sees_ship = (hit && hit.collider.gameObject.CompareTag("Ship"));
            bool sees_obstacle = (hit && (hit.collider.gameObject.CompareTag("Obstacle") || hit.collider.gameObject.CompareTag("Target")));
            bool saw_something = sees_ship || sees_obstacle;

            if (!saw_something)
            {
                hit = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), 5.0f, LayerMask.GetMask("Darken"));
            }

            sees_ship = (hit && hit.collider.gameObject.CompareTag("Ship"));
            sees_obstacle = (hit && (hit.collider.gameObject.CompareTag("Obstacle") || hit.collider.gameObject.CompareTag("Target")));
            saw_something = sees_ship || sees_obstacle;

            Gizmos.color = Color.white;
            if (sees_ship)
            {
                Gizmos.color = Color.red;
            }
            if (sees_obstacle)
            {
                Gizmos.color = Color.black;
            }
            if (saw_something)
            {
                Vector2 dp = hit.point - new Vector2(p.x, p.y);
                Gizmos.DrawRay(new Vector2(p.x, p.y), dp);
            }
            else
            {
                RaycastHit2D hit_light = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), light_vision_distance, LayerMask.GetMask("Light"));
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

    private void setVel(float scale)
    {
        moveSpeed = (scale + 1) / 2 * maxMoveSpeed;
    }

    private void turn(float scale)
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

        float directionNoise = UnityEngine.Random.value * 360;
       
        rb.transform.rotation = Quaternion.Euler(0, 0, directionNoise);
    }

    public void MakeSound()
    {
        Vector3 p = transform.position;
        Transform tf = transform;
        foreach (float visionAngle in visionAngles)
        {

            float x = Mathf.Cos(Mathf.Deg2Rad * visionAngle);
            float y = Mathf.Sin(Mathf.Deg2Rad * visionAngle);

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), vision_distance, LayerMask.GetMask("Ship", "Obstacles"));
            bool sees_ship = (hit && hit.collider.gameObject.CompareTag("Ship"));
            bool sees_obstacle = (hit && (hit.collider.gameObject.CompareTag("Obstacle") || hit.collider.gameObject.CompareTag("Target")));
            bool saw_something = sees_ship || sees_obstacle;

            if (!saw_something)
            {
                hit = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), 5.0f, LayerMask.GetMask("Darken"));
            }

            sees_ship = (hit && hit.collider.gameObject.CompareTag("Ship"));
            sees_obstacle = (hit && (hit.collider.gameObject.CompareTag("Obstacle") || hit.collider.gameObject.CompareTag("Target")));
            saw_something = sees_ship || sees_obstacle;

            if (sees_ship)
            {
                //do stuff
            }
            if (sees_obstacle)
            {
                //do stuff
            }
            if (saw_something)
            {
                Vector2 dp = hit.point - new Vector2(p.x, p.y);
                //do stuff
            }
            else
            {
                RaycastHit2D hit_light = Physics2D.Raycast(new Vector2(p.x, p.y), ((tf.right * x) + (tf.up * y)), light_vision_distance, LayerMask.GetMask("Light"));
                if (hit_light)
                {
                    //my eyesssss
                }
                //else
            }
        }
        }
}
