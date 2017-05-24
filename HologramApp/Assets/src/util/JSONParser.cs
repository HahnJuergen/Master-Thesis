using System.Collections.Generic;
using SimpleJSON;


#region JSONParser
namespace de.ur.juergenhahn.ma
{
    public class JSONParser<T> where T : class
    {
        public static T ParseToDictionary(string response, string handle)
        {
            switch (handle)
            {
                case "connect": return _ParseConnectResponse(response) as T;
                case "dummy": return _ParseDummyResponse(response) as T;
            }

            return default(T);
        }

        private static Dictionary<string, string> _ParseConnectResponse(string response)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            ret.Add("connect", JSON.Parse(response)["connect"].Value);

            return ret;
        }

        private static Dictionary<string, string> _ParseDummyResponse(string response)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            ret.Add("dummy", JSON.Parse(response)["dummy"].Value);

            /* JSON Array Iteration Sample

            string jdummy = @"{""data"":[{""obj"":""objv""}, {""obj1"":""obj1v""}]";

            JSONNode rootNode = JSON.Parse(jdummy);

            for(int i = 0; i < rootNode["data"].Count; i++)
            {
                print(rootNode["data"][i]);
            }
            */

            return ret;
        }
    }
}
#endregion
