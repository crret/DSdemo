using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using SG;
public class ABManager : SingletonBase<ABManager>
{
    public Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string,AssetBundle>();

    private AssetBundle mainAB;
    private AssetBundleManifest ABManifest;
    private Object obj;
    private string Path
    {
        get
        {
            return Application.streamingAssetsPath+"/";
        }
    }

    private string mainABName
    {
        get
        {
            return "PC";
        }
    }

    private void Start()
    {
        // GameObject cube=LoadResource<GameObject>("cube","Cube");
     //   GameObject cube;
     //   LoadResourceAsync("cube", "Cube", obj);
        /*if (cube!=null)
        {
            GameObject.Instantiate(cube);
        }*/
        
        /*AnimationEventsData[] animationEventsDatas=LoadAllResources("clipevent").Cast<AnimationEventsData>().ToArray();
        Debug.Log(animationEventsDatas.Length);*/
       
    }

    public void LoadAB(string abName)
    {
        if (mainAB == null)
        {
            mainAB=AssetBundle.LoadFromFile(Path+mainABName);
        }

        if (ABManifest == null)
        {
            ABManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

      
        string[] strs=ABManifest.GetAllDependencies(abName);
        foreach (var str in strs)
        {
            if (assetBundleDict.ContainsKey(str)==false)
            {   
                AssetBundle ab = AssetBundle.LoadFromFile(Path+str);
                assetBundleDict.Add(str,ab);
            }
        }
        
        if (assetBundleDict.ContainsKey(abName) == false)
        {
            AssetBundle ab=AssetBundle.LoadFromFile(Path+abName);
            assetBundleDict.Add(abName,ab);
        }

    }
    
    public void LoadABAsync(string abName,ref bool isLoadDown)
    {
        
        IEnumerator loadMainAB()
        {
            if (mainAB == null)
            {
                AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(Path + mainABName);
                yield return createRequest;
                mainAB = createRequest.assetBundle;
            }

            if (ABManifest == null)
            {
                AssetBundleRequest request = mainAB.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                yield return request;
                ABManifest = request.asset as AssetBundleManifest;
            }
            
            string[] strs=ABManifest.GetAllDependencies(abName);
            foreach (var str in strs)
            {
                if (assetBundleDict.ContainsKey(str) == false)
                {
                    AssetBundleCreateRequest tmpRequest = AssetBundle.LoadFromFileAsync(Path+str);
                    yield return tmpRequest;
                    assetBundleDict.Add(str,tmpRequest.assetBundle);
                }
            }

            if (assetBundleDict.ContainsKey(abName) == false)
            {
                AssetBundleCreateRequest tmpRequest2 = AssetBundle.LoadFromFileAsync(Path + abName);
                yield return tmpRequest2;
                assetBundleDict.Add(abName,tmpRequest2.assetBundle);
            }
            
        }
        
        StartCoroutine(loadMainAB());
    }
    IEnumerator loadMainAB(string abName)
    {
        if (mainAB == null)
        {
            AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(Path + mainABName);
            yield return createRequest;
            mainAB = createRequest.assetBundle;
        }

        if (ABManifest == null)
        {
            AssetBundleRequest request = mainAB.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
            yield return request;
            ABManifest = request.asset as AssetBundleManifest;
        }
            
        string[] strs=ABManifest.GetAllDependencies(abName);
        foreach (var str in strs)
        {
            if (assetBundleDict.ContainsKey(str) == false)
            {
                AssetBundleCreateRequest tmpRequest = AssetBundle.LoadFromFileAsync(Path+str);
                yield return tmpRequest;
                assetBundleDict.Add(str,tmpRequest.assetBundle);
            }
        }

        if (assetBundleDict.ContainsKey(abName) == false)
        {
            AssetBundleCreateRequest tmpRequest2 = AssetBundle.LoadFromFileAsync(Path + abName);
            yield return tmpRequest2;
            assetBundleDict.Add(abName,tmpRequest2.assetBundle);
        }
            
    }
    public T LoadResource<T>(string ABName, string resourceName) where T : UnityEngine.Object
    {
        LoadAB(ABName);

        T res = assetBundleDict[ABName].LoadAsset<T>(resourceName);

        return res;
    }

    public Object LoadResource(string ABName, string resourceName,System.Type type)
    {
        LoadAB(ABName);
        
        Object res= assetBundleDict[ABName].LoadAsset(resourceName,type);

        return res;
    }

    public Object[] LoadAllResources(string ABName)
    {
        LoadAB(ABName);
        Object[] res = assetBundleDict[ABName].LoadAllAssets();
        return res;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ABName"></param>
    /// <param name="resourceName"></param>
    /// <param name="obj">return object</param>
    public void LoadResourceAsync(string ABName, string resourceName,Object obj)
    {   
        
        IEnumerator loadAssetAsync()
        {
          yield return  StartCoroutine(loadMainAB(ABName)); 
            
          AssetBundleRequest req=assetBundleDict[ABName].LoadAssetAsync(resourceName);
          yield return req;
          obj = req.asset;
          GameObject.Instantiate(obj as GameObject);
        }
        
        StartCoroutine(loadAssetAsync());
    }
    
    public void LoadResourceAsync<T>(string ABName, string resourceName,T obj) where T : UnityEngine.Object
    {   
        LoadAB(ABName);
        
        IEnumerator loadAssetAsync()
        {
            
            AssetBundleRequest req=assetBundleDict[ABName].LoadAssetAsync<T>(resourceName);
            yield return req;
            obj = req.asset as T;
        }

        if (assetBundleDict.ContainsKey(ABName))
        {
            StartCoroutine(loadAssetAsync());
        }
    }
}

