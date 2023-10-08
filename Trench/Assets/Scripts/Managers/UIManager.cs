using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject levelName;
    [SerializeField] private CanvasGroup levelText;
    [SerializeField] private bool fadeOut;
    public static UIManager instance;
    public float timeToFade;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(instance);
        }
    }
    void Start()
    {
        levelName.SetActive(true);
    }

    void Update()
    {
        StartCoroutine(FadeLevelText());
        if (levelName.activeSelf)
        {
            fadeOut = true;
            Debug.Log(levelText.alpha);
        }
        else
        {
            fadeOut = false;
        }
    }

    public bool LevelNameFading()
    {
        return fadeOut;
    }
    private IEnumerator FadeLevelText()
    {
        if(levelText.alpha >= 0)
        {
            levelText.alpha -= timeToFade * Time.deltaTime;
        }
        yield return new WaitForSeconds(5.0f);
        levelName.SetActive(false);
    }
}
