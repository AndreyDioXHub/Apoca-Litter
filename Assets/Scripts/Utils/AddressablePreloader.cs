using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.UI;

public class AddressablePreloader : MonoBehaviour
{
    public UnityEvent<float> LoadedPercentage;
    public UnityEvent OnLoadComplete;
    public UnityEvent OnLoadFailed;

    [SerializeField]
    private List<string> _groupTags = new List<string>() { "data", "bots" };
    private Dictionary<string, long> _sizesForDownloads = new Dictionary<string, long>();

    // Start is called before the first frame update
    void Awake() {
        StartCoroutine(PreloadAssets());
    }

    private IEnumerator PreloadAssets() {
        List<long> sizes = new List<long>();
        foreach (var item in _groupTags) {
            AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(item);
            yield return getDownloadSize;

            if(getDownloadSize.Result > 0) {
                _sizesForDownloads.Add(item, getDownloadSize.Result);
            }
        }

        float totalSize = 0;
        float downloadedSize = 0;
        
        // Подсчет общего размера загрузки
        foreach (var size in _sizesForDownloads.Values) {
            totalSize += size;
        }

        // Загрузка ассетов для каждого тега с размером больше 0
        foreach (var sizeEntry in _sizesForDownloads) {
            string tag = sizeEntry.Key;
            float currentAssetSize = sizeEntry.Value;
            AsyncOperationHandle downloadDependencies = Addressables.DownloadDependenciesAsync(tag);
            
            downloadDependencies.Completed += (AsyncOperationHandle handle) => {
                string errorhandle = CheckForError(handle);
                if(!string.IsNullOrEmpty(errorhandle)) {
                    //TODO Error
                    Debug.Log($"Error download asset: {tag}. Error text: {errorhandle}");
                    OnLoadFailed?.Invoke();
                }
                Addressables.Release(handle);
                Debug.Log($"Загружены ассеты для тега: {tag}");
                downloadedSize += currentAssetSize;
            };

            // Обновление прогресс-бара во время загрузки
            while (!downloadDependencies.IsDone) {
                float assetProgress = downloadDependencies.GetDownloadStatus().Percent;
                float currentProgress = (downloadedSize + (currentAssetSize * assetProgress)) / totalSize;
                LoadedPercentage.Invoke(currentProgress);
                
                yield return null;
            }
            
        }

        // Установка максимального значения после завершения всех загрузок
        LoadedPercentage.Invoke(1f);
        yield return new WaitForSeconds(0.5f);
        OnLoadComplete?.Invoke();
    }

    private string CheckForError(AsyncOperationHandle fromHandle) {
        if (fromHandle.Status != AsyncOperationStatus.Failed)
            return null;

        RemoteProviderException remoteException;
        System.Exception e = fromHandle.OperationException;
        while (e != null) {
            remoteException = e as RemoteProviderException;
            if (remoteException != null)
                return remoteException.WebRequestResult.Error;
            e = e.InnerException;
        }

        return null;
    }

    private void Start() {
        
    }
}
