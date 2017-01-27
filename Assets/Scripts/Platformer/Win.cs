using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour {
    private bool won = false;
    public Agent agent;
    public TimeKeep timeKeep;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    } 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !won)
        {
            won = true;
            other.GetComponent<Agent>().grabWin();
            //Debug.Log("Won! "+timeKeep.getRestart());
            agent.LevelEnd();
        }
    }
}
