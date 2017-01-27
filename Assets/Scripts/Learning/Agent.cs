using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Agent : Player
{
    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool jump = false;
    public Transform groundCheck;
    public TimeKeep timeKeep;

    protected bool grounded = false;
    protected Animator anim;
    protected bool viewing = false;

    public GUIText leftText;
    public GUIText upText;
    public GUIText rightText;
    public GUIText learningText;
    public GUIText learningText2;
    public GUIText learningText3;

    public float raycastDistance = 0.05f;

    public float horizontalForce = 0.275f;
    public float jumpForce = 1f;
    public float gravityForce = 0.0825f;
    
    protected bool[] actions = new bool[3];
    
    protected float velocityX = 0f;
    protected float velocityY = 0f;
    protected float tickCount = 0f;

    protected float restart = 0f;
    protected float score = 0f;

    // Use this for initialization
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        restart = timeKeep.getRestart();
    }

    protected abstract void Start();

    protected abstract void setLearningText();

    // Update is called once per frame
    protected virtual void Update() {}

    protected virtual void FixedUpdate()
    {
        tick();
        if (Input.GetKey("v"))
            LevelEnd();
    }

    public virtual void tick()
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
            velocityX = h * horizontalForce;
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

    protected virtual void gravity()
    {
        if (!grounded && velocityY > -1f)
        {
            velocityY -= gravityForce;
        }
    }

    protected virtual void move()
    {
        if (velocityX != 0)
            checkHorizontal(velocityX > 0 ? true : false);
        if (velocityY != 0)
            checkVertical(velocityY > 0 ? true : false);
        transform.position = new Vector3(transform.position.x + velocityX, transform.position.y + velocityY, transform.position.z);
    }

    protected virtual void checkHorizontal(bool right)
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

    protected virtual void checkVertical(bool up)
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

    protected virtual void checkDeath(bool up)
    {
        float boxSize = 0.6f;
        Vector2 topRight = new Vector2(transform.position.x + boxSize / 2 + .25f, transform.position.y + 1f);
        Vector2 topLeft = new Vector2(transform.position.x - boxSize / 2 - .25f, transform.position.y + 1f);

        Vector2 bottomRight = new Vector2(transform.position.x + boxSize / 2 + .25f, transform.position.y - 1f);
        Vector2 bottomLeft = new Vector2(transform.position.x - boxSize / 2 - .25f, transform.position.y - 1f);

        if (up)
        {
            if (Physics2D.Raycast(topRight, Vector2.up, velocityY, 1 << LayerMask.NameToLayer("Death")) || Physics2D.Raycast(topLeft, Vector2.up, velocityY, 1 << LayerMask.NameToLayer("Death")))
            {
                LevelEnd();
            }
        }
        else
        {

            RaycastHit2D botRight = Physics2D.Raycast(bottomRight, Vector2.down, -velocityY, 1 << LayerMask.NameToLayer("Death"));
            RaycastHit2D botLeft = Physics2D.Raycast(bottomLeft, Vector2.down, -velocityY, 1 << LayerMask.NameToLayer("Death"));
            if (botRight || botLeft)
            {
                LevelEnd();
            }
        }
    }


    protected abstract void getAction();

    protected virtual void highlightActions()
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

    protected virtual double probe(float x, float y, int i, int j)
    {
        float x2 = x + (2 * i);
        float y2 = y + (2 * j);
        GameObject obj = FindAt(new Vector2(x2, y2));
        if (obj == null)
            return 0;
        if (obj.tag == "Coin")
            return 2;
        if (obj.tag == "Finish")
            return 3;
        return 1;
    }

    protected virtual GameObject FindAt(Vector2 pos)
    {
        // get all colliders that intersect pos:
        Collider2D col = Physics2D.OverlapCircle(pos, .5f);
        if (col == null)
            return null;
        return col.gameObject;
    }

    protected virtual void OnDrawGizmos()
    {
        float x = (float)GetNearestEven(gameObject.transform.position.x);
        float y = (float)GetNearestEven(gameObject.transform.position.y);
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                float x2 = x + (2 * i);
                float y2 = y + (2 * j);
                Gizmos.DrawWireSphere(new Vector3(x2, y2, -1f), .5f);
            }
        }

    }

    protected virtual double GetNearestEven(double input)
    {
        double output = Math.Round(input / 2);
        if (output == 0 && input > 0) output += 1;
        output *= 2;

        return output;
    }

    protected virtual float getHorizontal()
    {
        float h = 0;
        if (actions[0])
            h++;
        if (actions[1])
            h--;
        return h;
    }
    
    protected virtual void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public override void LevelEnd()
    {
        //float time = Mathf.Round(timeKeep.time) > timeKeep.timeOut ? timeKeep.timeOut : Mathf.Round(timeKeep.time);
        float time = timeKeep.time > timeKeep.timeOut ? timeKeep.timeOut : timeKeep.time;
        if (!viewing)
        {
            score = scoreKeep.score - time + timeKeep.timeOut;
            submitScore();
        }
        timeKeep.addRestart();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    protected abstract void submitScore();
}