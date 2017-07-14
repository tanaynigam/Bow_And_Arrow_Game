using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioManager.cs
/// This class handles all logic related to any audio in the game.
/// As a simple optimization, we spawn 10 audio sources and recycle those objects across scenes
/// By interacting with this manager, a mini-game will gain access to fading in / out audio between scenes (TO BE IMPLEMENTED)
/// and also global access to turn off all ambient music or sound effects. 
/// </summary>
namespace FusedVR {
    public class AudioManager : MonoBehaviour {

        [Tooltip("The number of Audio Sources to have in our pool. Set in the inspector and this will be the value we try to maintain.")]
        public int numPoolObjects = 10;

        private static AudioManager audioManager; //backing variable for Instance access for the Singleton
        public static AudioManager Instance { //Instance access for the Singleton
            get {
                if (audioManager == null) { // if we access the player manager and it does not exist
                    GameObject managers = GameObject.Find("Managers"); //generic string name for the manager
                    if (managers == null)
                        managers = new GameObject("Managers");

                    audioManager = managers.AddComponent<AudioManager>();
                    audioManager.CreateAudioSources();
                }

                return audioManager;
           } // can only get it; we do not want to instantiate if null since the box will be missing
        }

        private List<AudioSource> audioSources = new List<AudioSource>(); //the list of all audio sources we have created as child objects

        private int AMBIENT_SOUND_ID = 0; //the id in the array where the ambient sound is located

        void Awake() {
            if (audioManager == null)
                audioManager = this;
            else
                Destroy(this.gameObject);
        }

        void OnDestroy() {
            if (audioManager == this)
                audioManager = null;
        }

        // Use this for initialization
        void Start () {
            DontDestroyOnLoad(this.gameObject);
            if (audioSources == null) // there is a possibility someone already requested for audio before this existed, important for standalone mini-game testing
                CreateAudioSources();
        }

        public void SetAmbientSound(AudioClip newSound, float volume) {
            //TODO: add any fades, etc.
            AudioSource source = audioSources[AMBIENT_SOUND_ID]; 
            source.clip = newSound;
            source.volume = volume;
            source.spatialBlend = 0f; //ambient is always in the background i.e. 2D
            source.loop = true; //ambient must always loop
            source.Play();
        }

        public void EndAmbientSound() {
            //TODO: add any fades, etc.
            AudioSource source = audioSources[AMBIENT_SOUND_ID];
            source.volume = 0f;
            source.Stop();
        }

        public AudioSource GetAudioSource() {
            foreach(AudioSource a in audioSources) { //loop to get any inactive audio sources
                if (!a.gameObject.activeInHierarchy) {
                    a.gameObject.SetActive(true);
                    return a;
                }
            }

            //if we get here then we don't have an extra audio source
            AudioSource extra = CreateAudioSource("Extra Source");
            extra.gameObject.SetActive(true);
            return extra;
        }

        //method is called from the outside when a mini-game is done with its Audio source
        public void AudioIsDone(AudioSource source) {
            //Audio Manger is ALWAYS LOCATED AT ORIGIN
            source.transform.position = Vector3.zero;
            source.transform.rotation = Quaternion.identity;
            source.transform.localScale = Vector3.one;
            source.transform.parent = transform;

            source.volume = 0f;
            source.spatialBlend = 1f;
            source.gameObject.SetActive(false);
        }

        //creates all inital audio sources
        private void CreateAudioSources() {
            audioSources = new List<AudioSource>();

            for (int i = 0; i < numPoolObjects; i++) {
                CreateAudioSource("AudioSource " + i);
            }

            audioSources[AMBIENT_SOUND_ID].gameObject.SetActive(true); //the first one is always active so that it can never get pulled for other use
        }

        //creates an individual audio source with the given GameObject name
        private AudioSource CreateAudioSource(string name) {
            GameObject go = new GameObject(name);
            go.transform.parent = transform;
            go.SetActive(false);
            go.AddComponent<ONSPAudioSource>(); //add spatial audio
            AudioSource a = go.AddComponent<AudioSource>();
            audioSources.Add(a);
            return a;
        }

    }
}
