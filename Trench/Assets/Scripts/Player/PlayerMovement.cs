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

    [Header("Player State")]
    public PlayerState state;

    [Header("Player Attributes")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float rotationSpeed = 90.0f;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isSprinting;


    [Header("Custom Strings")]
    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string verticalInput = "Vertical";


    [Header("Inputs")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
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
        state = PlayerState.IDLE;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            ControlPlayerState(PlayerState.WALKING);
        }
        else if (isSprinting == true)
        {
            ControlPlayerState(PlayerState.SPRINTING);
        }
        else//When magnitude of vector3 is zero or when walkspeed = 0
        {
            ControlPlayerState(PlayerState.IDLE);
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
        }

    }

    private void Idle()
    {
        //Set idle anim
        isMoving = false;
        Debug.Log("Im Idle");
        anim.SetTrigger("Idle");

    }
    //Walk sideways, forwards and backwards
    private void Walking()
    {
        anim.SetTrigger("Walking");
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
    }

    private void Sprinting()
    {
        Debug.Log("Im Sprinting");
        Vector2 movementDir = new Vector2(Input.GetAxisRaw(horizontalInput), Input.GetAxisRaw(verticalInput));
        movementDir.Normalize();
        Vector3 velocity = (transform.forward * movementDir.y + transform.right * movementDir.x) * walkSpeed * 2;
        characterController.Move(velocity * Time.deltaTime);
        isMoving = true;
        //walkSpeed *= 2;
    }
}
