using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

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
    //Unarmed States, inherit from unarmed class/interface
    PlayerState CurrentState
    {
        set
        {
            //Way to lock states
            //Constructor to control transition of states
            if (stateLocked == true)
            {

            }
            state = value;
            switch (state)
            {
                case PlayerState.IDLE:
                    Idle();
                    break;
                case PlayerState.WALKING:
                    Walking();
                    //If key == W, walk forwards anim
                    //If key == A walk left anim
                    //If key == D walk right anim
                    //If key == S walk backward anim
                    break;
                case PlayerState.SPRINTING:
                    Sprinting();
                    break;
                case PlayerState.JUMPING:
                    Jumping();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

        }
    }

    [Header("Player State")]
    public PlayerState state;

    [Header("Player Attributes")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float rotationSpeed = 90.0f;
    [SerializeField] private float jumpHeight = 5.0f;
    [SerializeField] private float sprintTime = 10.0f;
    [SerializeField] private float ySpeed;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool canSprint;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isGrounded;
    [SerializeField] private AnimationClip[] animationClips;
    bool stateLocked;
    bool canMove;
    Vector3 moveInput = Vector3.zero;

    [Header("Custom Strings")]
    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string verticalInput = "Vertical";
    [SerializeField] private GameObject nametag;


    [Header("Inputs")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    //Components
    private CharacterController characterController = null;
    private Transform playerTransform = null;
    private Animator anim = null;
    public static PlayerMovement instance;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerTransform = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        animationClips = anim.runtimeAnimatorController.animationClips;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CurrentState = PlayerState.IDLE;
    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            //isGrounded = true;
            //ySpeed = 0f;

        }
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("WalkUnarmed", true);
            canSprint = true;
            CurrentState = PlayerState.WALKING;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            anim.SetBool("WalkLeftUnarmed", true);
            canSprint = false;
            CurrentState = PlayerState.WALKING;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            anim.SetBool("WalkBackUnarmed", true);
            canSprint = false;
            CurrentState = PlayerState.WALKING;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            anim.SetBool("WalkRightUnarmed", true);
            canSprint = false;
            CurrentState = PlayerState.WALKING;
        }
        else if (Input.GetKey(jumpKey))
        {
            ySpeed = jumpHeight;
            canSprint = false;
            CurrentState = PlayerState.JUMPING;
        }
        else if (isSprinting == true)
        {
            CurrentState = PlayerState.SPRINTING;
        }
        else if (moveInput == Vector3.zero)//When magnitude of vector3 is zero or when walkspeed = 0
        {
            canSprint = false;
            CurrentState = PlayerState.IDLE;
        }
    }

    public void ControlPlayerState(PlayerState playerState)
    {
        state = playerState;

        switch (playerState)
        {
            case PlayerState.IDLE:
                Idle();
                break;
            case PlayerState.WALKING:
                Walking();
                //Movement();
                break;
            case PlayerState.SPRINTING:
                Sprinting();
                break;
            case PlayerState.JUMPING:
                Jumping();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(playerState), playerState, null);
        }
    }

    private void Idle()
    {
        //Set idle anim
        isMoving = false;
        isJumping = false;
        isGrounded = true;
        anim.SetBool("IdleUnarmed", true);
        //Foreach anim from array set to false when transitioning to Idle
        foreach(AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }
    }

    private void Walking()
    {
        anim.SetBool("SprintUnarmed", false);
        Vector2 movementDir = new Vector2(Input.GetAxisRaw(horizontalInput), Input.GetAxisRaw(verticalInput));
        movementDir.Normalize();
        Vector3 velocity = (playerTransform.forward * movementDir.y + playerTransform.right * movementDir.x) * walkSpeed;
        characterController.Move(velocity * Time.deltaTime);
        //Check to only sprint when W key has been held, else set walk speed to default
        if (Input.GetKey(KeyCode.LeftShift) && canSprint == true /* transform.position.z >= 0*/)
        {
            state = PlayerState.SPRINTING;
            Sprinting();
        }
        else
        {
            walkSpeed = 3.0f;
            sprintTime = 10.0f;
        }
        isMoving = true;
        isSprinting = false;
        isJumping = false;
        isGrounded = true;
        //LINK WALKING TO SPRINTING WITH BLEND TREE
    }

    private void Sprinting()
    {
        isSprinting = true;
        anim.SetBool("SprintUnarmed", true);
        anim.SetBool("WalkUnarmed", false);
        //Can only sprint when facing forward and holding shift
        Vector2 movementDir = new Vector2(Input.GetAxisRaw(horizontalInput), Input.GetAxisRaw(verticalInput));
        movementDir.Normalize();
        Vector3 velocity = (playerTransform.forward * movementDir.y + playerTransform.right * movementDir.x) * walkSpeed * 2;
        characterController.Move(velocity * Time.deltaTime);
        isMoving = true;
        walkSpeed = 5.0f;
        //lerp from a time to sprint to 10 seconds then after 10 seconds decrease speed back to 3 and set canSprint = false, whilst in sprint state
        float capTime = 0f;
        float decrease = Mathf.Lerp(sprintTime, capTime,   Time.time/10);
        Debug.Log(decrease.ToString());
        if (decrease == capTime)
        {
            state = PlayerState.WALKING;
        }
        else
        {
            decrease = sprintTime;
        }
    }

    private void Jumping()
    {
        anim.SetBool("JumpUnarmed", true);
        isJumping = true;
        isGrounded = false;
        Vector3 movementDirection = new Vector3(Input.GetAxisRaw(verticalInput), 0, Input.GetAxisRaw(verticalInput));
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * walkSpeed;
        movementDirection.Normalize();
        //Vector3 jumpVel = transform.up  * 10;
        Vector3 jumpVel = movementDirection * magnitude;
        jumpVel.y = ySpeed;
        ySpeed += Physics.gravity.y * Time.deltaTime;
        characterController.Move(jumpVel * Time.deltaTime);
    }

    private void Crouching()
    {

    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * walkSpeed;
        movementDirection.Normalize();
        characterController.SimpleMove(movementDirection * magnitude);
        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
    }
}
