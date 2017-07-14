using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ControllerCollider.cs
/// This class handles all logic related to whether the controllers have hit any Interactable Objects
/// It then calls the appropriate methods on the InteractableObjects, which then handle whether they 
/// are selected or not.
/// </summary>

namespace FusedVR {
    [RequireComponent(typeof(Rigidbody))] // needed for Trigger events
    [RequireComponent(typeof(Collider))] //needed for being a trigger
    public class ControllerCollider : MonoBehaviour {
	    [Tooltip("Indicates whether this is the left or right hand controller")]
	    public bool isLeftController; 

	    private Collider controllerCollision;
        private SteamVR_Controller.Device device;

	    void Start() {
		    controllerCollision = GetComponent<Collider> ();
        }

	    // Update is called once per frame
	    void Update () {
		    SteamVR_TrackedObject controller = GetController ();
		    if (controller != null) {
			    transform.position = controller.transform.position;
			    transform.rotation = controller.transform.rotation;
			    transform.localScale = controller.transform.lossyScale; //sets it to the CameraRig's scale
		    }
	    }

	    void OnTriggerEnter(Collider other){
		    InteractableObject obj = other.GetComponent<InteractableObject> ();
		    if (obj != null) {
			    obj.ControllerTouched (this); // lets the object know the controller is touching it
		    }
	    }

	    void OnTriggerExit(Collider other){
		    InteractableObject obj = other.GetComponent<InteractableObject> ();
		    if (obj != null) {
			    obj.ControllerDeTouched (this); // lets the object know the controller has stopped touching it
		    }
	    }

	    public void EnableCollisions(){
		    controllerCollision.enabled = true;
	    }

	    public void DisableCollisions(){
		    controllerCollision.enabled = false;
	    }

	    public SteamVR_TrackedObject GetController() {
		    if (isLeftController)
			    return PlayerManager.Instance.LeftHand;

		    return  PlayerManager.Instance.RightHand;
	    }
    }
}