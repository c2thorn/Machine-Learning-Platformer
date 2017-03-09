using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * Player class. Root of human player classes and non-human agents. 
 * Handles physics and what to do once actions are determined.
 * How the action gets determined is up to children classes.
 */
public abstract class Player : MonoBehaviour
{
    public TimeKeep timeKeep;
    public ScoreKeep scoreKeep;
    public GUIText leftText;
    public GUIText upText;
    public GUIText rightText;
    public Transform groundCheck;

    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool jump = false;

    protected Animator anim;
    
    public float horizontalForce = 0.275f;
    public float jumpForce = 1f;
    public float gravityForce = 0.0825f;
    
    //Possible actions
    protected bool[] actions = new bool[3];

    public bool grounded = false;
    protected float velocityX = 0f;
    protected float velocityY = 0f;
    public int wallRiding = 0;

    //Boolean to control when to freeze physics
    protected bool stopTick = false;

    //Is the current tick finished?
    protected bool tickDone = true;

    protected Vector3 beginningPosition;

    protected GameObject[] coinObjects;

    //Horizontal movement
    protected float h;
    
    private float halfBox = 0.61f / 2f;


    public int pushedCount = 0;
    public int pushLength = 14;
    //public float pushedForce = 0.275f;
    public float pushedForce = 0.225f;
    public float wallRideFallTolerance = 0.1f;

    // Use this for initialization
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        if (!anim)
            anim = GetComponentInChildren<Animator>();
        groundCheck = transform.FindChild("groundCheck");
        //groundCheck = GameObject.Find("groundCheck").transform;
        scoreKeep = GameObject.Find("Score Text").GetComponent<ScoreKeep>();
        timeKeep = GameObject.Find("Time Text").GetComponent<TimeKeep>();
        leftText = GameObject.Find("Left Text").GetComponent<GUIText>();
        rightText = GameObject.Find("Right Text").GetComponent<GUIText>();
        upText = GameObject.Find("Up Text").GetComponent<GUIText>();

        beginningPosition = transform.position;
        coinObjects = GameObject.FindGameObjectsWithTag("Coin");

        BeginLevel();
    }

    protected virtual void BeginLevel()
    {
        InitialSettings();
    }

    protected virtual void InitialSettings()
    {
        transform.position = beginningPosition;
        FlipRight();
        jump = false;
        grounded = false;
        velocityX = 0f;
        velocityY = 0f;
        stopTick = false;
        actions = new bool[3];
        scoreKeep.score = 0f;
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        highlightActions();
    }

    // FixedUpdate is called on set intervals
    protected virtual void FixedUpdate()
    {
        if (!stopTick && tickDone)
            tick();
    }

    //Abstract class for child classes to implement
    protected abstract void GetActions();

    //Interpret action booleans
    protected virtual void ApplyActions()
    {
        if (actions[2] && ((grounded && velocityY == 0) || wallRiding > 0))
        {
            jump = true;
        }

        h = getHorizontal();
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

    //Physics method
    public virtual void tick()
    {
        tickDone = false;

        //check if on ground
        grounded = Physics2D.Linecast(transform.position + (halfBox * Vector3.left), groundCheck.position + (halfBox * Vector3.left), 1 << LayerMask.NameToLayer("Ground"))
    || Physics2D.Linecast(transform.position + (halfBox * Vector3.right), groundCheck.position + (halfBox * Vector3.right), 1 << LayerMask.NameToLayer("Ground"));

        if (grounded)
        {
            wallRiding = 0;
            anim.SetBool("WallRiding", false);
        }

        GetActions();
        ApplyActions();

        anim.SetFloat("Speed", Mathf.Abs(h));

        if (pushedCount > 0)
            pushedCount++;

        if (pushedCount > pushLength)
        {
            pushedCount = 0;
            velocityX = 0f;
        }

        if (pushedCount == 0)
        {
            if (h != 0)
            {
                velocityX = h * horizontalForce;
            }
            else
            {
                velocityX = 0f;
            }
        }
  

        gravity();

        if (jump)
        {
            anim.SetTrigger("Jump");
            velocityY = jumpForce;
            jump = false;

            if (wallRiding > 0)
            {
                pushedCount += 1;
            }
            switch (wallRiding)
            {
                case 1:
                    velocityX = pushedForce;
                    break;
                case 2:
                    velocityX = -pushedForce;
                    break;
            }
        }

        if (pushedCount > 0)
        {
            wallRiding = 0;
            if  (velocityX > 0)
            {
                velocityX = h > 0 ? pushedForce + horizontalForce : pushedForce;
            } else
            {
                velocityX = -(h < 0 ? pushedForce + horizontalForce : pushedForce);
            }
        }

        move();
        
        if (velocityX > 0 && !facingRight && wallRiding == 0)
            Flip();
        else if (velocityX < 0 && facingRight && wallRiding == 0)
            Flip();
        tickDone = true;
    }

    protected void gravity()
    {
        if (!grounded && velocityY > -1f)
        {
            velocityY -= gravityForce;
        }
        if (wallRiding > 0)
            velocityY /= 2;
        if (velocityY < 0)
            anim.SetBool("Falling", true);
        else
            anim.SetBool("Falling", false);
    }

    protected void move()
    {
        if (velocityY != 0)
            checkVertical(velocityY > 0 ? true : false);
        transform.position = new Vector3(transform.position.x, transform.position.y + velocityY, transform.position.z);
        if (velocityX != 0 || wallRiding > 0)
            checkHorizontal(velocityX > 0 ? true : false);
        transform.position = new Vector3(transform.position.x + velocityX, transform.position.y, transform.position.z);
        checkCollisions();
    }

    protected void checkHorizontal(bool right)
    {
        Vector2 topRight = new Vector2(transform.position.x + halfBox, transform.position.y + .95f);
        Vector2 topLeft = new Vector2(transform.position.x - halfBox, transform.position.y + .95f);

        Vector2 bottomRight = new Vector2(transform.position.x + halfBox, transform.position.y - .95f);
        Vector2 bottomLeft = new Vector2(transform.position.x - halfBox, transform.position.y - .95f);

        Vector2 midRight = new Vector2(transform.position.x + halfBox, transform.position.y);
        Vector2 midLeft = new Vector2(transform.position.x - halfBox, transform.position.y);

        bool hitWall = false;

        float distanceToCheck = wallRiding > 0 ? horizontalForce : Mathf.Abs(velocityX);
        
        if (right || wallRiding == 2)
        {
            RaycastHit2D topRightRay = Physics2D.Raycast(topRight, Vector2.right, distanceToCheck, 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D botRightRay = Physics2D.Raycast(bottomRight, Vector2.right, distanceToCheck, 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D midRightRay = Physics2D.Raycast(midRight, Vector2.right, distanceToCheck, 1 << LayerMask.NameToLayer("Ground"));
            if (topRightRay || botRightRay)
            {
                if (right)
                    velocityX = 0f;
                if (midRightRay)
                    hitWall = true;
               
            }
        }
        if (!right || wallRiding == 1)
        {
            RaycastHit2D topLeftRay = Physics2D.Raycast(topLeft, Vector2.left, distanceToCheck, 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D botLeftRay = Physics2D.Raycast(bottomLeft, Vector2.left, distanceToCheck, 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D midLeftRay = Physics2D.Raycast(midLeft, Vector2.left, distanceToCheck, 1 << LayerMask.NameToLayer("Ground"));
            if (topLeftRay || botLeftRay)
            {
                if (!right)
                    velocityX = 0f;
                if (midLeftRay)
                    hitWall = true;
            }
        }
        if (hitWall && !grounded && velocityY < wallRideFallTolerance)
        {
            //We are wall riding!
            if (wallRiding == 0)
            {
                pushedCount = 0;
                wallRiding = right ? 2 : 1;
                WallRideAnimation(right);
            }
        }
        else
        {
            //We are not wall riding
            wallRiding = 0;
            anim.SetBool("WallRiding", false);
        }
    }

    protected void checkVertical(bool up)
    {
        Vector2 topRight = new Vector2(transform.position.x + halfBox, transform.position.y + 1f);
        Vector2 topLeft = new Vector2(transform.position.x - halfBox, transform.position.y + 1f);

        Vector2 bottomRight = new Vector2(transform.position.x + halfBox, transform.position.y - 1f);
        Vector2 bottomLeft = new Vector2(transform.position.x - halfBox, transform.position.y - 1f);

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

    protected void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    protected virtual void checkCollisions()
    {
        float boxSize = 0.6f;
        Vector2 topLeft = new Vector2(transform.position.x - boxSize / 2 - .25f, transform.position.y + 1f);
        Vector2 bottomRight = new Vector2(transform.position.x + boxSize / 2 + .25f, transform.position.y - 1f);

        Collider2D[] colliderArray = Physics2D.OverlapAreaAll(topLeft, bottomRight);

        if (colliderArray.Length > 1)
        {
            foreach (Collider2D col in colliderArray)
            {
                if (col.tag.Equals("Death"))
                {
                    stopTick = true;
                    LevelEnd();
                }
                if (col.tag.Equals("Finish"))
                {
                    stopTick = true;
                    grabWin();
                    LevelEnd();
                }
                if (col.tag.Equals("Coin"))
                {
                    grabCoin(col.gameObject.name);
                    col.gameObject.SetActive(false);
                }
            }

        }
    }

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

    protected virtual void FlipRight()
    {
        facingRight = true;
        Vector3 theScale = transform.localScale;
        if (theScale.x < 0)
            theScale.x *= -1;
        transform.localScale = theScale;
    }

    protected virtual void WallRideAnimation(bool right)
    {
        Flip(right);
        anim.SetBool("WallRiding", true);
    }
    
    protected virtual void Flip(bool right)
    {
        facingRight = !right;
        Vector3 theScale = transform.localScale;
        if (right)
        {
            if (theScale.x > 0)
                theScale.x *= -1;
            transform.localScale = theScale;
        }
        else
        {
            if (theScale.x < 0)
                theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    public virtual void grabCoin(string coinName)
    {
        scoreKeep.score += 50;
    }

    public virtual void grabWin()
    {
        scoreKeep.score += 500;
    }

    public virtual void LevelEnd()
    {
        LevelRestart();
    }

    public virtual void RestoreCoins()
    {
        foreach (GameObject coin in coinObjects)
        {
            coin.SetActive(true);
        }
    }

    public virtual void LevelRestart()
    {
        timeKeep.addRestart();
        RestoreCoins();
        timeKeep.BeginLevel();
        BeginLevel();
    }
}
