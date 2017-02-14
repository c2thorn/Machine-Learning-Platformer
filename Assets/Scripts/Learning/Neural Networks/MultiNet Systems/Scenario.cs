using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario  {
    public NeuralNet net;
    public string coinName = "None";
    public float fitness = -1;
    public Vector3 position;
    public float velocityX;
    public float velocityY;
    public bool facingRight;
    public bool grounded;
    public bool jump;

    public Scenario() { }

    public Scenario Copy()
    {
        Scenario copy = new Scenario();

        copy.net = net.copy();
        copy.coinName = (string)coinName.Clone();
        copy.fitness = fitness;
        copy.position = position;
        copy.velocityX = velocityX;
        copy.velocityY = velocityY;
        copy.facingRight = facingRight;
        copy.grounded = grounded;
        copy.jump = jump;

        return copy;
    }

    public override string ToString()
    {
        return "Coin: " + coinName + ",  Pos: " + position + ", VX: " + velocityX + ", VY: " + velocityY + ", FR: " + facingRight + ", GR: " +
             grounded + ", Jump: " + jump;
    }
}
