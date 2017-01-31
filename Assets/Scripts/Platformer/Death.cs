using UnityEngine;
using System.Collections;

public class Death : MonoBehaviour
{
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
        /*if (other.gameObject.CompareTag("Player") && !dead)
        {
            dead = true;
            //GameObject.Find("Score Text").GetComponent<ScoreKeep>().score -= 48;
            agent.LevelEnd();
        }*/
    }
}