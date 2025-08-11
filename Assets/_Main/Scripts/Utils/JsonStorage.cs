using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace PromVR.Utils
{
    public static class JsonStorage
    {
        private const string ROOT_DIRECTORY_NAME = "JsonStorage";

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            var rootDirectoryPath = Path.Combine(
                Application.persistentDataPath,
                ROOT_DIRECTORY_NAME
            );

            Directory.CreateDirectory(rootDirectoryPath);
        }

        /// <summary>
        /// Type of <c>T</c> must be Serializable
        /// </summary>
        public static async Awaitable<T> LoadAsync<T>(string fileName, JsonSerializerSettings serializerSettings = null)
        {
            T output = default;

            var filePath = GetFilePath(fileName);

            if (File.Exists(filePath))
            {
                await Awaitable.BackgroundThreadAsync();

                try
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    output = JsonConvert.DeserializeObject<T>(json, serializerSettings);
                }
                catch (Exception e)
                {
                    await Awaitable.MainThreadAsync();

                    Debug.LogError(
                        $"Failed to load instance of '{typeof(T)}'"
                        + $" from file '{filePath}'.\nError: {e.Message}"
                    );
                }
            }

            return output;
        }

        /// <summary>
        /// Type of <c>T</c> must be Serializable
        /// </summary>
        public static async Awaitable SaveAsync<T>(T data, string fileName)
        {
            var filePath = GetFilePath(fileName);

            await Awaitable.BackgroundThreadAsync();

            try
            {
                var json = JsonUtility.ToJson(data);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception e)
            {
                await Awaitable.MainThreadAsync();

                Debug.LogError(
                    $"Failed to save instance of '{typeof(T)}'"
                    + $" to file '{filePath}'.\nError: {e.Message}"
                );
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
