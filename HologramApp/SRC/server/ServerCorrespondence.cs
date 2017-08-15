using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

{
    public class ServerCorrespondence : Singleton<ServerCorrespondence>
    {
        private const string BASE_URL = "";
        private const string CONNECTION = BASE_URL + "connect";
        private const string RECONNECTION = BASE_URL + "reconnect";
        private const string MACHINE_COMPONENT_SETUP = BASE_URL + "machine/component_setup/";
        public const string MACHINE_ORDER_HEADER_DATA = BASE_URL + "machine/order/";
        public const string MACHINE_COMPONENT_REJECTION = BASE_URL + "machine/component_rejection/";
        public const string MACHINE_OK_NOK_Distribution = BASE_URL + "machine/ok_nok_distribution/";

        private bool isServerConnected = false;
        private bool isInitialized = false;
        private bool hasComponentSetupError = false;

        private string lastNotification = "";

        private Coroutine UpdateCoroutine;
        private Coroutine UpdateComponentRejectionCoroutine;

        public enum AutomaticUpdateMode
        {
            COMPONENT_SETUP,
            COMPONENT_SETUP_LIGHT,
            COMPONENT_REJECTION,
            COMPONENT_REJECTION_LIGHT
        };

        public void Initialize()
        {
            if(!isInitialized)
            {
                SpeechHandler.Instance.SpeakText("Hello");
                DownloadHandler.Instance.Initialize();

                isInitialized = true;
            }           
        }

        public void Connect()
        {
            GET(CONNECTION, new DownloadHandler.Callback(Process), new DownloadHandler.OnSuccess(OnConnectionEstablished), new DownloadHandler.OnFailed(OnConnectionFailed));
        }

        public void FetchMachineComponentSetupByMachineID(string machineId, int light = 0)
        {
            string url = MACHINE_COMPONENT_SETUP + machineId;

            if(light == 0)
            {
                GET(url, new DownloadHandler.Callback(Process), new DownloadHandler.OnSuccess(OnFetchMachineComponentSetupSuccessfull), new DownloadHandler.OnFailed(OnFetchMachineComponentSetupFailed));
                EnableAutomaticUpdate(url, AutomaticUpdateMode.COMPONENT_SETUP);
            }
            else
            {
                GET(url, new DownloadHandler.Callback(Process), new DownloadHandler.OnSuccess(OnFetchMachineComponentSetupLightSuccessfull), new DownloadHandler.OnFailed(OnFetchMachineComponentSetupLightFailed));
                //EnableAutomaticUpdate(url, AutomaticUpdateMode.COMPONENT_SETUP_LIGHT);
            }            
        }

        public void Fetch_OK_NOK_DisributionByMachineID(string machineId, int light = 0)
        {
            string url = MACHINE_OK_NOK_Distribution + machineId;

            GET(url, new DownloadHandler.Callback(Process), new DownloadHandler.OnSuccess(OnFetchMachineOKNOKDistributionSuccessfull), new DownloadHandler.OnFailed(OnFetchMachineOKNOKDistributionFailed));
        }

        private void OnFetchMachineOKNOKDistributionSuccessfull(WWW www)
        {
            Dictionary<string, string> ok_nokValues = JSONParser<Dictionary<string, string>>.Parse(www.text, "ok_nok_distribution");

            int ok = int.Parse(ok_nokValues["ok"]);
            int nok = int.Parse(ok_nokValues["nok"]);

            string request = ok_nokValues["request"];

            UIHandler.Instance.Visualize_OK_NOK_Distribution(ok, nok, request);
        }

        private void OnFetchMachineOKNOKDistributionFailed(WWW www)
        {
            print("OnFetchMachineOKNOKDistributionFailed");
        }

        private void OnFetchMachineOKNOKDistributionLightSuccessfull(WWW www)
        {
            Dictionary<string, string> ok_nokValues = JSONParser<Dictionary<string, string>>.Parse(www.text, "ok_nok_distribution");

            int ok = int.Parse(ok_nokValues["ok"]);
            int nok = int.Parse(ok_nokValues["nok"]);

            string request = ok_nokValues["request"];

            UIHandler.Instance.Visualize_OK_NOK_DistributionLight(ok, nok, request);
        }

        private void OnFetchMachineOKNOKDistributionLightFailed(WWW www)
        {
            print("OnFetchMachineOKNOKDistributionFailed");
        }

        public void FetchOrderHeaderDataByMachineID(string machineId)
        {
            string url = MACHINE_ORDER_HEADER_DATA + machineId;

            GET(url, new DownloadHandler.Callback(Process), new DownloadHandler.OnSuccess(OnFetchOrderHeaderDataSuccessfull), new DownloadHandler.OnFailed(OnFetchOrderHeaderDataFailed));
        }

        public void FetchComponentRejectionRateByMachineID(string machineId, int light = 0)
        {
            string url = MACHINE_COMPONENT_REJECTION + machineId;

            if (light == 0)
            {
                GET(url, new DownloadHandler.Callback(Process), new DownloadHandler.OnSuccess(OnFetchMachineComponentRejectionSuccessfull), new DownloadHandler.OnFailed(OnFetchMachineComponentRejectionFailed));
                EnableAutomaticUpdate(url, AutomaticUpdateMode.COMPONENT_REJECTION);
            }
            else
            {
                GET(url, new DownloadHandler.Callback(Process), new DownloadHandler.OnSuccess(OnFetchMachineComponentRejectionLightSuccessfull), new DownloadHandler.OnFailed(OnFetchMachineComponentRejectionLightFailed));
                //EnableAutomaticUpdate(url, AutomaticUpdateMode.COMPONENT_REJECTION_LIGHT);
            }
        }

        private void EnableAutomaticUpdate(string url, AutomaticUpdateMode mode)
        {
            DownloadHandler.OnSuccess onSuccess = null;
            DownloadHandler.OnFailed onFailed = null;

            switch (mode)
            {
                case AutomaticUpdateMode.COMPONENT_SETUP:
                    {
                        onSuccess = new DownloadHandler.OnSuccess(OnFetchMachineComponentSetupSuccessfull);
                        onFailed = new DownloadHandler.OnFailed(OnFetchMachineComponentSetupFailed);

                        UpdateCoroutine = StartCoroutine(UpdateProcessData(url, onSuccess, onFailed, 20.0f));
                    }
                    break;

                case AutomaticUpdateMode.COMPONENT_REJECTION:
                {
                    onSuccess = new DownloadHandler.OnSuccess(OnFetchMachineComponentRejectionSuccessfull);
                    onFailed = new DownloadHandler.OnFailed(OnFetchMachineComponentRejectionFailed);

                    UpdateComponentRejectionCoroutine = StartCoroutine(UpdateProcessData(url, onSuccess, onFailed, 90.0f));
                }
                break;

                case AutomaticUpdateMode.COMPONENT_SETUP_LIGHT:
                {
                    onSuccess = new DownloadHandler.OnSuccess(OnFetchMachineComponentSetupLightSuccessfull);
                    onFailed = new DownloadHandler.OnFailed(OnFetchMachineComponentSetupLightFailed);

                    UpdateCoroutine = StartCoroutine(UpdateProcessData(url, onSuccess, onFailed, 20.0f));
                }
                break;

                case AutomaticUpdateMode.COMPONENT_REJECTION_LIGHT:
                {
                    onSuccess = new DownloadHandler.OnSuccess(OnFetchMachineComponentRejectionLightSuccessfull);
                    onFailed = new DownloadHandler.OnFailed(OnFetchMachineComponentRejectionLightFailed);

                    UpdateComponentRejectionCoroutine = StartCoroutine(UpdateProcessData(url, onSuccess, onFailed, 90.0f));
                }
                break;
            }
        }

        private IEnumerator UpdateProcessData(string url, DownloadHandler.OnSuccess onSuccess, DownloadHandler.OnFailed onFailed, float timeInterval)
        {
            for(;;)
            {
                yield return new WaitForSeconds(timeInterval);

                GET(url, new DownloadHandler.Callback(Process), onSuccess, onFailed);
            }
        }

        public void DisableAutomaticUpdate(AutomaticUpdateMode mode)
        {
            switch(mode)
            {
                case AutomaticUpdateMode.COMPONENT_SETUP:
                case AutomaticUpdateMode.COMPONENT_SETUP_LIGHT:
                {
                    if (UpdateCoroutine != null)
                    {
                        StopCoroutine(UpdateCoroutine);
                        UpdateCoroutine = null;
                    }
                }
                break;

                case AutomaticUpdateMode.COMPONENT_REJECTION:
                case AutomaticUpdateMode.COMPONENT_REJECTION_LIGHT:
                {
                    if(UpdateComponentRejectionCoroutine != null)
                    {
                        StopCoroutine(UpdateComponentRejectionCoroutine);
                        UpdateComponentRejectionCoroutine = null;
                    }
                }
                break;
            }
                   
        }

        private void GET(string url, DownloadHandler.Callback callback, DownloadHandler.OnSuccess onSuccess, DownloadHandler.OnFailed onFailed)
        {
            DownloadHandler.Instance.Download(url, callback, onSuccess, onFailed);
        }

        private IEnumerator CheckServerConnection()
        {
            yield return new WaitForSeconds(30.0f);

            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateNotification("Checking Server Connection"));

            GET(RECONNECTION, new DownloadHandler.Callback(Process), new DownloadHandler.OnSuccess(OnReconnectionEstablished), new DownloadHandler.OnFailed(OnReconnectionFailed));
        }

        private void Process(WWW www, DownloadHandler.OnSuccess onSuccess, DownloadHandler.OnFailed onFailed)
        {
            if (www.error == null) onSuccess(www);
            else onFailed(www);
        }

        private void OnConnectionEstablished(WWW www)
        {
            SpeechHandler.Instance.SpeakText("Server Connection Established");

            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateNotification("Server Connection Established"));
            UIHandler.Instance.Update(() => UIHandler.Instance.InitializeProcessUI(JSONParser<Dictionary<string, string>[]>.Parse(www.text, "connect")));

            StartCoroutine(CheckServerConnection());
        }

        private void OnConnectionFailed(WWW www)
        {            
            SpeechHandler.Instance.SpeakText("Server Connection Failed! Retrying");
            lastNotification = "Server Connection Failed! Retrying";            

            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateNotification("Server Connection Failed!\nRetrying"));

            Connect();
        }

        private void OnReconnectionEstablished(WWW www)
        {
            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateNotification("Connected to Server"));

            StartCoroutine(CheckServerConnection());
        }

        private void OnReconnectionFailed(WWW www)
        {
            SpeechHandler.Instance.SpeakText("Server Connection Lost. Reconnecting");

            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateNotification("Server Connection Lost\nReconnecting..."));

            StartCoroutine(CheckServerConnection());
        }

        private void OnFetchMachineComponentSetupSuccessfull(WWW www)
        {
            string[] urlParts = www.url.Split('/');        
            string request = urlParts[urlParts.Length - 2];
            string id = urlParts[urlParts.Length - 1];

            Dictionary<string, string>[] componentSetup = JSONParser<Dictionary<string, string>[]>.Parse(www.text, request, id);

            if(componentSetup.Length > 0 && componentSetup[0].ContainsKey("error"))
            {
                hasComponentSetupError = true;
                
                UIHandler.Instance.UpdateEnablePushNotificationVisualization(componentSetup, "Duplicate Setup for " + request + ":", Color.red);
            }
            else
            {
                if (hasComponentSetupError)                
                    UIHandler.Instance.UpdateDisablePushNotificationVisualization();                

                hasComponentSetupError = false;
                UIHandler.Instance.Update(() => UIHandler.Instance.UpdateInformationBoxFillingLevelsWithComponentSetup(componentSetup));
            }
        }

        private void OnFetchMachineComponentSetupLightSuccessfull(WWW www)
        {
            string[] urlParts = www.url.Split('/');
            string request = urlParts[urlParts.Length - 2];
            string id = urlParts[urlParts.Length - 1];

            Dictionary<string, string>[] componentSetup = JSONParser<Dictionary<string, string>[]>.Parse(www.text, request, id);

            if (componentSetup.Length > 0 && componentSetup[0].ContainsKey("error"))
            {
                hasComponentSetupError = true;

                UIHandler.Instance.UpdateEnablePushNotificationVisualization(componentSetup, "Duplicate Setup for:", Color.red);
            }
            else
            {
                if (hasComponentSetupError)
                    UIHandler.Instance.UpdateDisablePushNotificationVisualization();

                hasComponentSetupError = false;

                UIHandler.Instance.Update(() => UIHandler.Instance.UpdateComponentSetupLightUI(componentSetup));
            }
        }

        private void OnFetchMachineComponentSetupFailed(WWW www)
        {
            print(www.text);
        }

        private void OnFetchMachineComponentSetupLightFailed(WWW www)
        {
            print(www.text);
        }

        private void OnFetchOrderHeaderDataSuccessfull(WWW www)
        {
            UIHandler.Instance.UpdateInformationBoxOrderHeaderData(JSONParser<string[]>.Parse(www.text, "order"));
        }

        private void OnFetchOrderHeaderDataFailed(WWW www)
        {
            print("order header data fetching failed");
        }

        private void OnFetchMachineComponentRejectionSuccessfull(WWW www)
        {
            Dictionary<string, string> componentRejectionRateDict = JSONParser<Dictionary<string, string>>.Parse(www.text, "component_rejection");

            string componentRejectionRate = componentRejectionRateDict["component_rejection"];

            int dotIndex = componentRejectionRate.IndexOf('.');            

            int floatingPointAccuracy = componentRejectionRate.Length < 4 ? componentRejectionRate.Length - 1 : 3;

            componentRejectionRate = componentRejectionRate.Substring(0, dotIndex + floatingPointAccuracy);

            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateComponentRejectionVisualization(componentRejectionRate));
        }

        private void OnFetchMachineComponentRejectionFailed(WWW www)
        {
            print(www.text);
        }

        private void OnFetchMachineComponentRejectionLightSuccessfull(WWW www)
        {
            Dictionary<string, string> componentRejectionRateDict = JSONParser<Dictionary<string, string>>.Parse(www.text, "component_rejection");

            string componentRejectionRate = componentRejectionRateDict["component_rejection"];

            int dotIndex = componentRejectionRate.IndexOf('.');

            int floatingPointAccuracy = componentRejectionRate.Length < 4 ? componentRejectionRate.Length - 1 : 3;

            componentRejectionRate = componentRejectionRate.Substring(0, dotIndex + floatingPointAccuracy);

            componentRejectionRateDict["component_rejection"] = componentRejectionRate;

            UIHandler.Instance.Update(() => UIHandler.Instance.UpdateComponentRejectionVisualizationLightUI(componentRejectionRateDict));
        }

        private void OnFetchMachineComponentRejectionLightFailed(WWW www)
        {
            print(www.text);
        }
    }
}