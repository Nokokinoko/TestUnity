using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InstantiateAddressables : MonoBehaviour
{
    [SerializeField] private AssetReference cube;
    [SerializeField] private AssetReferenceGameObject sphere;

    [Space]
    [SerializeField] private List<GameObject> listCube = new List<GameObject>();
    [SerializeField] private List<GameObject> listSphere = new List<GameObject>();

    private AsyncOperationHandle<GameObject> _cubeHandle;
    private AsyncOperationHandle<GameObject> _sphereHandle;

    private GameObject _loadedCube;
    private GameObject _loadedSphere;

    private async void Start()
    {
        Addressables.LoadAssetAsync<GameObject>(cube).Completed += (obj) =>
        {
            _cubeHandle = obj;
            _loadedCube = _cubeHandle.Result;
                
            listCube.Add(Instantiate(_loadedCube, new Vector3(0f, 0f, 0f), quaternion.identity));
            listCube.Add(Instantiate(_loadedCube, new Vector3(2f, 0f, 0f), quaternion.identity));
        };

        sphere.LoadAssetAsync<GameObject>().Completed += Loaded;

        await UniTask.Delay(TimeSpan.FromSeconds(5.0f), cancellationToken: this.GetCancellationTokenOnDestroy());
        
        Delete();
    }

    private void Loaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status != AsyncOperationStatus.Succeeded)
        {
            return;
        }

        _sphereHandle = obj;
        _loadedSphere = _sphereHandle.Result;
        
        listSphere.Add(Instantiate(_loadedSphere, new Vector3(0f, 2f, 0f), quaternion.identity));
        listSphere.Add(Instantiate(_loadedSphere, new Vector3(2f, 2f, 0f), quaternion.identity));
    }

    private void Delete()
    {
        listCube.ForEach(item => Destroy(item));
        listSphere.ForEach(item => Destroy(item));
        
        listCube.Clear();
        listSphere.Clear();

        Addressables.ReleaseInstance(_cubeHandle);
        Addressables.Release(_sphereHandle);
    }
}
