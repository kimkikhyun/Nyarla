using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    float moveSpeed = 6;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;

    public float timeToJumpApex = 0.4f;
    float jumpVelocity = 8;

    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    public float timeToWallUnstick;

    float gravity = -20;
    float maxJumpVelocity;
    float minJumpVelocity;

    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
	
    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        print("Gravity : " + gravity + " jump Velocity : " + jumpVelocity);
    }

    void Update ()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        bool wallSliding = false;
        //Check wallSiliding State and modify velocity.y to wallsiliding
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
                velocity.y = -wallSlideSpeedMax;

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0)
                    timeToWallUnstick -= Time.deltaTime;

                else
                    timeToWallUnstick = wallStickTime;
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }


        //Jump
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (wallSliding)
                //while wallSliding, Input x to wall
                if(wallDirX == input.x)
                {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                //while wallSliding, No Input X
                else if (input.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                //while wallSliding, Input x to opposite side of wall
                else
                {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }

            if (controller.collisions.below)
                velocity.y = jumpVelocity;

           
        }
        
        //Input min Jump
        if(Input.GetKeyUp (KeyCode.Z))
        {
            if(velocity.y > minJumpVelocity)
                velocity.y = minJumpVelocity;
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime, input);


        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0;
    }

}
