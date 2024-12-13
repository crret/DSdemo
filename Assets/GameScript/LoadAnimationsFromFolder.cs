using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;

public class LoadAnimationsFromFolder : MonoBehaviour
{      // AssetBundle的URL或本地路径
    public string assetBundlePath = "Assets/AssetBundles/PC/animations"; // 可以是远程URL或本地路径
    public ActorController actorController;
    public PlayerInventory playerInventory;
    public  AnimationClip[] AnimationClips{ get; private set; } 
    // 存储加载的动画片段，字典的键是文件的名字
    public static Dictionary<string, AnimationClip> AnimationClipsDict =new Dictionary<string, AnimationClip>();

    public bool loadAnimComplete=false;
    // Start is called before the first frame update
    void Start()
    {   actorController = GetComponentInChildren<ActorController>();
        playerInventory = GetComponent<PlayerInventory>();
        loadAnimComplete=false;
        StartCoroutine(DownloadAndLoadAssetBundle(assetBundlePath));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator DownloadAndLoadAssetBundle(string path)
    {
        AssetBundle bundle;
        if (ABManager.Instance.assetBundleDict.ContainsKey("animations")==false)
        {
            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(path);

            // 等待AssetBundle加载完成
            yield return bundleRequest;
            bundle = bundleRequest.assetBundle;
        }
        else
        {
            bundle = ABManager.Instance.assetBundleDict["animations"];
        }
        

        if (bundle != null)
        {
            Debug.Log("AssetBundle successfully loaded.");

            // 获取AssetBundle中所有的AnimationClip
            AnimationClips = bundle.LoadAllAssets<AnimationClip>();

            // 将每个AnimationClip加载到字典中
            foreach (AnimationClip clip in AnimationClips)
            {
                // 使用文件名（不带扩展名）作为键
                string clipName = clip.name;
                if (!LoadAnimationsFromFolder.AnimationClipsDict.ContainsKey(clipName))
                {
                    LoadAnimationsFromFolder.AnimationClipsDict.Add(clipName, clip);
                   // Debug.Log("Loaded AnimationClip: " + clipName);
                }
                else
                {
                   // Debug.LogWarning("Duplicate AnimationClip found: " + clipName);
                }
            }

            // 使用完后释放AssetBundle资源，保留加载的动画片段
            bundle.Unload(false);
            loadAnimComplete = true;
            StartCoroutine(actorController.OverrideWeaponAnim(playerInventory.curRightWeapon, false, false));
        }
        else
        {
            Debug.LogError("Failed to load AssetBundle from path: " + path);
        }
    }
}
