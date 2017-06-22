using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    //Input variables
    public string Horizontal = "Horizontal_P1";
    public string Vertical = "Vertical_P1";
    public string Jump = "Jump_P1";
    public string Dash = "Dash_P1";
    public string Shield = "Shield_P1";

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
    private float angle;

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

    //Attack
    public AttackGeneral attack;
    private HitDetection hit;
    private bool isInHitstun;

    //Shielding
    public GameObject shield;
    private bool isShielding;
    private float shieldingDir;
    public float shieldSpeed;
    public float kbModifier;

    //Sprites
    public Sprite runDashBack;

    //Debuging
    public bool permaShield = false;


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
        isShielding = false;
        shield.SetActive(false);

        spriteColor = GetComponent<SpriteRenderer>().color;
        hit = GetComponent<HitDetection>();
        hit.OnStart += startHitstun;
        hit.OnFinish += endHitstun;

    }

	
	// Update is called once per frame
	void Update () {
        moveX = Input.GetAxis(Horizontal);//Mathf.RoundToInt(Input.GetAxis(Horizontal));
        moveY = Input.GetAxis(Vertical);//Mathf.RoundToInt(Input.GetAxis(Vertical));
        dashButt = Input.GetButtonDown(Dash);
        isShielding = Input.GetButton(Shield) || permaShield;
        if (Input.GetButtonDown(Jump))
        {
            jumpButton = true;
        }


        if (isInHitstun)
        {
            if (rb.velocity.x > 0 && facingRight)
            {
                dir = "Right";
                flip();
            }
            else if (rb.velocity.x < 0 && !facingRight)
            {
                dir = "Left";
                flip();
            }
        }
        else
        {
            if (!isShielding)
            {

                if (moveX < 0 && facingRight)
                {
                    dir = "Left";
                    flip();
                }
                else if (moveX >= 0 && !facingRight)
                {
                    dir = "Right";
                    flip();
                }
            }
            else
            {
                if (moveX < 0 && shieldingDir > 0 || moveX > 0 && shieldingDir < 0)
                {
                    anim.SetBool("WalkingBack", true);
                }
                else 
                {
                    anim.SetBool("WalkingBack", false);
                }

            }
        }

        //jumpHeld = Input.GetButton("Jump");
        if (isShielding)
        {
            shield.SetActive(true);
            shieldingDir = transform.localScale.x;            
        }
        else
        {
            shield.SetActive(false);
            anim.SetBool("WalkingBack", false);            
        }
        //Dashing
        if (dashButt && !dashing && !isShielding)
        {
            dashing = true;
            canMove = false;
            dashX = moveX;
            dashY = moveY;
            dashDoneTime = Time.time + dashTime;
            dashCooltime = Time.time + dashTime + dashCool;
            changeColor(true);
            if (facingRight)//Input.GetAxis(Horizontal) >= 0)
            {
                angle = 90 + Mathf.Atan2(-Input.GetAxis(Horizontal), Input.GetAxis(Vertical)) * Mathf.Rad2Deg;
            }
            else
            {
                angle = 90 + Mathf.Atan2(Input.GetAxis(Horizontal), -Input.GetAxis(Vertical)) * Mathf.Rad2Deg;
            }
        }

        if (dashing && Time.time > dashDoneTime) {
			dashing = false;
            canMove = true;
            changeColor(false);
        }

		if (dashing == false && Time.time > dashCooltime) {
            //canMove = true;
            //changeColor(false);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    void FixedUpdate()
    {
        //Get Input

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("Grounded", grounded);
        if (!dashing && canMove)
        {
            attack.deactivateHitbox();
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
                if (!isShielding)
                {
                    rb.velocity = new Vector2(moveX * maxSpeed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(moveX * shieldSpeed, rb.velocity.y);
                }
            }
        }
        else
        {
            if (dashing)
            {
                //Debug.Log("Dash pressed");
                //anim.SetTrigger("Dashed");
                transform.eulerAngles = new Vector3(0, 0, angle);
                rb.velocity =  new Vector2(dashX * dashSpeed, dashY * dashSpeed);
                attack.activateHitbox();
            }
            //else
            //{
              //  rb.velocity = new Vector2(0f, 0f);
            //}
        }

        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        
       
    }

    //Flips the character
    private void flip()
    {
        if (!isShielding)
        {
            facingRight = !facingRight;
            Vector3 temp = transform.localScale;
            temp.x *= -1;
            transform.localScale = temp;
            attack.updateDirection();
        }    
        
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

    private void startHitstun()
    {
        canMove = false;
        isInHitstun = true;
        anim.SetBool("Hitstun", true);
    }

    private void endHitstun()
    {
        canMove = true;
        isInHitstun = false;
        anim.SetBool("Hitstun", false);
    }

}
