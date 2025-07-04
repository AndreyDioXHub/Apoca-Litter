using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace Utils 
{
    public class DataLoader {
        public static string RemotePath { get; set; } = "https://backroomshooter.s3.eu-north-1.amazonaws.com/";
        public static string Subfolder => SubfolderVariants.TryGetValue(Application.platform, out string subfolder) ? subfolder : "StandaloneWindows64/";

        private static Dictionary<string, string> CachedData = new Dictionary<string, string>();
        public static bool UseCachingData { get; set; } = true;

        private static readonly Dictionary<RuntimePlatform, string> SubfolderVariants = new Dictionary<RuntimePlatform, string>
       {
            { RuntimePlatform.Android, "Android/" },
            { RuntimePlatform.IPhonePlayer, "ios/" },
            { RuntimePlatform.WindowsPlayer, "StandaloneWindows64/" }
        };

        private static readonly Dictionary<string, UniTaskCompletionSource<object>> _loadingTasks = new Dictionary<string, UniTaskCompletionSource<object>>();

        /// <summary>
        /// Чтение существующего json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T ReadJson<T>(string fileName) {
            string path = Path.Combine(Application.persistentDataPath, fileName);

            if (!File.Exists(path)) {
                Debug.LogError($"Файл не найден: {path}");
                return default(T);
            }

            string jsonContent = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(jsonContent);
        }

        /// <summary>
        /// Чтение json из Addressables
        /// </summary>
        /// <typeparam name="T">Тип объекта для десериализации</typeparam>
        /// <param name="addressableKey">Ключ ресурса в Addressables</param>
        /// <returns>Десериализованный объект или default(T) в случае ошибки</returns>
        public static async Task<T> LoadJson<T>(string addressableKey) {
            if (UseCachingData && CachedData.TryGetValue(addressableKey, out var rawjson)) {
                return JsonConvert.DeserializeObject<T>(rawjson);
            }
            try {
                var textAssetHandle = Addressables.LoadAssetAsync<TextAsset>(addressableKey);
                await textAssetHandle.Task;

                if (textAssetHandle.Status == AsyncOperationStatus.Succeeded) {
                    string jsonContent = textAssetHandle.Result.text;
                    if (UseCachingData) {
                        CachedData[addressableKey] = jsonContent;
                    }
                    Addressables.Release(textAssetHandle);
                    return JsonConvert.DeserializeObject<T>(jsonContent);
                }

                Debug.LogError($"Не удалось загрузить ресурс: {addressableKey}");
                return default(T);
            }
            catch (Exception ex) {
                Debug.LogError($"Ошибка при загрузке json из Addressables: {ex.Message}");
                return default(T);
            }
        }

        public static string GetFileHash(string filepath) {
            using (var md5 = System.Security.Cryptography.MD5.Create()) {
                using (var stream = File.OpenRead(Path.Combine(Application.persistentDataPath, filepath))) {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static string GetStringHash(string data) {
            using (var md5 = System.Security.Cryptography.MD5.Create()) {
                byte[] hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        public static async UniTask<T> LoadJsonFile<T>(string name) {
            //Cache
            if (UseCachingData && CachedData.TryGetValue(name, out var rawjson)) {
                return JsonConvert.DeserializeObject<T>(rawjson);
            }
            string localPath = Path.Combine(Application.persistentDataPath, name);
            DateTime localFileDate = File.Exists(localPath) ? File.GetLastWriteTime(localPath) : DateTime.MinValue;
            string remote = $"{RemotePath}{Subfolder}{name}";
            Debug.Log($"<color=red>Remote path</color> = {remote}");

            // Проверяем, загружается ли файл в данный момент
            if (_loadingTasks.TryGetValue(name, out var existingTask)) {
                await existingTask.Task;
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(localPath));
            }

            var taskCompletionSource = new UniTaskCompletionSource<object>();
            _loadingTasks[name] = taskCompletionSource;

            try {
                using (UnityWebRequest request = UnityWebRequest.Get(remote)) {
                    await request.SendWebRequest().ToUniTask();

                    if (request.result != UnityWebRequest.Result.Success) {
                        Debug.LogError($"Ошибка загрузки: {request.error}");
                        if (File.Exists(localPath)) {
                            string jsonContent = await File.ReadAllTextAsync(localPath);
                            return JsonConvert.DeserializeObject<T>(jsonContent);
                        }
                        return default(T);
                    }

                    // Получаем дату последней модификации из заголовков
                    string lastModifiedStr = request.GetResponseHeader("Last-Modified");
                    DateTime remoteFileDate = !string.IsNullOrEmpty(lastModifiedStr)
                        ? DateTime.Parse(lastModifiedStr, System.Globalization.CultureInfo.InvariantCulture)
                        : DateTime.MaxValue;

                    if (remoteFileDate > localFileDate) {
                        string jsonContent = request.downloadHandler.text;
                        if (UseCachingData) {
                            CachedData[name] = jsonContent;
                        }
                        Debug.Log("Remote file is newer. Update local file.");
                        await File.WriteAllTextAsync(localPath, jsonContent);
                        taskCompletionSource.TrySetResult(jsonContent);
                        return JsonConvert.DeserializeObject<T>(jsonContent);
                    } else if (File.Exists(localPath)) {
                        string jsonContent = await File.ReadAllTextAsync(localPath);
                        if (UseCachingData) {
                            CachedData[name] = jsonContent;
                        }
                        taskCompletionSource.TrySetResult(jsonContent);
                        return JsonConvert.DeserializeObject<T>(jsonContent);
                    } else {
                        Debug.LogError($"Локальный файл не найден: {localPath}");
                        taskCompletionSource.TrySetResult(null);
                        return default(T);
                    }
                }
            }
            catch (Exception ex) {
                Debug.LogError($"Ошибка при загрузке json с сервера: {ex.Message}");
                if (File.Exists(localPath)) {
                    string jsonContent = await File.ReadAllTextAsync(localPath);
                    taskCompletionSource.TrySetResult(jsonContent);
                    return JsonConvert.DeserializeObject<T>(jsonContent);
                }
                taskCompletionSource.TrySetResult(null);
                return default(T);
            }
            finally {
                _loadingTasks.Remove(name);
            }
        }
    }
}