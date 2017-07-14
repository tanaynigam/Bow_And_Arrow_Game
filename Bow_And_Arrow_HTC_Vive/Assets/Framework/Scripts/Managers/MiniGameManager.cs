using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MiniGameManager.cs
/// This is the class to inherit from for any MiniGame
/// It will handle most interactions with the Game Manager
/// </summary>
namespace FusedVR {
    public class MiniGameManager : MonoBehaviour {

        // A delegate type for hooking up target notifications.
        public delegate void MiniGameBeginAction(MiniGameManager manager);
        public static event MiniGameBeginAction OnBeginGame; //called when game begins

        // A delegate type for hooking up target notifications.
        public delegate void MiniGameEndAction(MiniGameManager manager, bool passedLevel);
        public static event MiniGameEndAction OnEndGame; //called when the mini game ends


        [HideInInspector]
	    public int level { get; set; } // the level assigned by the Game Manager
	    [HideInInspector]
	    public bool isEndless = true; //whether or not this is the full simulation or the player should play till Game Over
        [HideInInspector]
        public bool isTutorial = false; //whether or not this is a tutorial simulation

        [Tooltip("The Mini Game's CameraRig - MUST BE ASSIGNED in the Editor")]
	    public GameObject MiniGameRig;

	    public static MiniGameManager Instance;

        //initalizes the game but can be overwritten for more custom begin game settings
	    protected virtual void Initialize(){
		    if (Instance == null)
			    Instance = this;

            if (OnBeginGame != null)
                OnBeginGame(this);
	    }

        //called to let the Game Manger the game is over
	    public virtual void EndGame(bool passedLevel){
            if (OnEndGame != null)
                OnEndGame(this, passedLevel);
	    }

	    void Awake(){ // this must remain AWAKE. if it is changed to Start, classes that inherit will overwrite Start
		    level = 1; //always start with 1
            Initialize();
        }

        void OnDestroy(){
		    if (Instance == this)
			    Instance = null;
	    }
		
    }
}