using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class playerCupcake : MonoBehaviour
{
    Vector2 move;
    float jump, minHeight = 4;
    PlayerInput playerInput;
    CharacterController characterController;
    [SerializeField] Animator playerAnimator;
    [SerializeField] float  height, gravity;
    public float playerSpeed;
    [SerializeField] Transform cameraTransform, lookDirection;
    Vector3 velocity, velocityGravity;
    public Vector3 velocityInput;
    [SerializeField] ParticleSystem cookieCrumbParticle;
    [SerializeField] GameObject playerVisual;
    public bool hit = false, squishTime = false;

    int isFlyingCounter = 0;
    bool jumped = false, animJumped = false;
    float jumpedYDirection = 0;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cookieCrumbParticle.Stop();
    }

    void FixedUpdate()
    {
        //MOVE the player
        move = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 viewDirection = transform.position - new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
        lookDirection.forward = viewDirection;
        transform.forward = Vector3.Slerp(transform.forward, lookDirection.forward, 10 * Time.deltaTime); //leaving slerp because it does not do the thing i thought it did
        velocityInput = lookDirection.forward * move.y + lookDirection.right * move.x;        


        //JUMP the player
        jump = playerInput.actions["Jump"].ReadValue<float>();

        if (characterController.isGrounded) //is grounded (50%)
        {
            velocityGravity.y = 0;
            isFlyingCounter = 0;
        }
        else  //note to self: these two statements alternate every other frame so do not rely 
        {
            velocityGravity.y += gravity * Time.deltaTime; //in the air (150%)
            isFlyingCounter += 1;

            if (hit == true)
            {
                velocityGravity.y = 0;
                hit = false;
                playerAnimator.SetBool("isJumping", false);
                playerAnimator.SetBool("squishTime", false);
                //ouch bird code goes here
            }
        }
        //LIFTOFF the ground
        if (jump == 1 && jumped == false)
        {
            height += 5f * Time.deltaTime;
            if (height > 10) { height = 10; }
            squishTime = true;
        }
        if (jump == 0 && height > minHeight && isFlyingCounter < 3 && jumped == false)
        {
            velocityGravity.y = Mathf.Sqrt(-2f * gravity);
            playerVisual.transform.rotation *= Quaternion.Euler(10 * jumpedYDirection, 0, 0);
            jumpedYDirection = move.y;
            jumped = true;
        }
        if (jump == 0 && jumped == true && velocityGravity.y < 0) //resetting height after cupcake reaches peak hieght
        {
            height = minHeight;
        }


        //ROLL they see me rollin, they hatin'
        //Quaternion.Lerp(playerVisual.transform.rotation, transform.rotation, 2*Time.deltaTime); //might help subtly idk
        //Debug.Log(Quaternion.Angle(playerVisual.transform.rotation, transform.rotation)); //feel free to delete my debugs
        Debug.Log("jumped: " + jumped);
        Debug.Log("jumpedY: " + jumpedYDirection);
        Debug.Log("height: " + height);
        

        if (isFlyingCounter > 4 && jumped == true) //is not grounded (permanent w 5-6 frame delay)
        {
            move = new Vector2(move.x, jumpedYDirection);
            playerVisual.transform.rotation *= Quaternion.Euler(10*jumpedYDirection, 0, 0);
            
        }
        else if (isFlyingCounter > 4 && jumped == false) //we all fall down
        {
            jumpedYDirection = move.y;
            playerVisual.transform.rotation *= Quaternion.Euler(10 * move.y, 0, 0);
            jumped = true;
        }
        else if (jumped == true && isFlyingCounter < 4 && Quaternion.Angle(playerVisual.transform.rotation, transform.rotation) > 6f) //landed imperfectly
        {

            if (move.y != 0) //permits roll direction change
            {
                playerVisual.transform.rotation *= Quaternion.Euler(10*move.y, 0, 0);
                jumpedYDirection = Mathf.Lerp(jumpedYDirection, move.y, 10*Time.deltaTime);
            }
            else //rolls even if you dont touch anything
            {
                playerVisual.transform.rotation *= Quaternion.Euler(10*jumpedYDirection, 0, 0);
                move = new Vector2(move.x, jumpedYDirection);
                jumpedYDirection = Mathf.Lerp(jumpedYDirection, move.y, 10 * Time.deltaTime);
            }
        }
        else if (jumpedYDirection <= 0.01f && jumpedYDirection >= -0.01f || Quaternion.Angle(playerVisual.transform.rotation, transform.rotation) < 6f) //rotation perfection- back to normal movement unless you don't want to
        {
            jumped = false;
            playerVisual.transform.rotation = transform.rotation;
            Quaternion.Lerp(playerVisual.transform.rotation, transform.rotation, 10 * Time.deltaTime); //might help idk

        }

        //more debug yay and slowmo
        Debug.Log("velGrav: " + velocityGravity.y);
        //Time.timeScale = 0.25f;


        //ANIMATION attempt2
        if (squishTime == true)
        {
            playerAnimator.SetBool("squishTime", true);
            squishTime = false;
        }
        else if (jumped && !animJumped && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("cupcake_squish")){
            playerAnimator.SetBool("squishTime", false);
            playerAnimator.SetBool("isJumping", true);
            animJumped = true;
            Debug.Log("sqeuuueezing");
        }
        else if (!jumped){
            animJumped = false;
            playerAnimator.SetBool("isJumping", false);
            Debug.Log("not stretching or squeezing");
        }

        //the values if needed
        //i changed move's values during runtime so it wont be reflective of actual input but what the cupcake is doing
        //feel free to delete
        Debug.Log("move: " + move);
        Debug.Log("jump: " + jump);



        //PARTICLE system
        if (jumped && velocityGravity.y < -0.5 || !jumped && velocityInput.magnitude == 0)
        {
            cookieCrumbParticle.Stop();
        }
        else
        {
            cookieCrumbParticle.Play();
        }



        velocityInput = lookDirection.forward * move.y + lookDirection.right * move.x; //I updated these values a bunch so we need it again
        characterController.Move(velocityInput * playerSpeed * Time.deltaTime + velocityGravity * height * Time.deltaTime);


    }
}
