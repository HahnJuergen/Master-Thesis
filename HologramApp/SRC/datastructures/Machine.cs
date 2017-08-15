using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

{
    public class Machine : MonoBehaviour, IInputClickHandler
    {
        public enum MachineType
        {
            
        }

        private GameObject machineModel;
        private GameObject selectionPlane;
        private GameObject machineNameTextBox;
        private GameObject machineInformationAssociationPlane;
        private GameObject machineAssociationConnectionPlane;
        private GameObject machineAssociationConnectionPlaneRight;
        private GameObject machineIDTextBox;

        private ParticleSystem selectionEffect;

        private string name;
        private string id;

        private Quaternion startRotation;

        private Vector3 size;

        private int indexNumber;
        private MachineType type;

        private void Start()
        {
            this.machineModel = gameObject.transform.GetChild(0).gameObject;          
            this.startRotation = gameObject.transform.rotation;
        }   

        public string ID
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        public GameObject MachineModel
        {
            get
            {
                return this.machineModel;
            }

            set
            {
                this.machineModel = value;
            }
        }

        public GameObject SelectionPlane
        {
            get
            {
                return this.selectionPlane;
            }

            set
            {
                this.machineModel = value;
            }
        }

        public ParticleSystem SelectionEffect
        {
            get
            {
                return this.selectionEffect;
            }

            set
            {
                this.selectionEffect = value;
            }
        }

        public GameObject MachineNameTextBox
        {
            get
            {
                return this.machineNameTextBox;
            }

            set
            {
                this.machineNameTextBox = value;
            }
        }

        public GameObject MachineInformationAssociationPlane
        {
            get
            {
                return this.machineInformationAssociationPlane;
            }

            set
            {
                this.machineInformationAssociationPlane = value;
            }
        }

        public GameObject MachineAssociationConnectionPlane
        {
            get
            {
                return this.machineAssociationConnectionPlane;
            }

            set
            {
                this.machineAssociationConnectionPlane = value;
            }
        }

        public GameObject MachineAssociationConnectionPlaneRight
        {
            get
            {
                return this.machineAssociationConnectionPlaneRight;
            }

            set
            {
                this.machineAssociationConnectionPlaneRight = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;

                print(value);

                switch(value)
                {
                    case "":
                    case "":
                    case "":
                    case "":
                    {
                        this.type = MachineType.;
                    }                  
                    break;
                    case "":
                    {
                        this.type = MachineType.;
                    }
                    break;
                    case "":
                    {
                        this.type = MachineType.;
                    }
                    break;

                    case "":
                    {
                        this.type = MachineType.;
                    }
                    break;
                }
            }
        }

        public Quaternion StartRotation
        {
            get
            {
                return this.startRotation;
            }
            set
            {
                this.startRotation = value;
            }
        }

        public Vector3 Size
        {
            get
            {
                return this.size;
            }
        }

        public int IndexNumber
        {
            get
            {
                return indexNumber;
            }
            set
            {
                indexNumber = value;
            }
        }

        public MachineType Type
        {
            get
            {
                return this.type;
            }

            set
            {
                type = value;
            }
        }

        public void Rotate(float x, float y, float z)
        {
            this.machineModel.transform.Rotate(x, y, z);
        }

        public void Rotate(Vector3 eulerAngles)
        {
            this.machineModel.transform.Rotate(eulerAngles);
        }    
    }
}
