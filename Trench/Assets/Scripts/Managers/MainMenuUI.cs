using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    private float fadeTime = 4.0f;
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }

    public void PlayGame()
    {
        //Scenes.instance.LoadScene(Scenes.Scene.Level_1);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        mainMenuCanvas.SetActive(false);
        //Scenes.instance.LoadAsyncScene(Scenes.Scene.Level_1);
       // StartCoroutine(BeginTextFade());
        Scenes.instance.LoadAsyncLevel(Scenes.Scene.Level_1);

        //Wait for canvas to fade out before triggering the async load
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
