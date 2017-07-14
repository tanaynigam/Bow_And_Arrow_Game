using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FusedVR {
    [RequireComponent(typeof(Animator))]
    public class ArcheryTarget : MonoBehaviour {

        // A delegate type for hooking up target notifications.
        public delegate void TargetEvent(ArcheryTarget thisTarget);

        public static event TargetEvent OnTargetUp; //when target goes up
        public static event TargetEvent OnTargetDown; //when target goes up
        public static event TargetEvent OnTargetHit; //when target goes up

        [Tooltip("The location of the ballon spawn from this target")]
	    public Transform balloonSpawn;

	    [Tooltip("The speed the target should swing back and forth")]
	    public float movement = 1f;

	    private Animator targetMotions;

	    private string trigger = "Switch"; //animator parameter

	    private float moveSpeed = 1f;

	    private Vector3 startPosition;

	    void OnCollisionEnter(Collision other) {
            //TODO: Check if the target collided with an Arrow
            //if so, set the animation for target down and call the proper event
            if(other.gameObject.GetComponent<Arrow>() != null)
            {
                StopAllCoroutines();
                targetMotions.SetBool(trigger, false);
                if (OnTargetHit != null)
                    OnTargetHit(this);
            }
	    }

	    void Awake() {
            //get access to your components like the Animator
            targetMotions = GetComponent<Animator>();

	    }

	    void Start() {
            startPosition = transform.position; //sets the center for Sin wave
                                                // targetMotions.SetBool(trigger, true);
	    }

	    void Update() {
            //TODO: Use the sin function to move left and right based on the moveSpeed parameter
            //make sure to use startPosition for the center
            transform.position = startPosition + transform.right * Mathf.Sin(Time.time * moveSpeed);
	    }

	    public void SetSpeed(float speed){
		    moveSpeed = speed;             //set your moveSpeed
        }

	    public void PopUp(float timeUp) {
            StartCoroutine(PopUpForTime(timeUp)); //pop up the target for a set duration
	    }

        IEnumerator PopUpForTime(float duration) {
            //TODO: Fill method using instructions below
            //pop up the target
            //wait for the animation to end - GetCurrentAnimatorStateInfo(0).length
            //call the TargetUp event
            //wait for the duration
            //Set the target down
            //wait for the animation - GetCurrentAnimatorStateInfo(0).length
            //call the TargetDown Event
            //yield return null; //remove this placeholder code

            targetMotions.SetBool(trigger, true);
            yield return new WaitForSeconds(targetMotions.GetCurrentAnimatorStateInfo(0).length);

            if (OnTargetUp != null)
                OnTargetUp(this);

            yield return new WaitForSeconds(duration);

            if (OnTargetDown != null)
                OnTargetDown(this);

            targetMotions.SetBool(trigger, false);
            yield return new WaitForSeconds(targetMotions.GetCurrentAnimatorStateInfo(0).length);

        }
    }
}