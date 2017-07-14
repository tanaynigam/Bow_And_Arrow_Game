using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Handles the bow and all interactions it makes with the Arrow
namespace FusedVR {
    public class BowArrow : InteractableObject {

	    private Vector3 ARROW_CONTROLLER_OFFSET = new Vector3 (0f, 0f, 0.3f);

	    public const float PULL_SCALAR = 5f;
	    public const float MAX_PULL = 4f;

	    [Tooltip("The Arrow's Prefab with all neccesary components attached.")]
	    public GameObject arrowPrefab;

	    [Tooltip("The Gameobject that controls the riggedString and moves the string in the Bow's Skinned Mesh Renderer.")]
	    public GameObject riggedString;

	    [Tooltip("The location of where the string should be at rest. This should be a seperate Gameobject compared to the actual string")]
	    public Transform stringStart;
	    [Tooltip("The location of where the arrow should be at rest relative to the string. This should be a seperate Gameobject nested under the rest string")]
	    public Transform arrowStart;  //the location of where the arrow should be at rest

	    [Tooltip("The Audio Source for the bow pulling sound.")]
	    public PlayAudio bowPullSound;  
	    [Tooltip("The Audio Source for firing the arrow sound.")]
	    public PlayAudio arrowFireSound;  

	    //private variables to control the state of the bow
	    private bool isArrowAttached = false; //detects whether the arrow is attached to the bow
	    private Arrow currArrow; //a reference to the current spawned arrow
	    private float prevDist; //keeps track of the previous distance to compare for audio purposes

	    protected override void OnSelected (SteamVR_TrackedObject bowController){
		    base.OnSelected (bowController); //turns of current hand model
		    SteamVR_TrackedObject offHand = GetOffhand (); //turns off offhand model

		    SteamVR_RenderModel model = offHand.GetComponentInChildren<SteamVR_RenderModel> ();
		    if (model != null) {
			    model.gameObject.SetActive (false);
		    }
	    }

	    protected override void OnDeSelected (SteamVR_TrackedObject bowController){
		    base.OnDeSelected (bowController);

		    //resets
		    isArrowAttached = false;
		    if (currArrow != null)
			    Destroy (currArrow.gameObject);

		    SteamVR_TrackedObject offHand = GetOffhand (); //returns the offhand

		    SteamVR_RenderModel model = offHand.GetComponentInChildren<SteamVR_RenderModel> (true);
		    if (model != null) {
			    model.gameObject.SetActive (true);
		    }
	    }

	    protected override void Update () {
		    base.Update ();

		    if (isSelected) {
			    Transform offHand = GetOffhand ().transform;
                Transform currHand = controllerCollider.GetController().transform;

                if (isArrowAttached) {
				    transform.rotation = Quaternion.LookRotation(currHand.position - offHand.position,
                        -currHand.right) * Quaternion.Euler(0f, -270f, 270f); //enables two handed movement of the bow
			    }
		    }

		    CreateAndUpdateArrow (); //if we are selected, then we will create an arrow to use otherwise update its position
		    PullString (); //calculation of how far to pull the bow
	    }

	    private void PullString() {
            //TODO: check if the arrow is attached
            //if it is, get the distance from the offhand string start position
            //cap the distance to make it feel realistic
            //add some haptic feedback based on the distance pulled. Hint use the PlayerManager
            //riggedString's relative position by the distance from the start position
            //check the offhand trigger to see if it let go of the trigger
            //if so, Fire the Arrow

            if (isArrowAttached)
            {
                SteamVR_TrackedObject offhand = GetOffhand();

                float dist = PULL_SCALAR * (stringStart.position - offhand.transform.position).magnitude;
                if (dist > MAX_PULL)
                    dist = MAX_PULL;

                riggedString.transform.localPosition = (stringStart.localPosition + dist * Vector3.right);

                if (VRTK.VRTK_SDK_Bridge.IsTriggerPressedUpOnIndex((uint)offhand.index))
                    FireArrow();
            }
	    }

	    void OnTriggerStay(Collider other) {
		    if (other.GetComponent<Arrow>() != null) 
			    AttachArrow ();
	    }

	    void OnTriggerEnter(Collider other) {
		    if (other.GetComponent<Arrow>() != null) 
			    AttachArrow ();
	    }

	    private void AttachArrow() {
            //TODO: If the arrow is NOT attached and the Trigger on the offhand is touched down, attach the arrow
            SteamVR_TrackedObject offhand = GetOffhand();
            if (!isArrowAttached && VRTK.VRTK_SDK_Bridge.IsTriggerTouchedUpOnIndex((uint) offhand.index))
            {
                AttachArrowToString();
            }
        }

	    private void FireArrow() {
            //TODO: get the distance of the offhand and the startString
            //Based on the distance, calculate a velocity and let the Arrow know that it has been released
            //Make sure to unparent it
            //Play an arrow released sound
            //reset the string position to the start.
            SteamVR_TrackedObject offhand = GetOffhand();
            float dist = (stringStart.position - offhand.transform.position).magnitude;
            currArrow.isReleased(currArrow.transform.forward * 50f * dist);
            currArrow.transform.parent = null;
            currArrow = null;
            isArrowAttached = false;

            riggedString.transform.localPosition = stringStart.localPosition;
        }

	    private void AttachArrowToString(){
            //TODO: Attach the string to the riggedString
            //Mark that an arrow is attached
            currArrow.transform.parent = riggedString.transform;
            currArrow.transform.position = arrowStart.position;
            currArrow.transform.rotation = arrowStart.rotation;
            currArrow.transform.localScale = arrowStart.localScale;
            isArrowAttached = true;
	    }

	    private void CreateAndUpdateArrow() {
            if (isSelected) {
                if (currArrow == null) {
                    currArrow = (Instantiate(arrowPrefab) as GameObject).GetComponent<Arrow>();
                }

                SteamVR_TrackedObject offHand = GetOffhand();
                if (offHand != null && !isArrowAttached) {
                    currArrow.transform.position = offHand.transform.position + (offHand.transform.rotation * ARROW_CONTROLLER_OFFSET * offHand.transform.localScale.z);
                    currArrow.transform.rotation = offHand.transform.rotation;
                }
            }
        }

	    private SteamVR_TrackedObject GetOffhand() {
            //TODO: check if the current hand (controllerCollider) is the Left or Right
            //this check can be made against the PlayerManager
            if (controllerCollider.GetController() == PlayerManager.Instance.LeftHand)
                return PlayerManager.Instance.RightHand;
            return PlayerManager.Instance.RightHand; //remove this line when done
	    }
    }
}
