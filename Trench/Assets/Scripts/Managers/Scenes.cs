using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class Scenes : MonoBehaviour
{

    //Singleton
    public static Scenes instance;


    [SerializeField] private GameObject canvasLoad;
    [SerializeField] private Image loadFill;
    private float targetFill;
    
    public enum Scene
    {
        MainMenu,
        Level_1,
        Level_2,
        Level_3,
        Credits

    }
    
    private void Awake()
    {
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
    public async void LoadAsyncScene(Scene sceneName)
    {
        //Reset fill values 
        targetFill = 0;
        loadFill.fillAmount = 0;

        var scene = SceneManager.LoadSceneAsync(sceneName.ToString());
        //Prevent transition to scene as it loads
        scene.allowSceneActivation = false;

        canvasLoad.SetActive(true);
        //Runs at least once
        do
        {
            await Task.Delay(100);
            targetFill = scene.progress;

        } while (scene.progress < 0.9f);
        //Enable scene activation after progress fills
        scene.allowSceneActivation = true;
        //Disable canvas after scene loads
        canvasLoad.SetActive(false);
    }

    public void LoadScene(Scene sceneName)
    {
        SceneManager.LoadScene(sceneName.ToString());
    }

    private void Update()
    {
        //Lerp to a targetted amount over time
        loadFill.fillAmount = Mathf.Lerp(loadFill.fillAmount, targetFill, 3 * Time.deltaTime);
    }
}
