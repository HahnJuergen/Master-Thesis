using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

{
    public class UIHandler : Singleton<UIHandler>
    {        
        protected UIHandler() { }
        
        private bool isInitialized = false;

        private float machineLineLengthX;
        private float machineLineLengthY;

        private GameObject activeMachine;
        private GameObject userInterface;
        private GameObject machineLine;
        private GameObject cursor;        

        private GameObject pushNotification;
        private GameObject loadingIcon;
        private GameObject errorPushNotification;

        private Coroutine LoadingVisualization;
        private Coroutine ErrorPushMessageStyle;
        private Coroutine LightUIDataUpdate;

        private string lineName = "";

        public void Initialize(GameObject userInterface)
        {
            if (!isInitialized)
            {
                this.userInterface = userInterface;
                this.cursor = userInterface.transform.GetChild(0).gameObject;
                this.machineLine = userInterface.transform.GetChild(1).gameObject;
                this.pushNotification = userInterface.transform.GetChild(6).GetChild(0).gameObject;
                this.errorPushNotification = userInterface.transform.GetChild(8).gameObject;

                this.cursor.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.9f));

                this.loadingIcon = this.userInterface.transform.GetChild(7).transform.GetChild(0).gameObject;

                this.errorPushNotification.SetActive(false);

                this.isInitialized = true;

                MonitorSystemTime();
                MonitorBatteryCharge();
            }
            else            
                Debug.LogError("UIHandler already initialized!"); 
        }

        public void InitializeProcessUI(Dictionary<string, string>[] machineObjects)
        {
            for(int i = 0; i < machineObjects.Length; i++)
            {
                string id = "";
                string name = "";

                foreach (string key in machineObjects[i].Keys)
                    id = key;

                name = machineObjects[i][id];

                GameObject machine = GetMachineObjectToInstantiateByName(name);

                SpawnMachine(machine, i, id, name);                
            }
            
            PositionMachines();
            ShowLightUI();

            this.userInterface.transform.GetChild(12).GetComponent<TextMesh>().text = lineName;
        }

        public void UpdateInformationBoxFillingLevelsWithComponentSetup(Dictionary<string, string>[] componentSetup)
        {
            Transform fillingLevels = this.userInterface.transform.GetChild(3).GetChild(0);

            for(int i = 1; i < componentSetup.Length; i++)
            {
                if(componentSetup[i] != null)
                {
                    GameObject bay = fillingLevels.GetChild(i - 1).GetChild(0).gameObject;
                    GameObject track = fillingLevels.GetChild(i - 1).GetChild(1).gameObject;
                    GameObject component = fillingLevels.GetChild(i - 1).GetChild(2).gameObject;
                    GameObject time = fillingLevels.GetChild(i - 1).GetChild(3).gameObject;

                    bay.GetComponent<TextMesh>().text = "" + componentSetup[i]["bay"];
                    track.GetComponent<TextMesh>().text = "" + componentSetup[i]["track"];
                    component.GetComponent<TextMesh>().text = "" + componentSetup[i]["component_bc"];
                    time.GetComponent<TextMesh>().text = GetTimeUntilDepletionLeftForCurrentComponentSetup(int.Parse(componentSetup[i]["time"]));
                }
            }
        }

        public void Visualize_OK_NOK_Distribution(int ok, int nok, string machineId)
        {    
            if(nok > 0)
            {
                switch (activeMachine.GetComponent<Machine>().Type)
                {
                    case Machine.MachineType.:
                    {                       
                        Dictionary<string, string> error = new Dictionary<string, string>();

                        error.Add("error", "");

                        this.UpdateEnablePushNotificationVisualization(new Dictionary<string, string>[] { error }, "Error with ", new Color(255f, 0f, 0f));  
                    }
                    break;

                    case Machine.MachineType.:
                    {
                        float distribution = (ok == 0) ? 0.0f : (ok / (ok + nok));

                        if (distribution > 0.98f)
                        {
                            // basically show distribution with no further actions
                        }
                        else if (distribution > 0.95f)
                        {
                            // implement a yellow warning type
                        }
                        else
                        {
                            Dictionary<string, string> error = new Dictionary<string, string>();

                            error.Add("error", "OK / NOK Distribution at " + distribution + "%!");

                            this.UpdateEnablePushNotificationVisualization(new Dictionary<string, string>[] { error }, "Error with ", new Color(255f, 0f, 0f));
                        }
                    }
                    break;

                    case Machine.MachineType.:
                    {
                        // serial error implementation
                    }
                    break;
                }
            }
            else
            {
                // visualize default 
            }           
        }

        public void Visualize_OK_NOK_DistributionLight(int ok, int nok, string machineId)
        {
            int i;
            GameObject m = null;

            for(i = 0; i < this.machineLine.transform.childCount; i++)
                if(this.machineLine.transform.GetChild(i).GetComponent<Machine>().ID == machineId)
                    break;             

            GameObject targetMachine = this.userInterface.transform.GetChild(11).GetChild(0).GetChild(i).gameObject;

            if (nok > 0)
            {
                switch (targetMachine.GetComponent<Machine>().Type)
                {
                    case Machine.MachineType.:
                    {
                        Dictionary<string, string> error = new Dictionary<string, string>();

                        error.Add("error", "");

                        this.UpdateEnablePushNotificationVisualization(new Dictionary<string, string>[] { error }, "Error with ", new Color(255f, 0f, 0f));
                    }
                    break;

                    case Machine.MachineType.:
                    {
                        float distribution = (ok == 0) ? 0.0f : (ok / (ok + nok));

                        if (distribution > 0.98f)
                        {
                            // basically show distribution with no further actions
                        }
                        else if (distribution > 0.95f)
                        {
                            // implement a yellow warning type
                        }
                        else
                        {
                            Dictionary<string, string> error = new Dictionary<string, string>();

                            error.Add("error", "OK / NOK Distribution at " + distribution + "%!");

                            this.UpdateEnablePushNotificationVisualization(new Dictionary<string, string>[] { error }, "Error with ", new Color(255f, 0f, 0f));
                        }
                    }
                    break;

                    case Machine.MachineType.:
                    {
                        // serial error implementation
                    }
                    break;
                }
            }
            else
            {
                // visualize default 
            }
        }

        public void UpdateSelectionMachineAssociation(bool isActivated, int index)
        {                
            if (!isActivated)
            {
                GameObject target = GameObject.FindGameObjectWithTag("Floor").transform.GetChild(index).gameObject;

                target.GetComponent<MeshRenderer>().material = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<MeshRenderer>().material;
                target.transform.GetChild(0).gameObject.SetActive(isActivated);
            }
            else
            {
                GameObject target = GameObject.FindGameObjectWithTag("Floor").transform.GetChild(index).gameObject;

                Vector3 forward = Camera.main.transform.forward;

                forward.y = 0.0f;

                target.transform.GetChild(0).gameObject.SetActive(isActivated);
                target.transform.GetChild(0).localRotation = Quaternion.LookRotation(forward);  
                target.GetComponent<MeshRenderer>().material = Resources.Load<Material>("materials/MachineHighlight");

                //GameObject machine = machineLine.transform.GetChild(index).gameObject;
                //Machine.MachineType type = machine.GetComponent<Machine>().Type;

                /*
                switch (type)
                {
                    case Machine.MachineType.:
                    {
                          
                    }
                    break;

                    case Machine.MachineType.:
                    {

                    }
                    break;

                    case Machine.MachineType.:
                    {

                    }
                    break;

                    case Machine.MachineType.:
                    {
                        ServerCorrespondence.Instance.Fetch_OK_NOK_DisributionByMachineID(machine.GetComponent<Machine>().ID, 2);
                    }
                    break;
                }
                */
            }
        }

        private string GetTimeUntilDepletionLeftForCurrentComponentSetup(int s)
        {
            int m = s / 60;
            s %= 60;

            int h = m / 60;
            m %= 60;

            return ((h > 9) ? ("" + h) : ("0" + h)) 
                + ((m > 9) ? (":" + m) : (":0" + m)) 
                + ((s > 9) ? (":" + s) : (":0" + s));
        }

        public void UpdateInformationBoxOrderHeaderData(string[] orderHeaderData)
        {
            Transform orderInformation = this.userInterface.transform.GetChild(5).GetChild(0).GetChild(0);
            Transform productInformation = this.userInterface.transform.GetChild(5).GetChild(0).GetChild(1);
            Transform customerInformation = this.userInterface.transform.GetChild(5).GetChild(0).GetChild(2);

            orderInformation.GetChild(2).GetComponent<TextMesh>().text = orderHeaderData[0];
            orderInformation.GetChild(4).GetComponent<TextMesh>().text = orderHeaderData[4];
            orderInformation.GetChild(6).GetComponent<TextMesh>().text = orderHeaderData[5];

            productInformation.GetChild(1).GetComponent<TextMesh>().text = orderHeaderData[2];
            productInformation.GetChild(3).GetComponent<TextMesh>().text = orderHeaderData[1];

            customerInformation.GetChild(1).GetComponent<TextMesh>().text = orderHeaderData[3];
        }        

        public void UpdateEnablePushNotificationVisualization(Dictionary<string, string>[] errors, string headline, Color color)
        {
            string notification = headline;

            int targetLength = errors.Length > 4 ? 4 : errors.Length;

            for(int i = 0; i < targetLength; i++)
            {
                if (Utility.Math.IsEven(i))
                    notification += "\n";

                notification += errors[i]["error"];
            }

            if (errors.Length > 4)
                notification += "...";

            this.errorPushNotification.transform.GetChild(0).GetComponent<TextMesh>().text = notification;       
            this.errorPushNotification.transform.GetChild(0).GetComponent<TextMesh>().color = color;
            this.errorPushNotification.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().color = color;
            this.errorPushNotification.transform.GetChild(1).GetChild(1).GetComponent<Renderer>().material.color = color;
            this.errorPushNotification.transform.GetChild(1).GetChild(2).GetComponent<Renderer>().material.color = color;
            this.errorPushNotification.transform.GetChild(1).GetChild(3).GetComponent<Renderer>().material.color = color;
            this.errorPushNotification.transform.GetChild(1).GetChild(4).GetComponent<Renderer>().material.color = color;

            color.a = 85.0f / 255.0f;

            this.errorPushNotification.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<ParticleSystem>().startColor = color;
            this.errorPushNotification.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();
            this.errorPushNotification.SetActive(true);
        }

        public void UpdateDisablePushNotificationVisualization()
        {
            this.errorPushNotification.SetActive(false);
            this.errorPushNotification.transform.GetChild(0).GetComponent<TextMesh>().text = "";
        }

        private GameObject GetMachineObjectToInstantiateByName(string name)
        {
            GameObject go = null;
    
            switch(name)
            {
                case "":
                case "":
                case "":
                case "":
                {
                    go = Resources.Load("prefabs/") as GameObject;
                }
                break;

                case "":
                {
                    go = Resources.Load("prefabs/") as GameObject;
                }
                break;

                case "":
                {
                    go = Resources.Load("prefabs/") as GameObject;
                }
                break;

                case "":
                {
                    go = Resources.Load("prefabs/") as GameObject;
                }
                break;
            }

            return go;
        }        

        private void SpawnMachine(GameObject machine, int indexNumber, string id, string name)
        {
            GameObject m = Instantiate(machine, Constants.UIConstants.MACHINE_DEFAULT_POSITION, Quaternion.identity);

            m.GetComponent<Machine>().IndexNumber = indexNumber;
            m.GetComponent<Machine>().ID = id;

            int i = 0;

            for(i = 0; i < name.Length; i++)            
                if (name[i] == '_')
                    break;

            lineName = name.Substring(0, i);
            name = name.Substring(i + 1, name.Length - i - 1);

            m.GetComponent<Machine>().Name = name;
            m.transform.GetChild(1).GetComponent<TextMesh>().text = name;
            m.transform.parent = this.machineLine.transform;

            GameObject ml = Instantiate(Resources.Load("prefabs/MachineLight") as GameObject);

            ml.transform.parent = userInterface.transform.GetChild(11).GetChild(0);
            ml.transform.localScale = new Vector3(0.033f, 0.033f, 1e-05f);
            ml.transform.localPosition = new Vector3(0f, 0f, 0f);
            ml.GetComponent<Renderer>().material = Resources.Load("materials/MachineMaterial") as Material;
            ml.transform.GetChild(0).GetComponent<TextMesh>().text = name;
        }

        public void UpdateComponentSetupLightUI(Dictionary<string, string>[] dictionary)
        {
            if(dictionary.Length > 1)
            {      
                string targetMachineId = dictionary[0]["id"];

                int targetIndex = -1;

                for (int i = 0; i < machineLine.transform.childCount; i++)
                {
                    GameObject go = machineLine.transform.GetChild(i).gameObject;

                    if (targetMachineId == go.GetComponent<Machine>().ID)
                    {
                        targetIndex = i;

                        // refactor out of loop once expermint ready state is present

                        GameObject gol = userInterface.transform.GetChild(11).GetChild(0).GetChild(i).gameObject;

                        gol.transform.GetChild(2).GetComponent<TextMesh>().text = dictionary[1]["bay"] + ", T" + dictionary[1]["track"];
                        gol.transform.GetChild(3).GetComponent<TextMesh>().text = GetTimeUntilDepletionLeftForCurrentComponentSetup(int.Parse(dictionary[1]["time"]));
                    }
                }
            }
        }

        private void PositionMachines()
        {
            float startX = machineLine.transform.childCount * (0.07f + 0.03f) / -2.0f; 

            for (int i = 0; i < machineLine.transform.childCount; i++)
            {
                PositionMachine(i, startX);

                GameObject go = userInterface.transform.GetChild(11).GetChild(0).GetChild(i).gameObject;

                go.transform.localPosition = new Vector3(0.0f, 0.18f - i * 0.055f, 0.0f);
            }
        }            

        public void UpdateNotification(string notification)
        {
            this.pushNotification.GetComponent<TextMesh>().text = notification;
        }

        private void PositionMachine(int machineIndex, float startX)
        {
            Vector3 position = new Vector3(startX + machineIndex * 0.1f, Constants.UIConstants.DEFAULT_Y_POSITION_MACHINE, Constants.UIConstants.DEFAULT_Z_POSITION_MACHINE);

            machineLine.transform.GetChild(machineIndex).localPosition = position;
        }

        public void UpdateComponentRejectionVisualizationLightUI(Dictionary<string, string> componentRejectionRateDict)
        {
            string targetMachineId = componentRejectionRateDict["id"];
            string componentRejection = componentRejectionRateDict["component_rejection"];

            int targetIndex = -1;

            for(int i = 0; i < machineLine.transform.childCount; i++)
            {
                GameObject go = machineLine.transform.GetChild(i).gameObject;

                if(targetMachineId == go.GetComponent<Machine>().ID)
                {
                    targetIndex = i;

                    // refactor out of loop once expermint ready state is present

                    GameObject gol = userInterface.transform.GetChild(11).GetChild(0).GetChild(i).gameObject;

                    gol.transform.GetChild(1).GetComponent<TextMesh>().text = "CR: " + componentRejection + "%";
                }
            }
        }

        public void ShowProcessUI()
        {
            this.userInterface.transform.GetChild(13).gameObject.SetActive(true);
            this.userInterface.transform.GetChild(14).gameObject.SetActive(true);            

            if (LightUIDataUpdate != null)
                StopCoroutine(LightUIDataUpdate);

            ServerCorrespondence.Instance.DisableAutomaticUpdate(ServerCorrespondence.AutomaticUpdateMode.COMPONENT_SETUP_LIGHT);
            ServerCorrespondence.Instance.DisableAutomaticUpdate(ServerCorrespondence.AutomaticUpdateMode.COMPONENT_REJECTION_LIGHT);

            for (int i = 0; i < userInterface.transform.GetChild(11).GetChild(0).childCount; i++)
                userInterface.transform.GetChild(11).GetChild(0).GetChild(i).gameObject.SetActive(false);

            for (int i = 0; i < machineLine.transform.childCount; i++)            
                machineLine.transform.GetChild(i).gameObject.SetActive(true);
        }

        public void ShowLightUI()
        {
            UpdateSelection(-1);

            for (int i = 0; i < machineLine.transform.childCount; i++)
                machineLine.transform.GetChild(i).gameObject.SetActive(false);

            for (int i = 0; i < userInterface.transform.GetChild(11).GetChild(0).childCount; i++)
                userInterface.transform.GetChild(11).GetChild(0).GetChild(i).gameObject.SetActive(true);

            this.userInterface.transform.GetChild(13).gameObject.SetActive(false);
            this.userInterface.transform.GetChild(14).gameObject.SetActive(false);

            LightUIDataUpdate = StartCoroutine(LightUIDataUpdateRoutine());
        }

        private IEnumerator LightUIDataUpdateRoutine()
        {
            for (;;)
            {                
                for (int i = 0; i < userInterface.transform.GetChild(11).GetChild(0).childCount; i++)
                {
                    GameObject m = machineLine.transform.GetChild(i).gameObject;

                    string machineId = m.GetComponent<Machine>().ID;

                    switch (machineLine.transform.GetChild(i).GetComponent<Machine>().Type)
                    {
                        case Machine.MachineType.:
                        {
                            print("FETCH  DATA LIGHT");

                            ServerCorrespondence.Instance.FetchMachineComponentSetupByMachineID(machineId, 1);
                            ServerCorrespondence.Instance.FetchComponentRejectionRateByMachineID(machineId, 1);
                        }
                        break;

                        case Machine.MachineType.:
                        {
                            print("FETCH  DATA LIGHT");
                        }
                        break;

                        case Machine.MachineType.:
                        {
                            print("FETCH  DATA LIGHT");
                        }
                        break;

                        case Machine.MachineType.:
                        {
                            print("FETCH  DATA LIGHT");
                        }
                        break;
                    }
                }

                yield return new WaitForSeconds(30.0f);
            }
        }

        public void UpdateHoverOverMachineElement(int index)
        {
            ResetMachineHover();

            GameObject go = this.machineLine.transform.GetChild(index).gameObject;

            go.transform.localScale *= 1.25f;
        }

        public void UpdateHoverOverButtonElement(string target)
        {
            ResetButtonHover();

            GameObject button = GameObject.FindWithTag(target);

            button.transform.localScale *= 1.25f;            
        }

        public void ResetHover()
        {
            ResetMachineHover();
            ResetButtonHover();
        }

        private void ResetMachineHover()
        {            
            foreach (Transform t in machineLine.transform)            
                if (t.localScale != new Vector3(0.0008f, 0.0008f, 1.0f))
                    t.localScale /= 1.25f;                              
        }

        private void ResetButtonHover()
        {
            foreach(GameObject button in new GameObject[] {
                GameObject.FindWithTag("DetailButton"),
                GameObject.FindWithTag("MIButton"),
                GameObject.FindWithTag("SettingsButton")
            })
                if (button != null && button.transform.localScale != new Vector3(0.0008f, 0.0008f, 1.0f))
                    button.transform.localScale /= 1.25f;
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

        public bool IsInformationBoxActive
        {
            get
            {
                return userInterface.transform.GetChild(3).gameObject.activeInHierarchy;
            }
        }
                
        public void Update(Action callback)
        {
            if (isInitialized)
                callback();
        }

        public void UpdateSelection(int currentIndexNumber)
        {
            GameObject button = GameObject.FindGameObjectWithTag("DetailButton");

            if(button)
                button.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("textures/Icon_Details");

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

        public void UpdateButtonSelection(string tag)
        {
            switch (tag)
            {
                case "DetailButton": OnDetailButtonClicked(); break;
                case "MIButton": OnManualInspectionButtonClicked(); break;
                case "SettingsButton": OnSettingsButtonClicked(); break;
            }
        }

        private void OnDetailButtonClicked()
        {
            GameObject button = GameObject.FindGameObjectWithTag("DetailButton");

            string currentTextureName = button.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite.name;

            if (currentTextureName == "Icon_Details")
            {
                StartCoroutine(EnableInformationBoxVisualizationRight());

                button.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("textures/Icon_Close");

                ServerCorrespondence.Instance.FetchOrderHeaderDataByMachineID(activeMachine.GetComponent<Machine>().ID);
            }
            else if (currentTextureName == "Icon_Close")
            {
                GameObject informationDisplay = this.userInterface.transform.GetChild(5).gameObject;

                informationDisplay.SetActive(false);

                button.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("textures/Icon_Details");
            }
        }

        private void OnManualInspectionButtonClicked()
        {
            print("OnManualInspectionButtonClicked");
        }

        private void OnSettingsButtonClicked()
        {
            print("OnSettingsButtonClicked");
        }

        public void UpdateLoadingIconRotation(bool isEnabled)
        {
            if(isEnabled)
            {
                this.loadingIcon.SetActive(true);
                LoadingVisualization = StartCoroutine(HandleLoadingIconRotation());
            }
            else
            {
                StopCoroutine(LoadingVisualization);
                this.loadingIcon.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                this.loadingIcon.SetActive(false);
            }
        }

        public void UpdateComponentRejectionVisualization(string componentRejection)
        {
            GameObject componentRejectionVisualization = this.userInterface.transform.GetChild(3).GetChild(1).GetChild(1).gameObject;

            componentRejectionVisualization.GetComponent<TextMesh>().text = componentRejection + "%"; 
        }

        private IEnumerator HandleLoadingIconRotation()
        {
            for(;;)
            {
                yield return new WaitForSeconds(0.1f);

                this.loadingIcon.GetComponent<RectTransform>().Rotate(new Vector3(0.0f, 0.0f, -45.0f));
            }
        }

        private void ResetMachine(GameObject machine)
        {
            this.userInterface.transform.GetChild(2).gameObject.SetActive(false);
            this.userInterface.transform.GetChild(3).gameObject.SetActive(false);
            this.userInterface.transform.GetChild(5).gameObject.SetActive(false);

            machine.transform.GetChild(2).gameObject.SetActive(false);
        }

        private void SetActiveMachine(GameObject machine)
        {
            if (machine != null)
            {                
                StartCoroutine(EnableInformationBoxVisualizationLeft(machine));

                machine.transform.GetChild(2).gameObject.SetActive(true);

                if (activeMachine != null)
                {
                    switch (activeMachine.GetComponent<Machine>().Type)
                    {
                        case Machine.MachineType.:
                        {
                            ServerCorrespondence.Instance.DisableAutomaticUpdate(ServerCorrespondence.AutomaticUpdateMode.COMPONENT_SETUP);
                            ServerCorrespondence.Instance.DisableAutomaticUpdate(ServerCorrespondence.AutomaticUpdateMode.COMPONENT_REJECTION);
                        }
                        break;

                        case Machine.MachineType.:
                        {
                            
                        }
                        break;

                        case Machine.MachineType.:
                        {
                            
                        }
                        break;

                        case Machine.MachineType.:
                        {
                            
                        }
                        break;
                    }
                }               

                switch (machine.GetComponent<Machine>().Type)         
                {
                    case Machine.MachineType.:
                    {
                        print(" DATA FETCH");

                        ServerCorrespondence.Instance.FetchMachineComponentSetupByMachineID(machine.GetComponent<Machine>().ID);
                        ServerCorrespondence.Instance.FetchComponentRejectionRateByMachineID(machine.GetComponent<Machine>().ID);
                    }
                    break;

                    case Machine.MachineType.:
                    {
                        ServerCorrespondence.Instance.Fetch_OK_NOK_DisributionByMachineID(machine.GetComponent<Machine>().ID);
                        print(" DATA FETCH");
                    }
                    break;

                    case Machine.MachineType.:
                    {
                        print("  DATA");
                    }
                    break;

                    case Machine.MachineType.:
                    {
                        ServerCorrespondence.Instance.Fetch_OK_NOK_DisributionByMachineID(machine.GetComponent<Machine>().ID);
                        print("  DATA");
                    }
                    break;
                }               
            }

            activeMachine = machine;
        }           

        private IEnumerator EnableInformationBoxVisualizationRight()
        {
            GameObject informationDisplay = this.userInterface.transform.GetChild(5).gameObject;

            informationDisplay.SetActive(true);

            Component[] compsInformationDisplay = informationDisplay.GetComponentsInChildren<MeshRenderer>();

            foreach (Component comp in compsInformationDisplay)
                StartCoroutine(Fader.Fade(comp.gameObject, Fader.FadeMode.IN, 0.0f, 1.0f, 0.5f));

            yield return null;
        }

        private IEnumerator EnableInformationBoxVisualizationLeft(GameObject machine)
        {
            // check which information box is displayed / fill information box dynamically based on active machien

            GameObject detailButton = this.userInterface.transform.GetChild(2).gameObject;            
            GameObject informationDisplay = this.userInterface.transform.GetChild(3).gameObject;

            detailButton.SetActive(true);            

            informationDisplay.SetActive(true);

            Component[] compsInformationDisplay = informationDisplay.GetComponentsInChildren<MeshRenderer>();

            foreach(Component comp in compsInformationDisplay)
                StartCoroutine(Fader.Fade(comp.gameObject, Fader.FadeMode.IN, 0.0f, 1.0f, 0.5f));

            yield return null;
        }

        private void MonitorSystemTime()
        {
            StartCoroutine(TrackSystemTime());
        }

        private void MonitorBatteryCharge()
        {
            StartCoroutine(TrackBatteryCharge());
        }

        private IEnumerator TrackSystemTime()
        {
            for(;;)
            {
                yield return new WaitForSeconds(1.0f);

                this.userInterface.transform.GetChild(9).GetChild(0).GetComponent<TextMesh>().text = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");
            }
        }

        private IEnumerator TrackBatteryCharge()
        {
            for(;;)
            {
                yield return new WaitForSeconds(60.0f);

                float batteryLevel = SystemInfo.batteryLevel;
                
                Image chargeLevelBar  = this.userInterface.transform.GetChild(10).GetChild(0).GetChild(1).GetComponent<Image>();
                Text chargeLevelText = this.userInterface.transform.GetChild(10).GetChild(0).GetChild(3).GetComponent<Text>();

                chargeLevelBar.fillAmount = batteryLevel;
                chargeLevelText.text = batteryLevel * 100.0f + "%";
            }
        }
    }
}
