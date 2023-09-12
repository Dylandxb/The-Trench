using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraMovement : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera firstPersonCamera;
    [SerializeField] CinemachineVirtualCamera thirdPersonCamera;

    //Mouse Inputs
    private float mouseSens = 2.0f;
    private float lookRange = 50.0f;


    private float verticalLook;
    private Camera fpCamera = null;

    private void Awake()
    {
        fpCamera = Camera.main;
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

    }

    private void OnEnable()
    {
        CameraControllers.LoadCam(firstPersonCamera);
        CameraControllers.LoadCam(thirdPersonCamera);
        CameraControllers.SwitchCamera(firstPersonCamera);
    }

    private void OnDisable()
    {
        CameraControllers.UnloadCam(firstPersonCamera);
        CameraControllers.UnloadCam(thirdPersonCamera);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //switch camera
            if (CameraControllers.IsActive(firstPersonCamera))
            {
                CameraControllers.SwitchCamera(thirdPersonCamera);
            }
            else if (CameraControllers.IsActive(thirdPersonCamera))
            {
                CameraControllers.SwitchCamera(firstPersonCamera);
            }
        }
        HandleCamera();
    }

    void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens;
        transform.Rotate(0, mouseX, 0);

        verticalLook -= Input.GetAxis("Mouse Y") * mouseSens;
        verticalLook = Mathf.Clamp(verticalLook, -lookRange, lookRange);
        //Transform the rotation of the camera about x
        fpCamera.transform.localRotation = Quaternion.Euler(verticalLook, 0, 0);


    }
}
