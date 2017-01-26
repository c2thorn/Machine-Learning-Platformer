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
        //Time.fixedDeltaTime = timeScale * 0.02f;
        text = GetComponent<GUIText>();
        time =  0f;

        if (restart % viewNumber == 0 && restart > 0)
        {
            Time.timeScale = 1f;
            timeOut = 30f;
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

        /*if (Input.GetKeyDown("space"))
        {
            Time.timeScale = 1f;
            //Time.fixedDeltaTime = 1f * 0.02f;
        }
        if (Input.GetKeyUp("space"))
        {

            Time.timeScale = timeScale;

            //Time.fixedDeltaTime = timeScale * 0.02f;
        }*/

        if (Input.GetKeyDown("f"))
        {
            int remainder = (int)restart % viewNumber;
            if (remainder != 0)
                restart =  restart + viewNumber - remainder - 1;
        }
        if (time >= timeOut)
        {
            agent.LevelEnd();
        }

    }
}
