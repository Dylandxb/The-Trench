using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum PlayerState
{
    IDLE =0,
    WALKING = 1,
    SPRINTING = 2,
    JUMPING = 3,
    CROUCHING = 4,
    CRAWLING = 5,
    SITTING = 6,
    STAND = 7,
    USE = 8,
    EQUIP = 9,
    FIRE = 10,
    SLEEP = 11
}
public class PlayerMovement : MonoBehaviour
{
    PlayerState CurrentState
    {
        set
        {
            //Way to lock states
            //Constructor to control transition of states
            state = value;
            switch (state)
            {
                case PlayerState.IDLE:
                    Idle();
                    break;
                case PlayerState.WALKING:
                    Walking();
                    break;
                case PlayerState.SPRINTING:
                    Sprinting();
                    break;
                case PlayerState.JUMPING:
                    Jumping();
                    break;
            }
        }
    }

    [Header("Player State")]
    public PlayerState state;

    [Header("Player Attributes")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float rotationSpeed = 90.0f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isGrounded;
    Vector3 moveInput = Vector3.zero;

    [Header("Custom Strings")]
    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string verticalInput = "Vertical";


    [Header("Inputs")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    //Components
    private CharacterController characterController = null;
    private Transform transform = null;
    private Animator anim = null;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        isMoving = false;
        CurrentState = PlayerState.IDLE;

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //anim.SetTrigger("WalkForwards")
            //Set in blend tree
            anim.SetTrigger("Walking");
            CurrentState = PlayerState.WALKING;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            anim.SetTrigger("WalkLeft");
            CurrentState = PlayerState.WALKING;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            anim.SetTrigger("WalkBack");
            CurrentState = PlayerState.WALKING;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            anim.SetTrigger("WalkRight");
            CurrentState = PlayerState.WALKING;
        }
        else if (isSprinting == true)
        {
            CurrentState = PlayerState.SPRINTING;
        }
        else if (Input.GetKey(jumpKey))
        {
            CurrentState = PlayerState.JUMPING;
        }
        else if (moveInput == Vector3.zero)//When magnitude of vector3 is zero or when walkspeed = 0
        {
            CurrentState = PlayerState.IDLE;
        }
    }

    private void ControlPlayerState(PlayerState playerState)
    {
        state = playerState;

        switch (playerState)
        {
            case PlayerState.IDLE:
                Idle();
                break;
            case PlayerState.WALKING:
                Walking();
                break;
            case PlayerState.SPRINTING:
                Sprinting();
                break;
            case PlayerState.JUMPING:
                Jumping();
                break;
        }
        //If key == W, walk forwards anim
        //If key == A walk left anim
        //If key == D walk right anim
        //If key == S walk backward anim
    }

    private void Idle()
    {
        //Set idle anim
        isMoving = false;
        isJumping = false;
        isGrounded = true;
        Debug.Log("Im Idle");
        anim.SetTrigger("Idle");

    }
    //Walk sideways, forwards and backwards
    private void Walking()
    {
        //anim.SetTrigger("Walking");
        Debug.Log("Im Walking");
        Vector2 movementDir = new Vector2(Input.GetAxisRaw(horizontalInput), Input.GetAxisRaw(verticalInput));
        movementDir.Normalize();
        Vector3 velocity = (transform.forward * movementDir.y + transform.right * movementDir.x) * walkSpeed;
        characterController.Move(velocity * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
            state = PlayerState.SPRINTING;
            walkSpeed *= 1.5f;
        }
        isMoving = true;
        isSprinting = false;
        isJumping = false;
        isGrounded = true;

    }

    private void Sprinting()
    {
        Debug.Log("Im Sprinting");
        //Can only sprint when facing forward and holding shift
        Vector2 movementDir = new Vector2(Input.GetAxisRaw(horizontalInput), Input.GetAxisRaw(verticalInput));
        movementDir.Normalize();
        Vector3 velocity = (transform.forward * movementDir.y + transform.right * movementDir.x) * walkSpeed * 2;
        characterController.Move(velocity * Time.deltaTime);
        isMoving = true;
        //walkSpeed *= 2;
    }

    private void Jumping()
    {
        anim.SetTrigger("Jump");
        Debug.Log("Im Jumping");
        isJumping = true;
        isGrounded = false;
        Vector3 jumpVel = transform.up  * 10;
        characterController.Move(jumpVel * Time.deltaTime);
    }
}
