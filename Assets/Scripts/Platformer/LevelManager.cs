using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public string agentType = "Human";
    public TimeKeep timeKeep;
    public ScoreKeep scoreKeep;
    public GUIText leftText;
    public GUIText upText;
    public GUIText rightText;

    public float horizontalForce = 0.275f;
    public float jumpForce = 1f;
    public float gravityForce = 0.0825f;
    public GUIText learningText;
    public GUIText learningText2;
    public GUIText learningText3;
    public string loadPath = "";
    public int SimPopulationSize = 20;

    [HideInInspector]
    public GameObject hero;

    public GameObject heroPrefab;

    public GameObject SimRoboPrefab;

    SimEvolution simEv;

    // Use this for initialization
    void Awake ()
    {
        if (agentType.Equals("Sim"))
        {
            GameObject[] robos = new GameObject[SimPopulationSize];
            for (int i = 0; i < SimPopulationSize; i++)
            {
                robos[i] = Instantiate(SimRoboPrefab);
                SimNEAgent controller = robos[i].AddComponent<SimNEAgent>();
                controller.horizontalForce = horizontalForce;
                controller.jumpForce = jumpForce;
                controller.gravityForce = gravityForce;
            }

            simEv = new SimEvolution(robos, loadPath);
            //hero = robos[0];
            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            camera.player = robos[0].transform;
            camera.simultaneous = true;
        }
        else
        {
            hero = Instantiate(heroPrefab);
            if (agentType.Equals("Human"))
            {
                HumanController controller = hero.AddComponent<HumanController>();
                controller.horizontalForce = horizontalForce;
                controller.jumpForce = jumpForce;
                controller.gravityForce = gravityForce;
            }
            else if (agentType.Equals("Linear"))
            {
                LinearMNAgent controller = hero.AddComponent<LinearMNAgent>();
                controller.horizontalForce = horizontalForce;
                controller.jumpForce = jumpForce;
                controller.gravityForce = gravityForce;
                controller.loadPath = loadPath;
                SetLearningMode();
            }
            else if (agentType.Equals("Optimizing"))
            {
                OptimizingMNAgent controller = hero.AddComponent<OptimizingMNAgent>();
                controller.horizontalForce = horizontalForce;
                controller.jumpForce = jumpForce;
                controller.gravityForce = gravityForce;
                controller.loadPath = loadPath;
                SetLearningMode();
            }
            else if (agentType.Equals("Evolution"))
            {
                NEAgent controller = hero.AddComponent<NEAgent>();
                controller.horizontalForce = horizontalForce;
                controller.jumpForce = jumpForce;
                controller.gravityForce = gravityForce;
                controller.loadPath = loadPath;
                timeKeep.timeOutEval = 15;
                SetLearningMode();
            }
            GameObject camera = GameObject.Find("Main Camera");
            camera.GetComponent<Camera>().player = hero.transform;
        }
	}

    public Agent GetAgent()
    {
        if (hero)
            if (hero.GetComponent("Agent"))
                return hero.GetComponent<Agent>();
        return null;
    }

    public void SetLearningMode()
    {
        TurnOffParticles();
        ToggleStaticFlame(true);
        ToggleAnimations(false);
    }

    public void SetViewingMode()
    {
        TurnOnParticles();
        ToggleStaticFlame(false);
        ToggleAnimations(true);
    }

    public void TurnOnParticles()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Particle"))
        {
            obj.GetComponent<ParticleSystem>().Play();
        }
    }

    public void TurnOffParticles()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Particle"))
        {
            obj.GetComponent<ParticleSystem>().Stop();
            obj.GetComponent<ParticleSystem>().Clear();
        }
    }

    public void ToggleStaticFlame(bool en)
    {
        GameObject flame = GameObject.Find("FlameDeath");
        if (flame)
            flame.GetComponent<SpriteRenderer>().enabled = en;
    }


    public void ToggleAnimations(bool en)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("NPC"))
        {
            obj.GetComponent<Animator>().enabled = en;
        }
        hero.GetComponentInChildren<Animator>().enabled = en;
    }
}
