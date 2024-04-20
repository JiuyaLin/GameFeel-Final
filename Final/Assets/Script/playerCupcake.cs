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


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cookieCrumbParticle.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        //move the player
        move = playerInput.actions["Move"].ReadValue<Vector2>();

        Vector3 viewDirection = transform.position 
        - new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
        lookDirection.forward = viewDirection;

        transform.forward = Vector3.Slerp(transform.forward, lookDirection.forward, 10 * Time.deltaTime);
        velocityInput = lookDirection.forward * move.y + lookDirection.right * move.x;
        

        //jump the player
        Debug.Log(playerInput.actions["Jump"]);
        jump = playerInput.actions["Jump"].ReadValue<float>();
        if(characterController.isGrounded) //is grounded
        {
            velocityGravity.y = 0;
        }
        else
        {
            velocityGravity.y += gravity * Time.deltaTime;
        }
        if (jump > 0 && characterController.isGrounded)
        {
            velocityGravity.y = Mathf.Sqrt(-2f * gravity);
        }

        
        //particle system
        if (velocityInput.magnitude > 0)  cookieCrumbParticle.Play();
        else cookieCrumbParticle.Stop();
        
        characterController.Move(velocityInput * speed * Time.deltaTime 
                                + velocityGravity * height * Time.deltaTime);


    }
}
