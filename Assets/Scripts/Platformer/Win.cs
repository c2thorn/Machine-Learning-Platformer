using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour {
    private bool won = false;
    public Agent agent;
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
            GameObject.Find("Score Text").GetComponent<ScoreKeep>().score += 500;
            Debug.Log("Won!");
            agent.LevelEnd();
        }
    }
}
