using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class AnimationEventEditor : EditorWindow
{   
    public StyleSo TrackStyle;
    
    private List<AnimationClip> clips = new List<AnimationClip>();
    private VisualElement clipList;
    private ListView listView;
    private VisualElement rightPanel;
    private Label infoLabel;
    private ScrollView eventsScrollView;
    private Vector2 ScrollglobalPosition;
    private AnimationClip selectedClip;
    private Button AddTrackBtn;
    private IMGUIContainer trackContainer;
    private int toggleCount = 0;
    
    [MenuItem("Window/UI Toolkit/AnimationEventEditor")]
    public static void ShowExample()
    {
        AnimationEventEditor wnd = GetWindow<AnimationEventEditor>();
        wnd.titleContent = new GUIContent("AnimationEventEditor");
    }

    public void CreateGUI()
    {
        
        VisualElement root = rootVisualElement;

      
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/AnimationEventEditor/AnimationEventEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        listView=root.Q<ListView>("ListView");
        rightPanel = root.Q<VisualElement>("RightPanel");
        infoLabel = rightPanel.Q<Label>("InfoLabel");
        eventsScrollView = rightPanel.Q<ScrollView>("EventsScrollView");
        AddTrackBtn =rightPanel.Q<Button>("AddNewTrack");
        AddTrackBtn.clicked += AddToggle;
        // 添加帧更新来持续跟踪位置变化
        eventsScrollView.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            // 获取全局位置
            ScrollglobalPosition = eventsScrollView.worldBound.position;
        });
        
        //  LoadAnimationClips();
        trackContainer=rightPanel.Q<IMGUIContainer>("IMGUITrack");
        trackContainer.onGUIHandler+= () =>
        {
            EditorGUILayout.Toggle(true,TrackStyle.style,GUILayout.Height(50));
            Rect toggleRect = GUILayoutUtility.GetLastRect();
            Debug.Log(toggleRect);
        };
        
        LoadAnimationClips();

    }

    private void OnGUI()
    {
  
    }

    private void AddToggle()
    {
      
    }

    private void LoadAnimationClips() 
    {
        // 清空之前的列表
        clips.Clear();
        
        // 设置搜索的文件夹路径（相对于项目根目录）
        string folderPath = "Assets/ds3anim"; 

        // 使用 AssetDatabase 查找 Assets 文件夹中的所有 AnimationClip
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip",new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip != null)
            {
                clips.Add(clip);
                Debug.Log(clip.name);
            }
        }
        
        listView.itemsSource = clips;
        // listView.itemHeight = 20;
        listView.makeItem = () => new Label();
        listView.bindItem = (element, i) => (element as Label).text = clips[i].name;
        listView.selectionType = SelectionType.Single;
        
        listView.onSelectionChange += OnAnimationClipSelected;

        listView.style.flexGrow = 1;
    }

    private void OnAnimationClipSelected(IEnumerable<object> obj)
    {
        selectedClip = obj?.FirstOrDefault() as AnimationClip;
        DisplaySelectedClipEvents();
    }

    private void DisplaySelectedClipEvents()
    {   
      //  Debug.Log(selectedClip.name);
      infoLabel.text = $"Events for: {selectedClip.name}";
      eventsScrollView.Clear();
      var animationEvents = AnimationUtility.GetAnimationEvents(selectedClip);
      
      foreach (var animEvent in animationEvents)
      {
          var eventElement = new VisualElement();
       //   eventElement.style.flexDirection = FlexDirection.Row;
       //   eventElement.style.marginBottom = 5;

          var timeField = new FloatField("Time");
          timeField.value = animEvent.time;
          timeField.isReadOnly = true;
          eventElement.Add(timeField);

          var functionNameField = new TextField("Function Name");
          functionNameField.value = animEvent.functionName;
          functionNameField.isReadOnly = true;
          eventElement.Add(functionNameField);

          eventsScrollView.Add(eventElement);
      }
      
    }
}