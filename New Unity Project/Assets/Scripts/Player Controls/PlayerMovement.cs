using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    //Input variables
    public string Horizontal = "Horizontal_P1";
    public string Vertical = "Vertical_P1";
    public string Jump = "Jump_P1";
    public string Dash = "Dash_P1";

    //Input values
    private float moveX;
    private float moveY;
    private bool jumpButton;
    private bool dashButt;

    //Dashing
    private bool dashing;
    public float dashCool;
    private float dashCooltime;
    private bool canMove;
    private float dashX;
    private float dashY;
    public float dashSpeed;
    public float dashTime;
    private float dashDoneTime;

    //Player variables
    public float maxSpeed;
    public float jumpSpeed;
    public string dir;
    private bool facingRight;
    private bool canJump;
    private bool jumping;
    private bool jumpHeld;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    Animator anim;
    bool grounded = false;
    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;

    private Color spriteColor;


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        facingRight = true;
        dir = "Right";
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        dashDoneTime = 0;
        dashing = false;
        canMove = true;

        spriteColor = GetComponent<SpriteRenderer>().color;
    }

	
	// Update is called once per frame
	void Update () {
        moveX = Mathf.RoundToInt(Input.GetAxis(Horizontal));
        moveY = Mathf.RoundToInt(Input.GetAxis(Vertical));
        dashButt = Input.GetButtonDown(Dash);
        if (Input.GetButtonDown(Jump))
        {
            jumpButton = true;
        }

        if (moveX < 0 && facingRight)
        {
            dir = "Left";
            flip();
        }
        else if (moveX > 0 && !facingRight)
        {
            dir = "Right";
            flip();
        }

        //jumpHeld = Input.GetButton("Jump");

        //Dashing
        if (dashButt && !dashing)
        {
            dashing = true;
            canMove = false;
            dashX = moveX;
            dashY = moveY;
            dashDoneTime = Time.time + dashTime;
            dashCooltime = Time.time + dashTime + dashCool;
            changeColor(true);
        }

        if (dashing && Time.time > dashDoneTime) {
			dashing = false;
            canMove = true;
            changeColor(false);
        }

		if (dashing == false && Time.time > dashCooltime) {
			//canMove = true;
            //changeColor(false);
        }
    }

    void FixedUpdate()
    {
        //Get Input

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("Grounded", grounded);
        if (!dashing && canMove)
        {
            if (jumpButton && grounded)
            {
                jumping = true;
                anim.SetTrigger("Jump");
                jumpButton = false;
                rb.velocity = new Vector2(moveX * maxSpeed, 0);
                rb.AddForce(new Vector2(0, jumpSpeed * 100));
            }
            else
            {
                rb.velocity = new Vector2(moveX * maxSpeed, rb.velocity.y);
            }
        }
        else
        {
            if (dashing)
            {
                //Debug.Log("Dash pressed");
                //anim.SetTrigger("Dashed");
                rb.velocity = new Vector2(dashX * dashSpeed, dashY * dashSpeed);
            }
            else
            {
                rb.velocity = new Vector2(0f, 0f);
            }
        }
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    //Flips the character
    private void flip()
    {
        facingRight = !facingRight;
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }

    //Changes the color of the character
    void changeColor(bool change)
    {
        /*if (change)
        {
            //Place on invencible layer
            gameObject.layer = 10;
            sprite.color = Color.grey;
        }
        else
        {
            //Place on regular layer
            gameObject.layer = 8;
            sprite.color = spriteColor;
        }*/
        anim.SetBool("Dashing", change);
    }

}
