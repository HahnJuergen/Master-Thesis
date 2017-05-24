using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.ur.juergenhahn.ma
{
    #region ServerCorrespondence
    public class ServerCorrespondence : MonoBehaviour
    {
        private const string BASE_URL = "http://localhost:8080/request/";
        private const string CONNECTION = BASE_URL + "connect";

        private bool isServerConnected = false;
        private bool isConnecting = false;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(EstablishServerConnection());
            StartCoroutine(HandleUserInput());
            StartCoroutine(HandleUserLocalization());
        }

        private void GET(string url, Callback callback)
        {
            StartCoroutine(WaitForGetRequest(new WWW(url), callback));
        }

        #region ConcurrentCoroutines
        private IEnumerator WaitForGetRequest(WWW getWWW, Callback callback)
        {
            yield return getWWW;

            callback(getWWW);
        }

        private IEnumerator EstablishServerConnection()
        {
            yield return null;

            GET(CONNECTION, new Callback(Connect));
        }

        private IEnumerator CheckServerConnection()
        {
            yield return new WaitForSeconds(5.0f);

            print("Checking Connection...");

            GET(CONNECTION, new Callback(Reconnect));
        }

        private IEnumerator HandleUserInput()
        {
            for (;;)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    GET(BASE_URL + "dummy/dummy", new Callback(Dummy));

                yield return null;
            }
        }

        private IEnumerator HandleUserLocalization()
        {
            yield return new WaitForSeconds(5.0f);

            GET(BASE_URL + "localization", new Callback(Localize));
        }
        #endregion

        #region CallbackFunctions
        public delegate void Callback(WWW www);


        private void Connect(WWW www)
        {
            isConnecting = true;
            print("Connecting...");

            if (www.error == null)
            {
                string response = JSONParser<Dictionary<string, string>>.ParseToDictionary(www.text, "connect")["connect"];
                print("WWW: " + response);
                print("Connection Established");
                isServerConnected = true;
                isConnecting = false;
                StartCoroutine(CheckServerConnection());
            }
            else
            {
                isServerConnected = false;
                print("Connection Failed...");
                print("Retrying...");
                StartCoroutine(EstablishServerConnection());
            }
        }

        private void Reconnect(WWW www)
        {
            if (www.error != null)
            {
                isServerConnected = false;
                isConnecting = false;
                print("Connection Lost...");
                print("Reconnecting...");
                StartCoroutine(EstablishServerConnection());
            }
            else
            {
                print("Connection: " + JSONParser<Dictionary<string, string>>.ParseToDictionary(www.text, "connect")["connect"]);
                StartCoroutine(CheckServerConnection());
            }
        }

        private void Dummy(WWW www)
        {
            if (www.error == null)
            {
                Dictionary<string, string> result = JSONParser<Dictionary<string, string>>.ParseToDictionary(www.text, "dummy");
                print(result["dummy"]);
            }
            else
                print(www.error);
        }

        private void Localize(WWW www)
        {
            if(www.error == null)
            {
                print(www.text);
            }
            else
            {
                print(www.error);
            }

            StartCoroutine(HandleUserLocalization());
        }
        #endregion
    }
    #endregion
}