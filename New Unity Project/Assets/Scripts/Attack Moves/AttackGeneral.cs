using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackGeneral : MonoBehaviour {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public abstract Vector2 giveKnockBack();
    public abstract void activateHitbox();
    public abstract void deactivateHitbox();
    public abstract float giveDuration();
    public abstract void updateDirection();
    public abstract bool getAlreadyHit(string id);



}
