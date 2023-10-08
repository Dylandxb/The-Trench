using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NametagHover : MonoBehaviour
{
    [SerializeField] private GameObject nameTagText;
    [SerializeField] private Transform followPlayer;

    void Start()
    {
        nameTagText.SetActive(true);
    }

    void Update()
    {
        nameTagText.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.25f,1.8f,0));
    }
}
