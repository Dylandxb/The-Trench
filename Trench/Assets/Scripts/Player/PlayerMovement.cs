using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public enum PlayerState
{
    IDLE =0,
    WALKING = 1,
    SPRINTING = 2,
    JUMPING = 3,
    WALK_LEFT = 4,
    WALK_RIGHT = 5,
    WALK_BACK = 6,
    TALKING = 7,
    CROUCHING = 8,
    SLEEP = 9
    //CRAWLING = 5,
    //SITTING = 6,
    //STAND = 7,
    //USE = 8,
    //EQUIP = 9,
    //FIRE = 10,


}
public class PlayerMovement : MonoBehaviour
{
    //Unarmed States, inherit from unarmed class/interface
    PlayerState CurrentState
    {
        set
        {
            //Lock state until a different key is pressed,                     //Prevents interruption of current state, only one state active at a time
            //Constructor to control transition of states
            if (stateLocked == false)
            {

            }
            state = value;
            switch (state)
            {
                case PlayerState.IDLE:
                    Idle();
                    break;
                //If key == W, walk forwards anim
                //If key == A walk left anim
                //If key == D walk right anim
                //If key == S walk backward anim
                case PlayerState.WALKING:
                    Walking();
                    //Only enable left and right keys when in tight spaces, for now enable left and right in a selected area
                    break;
                case PlayerState.SPRINTING:
                    Sprinting();
                    break;
                case PlayerState.JUMPING:
                    Jumping();
                    break;
                case PlayerState.WALK_LEFT:
                    WalkLeft();
                    break;
                case PlayerState.WALK_RIGHT:
                    WalkRight();
                    break;
                case PlayerState.WALK_BACK:
                    WalkBack();
                    break;
                //Non walking States
                case PlayerState.TALKING:
                    Talking();
                    break;
                case PlayerState.CROUCHING:
                    Crouch();
                    break;
                case PlayerState.SLEEP:
                    Sleep();
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
    [SerializeField] private float rotationSpeed = 600.0f;
    [SerializeField] private float jumpHeight = 5.0f;
    [SerializeField] private float sprintTime = 10.0f;
    [SerializeField] private float ySpeed;

    [Header("Player Conditions")]
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isCrouching;
    [SerializeField] bool canMove = true;
    [SerializeField] private bool canSprint;
    [SerializeField] bool stateLocked = false;
    [SerializeField] private AnimationClip[] animationClips;
    Vector3 moveInput = Vector3.zero;

    [Header("Custom Strings")]
    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string verticalInput = "Vertical";
    [SerializeField] private GameObject nametag;


    [Header("Inputs")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode walkForwardKey = KeyCode.W;
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
        //WAIT FOR LEVEL TO FADE OUT BEFORE ENABLING MOVEMENT & CAMERA VIEW
    }

    void Update()
    {
        if (canMove == true && UIManager.instance.LevelNameFading() == false)
        {
            if (Input.GetKey(walkForwardKey))
            {
                anim.SetBool("WalkUnarmed", true);
                canSprint = true;
                CurrentState = PlayerState.WALKING;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                anim.SetBool("WalkLeftUnarmed", true);
                canSprint = false;
                CurrentState = PlayerState.WALK_LEFT;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                anim.SetBool("WalkBackUnarmed", true);
                canSprint = false;
                CurrentState = PlayerState.WALK_BACK;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                anim.SetBool("WalkRightUnarmed", true);
                canSprint = false;
                CurrentState = PlayerState.WALK_RIGHT;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                CurrentState = PlayerState.CROUCHING;
            }
            else if (Input.GetKey(jumpKey))
            {
                //Set vertical height to jump height on key press, change state to jumping
                ySpeed = jumpHeight;
                canSprint = false;
                CurrentState = PlayerState.JUMPING;
            }
            else if (isSprinting == true)
            {
                CurrentState = PlayerState.SPRINTING;
            }
            //When magnitude of vector3 is zero or when walkspeed = 0
            else if (moveInput == Vector3.zero)
            {
                canSprint = false;
                CurrentState = PlayerState.IDLE;
            }
            else if (Input.GetKey(KeyCode.N))
            {
                CurrentState = PlayerState.SLEEP;
            }            
        }



    }


    private bool FreezeMovement() => canMove = false;
    private bool UnFreezeMovement() => canMove = true;
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
                break;
            case PlayerState.SPRINTING:
                Sprinting();
                break;
            case PlayerState.JUMPING:
                Jumping();
                break;
            case PlayerState.TALKING:
                Talking();
                break;
            case PlayerState.CROUCHING:
                Crouch();
                break;
            case PlayerState.SLEEP:
                Sleep();
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
        //Make a walk left state, walk rigght state and walk back state
    }

    private void WalkLeft()
    {
        Vector2 movementLeft = new Vector2(Input.GetAxisRaw(horizontalInput), 0);
        movementLeft.Normalize();
        Vector3 velocityL = (playerTransform.forward * 0 + playerTransform.right * movementLeft.x) * walkSpeed;
        characterController.Move(velocityL * Time.deltaTime);
    }

    private void WalkRight()
    {
        Vector2 movementRight = new Vector2(Input.GetAxisRaw(horizontalInput), 0);
        movementRight.Normalize();
        Vector3 velocityR = (playerTransform.forward * 0 + playerTransform.right * movementRight.x) * walkSpeed;
        characterController.Move(velocityR * Time.deltaTime);
    }

    private void WalkBack()
    {
        Vector2 movementBack= new Vector2(0, Input.GetAxisRaw(verticalInput));
        movementBack.Normalize();
        Vector3 velocityB = (playerTransform.forward * movementBack.y + playerTransform.right * 0) * walkSpeed;
        characterController.Move(velocityB * Time.deltaTime);
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
        //Lerp sprint speed back to walk speed when shift is released
    }

    //TODO!!!!
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

        if (characterController.isGrounded)
        {
            isGrounded = true;
            ySpeed = 0f;
        }
    }

    private void Crouch()
    {
        isCrouching = true;
        Debug.Log("Crouch");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DialogueBox"))
        {
            //Lock state in idle, wait a few seconds then unlock state
            FreezeMovement();
            ControlPlayerState(PlayerState.TALKING);
            StartCoroutine(EnableMovement(5.0f));
        }
    }

    private IEnumerator EnableMovement(float time)
    {
        yield return new WaitForSeconds(time);
        UnFreezeMovement();
    }

    void Talking()
    {
        anim.SetBool("Talking", true);
        //Find object of nametaghover and enable it
    }

    private void Sleep()
    {
        anim.SetBool("Sleeping", true);
        Debug.Log("Player sleeping");
    }


}
