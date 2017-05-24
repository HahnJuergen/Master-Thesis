using HoloToolkit.Unity.InputModule;
using System.Collections;
using UnityEngine;

namespace de.ur.juergenhahn.ma
{
    public class InputHandler : Singleton<InputHandler>, ISourceStateHandler, IInputClickHandler
    {
        private GameObject cursor;

        private bool isInitialized = false;
        private bool isTracking = false;

        private Coroutine HandPositionTracking;

        void Start()
        {
            this.cursor = Controller.Instance.m_UserInterface.transform.GetChild(0).gameObject;

            InputManager.Instance.PushFallbackInputHandler(this.gameObject);
        }

        public void Initialize()
        {
            if(!this.isInitialized)
            {
                this.isInitialized = true;

                this.cursor = Controller.Instance.m_UserInterface.transform.GetChild(0).gameObject;

                InputManager.Instance.PushFallbackInputHandler(gameObject);
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
                RaycastHit hit;

                if (Physics.Raycast(this.cursor.transform.position, UIHandler.Instance.UserInterface.transform.forward , out hit))
                {
                    GameObject go = hit.transform.parent.gameObject;

                    int index = go.GetComponent<Machine>().IndexNumber;

                    UIHandler.Instance.Update(() => UIHandler.Instance.UpdateSelection(index));
                }
                else
                {
                    UIHandler.Instance.Update(() => UIHandler.Instance.UpdateSelection(-1));
                }
            }
        }

        void ISourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
        {
            if (this.isInitialized && !isTracking)
            {
                isTracking = true;
                HandPositionTracking = StartCoroutine(TrackHandPosition(eventData.InputSource, eventData.SourceId));
            }
        }

        void ISourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            if (this.isInitialized && isTracking)
            {
                isTracking = false;
                StopCoroutine(this.HandPositionTracking);
            }
        }       

        private IEnumerator TrackHandPosition(IInputSource source, uint id)
        {
            this.cursor.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);

            for(;;)
            {
                Vector3 handPosition = Vector3.zero;

                source.TryGetPosition(id, out handPosition);
                this.cursor.transform.position = handPosition;

                // usable for highlighting effects, etc.
                RaycastHit hit;

                if (Physics.Raycast(this.cursor.transform.position, UIHandler.Instance.UserInterface.transform.forward, out hit))
                {

                }

                yield return null;
            }
        }
    }
}
