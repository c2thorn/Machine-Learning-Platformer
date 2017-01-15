using UnityEngine;
using System.Collections;

public class SimplePlatformController : MonoBehaviour
{

    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool jump = false;
    public float moveForce = 365f;
    public float maxSpeed = 5f;
    public float jumpForce = 1000f;
    public Transform groundCheck;


    private bool grounded = false;
    private Animator anim;
    private Rigidbody2D rb2d;

    public GUIText leftText;
    public GUIText upText;
    public GUIText rightText;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButton("Jump") && grounded && rb2d.velocity.y == 0)
        {
            jump = true;
        }
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
        float h = Input.GetAxis("Horizontal");
        if (h < 0)
            h = -1;
        if (h > 0)
            h = 1;
        anim.SetFloat("Speed", Mathf.Abs(h));


        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 topRight = new Vector2(rb2d.position.x + box.size.x / 2 + .25f, rb2d.position.y + 0.6f);
        Vector2 topLeft = new Vector2(rb2d.position.x + -box.size.x / 2 - .25f, rb2d.position.y + 0.6f);

        Vector2 bottomRight = new Vector2(rb2d.position.x + box.size.x / 2 + .25f, rb2d.position.y - 0.6f);
        Vector2 bottomLeft = new Vector2(rb2d.position.x + -box.size.x / 2 - .25f, rb2d.position.y - 0.6f);
        //Debug.DrawLine((Vector3)boxVectorStartLeft, (Vector3)boxVectorStartLeft - (.1f)*Vector3.right);
        //for (int i = 0; i < 15; i++)
        //{
        //  Debug.Log(i + " " + LayerMask.LayerToName(i));
        //}

        if (Physics2D.Raycast(topRight, Vector2.right, 0.01f, 1 << LayerMask.NameToLayer("Ground")) && Physics2D.Raycast(bottomRight, Vector2.right, 0.01f, 1 << LayerMask.NameToLayer("Ground")) && h > 0)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            //Debug.Log("There's a thing right");
        }
        else if (Physics2D.Raycast(topLeft, Vector2.left, .01f, 1 << LayerMask.NameToLayer("Ground")) && Physics2D.Raycast(bottomLeft, Vector2.right, 0.01f, 1 << LayerMask.NameToLayer("Ground")) && h < 0)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            //Debug.Log("There's a thing left");
        }
        else if (h != 0)
        {
            if (h * rb2d.velocity.x < maxSpeed)
                rb2d.AddForce(Vector2.right * h * moveForce);

            if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
                rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
            //rb2d.velocity = new Vector2(h * maxSpeed, rb2d.velocity.y);
        }
        else
        {
            // if (h * rb2d.velocity.x < maxSpeed)
            //   rb2d.AddForce(Vector2.right * h * moveForce);

            //if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            //rb2d.velocity = new Vector2(0, rb2d.velocity.y);
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



    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}