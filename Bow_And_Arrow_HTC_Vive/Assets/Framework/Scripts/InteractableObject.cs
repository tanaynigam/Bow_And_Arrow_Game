using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//extendable class
namespace FusedVR {
    [RequireComponent(typeof(Collider))]
    public abstract class InteractableObject : MonoBehaviour
    {

        protected bool isTouched; // can be set for any high light effects
        protected bool isSelected;
        protected ControllerCollider controllerCollider; //the collider for detecting the controller

        protected List<ControllerCollider> potentialControls = new List<ControllerCollider>();

        [Tooltip("The Collider in charge of detecting whether or not to interact")]
        public Collider grabCollider;  //the collider to detect whether or not to grab; this is used for the interactable object

        void OnDestroy()
        {
            if (controllerCollider != null)
                OnDeSelected(controllerCollider.GetController());
        }

        public void ControllerTouched(ControllerCollider obj)
        {
            isTouched = true;
            if (!potentialControls.Contains(obj))
            {
                potentialControls.Add(obj);
            }
        }

        public void ControllerDeTouched(ControllerCollider obj)
        {
            potentialControls.Remove(obj);
            if (potentialControls.Count == 0)
                isTouched = false;
        }

        protected virtual void MoveToController()
        {
            //TODO: add animations
            //TODO: hide the controller model
            transform.position = controllerCollider.GetController().transform.position;
            transform.rotation = controllerCollider.GetController().transform.rotation;
        }

        protected virtual void OnSelected(SteamVR_TrackedObject controller)
        {
            //optional method for child classes to overide to detect when a controller selects the object
            SteamVR_RenderModel model = controllerCollider.GetController().GetComponentInChildren<SteamVR_RenderModel>();
            if (model != null)
            {
                model.gameObject.SetActive(false);
            }
        }

        protected virtual void OnDeSelected(SteamVR_TrackedObject controller)
        {
            //optional method for child classes to overide to detect when a controller deselects the object
            SteamVR_RenderModel model = controllerCollider.GetController().GetComponentInChildren<SteamVR_RenderModel>(true);
            if (model != null)
            {
                model.gameObject.SetActive(true);
            }
        }

        protected void ForceDropObject() {
            if (isSelected) { 
                    isSelected = false;
                    controllerCollider.EnableCollisions();
                    grabCollider.enabled = true;
                    OnDeSelected(controllerCollider.GetController());
            }
        } 

        protected virtual void Update()
        {
            if (!isSelected && isTouched) {
                foreach (ControllerCollider obj in potentialControls) { //in case there are 2
                    if (VRTK.VRTK_SDK_Bridge.IsTriggerTouchedDownOnIndex((uint)obj.GetController().index))   {
                        isSelected = true;
                        controllerCollider = obj;
                        controllerCollider.DisableCollisions();
                        grabCollider.enabled = false;

                        OnSelected(controllerCollider.GetController());
                    }
                }
            }

            if (isSelected) {
                if (VRTK.VRTK_SDK_Bridge.IsGripTouchedDownOnIndex((uint)controllerCollider.GetController().index)) { //TODO: test what happens with holding trigger and grip
                    ForceDropObject();
                }
                MoveToController();
            }
        }
    }
}