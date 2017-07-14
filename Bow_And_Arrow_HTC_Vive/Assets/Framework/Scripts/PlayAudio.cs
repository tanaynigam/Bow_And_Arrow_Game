using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FusedVR {
    public class PlayAudio : MonoBehaviour {

        //TODO: Consider playing with parameters for Oculus Spatializer

        [Tooltip("The Audio Clip we want to play")]
        public AudioClip audioFile;

        public bool playOnAwake = false;

        [Tooltip("Whether or not to loop the audio")]
        public bool loop = false;

        [Tooltip("Indicate whether or not this is audio to be played in the background")]
        public bool isAmbientAudio = false;

        [Range(0f, 1f)]
        [Tooltip("Is this 3D or 2D audio? 0f is full 2D and 1f is full 3D")]
        public float spatializeBlend = 1f;

        [Range(0f, 1f)]
        [Tooltip("The volume percentage for this source")]
        public float volume = 1f;

        [Range(-3f, 3f)]
        [Tooltip("The pitch the audio is played at relative the recording")]
        public float pitch = 1f;

        [Range(0f, 1000000f)]
        [Tooltip("Within this range of the source, no spatialization fall off is applied. Head rotation is used")]
        public float minSpatialRange = 10f;

        [Range(0f, 1000000f)]
        [Tooltip("Between the min and max, spatial audio fall off is applied.")]
        public float maxSpatialRange = 100f;

        private AudioSource mySource;
        public AudioSource audioSource { //public for users to set their own audio values
            get {
                if (mySource == null) {
                    mySource = AudioManager.Instance.GetAudioSource();
                    spatialAudio = mySource.GetComponent<ONSPAudioSource>();
                }
                return mySource;
            } 
        }

        private ONSPAudioSource spatialAudio;

        // Use this for initialization
        void Start () {
            if (isAmbientAudio)
                AudioManager.Instance.SetAmbientSound(audioFile, volume);
            else if (playOnAwake)
                PlayClip();
	    }

        void OnDestroy() {
            if (mySource != null)
                AudioManager.Instance.AudioIsDone(mySource); //return the audio when done

            if (isAmbientAudio)
                AudioManager.Instance.EndAmbientSound();
        }

        void Update() {
            if (mySource != null) {
                mySource.transform.position = transform.position;
                mySource.transform.rotation = transform.rotation;
                mySource.transform.localScale = transform.localScale;
            }
        }
	
        //the coroutine is returned in case the user wants to wait for it
        public Coroutine PlayClip() {
            audioSource.clip = audioFile;
            audioSource.loop = loop;
            audioSource.volume = volume;
            audioSource.spatialBlend = spatializeBlend;
            audioSource.spatialize = spatializeBlend != 0.0f; //whether or not we spatialize the audio
            spatialAudio.Near = minSpatialRange;
            spatialAudio.Far = maxSpatialRange;
            return StartCoroutine(Play());
        }

        public void StopClip() {
            StopAllCoroutines();
            audioSource.Stop();
        }

        IEnumerator Play() {
            audioSource.Play();
            if (!audioSource.loop) {
                while (audioSource.isPlaying)
                    yield return null;
            }
        }

    }
}