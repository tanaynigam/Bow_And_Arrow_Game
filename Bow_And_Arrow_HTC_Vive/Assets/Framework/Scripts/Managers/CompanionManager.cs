using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CompanionManager.cs
/// This class handles all the API and events that
/// are associated with your Companion. This includes talking and moving
/// </summary>
namespace FusedVR {
    public class CompanionManager : MonoBehaviour {

        private static CompanionManager aiManager; //backing variable for Instance access for the Singleton
        public static CompanionManager Instance { //Instance access for the Singleton
            get { return aiManager; } // can only get it; we do not want to instantiate if null since the box will be missing
        }

        // A delegate type for hooking up target notifications.
        public delegate void CompanionAction(AIBeacon beacon);
        public static event CompanionAction OnMovementBegin; //when ai start moving
        public static event CompanionAction OnMovementEnd; //when ai start moving

        [Tooltip("The 3D model used for the AI")]
        public GameObject Model;

        [Tooltip("The animations applied to the AI")]
        public Animator stateMachine;

        private const string MOVE_PARAM = "Moving";

        private AudioSource audioOutput;
        public AudioClip introductionAudio;

        void Awake() {
            if (aiManager == null)
                aiManager = this;
            else
                Destroy(this.gameObject);
        }

        void Start() {
            DontDestroyOnLoad(this.gameObject);
            audioOutput = AudioManager.Instance.GetAudioSource(); //audio source for the companion
            CompanionManager.Instance.HideAI();
        }

        void OnDestroy() {
            if (aiManager == this)
                aiManager = null;
        }

        public void PlayIntroductionAudio() {
            //use Audio Output
        }

        public void HideAI() {
            Model.SetActive(false);
        }

        public void ShowAI(AIBeacon location) {
            transform.position = location.transform.position; //insta-port
            transform.rotation = location.transform.rotation; //insta-port
            Model.SetActive(true);
        }

        public void MoveToBeacon(AIBeacon beacon, float speed) {
            StartCoroutine(MoveToBeaconFromCurrLocation(beacon, speed));
        }

        IEnumerator MoveToBeaconFromCurrLocation(AIBeacon beacon, float speed) {
            if (OnMovementBegin != null)
                OnMovementBegin(beacon); //for now

            Vector3 currPosition = transform.position;
            Vector3 destPosition = beacon.transform.position;

            Quaternion currRotation = transform.rotation;
            Quaternion destRotation = beacon.transform.rotation;

            float totalTime = (destPosition - currPosition).magnitude / speed;
            float currTime = 0f;

            stateMachine.SetBool(MOVE_PARAM, true);
            while(currTime < totalTime) {
                transform.position = Vector3.Lerp(currPosition, destPosition, currTime / totalTime);
                transform.rotation = Quaternion.Slerp(currRotation, destRotation, currTime / totalTime);
                currTime += Time.deltaTime;
                yield return null;
            }

            transform.position = destPosition;
            transform.rotation = destRotation;

            stateMachine.SetBool(MOVE_PARAM, false);

            if (OnMovementEnd != null)
                OnMovementEnd(beacon);
        }
    }
}
