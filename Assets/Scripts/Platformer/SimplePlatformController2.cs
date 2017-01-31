using UnityEngine;
using System.Collections;
using System;

public class SimplePlatformController2 : Player
{

    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool jump = false;

    public Transform groundCheck;


    private bool grounded = false;
    private Animator anim;
    //private Rigidbody2D transform;

    public GUIText leftText;
    public GUIText upText;
    public GUIText rightText;

    public float raycastDistance = 0.05f;

    private float velocityX = 0f;
    private float velocityY = 0f;
    private float tickCount = 0f;

    public float horizontalForce = 0.275f;
    public float jumpForce = 1f;
    public float gravityForce = 0.0825f;

    protected bool stopTick = false;

    protected Vector3 beginningPosition;

    GameObject[] coinObjects;

    // Use this for initialization
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
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
        tickCount = 0f;
        stopTick = false;
    }

    // Update is called once per frame
    void Update()
    {
        highlightActions();
    }

    void highlightActions()
    {
        float h = Input.GetAxis("Horizontal");
        if (h < 0)
        {
            leftText.color = Color.red;
            leftText.fontStyle = FontStyle.Bold;
        }
        else
        {
            leftText.color = Color.white;
            leftText.fontStyle = FontStyle.Normal;
        }
        if (Input.GetButton("Jump"))
        {
            upText.color = Color.red;
            upText.fontStyle = FontStyle.Bold;
        }
        else
        {
            upText.color = Color.white;
            upText.fontStyle = FontStyle.Normal;
        }
        if (h > 0)
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

    void FixedUpdate()
    {
        tickCount++;
        //if (tickCount % 5 == 0)
        tick();
    }

    void tick()
    {
        grounded = Physics2D.Linecast(transform.position + (0.25f * Vector3.left), groundCheck.position + (0.25f * Vector3.left), 1 << LayerMask.NameToLayer("Ground"))
    || Physics2D.Linecast(transform.position + (0.25f * Vector3.right), groundCheck.position + (0.25f * Vector3.right), 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButton("Jump") && grounded && velocityY == 0)
        {
            jump = true;
        }
        float h = Input.GetAxis("Horizontal");
        if (h < 0)
            h = -1;
        if (h > 0)
            h = 1;

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

    void gravity()
    {
        if (!grounded && velocityY > -1f)
        {
            velocityY -= gravityForce;
        }
        //if (grounded)
        //    velocityY = 0f;
    }

    void move()
    {
        if (velocityX != 0)
            checkHorizontal(velocityX > 0 ? true : false);
        if (velocityY != 0)
            checkVertical(velocityY > 0 ? true : false);
        checkCollisions();
        transform.position = new Vector3(transform.position.x + velocityX, transform.position.y + velocityY, transform.position.z);
    }

    void checkHorizontal(bool right)
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
            if ( botRight || botLeft )
            {
                if (botRight)
                    velocityY = -botRight.distance;
                if (botLeft)
                    velocityY = -botLeft.distance;
            }
        }
    }


    void Flip()
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
                    stopTick = true;
                    grabCoin(col.gameObject.name);
                    col.gameObject.SetActive(false);
                }
            }

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

    public override void LevelEnd()
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
        RestoreCoins();
        BeginLevel();
    }
}