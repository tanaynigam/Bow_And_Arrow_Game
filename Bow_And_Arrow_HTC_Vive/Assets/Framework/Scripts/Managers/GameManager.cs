using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GameManager.cs
/// This class handles all logic related to the state of the game
/// It is responsible for the interface between the MiniGameManagers and the TransportationManager,
/// which is handled through Scene Transitions.
/// </summary>
namespace FusedVR {
    public class GameManager : MonoBehaviour {

	    private static GameManager gManager; //backing variable for Instance access for the Singleton
	    public static GameManager Instance { //Instance access for the Singleton
		    get { return gManager; } // can only get it; we do not want to instantiate if null since the box will be missing
	    }

        [Tooltip("The Excel Spreadsheet Level Data from QuickSheets. The WaveLevel.asset file goes here.")]
        public WaveLevels levelData;

        [Tooltip("The build number for the game room, which acts as a hub for the player")]
        public int GameRoomBuildNo = 1;

        private int waveNumber = 0; //the current wave 
        private List<int> nextPossibleLevels = new List<int>(); //set of next levels based on spreadsheet
	    private bool switchScene = false; //keeps track of state on when to switch to the next scene
        private int currScene = 0; //the current scene ID; SceneManager is a little buggy
        private List<WaveLevelsData>[] levelsInWave; //a mapping of which levels are in a given wave

	    void Awake(){
            if (gManager == null)
                gManager = this;
            else
                Destroy(this.gameObject);

            MiniGameManager.OnBeginGame += BeginMiniGame; //register to get events from minigames
            MiniGameManager.OnEndGame += EndMiniGame;
        }

	    void OnDestroy(){
		    if (gManager == this)
			    gManager = null;

            MiniGameManager.OnBeginGame -= BeginMiniGame; //de-register from minigames for completeness
            MiniGameManager.OnEndGame -= EndMiniGame;
        }

        void Start() {
            DontDestroyOnLoad(this.gameObject); //must persist across all levels
            SetupWaveData();
            StartCoroutine(SwitchScene(false)); // we load in to the lobby
        }

        //A Special Case Method to leave the very first scene 
        public void LeaveVoid() {
            if (currScene == 0) // if in void scene, which should ALWAYS be 0
                switchScene = true;
        }

        //A Special Case Method to leave the Lobby and Start the Parade of Mini Games
        public void StartMiniGames(LobbyManager manager){
            if (currScene == GameRoomBuildNo) { // if in Game Room scene
                RoomManager.Instance.ClearStrikes();
                RoomManager.Instance.SetWaveNumberText(1);
                RoomManager.Instance.TurnOnOffUI(true);
                EndMiniGame(manager, true); //LobbyManager is a MINI-GAME O_O
            }
        }

        //Reads Excel spreadsheet with the Wave Data and which waves are played
        private void SetupWaveData() {
            int maxWave = levelData.dataArray[levelData.dataArray.Length - 1].Wave; //assumes that the last entry has the largest wave number
            levelsInWave = new List<WaveLevelsData>[maxWave];
            foreach(WaveLevelsData data in levelData.dataArray) {
                if (levelsInWave[data.Wave - 1] == null)
                    levelsInWave[data.Wave - 1] = new List<WaveLevelsData>();
                levelsInWave[data.Wave - 1].Add(data); //assumes that wave data that is entered is 1 based
            }
        }

        #if UNITY_EDITOR
        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) //only for TESTING!!!!!
                LeaveVoid();
        }
        #endif

        //create a SteamVR_LoadLevel script that we can use to seamlessly transition between level
        private SteamVR_LoadLevel LoadSteamVRLevel(int sceneID) {
            SteamVR_LoadLevel loader = new GameObject("loader").AddComponent<SteamVR_LoadLevel>(); //TODO: Add loading window and skybox
            //loader.levelID = sceneID;
            loader.fadeInTime = 0.5f;
            loader.fadeOutTime = 0.5f;
            loader.loadAsync = true;
            //loader.switchScenes = false; // only used for async
            loader.backgroundColor = Color.white; //in the future add our own skybox or alternatively capture the skybox with a 360 cam
            loader.Trigger();

            return loader;
        }

        //Co-routine used to load the next mini-game or level
        IEnumerator LoadNextGame(int sceneID) {
            SteamVR_LoadLevel level = LoadSteamVRLevel(0); //load empty scene which is scene 0 AKA the start scene. NOTE: THAT THIS SCENE IS NOT EMPTY but everything in it will delete itself
            //yield return StartCoroutine( level.ActivateAsyncScene() );

            if (sceneID != GameRoomBuildNo)
                nextPossibleLevels.Remove(sceneID); //removes from list of possible levels

            while (SteamVR_LoadLevel.loading) //don't load if we are still loading another level
                yield return null;

            level = LoadSteamVRLevel(sceneID); //start loading the scene we want now

            while (!switchScene) { // waits until the scene is ready and it is time to switch           
                yield return null;
            }

            //yield return StartCoroutine(level.ActivateAsyncScene()); //wait for scene to load
            currScene = sceneID;

            yield return new WaitForSeconds (.1f); //let everything initialize

            switchScene = false;
        }

        //Called by the MiniGameManager to start the game via the Begin Game event
	    public void BeginMiniGame(MiniGameManager mGame){
		    //starts the game and does neccesary animations i.e. poofs or whatever
		    mGame.isEndless = false;
            mGame.isTutorial = true; //TODO: FOR TESTING THIS IS JUST TO SEE HOW A TUTORIAL WOULD FEEL
		    mGame.level = waveNumber; //TODO: adjust if this is wave based or num levels played

		    TransportationManager.Instance.MoveBox (mGame.MiniGameRig.transform);
            CompanionManager.Instance.ShowAI(RoomManager.Instance.AIPerch);
            PlayerManager.Instance.EnterMiniGame (mGame);
		    StartCoroutine (TransportationManager.Instance.OpenBox () );
	    }

        //Called by the MiniGameManager to end the game via the End Game event
        public void EndMiniGame(MiniGameManager mGame, bool passedLevel){
		    //ends the game and does neccesary animations i.e. poofs or whatever
		    TransportationManager.Instance.MoveBox (mGame.MiniGameRig.transform);
		    PlayerManager.Instance.ExitMiniGame (mGame);

            // just switches but we can do it so that there is a mini game that happens in the box
            // or dropping or picking up desired items
            StartCoroutine( SwitchScene (passedLevel) );
	    }
		
	    //this method gets called to alert us to switch when ready 
	    private IEnumerator SwitchScene(bool passedLevel) {
            if (!passedLevel)
                RoomManager.Instance.MarkStrike();

            bool gameOver = RoomManager.Instance.NumberOfLivesLeft() <= 0;

            if (currScene != 0 && !gameOver) {
                StartCoroutine(LoadNextGame(NextMiniGameScene())); //TODO: let the book that was choosen dictate which scene to load
            } else {
                ResetToLobby();
            }

            yield return StartCoroutine (TransportationManager.Instance.CloseBox ());
            StartCoroutine(RoomManager.Instance.ReturnAIHome()); //moves after box is closed. A bit hacky but sorta works for now

            //let's any activity we want happen in the room based on what just happened
            //TODO: get whether or not we passed or failed
            if (currScene != 0) { //not void scene 
                if (!gameOver) { 
                    yield return StartCoroutine(RoomManager.Instance.Activity(true, currScene, 0, passedLevel));
                }
                switchScene = true; // onto the next one
            }
        }

        //When the Games are over and we need to Reset to the lobby level
        private void ResetToLobby() {
            waveNumber = 0;
            nextPossibleLevels.Clear();
            StartCoroutine(LoadNextGame(GameRoomBuildNo)); //loads game room
        }

        //read data from the spreadsheet and get the next wave
        private void SetNextWave() {
            if (nextPossibleLevels.Count == 0) {
                waveNumber++;
                RoomManager.Instance.SetWaveNumberText(waveNumber);

                foreach (WaveLevelsData data in levelsInWave[(waveNumber - 1) % levelsInWave.Length]) {
                    nextPossibleLevels.Add(data.Buildno);
                }
            }
        }

        //get the next level
	    private int NextMiniGameScene(){
            SetNextWave();
		    return nextPossibleLevels[Random.Range(0, nextPossibleLevels.Count)];
	    }

    }
}