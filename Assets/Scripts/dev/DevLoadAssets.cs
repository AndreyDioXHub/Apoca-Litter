using Cysharp.Threading.Tasks;
using NewBotSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DevLoadAssets : MonoBehaviour
{
    public TMP_Text log;
    public string key = "test";
    public string assetname = "ZombiSkin Variant";

    // Start is called before the first frame update
    public IEnumerator Start()
    {
        
        //Clear all cached AssetBundles
        // WARNING: This will cause all asset bundles to be re-downloaded at startup every time and should not be used in a production game
        // Addressables.ClearDependencyCacheAsync(key);

        //Check the download size
        AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(key);
        yield return getDownloadSize;

        log.text = $"Download size: {getDownloadSize.Result}\n";

        //If the download size is greater than 0, download all the dependencies.
        if (getDownloadSize.Result > 0) {
            AsyncOperationHandle downloadDependencies = Addressables.DownloadDependenciesAsync(key);
            downloadDependencies.Completed += (AsyncOperationHandle handle) => {
                DownloadComplete(handle);
            };
            yield return downloadDependencies;

            /*while (!downloadDependencies.IsDone && downloadDependencies.IsValid()) {
                DownloadStatus downloadStatus = downloadDependencies.GetDownloadStatus();
                log.text += $"\n{downloadStatus.Percent}";
            }/**/

            if (downloadDependencies.IsDone) {
                log.text += "Load Complete";
            } else {
                log.text += $"Load error: {downloadDependencies.Status}";
            }
        }

        //LoadDemoBundle();
        
    }

    private void DownloadComplete(AsyncOperationHandle handle) {
        Addressables.Release(handle);
        log.text += "\nLoaded";
        log.text += $"\n{handle.Result}";
    }

    /*private void Start() {
        LoadDemoBundle();
    }/**/
    private async void LoadDemoBundle() {
        string address = assetname;
        // Загрузка ассета по адресу
        //AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded) {
            GameObject loadedAsset = handle.Result;
            Debug.Log("Asset loaded successfully: " + loadedAsset.name);
            //GameObject _activeSkin = Instantiate(loadedAsset);
            log.text += "\nAsset loaded successfully: " + loadedAsset.name;


            //Addressables.Release(handle);
        } else {
            log.text += "\nFailed to load asset.";
            Debug.LogError("Failed to load asset.");
            //TODO
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
