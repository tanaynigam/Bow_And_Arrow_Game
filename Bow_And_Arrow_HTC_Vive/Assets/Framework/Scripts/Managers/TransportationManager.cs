using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace FusedVR {
    public class TransportationManager : MonoBehaviour {

	    private static TransportationManager tManager; //backing variable for Instance access for the Singleton
	    public static TransportationManager Instance { //Instance access for the Singleton
		    get { return tManager; } // can only get it; we do not want to instantiate if null since the box will be missing
	    } 

        [Tooltip("Animator that handles the box animations")]
	    public Animator Box;
        [Tooltip("The GameObject that handles the scale of the box")]
        public GameObject ScaleBox;

        private const string openStr = "isOpen"; //paramter to open box

	    //TODO: for the box cull both sides to avoid light leaks

	    void Awake(){
            if (tManager == null)
                tManager = this;
            else
                Destroy(this.gameObject);
	    }

	    void OnDestroy(){
		    if (tManager == this)
			    tManager = null;
	    }

	    void Start(){
		    DontDestroyOnLoad (this.gameObject); //makes sure this persists through scenes
            ScaleBox.transform.localScale = new Vector3(
                2f + (PlayerManager.Instance.PlaySpaceWidth - PlayerManager.BASE_MES), 
                2f, 
                2f + (PlayerManager.Instance.PlaySpaceLength - PlayerManager.BASE_MES));
        }

	    public IEnumerator OpenBox(){
		    Box.SetBool (openStr, true); //animates box
		    yield return new WaitForSeconds (Box.GetCurrentAnimatorStateInfo(0).length); //wait for length of animation
	    }

	    public IEnumerator CloseBox(){
		    Box.SetBool (openStr, false); //animates box
		    yield return new WaitForSeconds (Box.GetCurrentAnimatorStateInfo(0).length); //waits for length of animation
	    }

	    //moves box to location of Transform (in general the location of the Mini Game Camera Rig)
	    public void MoveBox(Transform t) {
		    transform.position = t.position;
		    transform.rotation = t.rotation;
		    transform.localScale = t.localScale;
	    }
		


    }
}