using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// RoomManager.cs
/// This class handles all logic related to keeping the user occupied during a transition
/// This includes things such an activites in between, AI talking with the user, etc.
/// </summary>
namespace FusedVR {
    public class RoomManager : MonoBehaviour {

        //this will be in charge of any and all mini games / activities that happen in the Room area
        //this includes tutorial, handling items, etc. 

        [Tooltip("Table for the player to play on")]
        public GameObject PlayTable;

        [Tooltip("The location for the AI to float to")]
        public AIBeacon AIPerch;

        [Tooltip("Activites that the player can do while waiting for the game to load")]
        public Activity activityPrefab;

        [Tooltip("Instruction text for the user")]
        public Text instructions;

        [Tooltip("The text to display what wave we are currently on")]
        public Text waveText;
        [Tooltip("The frames that store how many lives are left")]
        public Image[] strikeFrames;
        [Tooltip("The strikes to apply to the frames.")]
        public Image strikePrefab;

        private int numStrikes = 0;
        private Image[] strikes;

        private static RoomManager bManager; //backing variable for Instance access for the Singleton
	    public static RoomManager Instance { //Instance access for the Singleton
		    get { return bManager; } // can only get it; we do not want to instantiate if null since the box will be missing
	    } 

        private bool activityOver = false;
        private bool aiMoving = false;

	    void Awake() {
		    if (bManager == null)
			    bManager = this;

            CreateStrikes();
        }

        void Start() {
            TurnOnOffUI(false);
        }

	    void OnDestroy() {
		    if (bManager == this)
			    bManager = null;
	    }

	    public IEnumerator Activity(bool isTutorial, int tutorialStage, int level, bool succeededLevel) {
            /*
		    if (isTutorial) {
			    switch (tutorialStage) {
			    case 0: LobbyWelcome (); break;
			    case 1: ArcheryWelcome (); break;
			    case 2: FruitNinjaWelcome (); break;
			    case 3: XortexWelcome (); break;
			    case 4: MedusaWelcome(); break;
			    }
		    }
            */

            yield return new WaitForSeconds(.5f); 

            Activity a = (Instantiate(activityPrefab.gameObject, PlayTable.transform) as GameObject).GetComponent<Activity>();
            a.transform.localPosition = activityPrefab.transform.position;
            a.transform.localRotation = activityPrefab.transform.rotation;
            a.transform.localScale = activityPrefab.transform.localScale;
            a.StartActivity(level);

            while (!activityOver)
                yield return null;

            activityOver = false;
            Destroy(a.gameObject);

		    instructions.text = "";

		    yield return null;
	    }

        public void ActivityOver(bool success) {
            //TODO: use success and destroy activity
            activityOver = true;
        }

	    private void LobbyWelcome(){
		    instructions.text = "";
	    }

	    private void ArcheryWelcome(){
		    instructions.text = "Welcome. You are about to \n enter an archery game. Grab and fire!";
	    }

	    private void FruitNinjaWelcome(){
		    instructions.text = "Great job! Now time to \n slice and dice like a ninja!";
	    }

	    private void XortexWelcome(){
		    instructions.text = "Want to go to outer space? \n Let's hitch a ride but watch out for asteroid!";
	    }

	    private void MedusaWelcome(){
		    instructions.text = "Woot! Time for a challenge. \n Avoid the eyes of the deadly Medusa.";
	    }

        //whether or not to Turn On or Off UI in the Box. Useful for the beginning of the game
        public void TurnOnOffUI(bool turnOn) {
            waveText.gameObject.SetActive(turnOn);
            foreach (Image image in strikeFrames)
                image.gameObject.SetActive(turnOn);
        }

        //creates a strike to convey to the user
        private void CreateStrikes() {
            if (strikes == null)
                strikes = new Image[strikeFrames.Length]; //initally the number of strikes as the number of frames to hold them

            for (int i = 0; i < strikeFrames.Length; i++) { 
                Image strike = Instantiate(strikePrefab); //create a strike prefab
                strike.gameObject.SetActive(false);
                Vector3 pos = strike.transform.position;
                Quaternion rot = strike.transform.rotation;
                Vector3 scale = strike.transform.localScale;
                strike.transform.parent = strikeFrames[i].transform;

                strike.transform.localPosition = pos;
                strike.transform.localRotation = rot;
                strike.transform.localScale = scale;

                strikes[i] = strike;
            }
        }

        //sets text for wave number
        public void SetWaveNumberText(int waveNo) {
            waveText.text = "Wave " + waveNo;
        }

        //marks a strike on the wall
        public void MarkStrike() {
            if (strikes != null && numStrikes < strikes.Length) {
                strikes[numStrikes].gameObject.SetActive(true);
                numStrikes++;
            }
        }

        public int NumberOfLivesLeft() {
            return strikeFrames.Length - numStrikes;
        }

        public void ClearStrikes() {
            foreach (Image image in strikes) {
                image.gameObject.SetActive(false);
            }

            numStrikes = 0;
        }

        //Moves AI to its perch which is relative to the box location
        public IEnumerator ReturnAIHome() {
            CompanionManager.Instance.MoveToBeacon(AIPerch, 3f);
            aiMoving = true;
            while (aiMoving)
                yield return null;
        }

        //called when AI stops moving
        void EndAIMovement(AIBeacon beacon) {
            aiMoving = false;
        }
    }
}
