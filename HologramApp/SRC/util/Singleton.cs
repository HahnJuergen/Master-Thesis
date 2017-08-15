using UnityEngine;

{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool applicationIsQuitting = false;

        private static T instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting) return null;

                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1) return instance;

                        if (instance == null)
                        {
                            GameObject singleton = new GameObject();
                            instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);
                        }
                    }

                    return instance;
                }
            }
        }
       
        public void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}