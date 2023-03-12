using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class JumpAgent : Agent
{
    // Start is called before the first frame update
    //Keycodes

    [Header("Jump Settings")]

    [Tooltip("Initial jump force")]
    public float jumpForce = 110f;
    [Tooltip("Continuous jump force")]
    public float jumpAccel = 10f;
    [Tooltip("Max jump up time")]
    public float jumpTime = 0.4f;
    [Tooltip("How long you have to jump after leaving a ledge (seconds)")]
    public float coyoteTime = 0.2f;
    [Tooltip("buffer jump input (seconds)")]
    public float jumpBuffer = 0.1f;
    [Tooltip("How long do I have to wait before I can jump again")]
    public float jumpCooldown = 0.6f;
    [Tooltip("Fall quicker")]
    public float extraGravity = 0.1f;
    [Tooltip("The tag that will be considered the ground")]
    public string groundTag = "Ground";


    [Tooltip("The key used to jump")]
    public KeyCode jump = KeyCode.Space;
    [Tooltip("Are we on the ground?")]
    public bool areWeGrounded = true;
    public float currentSpeed=5f;
    
    
    
    Vector3 input = new Vector3();
    private float coyoteTimeCounter, jumpBufferCounter, startJumpTime, endJumpTime;
    private bool wantingToJump = false, jumpCooldownOver = true;
    //Variables
    public Rigidbody rb;


    private bool left;
    private bool right;
    private bool back;
    private bool forward;
    private bool down;

    public GameObject Target = null;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
       
    }
    //mlagents-learn --run-id=test --resume
    // Update is called once per frame
    void Update()
    {
        /*input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        wantingToJump = Input.GetKey(jump);*/
    }

    private void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * 2.5f * Time.deltaTime;  //slam down jump
        }
    }

    private void jumpCoolDownCountdown()
    {
        jumpCooldownOver = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(groundTag))
            handleHitGround();

        if (other.gameObject.CompareTag("Target"))
        {
            AddReward(1.0f);
            EndEpisode();
        }
    }

   
    public void handleHitGround()
    {
        areWeGrounded = true;
    }
 
    public override void OnActionReceived(ActionBuffers Action)
    {
        /*var vectorAction = Action.DiscreteActions;
        //move
        this.transform.Translate(Vector3.right * vectorAction[0] * 5 * Time.deltaTime);
        this.transform.Translate(Vector3.forward * vectorAction[1] * 5 * Time.deltaTime);

        //jump
        if (areWeGrounded == true && vectorAction[2] != 0)
        {
            are
            rb.AddForce(transform.up * jumpForce * vectorAction[2], ForceMode.Impulse);

        }

        OffFloorCheck();*/
        var vectorAction = Action.ContinuousActions;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - transform.localScale.y + 0.1f, transform.position.z), Vector3.down, 0.1f))
        {
            handleHitGround();
            //Debug.Log("yodayo");
        }
        //Handle late jump like the cartoon coyote
        if (areWeGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;
        if (wantingToJump)
            jumpBufferCounter = jumpBuffer;
        else
            jumpBufferCounter -= Time.deltaTime;

        //there is a problem with coyotetime, where it will go down to minus for some reason
        //Debug.Log(coyoteTimeCounter + " " + jumpBufferCounter + " " + jumpCooldownOver);

        if (areWeGrounded == true && vectorAction[2] != 0)
        {
            areWeGrounded = false;
            rb.AddForce(transform.up * jumpForce * vectorAction[2], ForceMode.Impulse);
        }


        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && jumpCooldownOver)
        {
            //start jumping mechanism
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce * vectorAction[2], ForceMode.Impulse);

            jumpCooldownOver = false;
            areWeGrounded = false;
            jumpBufferCounter = 0f;
            endJumpTime = Time.time + jumpTime;

            Invoke(nameof(jumpCoolDownCountdown), jumpCooldown);
        }
        else if (wantingToJump && !areWeGrounded && endJumpTime > Time.time)
        {
            rb.AddForce(Vector3.up * jumpAccel * vectorAction[2], ForceMode.Acceleration);
        }
        input = input.normalized;
        Vector3 forwardVel = currentSpeed * transform.forward * vectorAction[0];
        Vector3 horizontalVel = currentSpeed * transform.right * vectorAction[1];
        rb.velocity = horizontalVel + forwardVel + new Vector3(0, rb.velocity.y, 0);

        rb.AddForce(new Vector3(0, -extraGravity, 0), ForceMode.Impulse);
        OffFloorCheck();
    }

    public override void Heuristic(in ActionBuffers actions)
    {

        var actionsOut = actions.ContinuousActions;
        actionsOut[0] = 0;  //movement x axis   
        actionsOut[1] = 0;  //movement z axis
        actionsOut[2] = 0;  //jump

        if (Input.GetKey(KeyCode.LeftArrow) == true)
            actionsOut[0] = -1;
        if (Input.GetKey(KeyCode.RightArrow) == true)
            actionsOut[0] = 1;
        if (Input.GetKey(KeyCode.UpArrow) == true)
            actionsOut[1] = 1;
        if (Input.GetKey(KeyCode.DownArrow) == true)
            actionsOut[1] = -1;

        if (Input.GetKey(KeyCode.Space) == true)
            actionsOut[2] = 1;
    }

    private void OffFloorCheck()
    {
        //nothing below agent, and lower than the floor
        if (down == false && this.transform.position.y < 0)
        {
            
            AddReward(-0.1f);
            EndEpisode();
        }
    }
    

    public override void CollectObservations(VectorSensor sensor)
    {
        //Note: these are the pieces of information I determined the agent needs to know (sense) to avoid holes and reach target

        //floor feelers 
        Vector3 p = this.transform.position;
        left = Physics.Raycast(new Vector3(p.x - 0.5f, p.y, p.z), Vector3.down + (Vector3.left * 0.5f));
        right = Physics.Raycast(new Vector3(p.x + 0.5f, p.y, p.z), Vector3.down + (Vector3.right * 0.5f));
        back = Physics.Raycast(new Vector3(p.x, p.y, p.z - 0.5f), Vector3.down + (Vector3.back * 0.5f));
        forward = Physics.Raycast(new Vector3(p.x, p.y, p.z + 0.5f), Vector3.down + (Vector3.forward * 0.5f));
        down = Physics.Raycast(p, Vector3.down);
        sensor.AddObservation(left);                        // +1
        sensor.AddObservation(right);                       // +1
        sensor.AddObservation(back);                        // +1
        sensor.AddObservation(forward);                     // +1   
        sensor.AddObservation(down);                        // +1 = 5

        //position of agent
        sensor.AddObservation(this.transform.position);     // +3 = 8

        //direction agent to target
        Vector3 direction = Vector3.Normalize(this.transform.position - Target.transform.position);
        sensor.AddObservation(direction);                   // +3 = 11

        //position of target
        sensor.AddObservation(Target.transform.position);   // +3 = 14 total observations
        Debug.Log(string.Format("down: {0}  {1},{2},{3},{4}", down, left, right, back, forward));
    }
    private void OnDrawGizmos()
    {
        Vector3 p = this.transform.position;

        //floor sensors (left, right, back, forward)
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(new Vector3(p.x - 0.5f, p.y, p.z), Vector3.down + (Vector3.left * 0.5f));
        Gizmos.DrawRay(new Vector3(p.x + 0.5f, p.y, p.z), Vector3.down + (Vector3.right * 0.5f));
        Gizmos.DrawRay(new Vector3(p.x, p.y, p.z - 0.5f), Vector3.down + (Vector3.back * 0.5f));
        Gizmos.DrawRay(new Vector3(p.x, p.y, p.z + 0.5f), Vector3.down + (Vector3.forward * 0.5f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y - transform.localScale.y + 0.1f, transform.position.z), Vector3.down);
        // down
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(p, Vector3.down);
    }
    
}
