using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {

    public Transform player;
    public Vector3 offset;
    Vector3 specificVector;
    public float smooth = .5f;

    public bool simultaneous = false;

    void Update()
    {
        if (simultaneous)
            FindFurthestRobot();

        specificVector = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, specificVector, smooth * Time.deltaTime);
        //transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z); // Camera follows the player with specified offset position
    }

    private void FindFurthestRobot()
    {
        GameObject[] robos = GameObject.FindGameObjectsWithTag("Player");
        List<SimNEAgent> agents = new List<SimNEAgent>();


        foreach(GameObject robo in robos)
        {
            SimNEAgent agent = robo.GetComponent<SimNEAgent>();
            if (agent)
            {
                if (!agent.terminated)
                    agents.Add(agent);

            }
        }

        if (agents.Count > 0)
            player = agents[0].transform;
        
        foreach(SimNEAgent agent in agents)
        {
            if (agent.transform.position.x > player.position.x)
            {
                player = agent.transform;
            }
        }
    }
}
