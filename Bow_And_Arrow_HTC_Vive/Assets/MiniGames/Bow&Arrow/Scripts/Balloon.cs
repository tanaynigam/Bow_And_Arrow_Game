using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FusedVR {
    [RequireComponent(typeof(Rigidbody))]
    public class Balloon : MonoBehaviour {

	    // Use this for initialization
	    void Start () {
		    GetComponent<Rigidbody> ().AddForce (Vector3.up * 100f);
	    }

	    void OnTriggerEnter() {
		    ArrowMiniGame.BowInstance.PopBaloon ();
		    Destroy (this.gameObject);
	    }
    }
}