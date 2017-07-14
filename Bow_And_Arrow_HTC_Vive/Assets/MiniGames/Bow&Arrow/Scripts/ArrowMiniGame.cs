using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The MiniGameManager that controls all Game Logic 
// such as difficulty and timing
namespace FusedVR {
    public class ArrowMiniGame : MiniGameManager {

	    public static ArrowMiniGame BowInstance;

	    [Tooltip("Balloon Prefabs to spawn after a target is hit")]
	    public Rigidbody[] baloons;

	    [Tooltip("The Excel Spreadsheet Level Data from QuickSheets. The CastleLevel.asset file goes here.")]
	    public CastleLevel levelData;

        [Tooltip("Manager for all the targets in the Bow&Arrow scene")]
	    public ArcheryTargetManager targets;

        [Tooltip("Instructions on the task you need to complete")]
        public Text instructions;

	    private int numTargetsLefts = 1;
	    private int numBaloonsLeft = 1;

	    // Use this for initialization
	    void Start () {
		    BowInstance = Instance as ArrowMiniGame;
		    if (BowInstance == null)
			    BowInstance = this;

		    //prompt user to shoot targets
		    StartCoroutine(Countdown());
		    SetWave (); //create all the level

            //TODO: Listen to ArcheryTarget Events
            ArcheryTarget.OnTargetHit += OnShotHit;
            ArcheryTarget.OnTargetDown += OnTargetDown;

        }

        void OnDestroy() {
            //TODO: Remove ArcheryTarget Events
            ArcheryTarget.OnTargetHit -= OnShotHit;
            ArcheryTarget.OnTargetDown -= OnTargetDown;
        }

	    IEnumerator Countdown(){
            //Create a Countdown wait timer
            //Call End game if failed
            yield return new WaitForSeconds(15f);
            EndGame(false);
            //yield return null; //remove this line of code
	    }

	    public override void EndGame (bool passedLevel) {
		    StopAllCoroutines (); //ends the countdown
		    base.EndGame (passedLevel);
	    }

	    private void SpawnBaloon(Transform spawnPoint) {
		    GameObject go = Instantiate(baloons[Random.Range(0, baloons.Length)].gameObject, 
			    spawnPoint.position, spawnPoint.rotation) as GameObject;
	    }


	    public void PopBaloon() { //called when a baloon has been popped
		    numBaloonsLeft--;
		    CheckGameOver();
	    }

	    private bool CheckGameOver() {
            //TODO: check number of Targets and number of balloons for the wave
            //if they are both at 0, increase the level and set the next wave. 
            //return true if it is game over, false otherwise. 
            //return false; //Override this
            if(numTargetsLefts == 0 && numBaloonsLeft == 0)
            {
                if (!isEndless)
                {
                    EndGame(true);
                }
                else
                {
                    level++;
                    SetWave();
                    StartCoroutine(Countdown());
                }
                return true;
            }
            return false;
	    }

        private void SetInfo(int numTargets, int numBalloons) { //sets instructions
            instructions.text = "Hit " + numTargets + " targets\n"
                + "Hit " + numBalloons + " balloons";
        }

	    private void SetWave() {
            //TODO: this is the current set of data, but you can customize this how you like
		    if (base.level <= levelData.dataArray.Length) {
			    CastleLevelData data = levelData.dataArray [level-1];
			    numTargetsLefts = data.Targethits;
			    numBaloonsLeft = data.Balloonhits;
			    targets.SetTargetMoveSpeed (data.Targetsmovespeed);
			    targets.PopUpRandomTarget ();
		    } else { //in case we don't have level data
			    numTargetsLefts = base.level; 
			    numBaloonsLeft = base.level;
			    targets.SetTargetMoveSpeed (3f);
			    targets.PopUpRandomTarget ();
		    }
            SetInfo(numTargetsLefts, numBaloonsLeft); //set instructions for this upcoming wave
        }

        private void OnTargetDown(ArcheryTarget target) {
            //TODO: when a target goes down, put another one back up.
            targets.PopUpRandomTarget();
             
        }

        private void OnShotHit(ArcheryTarget target) {
            //TODO: when a target is hit, keep track of the state of the game
            // subtract numTargets
            // check if you want to spawn any balloons

            numTargetsLefts--;
            if (numBaloonsLeft > 0)
                SpawnBaloon(target.balloonSpawn);

            if (numTargetsLefts > 0)
                targets.PopUpRandomTarget();
            CheckGameOver();
        }
    }
}