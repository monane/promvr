using System;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;

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
        public static async UniTask<T> LoadAsync<T>(string fileName)
        {
            T output = default;

            var filePath = GetFilePath(fileName);

            if (File.Exists(filePath))
            {
                await UniTask.SwitchToThreadPool();

                try
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    output = JsonUtility.FromJson<T>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        $"Failed to load instance of '{typeof(T)}'"
                        + $" from file '{filePath}'.\nError: {e.Message}"
                    );
                }
                finally
                {
                    await UniTask.SwitchToMainThread();
                }
            }

            return output;
        }

        /// <summary>
        /// Type of <c>T</c> must be Serializable
        /// </summary>
        public static async UniTaskVoid SaveAsync<T>(T data, string fileName)
        {
            await UniTask.SwitchToThreadPool();

            var json = JsonUtility.ToJson(data);
            var filePath = GetFilePath(fileName);

            try
            {
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Failed to save instance of '{typeof(T)}'"
                    + $" to file '{filePath}'.\nError: {e.Message}"
                );
            }
            finally
            {
                await UniTask.SwitchToMainThread();
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
