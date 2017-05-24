
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace de.ur.juergenhahn.ma
{
    public class UIHandler : Singleton<UIHandler>
    {        
        protected UIHandler() { }

        private GameObject activeMachine;

        private bool isInitialized = false;

        private float machineLineLengthX;
        private float machineLineLengthY;

        private GameObject userInterface;
        private GameObject machineLine;
        private GameObject cursor;

        public void Initialize(GameObject[] machines, GameObject userInterface)
        {
            if (!isInitialized)
            {
                this.userInterface = userInterface;
                this.cursor = userInterface.transform.GetChild(0).gameObject;
                this.machineLine = userInterface.transform.GetChild(1).gameObject;

                LoadUI(machines);
            }
            else            
                Debug.LogError("UIHandler already initialized!");            
        }

        private void LoadUI(GameObject[] machines)
        {
            for (int i = 0; i < machines.Length; i++)
                SpawnMachine(machines[i], i);

            this.machineLineLengthX = Utility.Math.CalculateLengthXOfParentFromWorldCoordinatesFromSortedTransform(
                machineLine.transform,
                Constants.UIConstants.EDGE_OFFSET);

            this.machineLineLengthY = machineLine.transform.GetChild(0).GetComponent<Machine>().LeftFrontSideLineLengthY;

            PositionMachines();
            CreateContrastPlaneMachines();

            this.isInitialized = true;
        }

        private void SpawnMachine(GameObject machine, int indexNumber)
        {
            GameObject m = Instantiate(machine, Constants.UIConstants.MACHINE_DEFAULT_POSITION, Quaternion.identity);

            m.GetComponent<Machine>().IndexNumber = indexNumber;
            m.GetComponent<Machine>().AABB = Utility.Math.ToAABBPoints(m.transform.GetChild(0).gameObject);

            if (m.tag == Constants.UIConstants.TAG_MACHINE)
            {
                m.transform.Rotate(Constants.UIConstants.TAG_MACHINE_DEFAULT_ROTATION);
                m.transform.localScale = Constants.UIConstants.TAG_MACHINE_DEFAULT_SCALE;
            }

            m.transform.parent = this.machineLine.transform;
        }

        private void PositionMachines()
        {
            float startPoint = this.machineLineLengthX / -2.0f;

            for(int i = 0; i < machineLine.transform.childCount; i++)            
                PositionMachine(i, ref startPoint);            
        }      

        private void CreateContrastPlaneMachines()
        {
            float lengthFirstMachine = machineLine.transform.GetChild(0).GetComponent<Machine>().LowerFrontLineLengthX;
            float lengthLastMachine = machineLine.transform.GetChild(machineLine.transform.childCount - 1).GetComponent<Machine>().LowerFrontLineLengthX;
            float planeXLength = this.machineLineLengthX + lengthFirstMachine + lengthLastMachine;

            Mesh m = Utility.Mesh.GenerateMesh(MeshType.Plane, planeXLength, machineLineLengthY);
            
            GameObject plane = Utility.Mesh.MeshToGameObject(m, MeshType.Plane, Constants.UIConstants.SHADER_STANDARD, 
                Constants.UIConstants.COLOR_CONTRAST_PLANE, 
                Constants.UIConstants.LAYER_UI,
                Constants.UIConstants.CONTRAST_PLANE_NAME,
                this.UserInterface.transform);
        }

        private void PositionMachine(int machineIndex, ref float coordX)
        {
            if (machineIndex == 0)
            {
                machineLine.transform.GetChild(0).position = new Vector3(coordX,
                    Constants.UIConstants.DEFAULT_Y_POSITION_MACHINE,
                    Constants.UIConstants.DEFAULT_Z_POSITION_MACHINE);
            }
            else
            {
                Transform previous = machineLine.transform.GetChild(machineIndex - 1);
                Transform current = machineLine.transform.GetChild(machineIndex);

                coordX += previous.GetComponent<Machine>().LowerFrontLineCornerPointRight.x
                    + Constants.UIConstants.EDGE_OFFSET
                    + current.GetComponent<Machine>().LowerFrontLineLengthX / 2.0f;

                current.position = new Vector3(coordX,
                    Constants.UIConstants.DEFAULT_Y_POSITION_MACHINE,
                    Constants.UIConstants.DEFAULT_Z_POSITION_MACHINE);
            }
        }

        public GameObject UserInterface
        {
            get
            {
                return this.userInterface;
            }
        }
        
        public GameObject MachineLine
        {
            get
            {
                return this.machineLine;
            }
        }

        public GameObject Cursor
        {
            get
            {
                return this.cursor;
            }
        }
                
        public void Update(Action callback)
        {
            if (isInitialized)
                callback();
        }

        public void UpdateSelection(int currentIndexNumber)
        {
            foreach (Transform machine in this.machineLine.transform)            
                ResetMachine(machine.gameObject);

            if (currentIndexNumber > Constants.UIConstants.RESET_MACHINE_INDEX)
            {
                if ((activeMachine == null) || (activeMachine.GetComponent<Machine>().IndexNumber != currentIndexNumber))
                    SetActiveMachine(this.machineLine.transform.GetChild(currentIndexNumber).gameObject);
                else if((activeMachine.GetComponent<Machine>().IndexNumber == currentIndexNumber))                
                    SetActiveMachine(null);                
            }
            else         
                SetActiveMachine(null);
        }        

        private void ResetMachine(GameObject machine)
        {
            machine.GetComponent<Machine>().SelectionRotation(false);
            machine.GetComponent<Machine>().Outline(false);

            StartCoroutine(ResetMachineSelectionRotation(machine));
        }

        private void SetActiveMachine(GameObject machine)
        {
            if (machine != null)
            {
                machine.GetComponent<Machine>().SelectionRotation(true);
                machine.GetComponent<Machine>().Outline(true);
            }

            activeMachine = machine;
        }

        private IEnumerator ResetMachineSelectionRotation(GameObject machine)
        {
            for(;  machine.GetComponent<Machine>().MachineModel.transform.localRotation != Quaternion.Euler(Vector3.zero); )
            {
                machine.GetComponent<Machine>().MachineModel.transform.localRotation =
                    Quaternion.Slerp(machine.GetComponent<Machine>()
                    .MachineModel
                    .transform.localRotation,
                    Quaternion.Euler(Vector3.zero),
                    Time.deltaTime * Constants.UIConstants.RESET_MACHINE_SELECTION_ROTATION_MODIFIER);

                yield return null;
            }
        }       
    }
}
