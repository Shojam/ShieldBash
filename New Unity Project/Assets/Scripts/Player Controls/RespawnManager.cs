using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour {

    //Positions
    public Transform[] spawnpoints;
    

	// Use this for initialization
	void Start () {
        //HitDetection.OnKilled += respawn;
            	
	}
	
	// Update is called once per frame
	void Update () {
		
	}    

    private void respawn(Transform player)
    {
        int pos = Random.Range(0, spawnpoints.Length);
        player.position = spawnpoints[pos].position;
    }
}
