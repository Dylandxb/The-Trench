using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainPlayerName;
    [SerializeField] private Transform lookAt;
    [SerializeField] private Vector3 offset;
    private Camera cam;
    private Transform entity;
    private Transform worldSpaceCanvas;
    void Start()
    {
        cam = Camera.main;
       // entity = transform.parent;
       // worldSpaceCanvas = GameObject.FindObjectOfType<Canvas>().transform;
       // transform.SetParent(worldSpaceCanvas);
    }

    void Update()
    {
        //Fixes the game object to the NPC transform position
        //Offset is used to displace the icon around the npc
        Vector3 pos = cam.WorldToScreenPoint(lookAt.position + offset);
        if (transform.position != pos)
        {
            transform.position = pos;
        }
        //transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        //transform.position = entity.position + offset;
    }
}
