using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetManager
{
    private static Dictionary<string, AsyncOperationHandle<IList<UnityEngine.Object>>> _labelHandles = new();
    private static Dictionary<string, UnityEngine.Object> _assetCache = new();

    // 라벨에 해당하는 모든 에셋을 로드합니다
    public static void LoadAssetByLabel(string label, Action<UnityEngine.Object> onAssetLoaded = null, Action onComplete = null)
    {
        if (_labelHandles.ContainsKey(label))
        {
            // 이미 로딩된 에셋
            Debug.LogWarning($"[{label}] already loaded.");
            return;
        }

        var handle = Addressables.LoadAssetsAsync<UnityEngine.Object>(label, asset =>
        {
            Debug.Log(asset);
            _assetCache[asset.name] = asset;
            onAssetLoaded?.Invoke(asset);
        });

        _labelHandles[label] = handle;

        handle.Completed += _ =>
        {
            Debug.Log($"[{label}] loading completed. Count: {handle.Result.Count}");
            onComplete?.Invoke();
        };
    }

    // 로드된 에셋을 반환합니다.
    public static T Get<T>(string assetName) where T : UnityEngine.Object
    {
        if (_assetCache.TryGetValue(assetName, out var obj))
            return obj as T;

        Debug.LogWarning($"Asset '{assetName}' not found in cache.");
        return null;
    }

    public static async Task<T> GetAsync<T>(string assetName) where T : UnityEngine.Object
    {
        if (_assetCache.TryGetValue(assetName, out var obj))
            return obj as T;

        var handle = Addressables.LoadAssetAsync<T>(assetName);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _assetCache[assetName] = handle.Result;
            return handle.Result;
        }

        Debug.LogWarning($"Asset '{assetName}' failed to load.");
        return null;
    }

    // public static Sprite GetSprite(string assetName)
    // {
    //     if (_assetCache.TryGetValue(assetName, out var obj))
    //     {
    //         var texture = obj as Texture2D;
    //         texture
    //     }
    // }

    // 라벨에 해당하는 모든 에셋을 해제합니다
    public static void ReleaseAssetByLabel(string label)
    {
        if (_labelHandles.TryGetValue(label, out var handle))
        {
            Addressables.Release(handle);
            _labelHandles.Remove(label);
            Debug.Log($"[{label}] released.");
        }
    }

    // 로딩된 전체 에셋을 해제합니다.
    public static void ReleaseAll()
    {
        foreach (var kvp in _labelHandles)
        {
            Addressables.Release(kvp.Value);
            Debug.Log($"[{kvp.Value}] released.");
        }
        _labelHandles.Clear();
    }

}
