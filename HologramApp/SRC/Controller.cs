using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using System;
using System.Collections;


{
    public class Controller : Singleton<Controller>
    {
        public GameObject m_UserInterface;
        public AudioSource m_AudioSource;

        private bool isTracking = false;

        protected Controller() {}

        void Start()
        {
            SpeechHandler.Instance.AudioSource = m_AudioSource;

            UIHandler.Instance.Initialize(m_UserInterface);
            UIHandler.Instance.UpdateNotification("Hello!");

            InputHandler.Instance.Initialize();

            ServerCorrespondence.Instance.Initialize();
            ServerCorrespondence.Instance.Connect();            
        }    
    }
}
 