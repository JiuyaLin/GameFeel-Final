using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class playerCupcake : MonoBehaviour
{
    Vector2 move;
    float jump;
    PlayerInput playerInput;
    CharacterController characterController;
    [SerializeField] float speed, height, gravity;
    [SerializeField] Transform cameraTransform, lookDirection;
    Vector3 velocity, velocityInput, velocityGravity;
    [SerializeField] ParticleSystem cookieCrumbParticle;
    [SerializeField] GameObject playerVisual;

    int isFlyingCounter = 0;
    bool jumped = false;
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
        //move the player
        move = playerInput.actions["Move"].ReadValue<Vector2>();
        

        Vector3 viewDirection = transform.position - new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
        lookDirection.forward = viewDirection;

        transform.forward = Vector3.Slerp(transform.forward, lookDirection.forward, 10 * Time.deltaTime); //leaving slerp because it does not do the thing i thought it did
        velocityInput = lookDirection.forward * move.y + lookDirection.right * move.x;        

        //jump the player
        jump = playerInput.actions["Jump"].ReadValue<float>();
        if(characterController.isGrounded) //is grounded (50%)
        {
            velocityGravity.y = 0;
            isFlyingCounter = 0;
        }
        else  //note to self: these two statements alternate every other frame so do not rely
        {
            velocityGravity.y += gravity * Time.deltaTime; //in the air (150%)
            isFlyingCounter += 1;
        }

        if (jump > 0 && characterController.isGrounded) //allows liftoff
        {
            velocityGravity.y = Mathf.Sqrt(-2f * gravity);
            playerVisual.transform.rotation *= Quaternion.Euler(10*jumpedYDirection, 0, 0);
            jumpedYDirection = move.y;

        }


        //they see me rollin, they hatin'
        Quaternion.Lerp(playerVisual.transform.rotation, transform.rotation, 2*Time.deltaTime); //might help subtly idk
        Debug.Log(Quaternion.Angle(playerVisual.transform.rotation, transform.rotation)); //feel free to delete my debugs


        if (isFlyingCounter > 4) //is not grounded (permanent w 5-6 frame delay)
        {
            move = new Vector2(move.x, jumpedYDirection);
            playerVisual.transform.rotation *= Quaternion.Euler(10*jumpedYDirection, 0, 0);
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
        else if (jumpedYDirection == 0) //rotation perfection- back to normal movement unless you don't want to
        {
            jumped = false;
            //playerVisual.transform.rotation = transform.rotation;
            Quaternion.Lerp(playerVisual.transform.rotation, transform.rotation, 10 * Time.deltaTime); //might help idk

        }

        //the values if needed
        //i changed move's values during runtime so it wont be reflective of actual input but what the cupcake is doing
        //feel free to delete
        Debug.Log("move: " + move);
        Debug.Log("jump: " + jump);

        //particle system
        if (velocityInput.magnitude > 0) cookieCrumbParticle.Play();
        else cookieCrumbParticle.Stop();

        velocityInput = lookDirection.forward * move.y + lookDirection.right * move.x; //I updated these values a bunch so we need it again
        characterController.Move(velocityInput * speed * Time.deltaTime + velocityGravity * height * Time.deltaTime);


    }
}
