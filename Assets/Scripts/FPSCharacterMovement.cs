using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCharacterMovement : MonoBehaviour
{
    [Header("Acceleration")]

    [Tooltip("How fast the character's speed increases.")]
    [SerializeField]
    private float acceleration = 12f;

    [Tooltip("Acceleration value used when the character is in the air. This means when falling.")]
    [SerializeField]
    private float accelerationInAir = 1.7f;

    [Tooltip("How fast the character's speed decreases.")]
    [SerializeField]
    private float deceleration = 11f;
    
    [Header("Speeds")]

    [Tooltip("The speed of the player while walking.")]
    [SerializeField]
    private float speedWalking = 7f;

    [Tooltip("The turning speed of the player.")]
    [SerializeField]
    private float turnSpeed = 15;

    [Header("Walking Multipliers")]
        
    [Tooltip("Value to multiply the walking speed by when the character is moving forward.")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float walkingMultiplierForward = 1.0f;

    [Tooltip("Value to multiply the walking speed by when the character is moving sideways.")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float walkingMultiplierSideways = 0.9f;

    [Tooltip("Value to multiply the walking speed by when the character is moving backwards.")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float walkingMultiplierBackwards = 0.9f;

    [Header("Air")]

    [Tooltip("How much control the player has over changes in direction while the character is in the air.")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float airControl = 0.15f;

    [Tooltip("The value of the character's gravity. Basically, defines how fast the character falls.")]
    [SerializeField]
    private float gravity = 25f;

    
   

    Camera mainCamera;
    Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    // Is the character on the ground.
    private bool isGrounded;
    // Was the character standing on the ground last frame.
    private bool wasGrounded;


    void Start()
    {
        mainCamera = Camera.main;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        MoveCharacter();

        wasGrounded = isGrounded;

    }

    private void MoveCharacter() {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        var desiredDirection = new Vector3(inputX, 0.0f, inputZ);
        desiredDirection *= speedWalking;
        desiredDirection.x *= walkingMultiplierSideways;
        desiredDirection.z *= (inputZ > 0 ? walkingMultiplierForward : walkingMultiplierBackwards);

        desiredDirection = transform.TransformDirection(desiredDirection);

        // Apply gravity when character is in the air.
        if(isGrounded == false) {

            if(wasGrounded)
                velocity.y = 0.0f;

            //Movement.
            velocity += desiredDirection * (accelerationInAir * airControl * Time.deltaTime);
            //Gravity.
            velocity.y -=  gravity * Time.deltaTime;
        } 
        // Normal movement on ground.
        else {
            velocity = Vector3.Lerp(velocity, new Vector3(desiredDirection.x, velocity.y, desiredDirection.z), Time.deltaTime * (desiredDirection.sqrMagnitude > 0.0f ? acceleration : deceleration));
        }

        Vector3 applied = velocity * Time.deltaTime;

        //Stick To Ground Force. Helps with making the character walk down slopes without floating.
        // if (controller.isGrounded)
        //     applied.y = -0.03f;

        controller.Move(applied);

        // Turn the character baseed on the mouse rotation.
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        float pitchCamera = mainCamera.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(pitchCamera, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);

        animator.SetFloat("InputX", inputX);
        animator.SetFloat("InputZ", inputZ);
    }
}
