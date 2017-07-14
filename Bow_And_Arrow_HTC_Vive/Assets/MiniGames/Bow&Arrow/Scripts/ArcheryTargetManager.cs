using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FusedVR {
    public class ArcheryTargetManager : MonoBehaviour {

	    public ArcheryTarget[] targets; //list of all targets

	    public void SetTargetMoveSpeed(float speed){
            //TODO: Set the movement speed of each target
            //Hint: Use the Archery Target property
            foreach (ArcheryTarget t in targets)
            {
                t.SetSpeed(speed);
            }
	    }

	    public void PopUpRandomTarget() {
            //TODO: Activiate a Random target for a set amount of time
            float randTime = Random.Range(5f, 8f);
            targets[Random.Range(0, targets.Length)].PopUp(randTime);
	    }

    }
}