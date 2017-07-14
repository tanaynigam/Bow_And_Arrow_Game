using UnityEngine;
using System.Collections;
using Valve.VR;

namespace FusedVR {
    public class PlayerManager : MonoBehaviour {

	    private static PlayerManager pManager; //backing variable for Instance access for the Singleton
	    public static PlayerManager Instance { //Instance access for the Singleton
		    get{
			    if (pManager == null) { // if we access the player manager and it does not exist
				    GameObject managers = GameObject.Find ("Managers"); //generic string name for the manager
				    if (managers == null)
					    managers = new GameObject ("Managers");

				    pManager = managers.AddComponent<PlayerManager> ();
			    }

			    return pManager;
		    }
	    } 

	    private GameObject boxRig; //the main rig that persists in all scenes. This NEVER changes

	    //TODO: if we want to detect the player head we can add in a player collider variable

	    [Tooltip("The CameraRig in charge of the Vive Player")]
	    public GameObject hmdRig;
	    [Tooltip("The Camera that represents the HMD")]
	    public Camera hmdEye; //NOTE: eyes never get reassigned currently
        [Tooltip("Hand Controllers Colliders to interact with Objects inside the box")]
        public GameObject Handcontrollers;

        private SteamVR_TrackedObject leftHand; //variable for the public instances
	    private SteamVR_TrackedObject rightHand;//variable for the public instances

	    public SteamVR_TrackedObject LeftHand { //access to the LeftHand variable in any scene with a Camera Rig
		    get{ 
			    if (leftHand == null) {
				    GameObject go = GameObject.Find ("Controller (left)"); //assumes this is the standard name for the left controller
				    if (go != null)
					    leftHand = go.GetComponent<SteamVR_TrackedObject>();
			    }
			    return leftHand; 
		    }
	    }

	    public SteamVR_TrackedObject RightHand{ //access to the Right variable in any scene with a Camera Rig
		    get{
			    if (rightHand == null) {
				    GameObject go = GameObject.Find ("Controller (right)"); //assumes this is the standard name for the right controller
				    if (go != null)
					    rightHand = go.GetComponent<SteamVR_TrackedObject>();
			    }
			    return rightHand; 
		    }
	    }

        private float playWidth;
        private float playLength;

        public float PlaySpaceWidth { get { if (playWidth == 0) FindPlaySpace(); return playWidth; } }
        public float PlaySpaceLength { get { if (playLength == 0) FindPlaySpace(); return playLength; } }
        public const float BASE_MES = 2f;

        private void FindPlaySpace() {
            SteamVR_PlayArea playSpace = hmdRig.GetComponentInChildren<SteamVR_PlayArea>(false);
            HmdQuad_t playSize = new HmdQuad_t();
            SteamVR_PlayArea.GetBounds(playSpace.size, ref playSize);

            playWidth = Mathf.Abs(playSize.vCorners0.v0 - playSize.vCorners2.v0); //subtract opposite corners
            playLength = Mathf.Abs(playSize.vCorners0.v2 - playSize.vCorners2.v2); //subtract opposite corners
        }

        void Awake() {
		    if (pManager == null)
			    pManager = this;

		    if (hmdRig == null) {
			    hmdRig = GameObject.Find ("[CameraRig]"); //assumes this is the standard name for the HMD
                AssignControllers();
            }

	    }

	    void OnDestroy(){
		    if (pManager == this)
			    pManager = null;
	    }

	    // Use this for initialization
	    void Start () {
		    DontDestroyOnLoad (this.gameObject); //makes sure the manager exists throughout scenes
		    AssignControllers ();
		    boxRig = hmdRig;
	    }

	    //swaps the reference to each hand so that it is apporiate for left / right hand users
	    //TODO: add colliders to detect this action
	    public void SwitchHands(){
		    SteamVR_TrackedObject temp = leftHand;
		    leftHand = rightHand;
		    rightHand = temp;
	    }

	    //TODO: Animate the models to fade in / fade out
	    public void EnterMiniGame(MiniGameManager mGameRig) {
		    hmdRig.SetActive (false);
            Handcontrollers.SetActive(false); //avoid double detection
            hmdRig = mGameRig.MiniGameRig;
		    AssignControllers ();
	    }

	    public void ExitMiniGame(MiniGameManager mGameRig) {
		    mGameRig.MiniGameRig.SetActive (false); //deactivate the Camera for the Mini Game
            
            hmdRig = boxRig; //get main reference back
		    hmdRig.SetActive (true); //activiate main reference
            Handcontrollers.SetActive(true); //reactivate colliders for Activity
            AssignControllers ();

		    //reactiviate controllers so that we can see them
		    leftHand.gameObject.SetActive (true); 
		    rightHand.gameObject.SetActive (true);
	    }

	    //Gets references to latest controllers
	    private void AssignControllers(){
		    leftHand = hmdRig.GetComponent<SteamVR_ControllerManager> ().left.GetComponent<SteamVR_TrackedObject>();
		    rightHand = hmdRig.GetComponent<SteamVR_ControllerManager> ().right.GetComponent<SteamVR_TrackedObject>();
            hmdEye = hmdRig.GetComponentInChildren<SteamVR_Camera>().GetComponent<Camera>();
	    }

	    //controller is a reference to which controller to use haptics on
	    //vibrationCount is how many vibrations
	    //vibrationLength is how long each vibration should go for
	    //gapLength is how long to wait between vibrations
	    //strength is vibration strength from 0-1
	    public IEnumerator ControllerHapticFeedback(SteamVR_TrackedObject controller, int vibrationCount, float vibrationLength, float gapLength, float strength) {
		    strength = Mathf.Clamp01(strength);
		    for(int i = 0; i < vibrationCount; i++) {
			    if(i != 0) yield return new WaitForSeconds(gapLength); //artifical pause
			    for(float time = 0; time < vibrationLength; time += Time.deltaTime) {
				    SteamVR_Controller.Input((int)controller.index).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength)); //provides feedback
				    yield return null;
			    }
		    }
	    }
    }
}