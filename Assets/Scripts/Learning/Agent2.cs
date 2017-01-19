using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Agent2 : MonoBehaviour
{
    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool jump = false;
    public Transform groundCheck;

    private bool grounded = false;
    private Animator anim;
    //private Rigidbody2D rb2d;
    bool[] actions = new bool[3];

    public ScoreKeep scoreKeep;
    public TimeKeep timeKeep;

    public static Evolution ev = new Evolution();
    private NeuralNet net;

    public GUIText leftText;
    public GUIText upText;
    public GUIText rightText;
    public GUIText learningText;
    public GUIText learningText2;
    public GUIText learningText3;

    private bool viewing = false;

    private float velocityX = 0f;
    private float velocityY = 0f;
    private float tickCount = 0f;

    public float raycastDistance = 0.05f;

    public float horizontalForce = 0.275f;
    public float jumpForce = 1f;
    public float gravityForce = 0.0825f;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        //rb2d = GetComponent<Rigidbody2D>();
        float restart = timeKeep.getRestart();
        if (restart % timeKeep.viewNumber == 0 && restart > 0)
        {
            viewing = true;
            net = ev.getBestNet();
        }
        else
        {
            viewing = false;
            net = ev.getCurrentNet();
        }
    }

    void Start()
    {
        setLearningText();
    }

    void setLearningText()
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

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        tickCount++;
        //if (tickCount % 5 == 0)
        //tick();

        //if (Input.GetKey("space"))
        //if (Input.GetKeyDown("space"))
            tick();
        if (Input.GetKey("v"))
            LevelEnd();
    }

    public void tick()
    {

        grounded = Physics2D.Linecast(transform.position + (0.25f * Vector3.left), groundCheck.position + (0.25f * Vector3.left), 1 << LayerMask.NameToLayer("Ground"))
    || Physics2D.Linecast(transform.position + (0.25f * Vector3.right), groundCheck.position + (0.25f * Vector3.right), 1 << LayerMask.NameToLayer("Ground"));

        if (actions[2] && grounded && velocityY == 0)
        {
            jump = true;
        }
        getAction();

        float h = getHorizontal();

        anim.SetFloat("Speed", Mathf.Abs(h));

        if (h != 0)
        {
            velocityX = h*horizontalForce;
        }
        else
        {
            velocityX = 0f;
        }

        if (h > 0 && !facingRight)
            Flip();
        else if (h < 0 && facingRight)
            Flip();

        gravity();

        if (jump)
        {
            anim.SetTrigger("Jump");
            velocityY = jumpForce;
            jump = false;
        }

        move();

    }

    void gravity()
    {
        if (!grounded && velocityY > -1f)
        {
            velocityY -= gravityForce;
        }
        //if (grounded)
         //   velocityY = 0f;
    }

    void move()
    {
        if (velocityX != 0)
            checkHorizontal(velocityX > 0 ? true : false);
        if (velocityY != 0)
            checkVertical(velocityY > 0 ? true : false);
        transform.position = new Vector3(transform.position.x + velocityX, transform.position.y + velocityY, transform.position.z);
    }

    void checkHorizontal (bool right)
    {
        float boxSize = 0.6f;
        Vector2 topRight = new Vector2(transform.position.x + boxSize / 2 + .25f, transform.position.y + .95f);
        Vector2 topLeft = new Vector2(transform.position.x - boxSize / 2 - .25f, transform.position.y + .95f);

        Vector2 bottomRight = new Vector2(transform.position.x + boxSize / 2 + .25f, transform.position.y - .95f);
        Vector2 bottomLeft = new Vector2(transform.position.x - boxSize / 2 - .25f, transform.position.y - .95f);

        if (right)
        {
            if (Physics2D.Raycast(topRight, Vector2.right, velocityX, 1 << LayerMask.NameToLayer("Ground")) || Physics2D.Raycast(bottomRight, Vector2.right, velocityX, 1 << LayerMask.NameToLayer("Ground")))
            {
                velocityX = 0f;
            }
        }
        else
        {
            if (Physics2D.Raycast(topLeft, Vector2.left, -velocityX, 1 << LayerMask.NameToLayer("Ground")) || Physics2D.Raycast(bottomLeft, Vector2.left, -velocityX, 1 << LayerMask.NameToLayer("Ground")))
            {
                velocityX = 0f;
            }
        }
    }

    void checkVertical(bool up)
    {
        float boxSize = 0.6f;
        Vector2 topRight = new Vector2(transform.position.x + boxSize / 2 + .25f, transform.position.y + 1f);
        Vector2 topLeft = new Vector2(transform.position.x - boxSize / 2 - .25f, transform.position.y + 1f);

        Vector2 bottomRight = new Vector2(transform.position.x + boxSize / 2 + .25f, transform.position.y - 1f);
        Vector2 bottomLeft = new Vector2(transform.position.x - boxSize / 2 - .25f, transform.position.y - 1f);

        if (up)
        {
            if (Physics2D.Raycast(topRight, Vector2.up, velocityY, 1 << LayerMask.NameToLayer("Ground")) || Physics2D.Raycast(topLeft, Vector2.up, velocityY, 1 << LayerMask.NameToLayer("Ground")))
            {
                velocityY = 0f;
            }
        }
        else
        {

            RaycastHit2D botRight = Physics2D.Raycast(bottomRight, Vector2.down, -velocityY, 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D botLeft = Physics2D.Raycast(bottomLeft, Vector2.down, -velocityY, 1 << LayerMask.NameToLayer("Ground"));
            if (botRight || botLeft)
            {
                if (botRight)
                    velocityY = -botRight.distance;
                if (botLeft)
                    velocityY = -botLeft.distance;
            }
        }
    }


    void getAction()
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
        inputs[inputs.Length - 2] = grounded && velocityY == 0 ? 1 : 0;
        inputs[inputs.Length - 1] = 1;

        double[] outputs = net.propagate(inputs);

        for (int i = 0; i < 3; i++)
            actions[i] = outputs[i] > 0;
        //Debug.Log(actions[0] + " " + actions[1] + " " + actions[2]);
        highlightActions();
    }

    void highlightActions()
    {
        if (actions[1])
        {
            leftText.color = Color.red;
            leftText.fontStyle = FontStyle.Bold;
        }
        else
        {
            leftText.color = Color.white;
            leftText.fontStyle = FontStyle.Normal;
        }
        if (actions[2])
        {
            upText.color = Color.red;
            upText.fontStyle = FontStyle.Bold;
        }
        else
        {
            upText.color = Color.white;
            upText.fontStyle = FontStyle.Normal;
        }
        if (actions[0])
        {
            rightText.color = Color.red;
            rightText.fontStyle = FontStyle.Bold;
        }
        else
        {
            rightText.color = Color.white;
            rightText.fontStyle = FontStyle.Normal;
        }
    }

    byte[][] getScene()
    {
        byte[][] scene = new byte[22][];
        for (int i = 0; i < 22; i++)
        {
            scene[i] = new byte[22];
        }
        return scene;
    }

    private double probe(float x, float y, int i, int j)
    {
        float x2 = x + (2 * i);
        float y2 = y + (2 * j);
        GameObject obj = FindAt(new Vector2(x2, y2));
        if (obj == null)
            return 0;
        //if (obj.name != "PlayerHero")
        //  obj.GetComponent<Renderer>().material.color = Color.red;
        if (obj.tag == "Coin")
            return 2;
        if (obj.tag == "Finish")
            return 3;
        return 1;
    }

    private GameObject FindAt(Vector2 pos)
    {
        // get all colliders that intersect pos:
        Collider2D col = Physics2D.OverlapCircle(pos, .5f);
        if (col == null)
            return null;
        return col.gameObject;
    }

    /* void OnDrawGizmos()
     {
         float x = (float)GetNearestEven(gameObject.transform.position.x);
         float y = (float)GetNearestEven(gameObject.transform.position.y);
         for (int i = -2; i < 3; i++)
         {
             for (int j = -2; j < 3; j++)
             {
                 float x2 = x + (2 * i);
                 float y2 = y + (2 * j);
                 Gizmos.DrawWireSphere(new Vector3(x2,y2,-1f), .5f);
             }
         }

     }*/

    double GetNearestEven(double input)
    {
        double output = Math.Round(input / 2);
        if (output == 0 && input > 0) output += 1;
        output *= 2;

        return output;
    }

    float getHorizontal()
    {
        float h = 0;
        if (actions[0])
            h++;
        if (actions[1])
            h--;
        return h;
    }



    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void LevelEnd()
    {
        float time = Mathf.Round(timeKeep.time) > timeKeep.timeOut ? timeKeep.timeOut : Mathf.Round(timeKeep.time);

        if (!viewing)
        {
            float score = scoreKeep.score - time + timeKeep.timeOut;
            ev.submitScore(score);
        }
        timeKeep.addRestart();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}