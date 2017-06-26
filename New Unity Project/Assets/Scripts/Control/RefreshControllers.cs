using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshControllers : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ControllerManager.Instance.Refresh();
	}
}
