using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttack : AttackGeneral {

    public float forceX;
    public float forceY;
    public float duration;
    private BoxCollider2D col;
    private ArrayList hits;
    // Use this for initialization
    void Awake () {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
        hits = new ArrayList();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override Vector2 giveKnockBack()
    {
        return new Vector2(forceX*100, forceY*100);
    }

    public override void activateHitbox()
    {
        col.enabled = true;
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
}
