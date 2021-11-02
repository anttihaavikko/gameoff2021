using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public static class Saver 
    {
        private static string Key => Application.productName + " Save";
        
        public static void Save(object data)
        {
            var json = JsonUtility.ToJson(data, true);
            PlayerPrefs.SetString(Key, json);
        }

        public static T Load<T>() where T : class
        {
            if (!PlayerPrefs.HasKey(Key)) return null;
            
            var json = PlayerPrefs.GetString(Key);
            return JsonUtility.FromJson<T>(json);
        }

        public static bool Exists()
        {
            return PlayerPrefs.HasKey(Key);
        }

        public static void Clear()
        {
            if (!PlayerPrefs.HasKey(Key)) return;
            
            PlayerPrefs.DeleteKey(Key);
        }
    }
}