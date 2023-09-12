using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChange : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided");
            //Scenes.instance.LoadAsyncScene(Scenes.Scene.MainMenu);
            Scenes.instance.LoadScene(Scenes.Scene.MainMenu);
        }
    }

    public void LoadLevel1()
    {
        //Scenes.instance.LoadAsyncScene(Scenes.Scene.Level_1);
        Scenes.instance.LoadScene(Scenes.Scene.Level_1); 
    }

    //Fix Async Loading!!!!
}
