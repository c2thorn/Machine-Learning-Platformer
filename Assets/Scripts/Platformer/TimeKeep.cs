using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeep : MonoBehaviour {
    private GUIText text;
    public float time;
    public float timeScale = 30f;

    public float timeOutView = 30f;
    public float timeOutEval = 10f;
    public int viewNumber = 20;
    
    [HideInInspector]
    public float timeOut = 10f;

    public static float restart = 0;
    public Agent agent;

    private bool viewing = false;
    public bool forceView = false;

	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
        text = GetComponent<GUIText>();
        BeginLevel();
    }

    public void BeginLevel()
    {
        time = 0f;
        if ((restart % viewNumber == 0 && restart > 0) || forceView)
        {
            Time.timeScale = 1f;
            timeOut = timeOutView;
            viewing = true;
            forceView = false;
        }
        else
        {
            viewing = false;
            Time.timeScale = timeScale;
            timeOut = timeOutEval;
        }
    }

    public bool GetViewing()
    {
        return viewing;
    }

    public float getRestart()
    {
        return restart;
    }

    public void addRestart()
    {
        restart++;
    }
    
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            forceView = true;
            agent.LevelRestart();
        }

        if (Input.GetKeyDown("v"))
            agent.LevelRestart();
    }

    void FixedUpdate () {
        time += Time.fixedDeltaTime;
        text.text = "Time: " + time.ToString("F2") +" "+ restart;

        if (time >= timeOut)
        {
            agent.LevelEnd();
        }

    }
}
