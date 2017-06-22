using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour {

    HitstunController hitstun;
    // Event that notifies when hitstun is done
    public delegate void finishedAction();
    public event finishedAction OnFinish;

    // Event that notifies when hitstun is done
    public delegate void startHitstunAction();
    public event startHitstunAction OnStart;
    //Player RigidBody
    private Rigidbody2D rb;
    //Shield Collider
    public BoxCollider2D shield;
    //Attack data
    private Vector2 knockBack;
    //Got hit by a move before
    private bool wasHit;
    //KB modifier
    private float kbModifier = 1;
    public float KBModifier
    {
        get { return kbModifier;}
        set { kbModifier = value; }
    }

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Contains("Attack") && !col.gameObject.CompareTag(this.tag))
        {
            Debug.Log(this.tag);
            AttackGeneral attack = col.GetComponent<AttackGeneral>();
            if (shield.IsTouching(col))
            {
                kbModifier = 0.6f;
            }
            else
            {
                kbModifier = 1f;
            }
            getAttackData(attack);
            if (!wasHit)
            {
                setKnockBack();
                startHitstun();
            }
        }
    }

    private void setKnockBack()
    {
        Debug.Log("Hello?");
        rb.velocity = Vector3.zero;
        rb.AddForce(knockBack);
    }

    private void getAttackData(AttackGeneral attack)
    {
        knockBack = attack.giveKnockBack() * kbModifier;
        wasHit = attack.getAlreadyHit(this.tag);
    }

    public void startHitstun()
    {
        int kbUnits = Mathf.FloorToInt(knockBack.magnitude * 0.04f);
        StartCoroutine("hitstunDuration", kbUnits);
    }

    private IEnumerator hitstunDuration(int duration)
    {
        int frames = 0;
        OnStart();
        while (frames < duration)
        {
            yield return new WaitForEndOfFrame();
            frames++;
            //Debug.Log("Frames: " + frames +" Duration: " + duration);
        }
        OnFinish();
    }
}
