using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitstunController : MonoBehaviour {

    // Event that notifies when hitstun is done
    public delegate void finishedAction();
    public static event finishedAction OnFinish;

    // Event that notifies when hitstun is done
    public delegate void startHitstunAction();
    public static event startHitstunAction OnStart;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void startHitstun(Vector2 kb)
    {
        int kbUnits = Mathf.FloorToInt(kb.magnitude * 0.4f);
        StartCoroutine("hitstunDuration", kbUnits);
    }

    private IEnumerator hitstunDuration(int duration)
    {
        int frames = 0;
        while (frames < duration)
        {
            yield return new WaitForEndOfFrame();
            frames++;
        }
        //OnFinish();
    }


    
}
