using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    public enum PlayerState
    {
        IDLE = 0,
        WALKING = 1,
        SPRINTING = 2,
        JUMPING = 3,
        CROUCHING = 4,
        CRAWLINGG = 5,
        SITTING = 6,
        STAND = 7,
        USE = 8,
        EQUIP = 9,
        FIRE = 10,
        SLEEP = 11
    }

    [Header("Player State")]
    public PlayerState state;

    [Header("Player Attributes")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float rotationSpeed = 90.0f;

    //Components
    private CharacterController characterController = null;
    private Transform transform = null;

    //String Customization
    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string verticalInput = "Vertical";
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
    }

    void Start()
    {
        state = PlayerState.IDLE;
    }
    //Find camera object
    
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        float horizontal = Input.GetAxis(horizontalInput) * walkSpeed;
        float vertical = Input.GetAxis(verticalInput) * walkSpeed;
        Vector3 movementDir = new Vector3(horizontal, 0, vertical);
        movementDir = transform.rotation * movementDir;
        characterController.SimpleMove(movementDir);
        //Rotate player with horizontal movement
        transform.rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime * horizontal, 0); 
        //Damp the rotation angle
        
    }
}
