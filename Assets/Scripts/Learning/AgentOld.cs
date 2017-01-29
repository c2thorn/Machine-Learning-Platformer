using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * Does this still work?
 */
public class AgentOld : Agent
{
    public float moveForce = 365f;
    public float maxSpeed = 5f;
    private Rigidbody2D rb2d;

    private NeuralNet net;

    public static Evolution ev = new Evolution();

    protected override void Awake()
    {
        jumpForce = 1000f;
    }

    protected override void BeginLevel()
    {
        base.BeginLevel();
        setLearningText();
    }

    protected override void setLearningText()
    {
        float restart = timeKeep.getRestart();
        if (restart % timeKeep.viewNumber == 0 && restart > 0)
        {
            learningText.text = "Viewing Elite";
            learningText.fontSize = 16;
            learningText2.text = "";
            learningText3.text = "";
        }
        else
        {
            String text1 = "";
            String text2 = "";
            String text3 = "";
            int j = ev.popSize / 3;
            int k = j * 2;
            for (int i = 0; i < j; i++)
            {
                String line = "";
                if (i == ev.popIndex)
                    line += "<color=#ff0000>[ " + i + " " + ev.fitness[i] + " ]</color>\n";
                else
                    line += i + " " + ev.fitness[i] + "\n";

                text1 += line;
            }
            for (int i = j; i < k; i++)
            {
                String line = "";
                if (i == ev.popIndex)
                    line += "<color=#ff0000>[ " + i + " " + ev.fitness[i] + " ]</color>\n";
                else
                    line += i + " " + ev.fitness[i] + "\n";

                text2 += line;
            }
            for (int i = k; i < ev.popSize; i++)
            {
                String line = "";
                if (i == ev.popIndex)
                    line += "<color=#ff0000>[ " + i + " " + ev.fitness[i] + " ]</color>\n";
                else
                    line += i + " " + ev.fitness[i] + "\n";

                text3 += line;
            }
            learningText.text = text1;
            learningText2.text = text2;
            learningText3.text = text3;
        }
    }
    

    protected override void FixedUpdate()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (actions[2] && grounded && rb2d.velocity.y == 0)
        {
            jump = true;
        }
        getAction();

        float h = getHorizontal();

        anim.SetFloat("Speed", Mathf.Abs(h));


        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 topRight = new Vector2(rb2d.position.x + box.size.x / 2 + .25f, rb2d.position.y + 0.6f);
        Vector2 topLeft = new Vector2(rb2d.position.x + -box.size.x / 2 - .25f, rb2d.position.y + 0.6f);

        Vector2 bottomRight = new Vector2(rb2d.position.x + box.size.x / 2 + .25f, rb2d.position.y - 0.6f);
        Vector2 bottomLeft = new Vector2(rb2d.position.x + -box.size.x / 2 - .25f, rb2d.position.y - 0.6f);

        //Debug.DrawLine((Vector3)boxVectorStartLeft, (Vector3)boxVectorStartLeft - (.1f) * Vector3.right);
        //for (int i = 0; i < 15; i++)
        //{
        //  Debug.Log(i + " " + LayerMask.LayerToName(i));
        //}

        if (Physics2D.Raycast(topRight, Vector2.right, raycastDistance, 1 << LayerMask.NameToLayer("Ground")) && Physics2D.Raycast(bottomRight, Vector2.right, raycastDistance, 1 << LayerMask.NameToLayer("Ground")) && h > 0)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            //Debug.Log("There's a thing right");
        }
        else if (Physics2D.Raycast(topLeft, Vector2.left, raycastDistance, 1 << LayerMask.NameToLayer("Ground")) && Physics2D.Raycast(bottomLeft, Vector2.right, raycastDistance, 1 << LayerMask.NameToLayer("Ground")) && h < 0)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            //Debug.Log("There's a thing left");
        }
        else if (h != 0)
        {
            rb2d.velocity = new Vector2(h * maxSpeed, rb2d.velocity.y);
            //if (h * rb2d.velocity.x < maxSpeed)
            //    rb2d.AddForce(Vector2.right * h * moveForce);

            //if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            //    rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
        }
        else
        {
            // if (h * rb2d.velocity.x < maxSpeed)
            //   rb2d.AddForce(Vector2.right * h * moveForce);

            //if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }

        if (h > 0 && !facingRight)
            Flip();
        else if (h < 0 && facingRight)
            Flip();

        if (jump)
        {
            anim.SetTrigger("Jump");
            rb2d.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }
    }


    protected override void getAction()
    {
        double[] inputs = new double[28];
        int which = 0;
        float x = (float)GetNearestEven(gameObject.transform.position.x);
        float y = (float)GetNearestEven(gameObject.transform.position.y);
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                inputs[which++] = probe(x, y, i, j);
            }
        }
        inputs[inputs.Length - 3] = grounded ? 1 : 0;
        inputs[inputs.Length - 2] = grounded && rb2d.velocity.y == 0 ? 1 : 0;
        inputs[inputs.Length - 1] = 1;

        double[] outputs = net.propagate(inputs);

        for (int i = 0; i < 3; i++)
            actions[i] = outputs[i] > 0;

        highlightActions();
    }
   
    public override void tick()
    {
       //Not used :[
    }

    protected override void submitScore()
    {
        ev.submitScore(score);
    }
}