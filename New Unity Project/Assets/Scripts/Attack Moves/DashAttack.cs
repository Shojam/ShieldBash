using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttack : AttackGeneral {

    public float forceX;
    public float forceY;
    public float duration;
    public bool hitShield;
    private Vector2 force;
    private BoxCollider2D col;
    private ArrayList hits;
    
    // Use this for initialization
    void Awake () {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
        hits = new ArrayList();
	}

    void Start()
    {
        //force = new Vector2(forceX * 100, forceY * 100);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override Vector2 giveKnockBack()
    {
        return (new Vector2(forceX*100, forceY*100));
    }

    public override bool giveHitShield()
    {
        return hitShield;
    }

    public override void activateHitbox()
    {
        col.enabled = true;
        hitShield = false;
    }

    public override void deactivateHitbox()
    {
        col.enabled = false;
        hits.Clear();
    }

    public override float giveDuration()
    {
        return duration;
    }

    public override void updateDirection()
    {
        forceX *= -1;
    }

    public override bool getAlreadyHit(String id)
    {
        if (hits.Contains(id))
        {
            return true;
        }
        else {
            hits.Add(id);
            return false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Contains("Shield") && !col.gameObject.CompareTag(this.tag))
        {
            ControllerManager.Instance.GetGamepad(1).AddRumble(0.5f, new Vector2(50, 50), 10f);
        }
    }
}
