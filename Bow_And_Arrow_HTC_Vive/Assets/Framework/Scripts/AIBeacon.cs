using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FusedVR { 
    public class AIBeacon : MonoBehaviour {

        // A delegate type for hooking up target notifications.
        public delegate void RegisterationAction();
        public static event RegisterationAction Register; //when beacon begins
        public static event RegisterationAction UnRegister; //when beacon begins

        // Use this for initialization
        void Start () {
            if (Register != null)
                Register();
	    }

        void OnDestroy() {
            if (UnRegister != null)
                UnRegister();
        }

    }
}
