using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace de.ur.juergenhahn.ma
{
    public class Machine : MonoBehaviour
    {
        private GameObject machineModel;
        private GameObject selectionPlane;

        private Quaternion startRotation;

        private Vector3[] aabb;

        private Vector3 size;

        private int indexNumber;

        private void Start()
        {
            this.machineModel = gameObject.transform.GetChild(0).gameObject;
            this.selectionPlane = gameObject.transform.GetChild(1).gameObject;
            this.startRotation = gameObject.transform.rotation;
            this.size = this.machineModel.GetComponent<Collider>().bounds.size;

            this.aabb = Utility.Math.ToAABBPoints(this.machineModel);

            this.selectionPlane.GetComponent<Renderer>().enabled = false;
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

        public Vector3[] AABB
        {
            get
            {
                return this.aabb;
            }

            set
            {
                this.aabb = value;
            }
        }

        public Vector3 LowerFrontLineCornerPointLeft
        {
            get
            {
                return this.aabb[0];
            }
        }

        public float LowerFrontLineLengthX
        {
            get
            {
                return this.LowerFrontLineCornerPointRight.x - this.LowerFrontLineCornerPointLeft.x;
            }
        }

        public float LeftFrontSideLineLengthY
        {
            get
            {
                return this.UpperFrontLineCornerPointRight.y - this.LowerFrontLineCornerPointLeft.y;
            }
        }

        public Vector3 LowerFrontLineCornerPointRight
        {
            get
            {
                return this.aabb[4];
            }
        }

        public Vector3 UpperFrontLineCornerPointRight
        {
            get
            {
                return this.aabb[7];
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

        public void Outline(bool isOutlined)
        {
            this.machineModel.transform.GetComponent<cakeslice.Outline>().enabled = isOutlined;
        }

        public void SelectionRotation(bool isSelected)
        {
            this.machineModel.transform.GetComponent<Rotation>().enabled = isSelected;
        }       
    }
}
