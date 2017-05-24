using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using System;
using System.Collections;


namespace de.ur.juergenhahn.ma
{
    public class Controller : Singleton<Controller>
    {
        public GameObject[] m_Machines;
        public GameObject m_UserInterface;

        private bool isTracking = false;

        void Start()
        {             
            UIHandler.Instance.Initialize(m_Machines, m_UserInterface);
            InputHandler.Instance.Initialize();
        } 
    }
}

