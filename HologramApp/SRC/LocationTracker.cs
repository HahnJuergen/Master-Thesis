using System.Collections;
using System.Collections.Generic;
using UnityEngine;

{ 
    public class LocationTracker : MonoBehaviour
    {
        private Coroutine TriggerExitSelectionHider = null;        

        void OnTriggerEnter(Collider other)
        {
            UIHandler.Instance.UpdateSelectionMachineAssociation(true, other.transform.GetSiblingIndex());
        }

        private void OnTriggerExit(Collider other)
        {
            UIHandler.Instance.UpdateSelectionMachineAssociation(false, other.transform.GetSiblingIndex());        
        }
    }
}
