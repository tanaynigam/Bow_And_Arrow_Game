using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FusedVR {
    public class SimonSaysButtons : InteractableObject {

        private Color myColor;
        private SimonSays simonActivity;
        private int myID;

        private float LowerIntensity = 150f / 255f;

        private bool firstTime = false;
        private Material buttonMaterial;

        void Awake() {
            buttonMaterial = GetComponent<Renderer>().material;
        }

        public void Setup(SimonSays manager, Color c, int index) {
            myColor = c;
            simonActivity = manager;
            myID = index;
            buttonMaterial.SetColor("_EmissionColor", myColor * LowerIntensity);
        }

        public IEnumerator FlashMyColor(float time) {
            buttonMaterial.SetColor("_EmissionColor", myColor);
            //TODO: make sound effects
            yield return new WaitForSeconds(.5f);
            buttonMaterial.SetColor("_EmissionColor", myColor * LowerIntensity);
        }

        protected override void Update() {
            base.Update();
            if (isTouched && !firstTime) {
                firstTime = true;
                buttonMaterial.SetColor("_EmissionColor", myColor);
                simonActivity.ButtonHit(myID);
            }

            if (!isTouched && firstTime) {
                buttonMaterial.SetColor("_EmissionColor", myColor * LowerIntensity);
                firstTime = false; //reset if we are not being hit
            }

            if (isSelected)
                OnDeSelected(controllerCollider.GetController());
        }

        protected override void MoveToController() {
            //DO NOTHING
        }
    }
}
