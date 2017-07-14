using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class controls the logic for starting the game
namespace FusedVR {
    public class BeginGameOrb : InteractableObject {

	    private bool firstTime = false;

	    protected override void Update(){
		    base.Update ();
		    if (isSelected && !firstTime) {
			    firstTime = true;
                GameManager.Instance.LeaveVoid();
                ForceDropObject(); //force drop 
                Destroy(this.gameObject);
		    }
	    }

	    protected override void MoveToController(){
		    //DO NOTHING
	    }

    }
}
