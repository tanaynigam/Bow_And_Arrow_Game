using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FusedVR {
    public abstract class Activity : MonoBehaviour {

        /// <summary>
        /// StartActivity is a method that gets called by the RoomManager as an activity to do while loading the next game.
        /// We pass the wave number to set difficulty.
        /// </summary>
        public abstract void StartActivity(float currentLevel);

        /// <summary>
        /// ActivityEnded is a method that gets called when the activity is over.
        /// It needs to be called at the end of the activity with whether the player won or lost
        /// </summary>
        protected void ActivityEnded(bool activityWon) {
            RoomManager.Instance.ActivityOver(false);
        }

    }
}
