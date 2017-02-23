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

    [HideInInspector]
    public GameObject hero;

    public GameObject heroPrefab;

    // Use this for initialization
    void Awake ()
    {
        hero = Instantiate(heroPrefab);
        if (agentType.Equals("Human")) {
            HumanController controller = hero.AddComponent<HumanController>();

            controller.horizontalForce = horizontalForce;
            controller.jumpForce = jumpForce;
            controller.gravityForce = gravityForce;
        } else if (agentType.Equals("Linear")){
            LinearMNAgent controller = hero.AddComponent<LinearMNAgent>();
            controller.horizontalForce = horizontalForce;
            controller.jumpForce = jumpForce;
            controller.gravityForce = gravityForce;
            controller.loadPath = loadPath;
            TurnOffParticles();
        } else if (agentType.Equals("Optimizing"))
        {
            OptimizingMNAgent controller = hero.AddComponent<OptimizingMNAgent>();
            controller.horizontalForce = horizontalForce;
            controller.jumpForce = jumpForce;
            controller.gravityForce = gravityForce;
            controller.loadPath = loadPath;
            TurnOffParticles();
        } else if (agentType.Equals("Evolution"))
        {
            NEAgent controller = hero.AddComponent<NEAgent>();
            controller.horizontalForce = horizontalForce;
            controller.jumpForce = jumpForce;
            controller.gravityForce = gravityForce;
            controller.loadPath = loadPath;
            timeKeep.timeOutEval = 15;
            TurnOffParticles();
        }
        GameObject camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().player = hero.transform;
	}

    public Agent GetAgent()
    {
        if (hero.GetComponent("Agent"))
            return hero.GetComponent<Agent>();
        return null;
    }

    public void TurnOnParticles()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Particle"))
        {
            obj.GetComponent<ParticleSystem>().Play();
        }
        ToggleStaticFlame(false);
    }

    public void TurnOffParticles()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Particle"))
        {
            obj.GetComponent<ParticleSystem>().Stop();
        }
        ToggleStaticFlame(true);
    }

    public void ToggleStaticFlame(bool enabled)
    {
        GameObject flame = GameObject.Find("FlameDeath");
        if (flame)
            flame.GetComponent<SpriteRenderer>().enabled = enabled;
    }
}
