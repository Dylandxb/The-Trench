using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private PlayableDirector playableDirector;
    private bool fixedState = false;
    public bool finishedPlaying = false;
    [SerializeField] private double timelineLength;

    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    private void Awake()
    {
        mainMenu.SetActive(false);
    }

    public bool IntroPlayed() => finishedPlaying = true;
    void Update()
    {
        //compares length of timeline to a variable
        timelineLength = playableDirector.time;
        //if play state has stopped playing enable the mainMenu UI
        if (playableDirector != null && finishedPlaying == false)
        {
            playableDirector.Play();
            if (playableDirector.duration == timelineLength)
            {
                //Once timeline is complete
                playableDirector.Stop();
                mainMenu.SetActive(true);
                finishedPlaying = true;
            }
        }
        //Fade text in and out with Animation track on timeline
    }
    
    public void HasPlayed()
    {
        finishedPlaying = true;
    }

}
