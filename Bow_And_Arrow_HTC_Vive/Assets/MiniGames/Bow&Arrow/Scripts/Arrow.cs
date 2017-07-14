using UnityEngine;
using System.Collections;

// Simulates a real Arrow
public class Arrow : MonoBehaviour {
	
	private bool released = false; //whether or not the bow is released from the arrow
	private bool isHit = false; //whether or not the arrow has hit any objects
	private Rigidbody rBody; //the Arrow's rigidbody

	void Start(){
		rBody = GetComponent<Rigidbody> ();
	}
		
	void OnCollisionEnter(Collision other) {
        //TOOD: check that we have NOT hit anything, have been released, and we collided with the terrain and not another arrow
        //then disable physics and start the timer to get rid of this arrow
        //make sure to mark that we hit the terrain
        if(!isHit && released && other.gameObject.GetComponent<Arrow>() == null)
        {
            rBody.velocity = Vector3.zero;
            rBody.isKinematic = true;
            isHit = true;
            GetComponent<Collider>().enabled = false;
            transform.parent = other.transform;
            StartCoroutine(RemoveAfter());
        }
	}

	void Update() {
        //TODO: check if the arrow is released
        //if so, make the the arrow look in the direction of its velocity
	}

    IEnumerator RemoveAfter() {
        yield return new WaitForSeconds(5f); //wait 5 seconds then delete
        Destroy(this.gameObject);
    }

	public void isReleased(Vector3 velocity) {
        //TODO: When the arrow is released enable Physics properties and set the velocity
        //Make sure it also looks at its target
		released = true;
        transform.LookAt(transform.position + velocity);
        rBody.velocity = velocity;
        rBody.useGravity = true;
        rBody.isKinematic = false;
	}
}
