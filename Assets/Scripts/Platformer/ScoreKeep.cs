using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeep : MonoBehaviour {
    private GUIText text;
    public float score;
    // Use this for initialization
    void Start()
    {
        text = GetComponent<GUIText>();
        score = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Score: " + score;
    }
}
