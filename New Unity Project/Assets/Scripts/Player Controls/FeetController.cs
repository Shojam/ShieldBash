using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetController : MonoBehaviour {

    public int playerIndex;
    private Controller control;
    public string Vertical = "Vertical_P1";
    private BoxCollider2D feet;
    // Use this for initialization
    void Start () {
        control = ControllerManager.Instance.GetGamepad(playerIndex);
        feet = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag.Contains("DropThroughPlat"))
        {
            Debug.Log("Heyo");
            dropThroughPlat();
        }
    }

    private void dropThroughPlat()
    {
        bool drop = false;
        if (control.IsConnected)
        {
            drop = control.GetStick_L().Y <= -0.9;
        }
        else
        {
            drop = Input.GetAxis(Vertical) <= -0.9; 
        }
        if (drop)
        {
            StartCoroutine("cycleFeetHitbox");
        }
    }

    private IEnumerator cycleFeetHitbox()
    {
        feet.enabled = false;
        yield return new WaitForSeconds(0.09f);
        feet.enabled = true;
    }
}
