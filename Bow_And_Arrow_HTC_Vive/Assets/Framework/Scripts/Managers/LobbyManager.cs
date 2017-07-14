using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FusedVR {
    public class LobbyManager : MiniGameManager { //we want to inherit the instance structure

        [Tooltip("The points and rotations that the AI uses to enter the room")]
        public AIBeacon[] enterRoom;

        [Tooltip("The points and rotations that the AI uses to move around the room")]
        public AIBeacon[] randomMovement;

        private bool isAIMoving = false;

        void Start() {
            CompanionManager.OnMovementEnd += EndAIMovement;
            StartCoroutine(EnterRoom());
        }

        void OnDestroy() {
            CompanionManager.OnMovementEnd -= EndAIMovement;
        }

        //gets called when book is placed
        public void StartGame() {
            GameManager.Instance.StartMiniGames(this);
	    }

        IEnumerator EnterRoom() {
            CompanionManager.Instance.ShowAI(enterRoom[0]);
            foreach(AIBeacon beacon in enterRoom) {
                isAIMoving = true;
                CompanionManager.Instance.MoveToBeacon(beacon, 2f); // speed in m/s
                while (isAIMoving)
                    yield return null;
            }

            if (isTutorial)
                CompanionManager.Instance.PlayIntroductionAudio();
            //else do something for non-tutorial
        }

        void EndAIMovement(AIBeacon beacon) {
            isAIMoving = false;
        }
    }
}
