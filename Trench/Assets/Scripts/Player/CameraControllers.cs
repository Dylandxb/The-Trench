using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraControllers
{
    private static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    public static CinemachineVirtualCamera activeCam = null;
    

    public static void LoadCam(CinemachineVirtualCamera camera)
    {
        //Add camera to list when loading it
        cameras.Add(camera);
    }

    public static void UnloadCam(CinemachineVirtualCamera camera)
    {
        cameras.Remove(camera);
    }

    public static void SwitchCamera(CinemachineVirtualCamera camera)
    {
        //Set priority of camera switched to higher
        camera.Priority = 3;
        activeCam = camera;
        //For every camera in the list that isnt the one being switched to, set prio to 0
        foreach(CinemachineVirtualCamera cam in cameras)
        {
            if (cam != camera)
            {
                cam.Priority = 0;
            }
        }

    }

    public static bool IsActive(CinemachineVirtualCamera camera)
    {
        return camera == activeCam;
    }
}
