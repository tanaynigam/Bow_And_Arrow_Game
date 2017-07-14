using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FusedVR {
        public class SimonSays : Activity {

        public SimonSaysButtons[] buttons;
        public Color[] colors;

        private int[] buttonsPressed;
        private int numButtonsToHit;

        private bool success = true;
        private int buttonsHit = 0;

        void Start() {
            for (int i = 0; i < buttons.Length; i++) {
                buttons[i].Setup(this, colors[i % colors.Length], i);
            }
        }

        public override void StartActivity(float currentLevel) {
            StartCoroutine(PlaySimonSay());
        }

        public IEnumerator PlaySimonSay() {
            numButtonsToHit = 4; //TODO: scale this based on level
            buttonsPressed = new int[numButtonsToHit];
            success = true;
            buttonsHit = 0;

            for (int i = 0; i < numButtonsToHit; i++) { //play just 4 colors
                yield return new WaitForSeconds(.5f);
                int index = Random.Range(0, buttons.Length);
                buttonsPressed[i] = index;

                yield return StartCoroutine( buttons[index].FlashMyColor(1f) );
            }

            yield return new WaitForSecondsRealtime(5f); //wait 5 seconds before the game is over

            ActivityEnded(false);
        }

        //TODO: detect if player hits buttons
        public void ButtonHit(int index) {
            if (index == buttonsPressed[buttonsHit]) {
                success &= true;
            } else {
                success = false;
            }

            if ( (buttonsHit + 1) == numButtonsToHit && success)
                ActivityEnded(true);

            buttonsHit = (buttonsHit + 1) % numButtonsToHit;
        }
    }


}
