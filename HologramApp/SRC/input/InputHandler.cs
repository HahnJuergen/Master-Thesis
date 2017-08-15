using HoloToolkit.Unity.InputModule;
using System.Collections;
using UnityEngine;
using System;

{
    public class InputHandler : Singleton<InputHandler>, ISourceStateHandler, IInputClickHandler
    {
        private GameObject cursor;

        private bool isInitialized = false;
        private bool isTracking = false;

        private Coroutine HandPositionTracking;

        private Vector3 handPosition = Vector3.zero;

        void Start()
        {
            this.cursor = Controller.Instance.m_UserInterface.transform.GetChild(0).gameObject;
        }

        public void Initialize()
        {
            if(!this.isInitialized)
            {
                this.isInitialized = true;

                this.cursor = Controller.Instance.m_UserInterface.transform.GetChild(0).gameObject;

                InputManager.Instance.PushFallbackInputHandler(gameObject);

                StartCoroutine(UpdateCursor());
            }
        }

        public void Deinitialize()
        {
            if(this.isInitialized)
            {
                this.isInitialized = false;

                InputManager.Instance.PushFallbackInputHandler(null);
            }
        }    

        void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
        {            
            if (this.isInitialized)
            {
                this.cursor.transform.rotation = Camera.main.transform.rotation;
                this.cursor.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                this.cursor.transform.position = this.handPosition;
                this.cursor.transform.position += this.cursor.transform.forward;               

                Vector3 viewPos = Camera.main.WorldToScreenPoint(this.cursor.transform.position);

                Ray ray = Camera.main.ScreenPointToRay(viewPos);

                RaycastHit hit;

                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    switch(hit.transform.tag)
                    {
                        case "DetailButton":
                        case "MIButton":
                        case "SettingsButton": HandleButtonClicked(hit); break;
                        case "Machine": HandleMachineElementClicked(hit); break;
                    }
                }
                else
                    UIHandler.Instance.Update(() => UIHandler.Instance.UpdateSelection(-1));               
            }
        }
        
        private void HandleMachineElementClicked(RaycastHit hit)
        {
            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateSelection(GetIndexOfSelectedMachine(hit)));            
        }

        private void HandleButtonClicked(RaycastHit hit)
        {
            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateButtonSelection(hit.transform.tag));
        }

        void ISourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
        {
            if (this.isInitialized && !isTracking)
            {
                isTracking = true;
                HandPositionTracking = StartCoroutine(TrackHandPosition(eventData.InputSource, eventData.SourceId));
                UIHandler.Instance.ShowProcessUI();
            }
        }

        void ISourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            if (this.isInitialized && isTracking)
            {
                isTracking = false;

                if(this.HandPositionTracking != null)
                    StopCoroutine(this.HandPositionTracking);

                if(!UIHandler.Instance.IsInformationBoxActive)
                    UIHandler.Instance.ShowLightUI();
            }
        }       

        private IEnumerator TrackHandPosition(IInputSource source, uint id)
        {
            for(;;)
            {
                source.TryGetPosition(id, out this.handPosition);

                this.cursor.transform.position = this.handPosition;
                this.cursor.transform.position += this.cursor.transform.forward;
              
                Vector3 viewPos = Camera.main.WorldToScreenPoint(this.cursor.transform.position);

                Ray ray = Camera.main.ScreenPointToRay(viewPos);

                RaycastHit hit;

                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    switch(hit.transform.tag)
                    {
                        case "DetailButton": HandleButtonHover("DetailButton"); break;
                        case "MIButton": HandleButtonHover("MIButton"); break;
                        case "SettingsButton": HandleButtonHover("SettingsButton"); break;
                        case "Machine": HandleMachineElementHoveredOver(hit); break;
                    }
                }
                else                
                    UIHandler.Instance.Update(() => UIHandler.Instance.ResetHover());

                yield return null;
            }            
        }

        private IEnumerator UpdateCursor()
        {
            for(;;)
            {
                this.cursor.transform.rotation = Camera.main.transform.rotation;
                this.cursor.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

                yield return null;
            }
        }

        private void HandleMachineElementHoveredOver(RaycastHit hit)
        {
            int index = GetIndexOfSelectedMachine(hit);

            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateHoverOverMachineElement(index));
        }

        private void HandleButtonHover(string tag)
        {
            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateHoverOverButtonElement(tag));
        }

        private int GetIndexOfSelectedMachine(RaycastHit hit)
        {
            GameObject go = hit.transform.gameObject;

            return go.GetComponent<Machine>().IndexNumber;
        }
    }
}
