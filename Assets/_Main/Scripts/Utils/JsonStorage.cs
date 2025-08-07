using System;
using System.IO;
using UnityEngine;

namespace PromVR.Utils
{
    public static class JsonStorage
    {
        private const string ROOT_DIRECTORY_NAME = "JsonStorage";

        /// <summary>
        /// Type of <c>T</c> must be Serializable
        /// </summary>
        public static bool TryLoad<T>(string fileName, out T loaded)
        {
            var filePath = GetFilePath(fileName);

            if (File.Exists(filePath))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    loaded = JsonUtility.FromJson<T>(json);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        $"Failed to load instance of '{nameof(T)}'"
                        + $" from file '{filePath}'.\nError: {e.Message}"
                    );
                }
            }

            loaded = default;
            return false;
        }

        /// <summary>
        /// Type of <c>T</c> must be Serializable
        /// </summary>
        public static bool TrySave<T>(T data, string fileName)
        {
            var json = JsonUtility.ToJson(data);
            var filePath = GetFilePath(fileName);

            try
            {
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Failed to save instance of '{nameof(T)}'"
                    + $" to file '{filePath}'.\nError: {e.Message}"
                );

                return false;
            }
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine(
                Application.persistentDataPath,
                ROOT_DIRECTORY_NAME,
                $"{fileName}.json"
            );
        }
    }
}
