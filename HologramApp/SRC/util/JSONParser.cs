using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

#region JSONParser
{
    public class JSONParser<T> : MonoBehaviour where T : class
    {
        public static T Parse(string response, string handle, string request="")
        {
            switch (handle)
            {
                case "connect": return _ParseConnectResponse(response) as T;
                case "component_setup": return _ParseComponentSetupResponse(response, request) as T;
                case "order": return _ParseOrderHeaderResponse(response) as T;
                case "component_rejection": return _ParseComponentRejectionResponse(response) as T;
                case "ok_nok_distribution": return _Parse_OK_NOK_DistributionResponse(response) as T;
            }

            return default(T);
        }

        private static Dictionary<string, string> _Parse_OK_NOK_DistributionResponse(string response)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            JSONNode root = JSON.Parse(response);

            d.Add("ok", root["ok"]);
            d.Add("nok", root["nok"]);
            d.Add("request", root["request"]);
        
            return d;
        }

        private static Dictionary<string, string>[] _ParseConnectResponse(string response)
        {
            JSONNode root = JSON.Parse(response);

            Dictionary<string, string>[] machineInformationDictionary = new Dictionary<string, string>[root["connect"].Count];

            for(int i = 0; i < root["connect"].Count; i++)
            {              
                machineInformationDictionary[i] = new Dictionary<string, string>();
                machineInformationDictionary[i].Add(root["connect"][i]["machine_id"], root["connect"][i]["machine_name"]);
            }

            return machineInformationDictionary;
        }       

        private static Dictionary<string, string>[] _ParseComponentSetupResponse(string response, string request)
        {
            JSONNode root = JSON.Parse(response);
            Dictionary<string, string>[] componentSetup = null;


            if (root["success"] == "false")
            {
                componentSetup = new Dictionary<string, string>[root["error"].Count];

                for (int i = 0; i < root["error"].Count; i++)
                {
                    string error = (root["error"][i]["bay"] + ": " + root["error"][i]["track"] + ", ");

                    componentSetup[i] = new Dictionary<string, string>();

                    componentSetup[i].Add("error", error);
                }
            }
            else if(root["success"] == "true")
            {
                componentSetup = new Dictionary<string, string>[root[request].Count + 1];

                componentSetup[0] = new Dictionary<string, string>();
                componentSetup[0].Add("id", request);

                for (int i = 0; i < root[request].Count; i++)
                {
                    componentSetup[i + 1] = new Dictionary<string, string>();

                    componentSetup[i + 1].Add("bay", root[request][i]["bay"]);
                    componentSetup[i + 1].Add("track", root[request][i]["track"]);
                    componentSetup[i + 1].Add("component_bc", root[request][i]["component_bc"]);
                    componentSetup[i + 1].Add("time", root[request][i]["time"]);
                }
            }            
            
            return componentSetup;
        }

        private static string[] _ParseOrderHeaderResponse(string response)
        {                        
            JSONNode root = JSON.Parse(response);

            return new string[]
            {
                root["order_bc"],
                root["product_bc"],
                root["product_name"],
                root["customer_name"],
                root["order_nominal_value"],
                root["revision"]
            };
        }

        private static Dictionary<string, string> _ParseComponentRejectionResponse(string response)
        {
            JSONNode root = JSON.Parse(response);

            Dictionary<string, string> d = new Dictionary<string, string>();

            d.Add("id", root["request"]);
            d.Add("component_rejection", root["component_rejection"]);

            return d;
        }
    }
}
#endregion
