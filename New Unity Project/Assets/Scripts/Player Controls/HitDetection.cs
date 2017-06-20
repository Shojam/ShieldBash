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
    //Player collider
    private Rigidbody2D rb;
    //Attack data
    private Vector2 knockBack;
    //Got hit by a move before
    private bool wasHit;

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
        knockBack = attack.giveKnockBack();
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
