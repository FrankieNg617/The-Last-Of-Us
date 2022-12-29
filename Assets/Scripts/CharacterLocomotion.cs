using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    [Header("Acceleration")]

    [Tooltip("Acceleration value used when the character is in the air. This means when falling.")]
    [SerializeField]
    private float accelerationInAir = 1.7f;

    [Header("Speeds")]

    [Tooltip("The speed of the player while walking.")]
    [SerializeField]
    private float speedWalking = 5f;

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

    
    private CharacterController controller;
    public Vector3 velocity;
    Animator animator;

    // Is the character on the ground.
    private bool isGrounded;
    // Was the character standing on the ground last frame.
    private bool wasGrounded;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
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

        //Normalize the movement vector3
        desiredDirection = Vector3.ClampMagnitude(desiredDirection, 1f);

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
            velocity = new Vector3(desiredDirection.x, velocity.y, desiredDirection.z);
        }
        

        Vector3 applied = velocity * Time.deltaTime;

        //Stick To Ground Force. Helps with making the character walk down slopes without floating.
        // if (controller.isGrounded)
        //     applied.y = -0.03f;

        controller.Move(applied);

        animator.SetFloat("InputX", inputX);
        animator.SetFloat("InputZ", inputZ);
    }
}
