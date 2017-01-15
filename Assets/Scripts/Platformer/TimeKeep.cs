using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeep : MonoBehaviour {
    private GUIText text;
    public float time;
    public float timeScale = 30f;
    public float timeOut = 20f;
    public int viewNumber = 20;

    public static float restart = 0;
    public Agent agent;
	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
        Time.timeScale = timeScale;
        text = GetComponent<GUIText>();
        time =  0f;

        if (restart % viewNumber == 0 && restart > 0)
        {
            Time.timeScale = 5f;
        }
    }

    public float getRestart()
    {
        return restart;
    }

    public void addRestart()
    {
        restart++;
    }

    // Update is called once per frame
    void Update () {
        time += Time.deltaTime;
        text.text = "Time: " + time.ToString("F2") +" "+ restart;

        if (Input.GetKeyDown("space"))
            Time.timeScale = 1f;
        if (Input.GetKeyUp("space"))
            Time.timeScale = timeScale;

        if (time >= timeOut)
        {
            agent.LevelEnd();
        }
    }
}
