using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float tempgrav;

    //Control
    private Controller control;
    public int controlNum;
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
    private bool outOfDashCoolDown;
    private bool groundDashRefresh;
    private bool canDash;

    //Player variables
    public float maxSpeed;
    public float jumpSpeed;
    public string dir;
    private bool facingRight;
    private bool canJump;
    private bool jumping;
    private bool jumpHeld;
    private Rigidbody2D rb;
    public BoxCollider2D feet;
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

    //Spawn
    public float respawnTime = 1;
    public BoxCollider2D col;
    public float invulTime = 1;
    public Transform[] spawnpoints;



    // Use this for initialization
    void Start () {
        control = ControllerManager.Instance.GetGamepad(controlNum);
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        facingRight = true;
        dir = "Right";
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        dashDoneTime = 0;
        dashing = false;
        canMove = true;
        isShielding = false;
        shield.SetActive(false);

        spriteColor = sprite.color;
        hit = GetComponent<HitDetection>();
        hit.OnStart += startHitstun;
        hit.OnFinish += endHitstun;
        hit.OnKilled += spawnReciever;
    }


    // Update is called once per frame
    void Update() {

        if (!control.IsConnected)
        {
            moveX = Input.GetAxis(Horizontal);//Mathf.RoundToInt(Input.GetAxis(Horizontal));
            moveY = Input.GetAxis(Vertical);//Mathf.RoundToInt(Input.GetAxis(Vertical));
            dashButt = Input.GetButtonDown(Dash);
            isShielding = Input.GetButton(Shield) || permaShield;
            if (Input.GetButtonDown(Jump))
            {
                jumpButton = true;
            }

        }
        else
        {
            moveX = control.GetStick_L().X;//Input.GetAxis(Horizontal);//Mathf.RoundToInt(Input.GetAxis(Horizontal));
            moveY = control.GetStick_L().Y;//Mathf.RoundToInt(Input.GetAxis(Vertical));
            dashButt = control.GetButton("X");
            isShielding = control.GetButton("RB") || permaShield;
        }
        if (control.GetButtonDown("A"))
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
        if (dashButt && canDash && !dashing && !isShielding)
        {
            dashing = true;
            canMove = false;
            canDash = false;
            outOfDashCoolDown = false;
            groundDashRefresh = false;
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
            transform.eulerAngles = new Vector3(0, 0, 0);
            canMove = true;
            changeColor(false);
        }

		if (dashing == false && Time.time > dashCooltime) {
            //canMove = true;
            //changeColor(false);
            outOfDashCoolDown = true;
        }
        canDash = outOfDashCoolDown && groundDashRefresh;
    }

    void FixedUpdate()
    {
        //Get Input

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        if (grounded)
        {
            groundDashRefresh = true;
        }
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
        if (!isShielding && !dashing)
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

    private void spawnReciever()
    {
        //Start couroutine
        StartCoroutine("controlRespawn");
    }

    private IEnumerator controlRespawn()
    {
        int pos = UnityEngine.Random.Range(0, spawnpoints.Length);
        rb.velocity = Vector2.zero;
        this.transform.position = spawnpoints[pos].position;
        //Stop the player
        canMove = false;
        tempgrav = rb.gravityScale;
        rb.gravityScale = 0;
        col.enabled = false;
        sprite.color = Color.yellow;
        yield return new WaitForSeconds(respawnTime);
        rb.gravityScale = tempgrav;
        canMove = true;
        yield return new WaitForSeconds(invulTime);
        sprite.color = spriteColor;
        col.enabled = true;

    }
}
