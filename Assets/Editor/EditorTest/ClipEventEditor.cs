using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SG;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Object = System.Object;

public class ClipEventEditor : EditorWindow
{  
    
    public StyleSo TrackStyle;
    public StyleSo BoxStyle;
    public StyleSo addTrackStyle;
    public StyleSo timeStyle;
    public StyleSo curTimeLineStyle;
    public StyleSo deleteTrackStyle;
    public StyleSo setIKStyle;
    public StyleSo clipLabelStyle;
    private SingleExecutionManager executionManager = new SingleExecutionManager();
   // public GUIStyle TrackStyle2=new GUIStyle();
    private bool toggle1=true;
    private Rect toggleRect;
    private Rect rect1;
    public TracksList currentTracksList;
    public List<ToggleTrack> toggleTracks=new List<ToggleTrack>();

    private int timeStartPoint;
    private bool isDragging=false;
    private bool isResizingLeft=false;
    private bool isResizingRight=false;
    private bool isExecuted;
    private Vector2 scrollPosition = Vector2.zero;
    private Vector2 scrollPosition2 = Vector2.zero;
    private float edgeRange = 10f;
    
    private int toggleSwitchIndex = 0;
    private int[] toggleSwitchIndexList  = Enumerable.Repeat(-1, 100).ToArray();
    private SetIKEvent setIKEvent;
    private float oneMoveStep = 0f;

    
    private bool isDelPopVisible=false;
    private int selectedClipEventIndex;
    private bool isPopVisible = false;
   
    private Rect popUpRect;
    private Rect deletePopUpRect;
    private ClipEventType popUpEventType;
    public EventTypeList eventTypeList;

    private UnityAction trackDeleteAction=null;
    private List<int> tracksToDelete = new List<int>();
    private int intSliderValue;
    
    private float scrollbarValue;
    private Rect scrollbarRect;
    private bool isDraggingScrollbar=false;

    private bool dropDownBtn = false;
    private Rect dropDownMenuRect;
    
    public AnimationClip currentAnimationClip=null;
    public AnimationClip displayAnimationClip=null;
    public ClipEventsConfig clipEventsConfig;
    
    private Rect timeRect;
    private float currentTimePositionX;
    
    public ControlAnimationPlay controlAnimationPlay;
    public static Action LoadAnimationCallBack;
    public string defaultAnimatorClipName=null;

    private bool selectOn = false;
    [MenuItem("Window/ClipEventEditor")]
    public static void ShowExample()
    {
        ClipEventEditor wnd = GetWindow<ClipEventEditor>();
        wnd.titleContent = new GUIContent("ClipEvent Editor");
        wnd.position = new Rect(Screen.width / 2, Screen.height / 2, 1000, 600); 
        wnd.maxSize = new Vector2(1000, 600);
        wnd.minSize = new Vector2(1000, 600);
       
    }

    private void OnEnable()
    {
        currentAnimationClip = null;
        timeStartPoint = 203;
        controlAnimationPlay=FindObjectOfType<ControlAnimationPlay>();
        defaultAnimatorClipName = "a000_00000";
        
        /*string folderPath = "Assets/Editor/ClipTracks/";

        // 查找所有 ToggleTrack 类型的资源
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folderPath });

        Debug.Log($"Found {guids.Length} ToggleTrack assets");

        // 遍历所有找到的资源并加载
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log($"Asset Path: {assetPath}");

            ToggleTrack scriptableObject = AssetDatabase.LoadAssetAtPath<ToggleTrack>(assetPath);
            toggleTracks.Add(scriptableObject);
            if (scriptableObject != null)
            {
                Debug.Log("Loaded ToggleTrack: " + scriptableObject.name);
            }
            else
            {
                Debug.LogWarning("Failed to load ToggleTrack at: " + assetPath);
            }
        }*/

        /*foreach (var v in tmp)
        {
            toggleTracks.Add(v);
        }*/
    }
    
    public void OnGUI()
    {   
        Event e = Event.current;
        
        
        /*if (e.type == EventType.MouseDrag && scrollbarRect.Contains(e.mousePosition))
        {
            if (scrollbarValue >= 0)
            {
                scrollbarValue += e.delta.y;
            }
        }*/
        
        
        EditorGUILayout.BeginVertical();
      
        
        #region ScrollView1
        scrollPosition=EditorGUILayout.BeginScrollView(scrollPosition);

        #region  Timeline and ObjectField
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUIUtility.labelWidth = 60;
        
        
        selectOn = controlAnimationPlay==null?false:true;
        GUI.enabled = selectOn;
        displayAnimationClip = EditorGUILayout.ObjectField("Animation", displayAnimationClip, typeof(AnimationClip),
            false, GUILayout.Height(23), GUILayout.Width(199)) as AnimationClip;
        GUI.enabled = true;
        
        if (currentAnimationClip !=displayAnimationClip)
        {   
            currentAnimationClip = displayAnimationClip;
            if (EditorApplication.isPlaying&&currentAnimationClip!=null) //运行模式下，更新animator目前的动画片段，同时设置loadComplete为false
            {
                controlAnimationPlay.overrideController[defaultAnimatorClipName] = currentAnimationClip;
                controlAnimationPlay.loadAnimationComplete = false;
            }
            RefreshTrackDisplay();//更新currentTracksList
            if (currentTracksList != null)//更新之后不为null 指config中存在
            {
                if (currentTracksList.deltaPositionsLoaded)
                {
                    controlAnimationPlay.alreadyExist = true;
                    controlAnimationPlay.animationControl = true;
                    controlAnimationPlay.deltaPositions = currentTracksList.deltaPositions;
                }
                else
                {
                    controlAnimationPlay.alreadyExist = false;
                    controlAnimationPlay.animationControl = false;
                }
            }
        }

        #region PlayAnimation
        EditorGUILayout.BeginHorizontal();
      
        GUI.enabled = EditorApplication.isPlaying&&currentAnimationClip!=null;
        if(GUILayout.Button("Load Preview",GUILayout.Width(90),GUILayout.Height(20)))
        {
            if (controlAnimationPlay == null)
            {
                controlAnimationPlay=FindObjectOfType<ControlAnimationPlay>();
            }
            if (currentTracksList.deltaPositionsLoaded == false)
            {   Debug.Log(controlAnimationPlay);
                controlAnimationPlay.alreadyExist = false;
                controlAnimationPlay.overrideController[defaultAnimatorClipName] = currentAnimationClip;
                controlAnimationPlay.loadAnimationComplete = false;
                controlAnimationPlay.playFlag = true;
                controlAnimationPlay.loadAnimation = true;
            }
        }
        if (GUILayout.Button("<",GUILayout.Width(16),GUILayout.Height(20)))
        {
            
        }
        if (GUILayout.Button("Play",GUILayout.Width(50),GUILayout.Height(20)))
        {
            
        }
        if (GUILayout.Button(">",GUILayout.Width(16),GUILayout.Height(20)))
        {
            
        }
      
        EditorGUILayout.EndHorizontal();
        #endregion
      
        EditorGUILayout.EndVertical();
        GUI.enabled = true;
        
        if (timeRect.Contains(e.mousePosition) && (e.type == EventType.MouseDown||e.type == EventType.MouseDrag)&&clipEventsConfig.animationClips.Contains(currentAnimationClip))
        {   if (controlAnimationPlay == null)
            {   
                controlAnimationPlay=FindObjectOfType<ControlAnimationPlay>();
                if (currentTracksList.deltaPositionsLoaded)
                {
                    controlAnimationPlay.alreadyExist = true;
                    controlAnimationPlay.animationControl = true;
                    if (controlAnimationPlay.deltaPositions.Count == 0)
                    {
                        controlAnimationPlay.deltaPositions=currentTracksList.deltaPositions;
                    }
                }
                else
                {
                    controlAnimationPlay.alreadyExist = false;
                    controlAnimationPlay.animationControl = false;
                }
            }
            
            if (controlAnimationPlay.animationControl)
            {
                if ((int)Mathf.Round(currentTimePositionX) != (((int)Mathf.Round(e.mousePosition.x - timeRect.x)) / 10) * 10 + (int)timeRect.x)
                {
                    currentTimePositionX = (((int)Mathf.Round(e.mousePosition.x - timeRect.x)) / 10) * 10 + timeRect.x;
                    Repaint();
                }

                controlAnimationPlay.currentFrame = (int)Mathf.Round((currentTimePositionX - timeRect.x) / 10);
                if (currentTracksList.deltaPositionsLoaded == false)
                {
                    currentTracksList.deltaPositionsLoaded = true;
                    currentTracksList.deltaPositions = controlAnimationPlay.deltaPositions;
                    EditorUtility.SetDirty(currentTracksList);
                }
            }
            else if(currentTracksList.deltaPositionsLoaded==true)
            {   
                controlAnimationPlay.animationControl=true;
            }
        }
       
        GUILayout.Box("",timeStyle.style,GUILayout.Width(3000),GUILayout.Height(50));
        
        if (e.type == EventType.Repaint)
        {   timeRect=GUILayoutUtility.GetLastRect();
            executionManager.ExecuteOnce(1, () => { currentTimePositionX = timeRect.x; });
        }
        
        if (clipEventsConfig.animationClips.Contains(currentAnimationClip))
        {
            int totalFrame = (int)Mathf.Round(currentAnimationClip.frameRate * currentAnimationClip.length );
            GUI.Box(new Rect(timeRect.x,timeRect.y,totalFrame*10,timeRect.height),totalFrame.ToString(),curTimeLineStyle.style);
        }
        
        #region  Timeline 刻度
        for (int i = 0; i <= 300; i++)
        {
            float tickX = timeRect.x + i *10; // 计算每个刻度的 X 坐标
            float H = i % 5 == 0 ? 15 : 10;
            float offsetY= i%5==0?35:40;
            // 绘制刻度线
            GUI.DrawTexture(new Rect(tickX, timeRect.y+offsetY, 1, H), Texture2D.grayTexture);
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.black;
            if (i % 5 == 0&&i!=300)
            {
                GUI.Label(new Rect(tickX-5, timeRect.y +24, 30, 12), $"{i}",labelStyle);
            }
            else if (i == 300)
            {
                GUI.Label(new Rect(tickX-20, timeRect.y +24, 30, 12), $"{i}",labelStyle);
            }
        }
        #endregion
        
        EditorGUILayout.EndHorizontal();
        #endregion  Time 
        
        if (clipEventsConfig.animationClips.Contains(currentAnimationClip))
        {
            #region ScrollView2

            scrollPosition2 = EditorGUILayout.BeginScrollView(scrollPosition2, false, false, GUIStyle.none,
                GUIStyle.none, GUIStyle.none);

            EditorGUILayout.BeginHorizontal();

            #region DeleteTrackBtn and Tag and AddTrackBtn

            EditorGUILayout.BeginVertical(GUILayout.Width(200));


            for (int i = 0; i < toggleTracks.Count; i++)
            {
                if (GUILayout.Button("Delete Track ", deleteTrackStyle.style, GUILayout.Height(25),
                        GUILayout.Width(200)))
                {

                    tracksToDelete.Add(i); // 收集要删除的索引 
                }

                GUILayout.Box(toggleTracks[i].clipEventType.ToString(), setIKStyle.style, GUILayout.Height(73),
                    GUILayout.Width(200));

            }

            // 在循环结束后，按逆序删除
            for (int i = tracksToDelete.Count - 1; i >= 0; i--)
            {
                int trackIndex = tracksToDelete[i];

                for (int j = toggleTracks[trackIndex].toggleEvents.Count - 1; j >= 0; j--)
                {
                    string path = AssetDatabase.GetAssetPath(toggleTracks[trackIndex].toggleEvents[j]);
                    string path2 = AssetDatabase.GetAssetPath(toggleTracks[trackIndex].toggleEvents[j].clipEvent);
                    AssetDatabase.DeleteAsset(path2);
                    AssetDatabase.DeleteAsset(path);
                }

                string path3 = AssetDatabase.GetAssetPath(toggleTracks[trackIndex]);
                AssetDatabase.DeleteAsset(path3);
                toggleTracks.RemoveAt(trackIndex); // 删除 Track
                EditorUtility.SetDirty(currentTracksList);
                tracksToDelete.RemoveAt(i);

            }

            if (GUILayout.Button("AddTrack", addTrackStyle.style, GUILayout.Height(40), GUILayout.Width(200)))
            {
                /*/*ToggleTrack newTrack = new ToggleTrack();
                newTrack.toggleState = false;
                toggleTracks.Add(newTrack);#1#
                ToggleTrack newTrack = ScriptableObject.CreateInstance<ToggleTrack>();
                newTrack.toggleState = false;
                string path="Assets/Editor/ClipTracks/"+System.Guid.NewGuid().ToString()+".asset";
                toggleTracks.Add(newTrack);
                EditorUtility.SetDirty(tracksList);
                AssetDatabase.CreateAsset(newTrack, path);
                AssetDatabase.SaveAssets();

                // 刷新编辑器，以便显示更新后的资源
                AssetDatabase.Refresh();*/
                // 获取按钮的矩形区域

                // 弹出菜单
                GenericMenu menu = new GenericMenu();
                foreach (var v in eventTypeList.clipEventTypes)
                {
                    string tmp4 = v.ToString();
                    menu.AddItem(new GUIContent(tmp4), false, () =>
                    {
                        ToggleTrack newTrack = ScriptableObject.CreateInstance<ToggleTrack>();
                        newTrack.toggleState = false;
                        newTrack.clipEventType = v;
                        string path = "Assets/Editor/ClipTracks/" + System.Guid.NewGuid().ToString() + ".asset";
                        toggleTracks.Add(newTrack);
                        EditorUtility.SetDirty(currentTracksList);
                        AssetDatabase.CreateAsset(newTrack, path);
                        AssetDatabase.SaveAssets();

                        // 刷新编辑器，以便显示更新后的资源
                        AssetDatabase.Refresh();

                    });
                }

                // 显示菜单
                menu.DropDown(new Rect(dropDownMenuRect.x, dropDownMenuRect.y + dropDownMenuRect.height, 0, 0));
            }

            if (e.type == EventType.Repaint)
            {
                dropDownMenuRect = GUILayoutUtility.GetLastRect();
            }

            EditorGUILayout.EndVertical();

            #endregion DeleteTrackBtn and Tag

            #region Tracks

            EditorGUILayout.BeginVertical(GUILayout.Width(3000));

            //处理拖拽并保存帧 ,顺便消耗掉鼠标左键点击事件
            for (int i = 0; i < toggleTracks.Count; i++)
            {
                for (int j = 0; j < toggleTracks[i].toggleEvents.Count; j++)
                {

                    if (toggleTracks[i].toggleEvents[j].isSelected == true &&
                        deletePopUpRect.Contains(e.mousePosition) == false &&
                        popUpRect.Contains(e.mousePosition) == false)
                    {
                        Rect tmpRect = toggleTracks[i].toggleEvents[j].box;

                        HandleBoxDragging(e, ref tmpRect); //e.use()

                        toggleTracks[i].toggleEvents[j].box = tmpRect;
                        SaveFrameChange(toggleTracks[i].toggleEvents[j]);

                    }
                }

            }



            //如果点击的地方是已经选中的clipEvent，如果是右键，处理删除相关，并消耗鼠标点击事件
            for (int i = 0; i < toggleTracks.Count; i++)
            {
                for (int j = 0; j < toggleTracks[i].toggleEvents.Count; j++)
                {
                    if (toggleTracks[i].toggleEvents[j].box.Contains(e.mousePosition) &&
                        e.type == EventType.MouseDown &&
                        toggleTracks[i].toggleEvents[j].isSelected == true && e.button == 1)
                    {
                        Selection.activeObject = toggleTracks[i].toggleEvents[j].clipEvent;
                        deletePopUpRect = new Rect(e.mousePosition.x, e.mousePosition.y, 80, 30);
                        isDelPopVisible = true;
                        isPopVisible = false;
                        e.Use();
                    }
                    else if (e.type == EventType.MouseDown && deletePopUpRect.Contains(e.mousePosition) == false)
                    {
                        isDelPopVisible = false;
                        deletePopUpRect = Rect.zero;
                        Repaint();
                    }
                }
            }

            //处理clipEvent单选
            for (int i = 0; i < toggleTracks.Count; i++)
            {
                for (int j = 0; j < toggleTracks[i].toggleEvents.Count; j++)
                {

                    if (toggleTracks[i].toggleEvents[j].box.Contains(e.mousePosition) &&
                        e.type == EventType.MouseDown &&
                        toggleTracks[i].toggleEvents[j].isSelected == false &&
                        deletePopUpRect.Contains(e.mousePosition) == false &&
                        popUpRect.Contains(e.mousePosition) == false)
                    {
                        toggleTracks[i].toggleEvents[j].isSelected = true;
                        selectedClipEventIndex = j;
                        Selection.activeObject = toggleTracks[i].toggleEvents[j].clipEvent;

                        isPopVisible = false;
                        e.Use();
                        for (int k = 0; k < toggleTracks.Count; k++)
                        {
                            for (int l = 0; l < toggleTracks[k].toggleEvents.Count; l++)
                            {
                                if (!(k == i && l == j))
                                {
                                    toggleTracks[k].toggleEvents[l].isSelected = false;
                                }
                            }
                        }

                    }

                }
            }

            for (int i = 0; i < toggleTracks.Count; i++)
            {

                if (toggleTracks[i].rect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 1 &&
                    toggleSwitchIndex == i)
                {

                    Debug.Log("hi");
                    isPopVisible = true;
                    popUpEventType = toggleTracks[i].clipEventType;
                    Debug.Log(e.mousePosition);
                    popUpRect = new Rect(e.mousePosition.x, e.mousePosition.y, 150, 100);
                    e.Use();
                    Repaint();
                }
                else if (e.type == EventType.MouseDown &&
                         popUpRect.Contains(e.mousePosition) == false) //点击popUP时不马上置为false，否则会被下面的toggle把点击吞掉
                {
                    isPopVisible = false;
                    popUpRect = Rect.zero;
                    Repaint();
                }

                if (toggleTracks[i].rect.Contains(e.mousePosition) && e.type == EventType.MouseDown)
                {
                    Debug.Log("hi2");

                    toggleSwitchIndex = i;
                }

                GUI.enabled = !(isPopVisible || isDelPopVisible);
                EditorGUILayout.Toggle(toggleSwitchIndex == i, TrackStyle.style, GUILayout.Height(100));
                GUI.enabled = true;
                // 仅在 Repaint 事件中获取最后一个控件的 Rect

                if (Event.current.type == EventType.Repaint)
                {
                    toggleTracks[i].rect = GUILayoutUtility.GetLastRect();

                    int tmp = i;

                    foreach (var toggleEvent in toggleTracks[tmp].toggleEvents)
                    {
                        // toggleEvent.isSelected = false;
                        toggleEvent.box = new Rect(toggleEvent.clipEvent.StartFrame * 10 + timeStartPoint,
                            toggleTracks[tmp].rect.y,
                            (toggleEvent.clipEvent.EndFrame - toggleEvent.clipEvent.StartFrame) * 10,
                            toggleTracks[tmp].rect.height);
                    }
                    /*toggleTracks[tmp].toggleEvents.Add(new ToggleEvent(new Rect(toggleRect.x, toggleRect.y, toggleRect.width / 2+3, toggleRect.height),
                        false));

                    toggleTracks[tmp].toggleEvents.Add(new ToggleEvent(
                        new Rect(toggleRect.x+50, toggleRect.y, toggleRect.width / 2+3, toggleRect.height),
                        false));*/

                }

            }



            for (int i = 0; i < toggleTracks.Count; i++)
            {

                for (int j = toggleTracks[i].toggleEvents.Count - 1; j >= 0; j--)
                {
                    GUI.enabled = !(isDelPopVisible || isPopVisible);
                    EditorGUI.Toggle(toggleTracks[i].toggleEvents[j].box, toggleTracks[i].toggleEvents[j].isSelected,
                        BoxStyle.style);
                    EditorGUI.LabelField(toggleTracks[i].toggleEvents[j].box,toggleTracks[i].toggleEvents[j].clipEvent.GetTypeName(),clipLabelStyle.style);
                    GUI.enabled = true;
                }
                //   DrawScaleLines(3,1000,tmp,10,100);
            }

            for (int i = 0; i < toggleTracks.Count; i++)
            {
                for (int j = 0; j < toggleTracks[i].toggleEvents.Count; j++)
                {

                    if (toggleTracks[i].toggleEvents[j].isSelected)
                    {
                        GUI.enabled = !(isDelPopVisible || isPopVisible);
                        EditorGUI.Toggle(toggleTracks[i].toggleEvents[j].box,
                            toggleTracks[i].toggleEvents[j].isSelected, BoxStyle.style);
                        EditorGUI.LabelField(toggleTracks[i].toggleEvents[j].box,toggleTracks[i].toggleEvents[j].clipEvent.GetTypeName(),clipLabelStyle.style);
                        GUI.enabled = true;
                        if (isDragging)
                        {   
                            GUI.DrawTexture(new Rect(toggleTracks[i].toggleEvents[j].box.x, 0,1,toggleTracks[i].toggleEvents[j].box.y),Texture2D.whiteTexture);
                            GUI.DrawTexture(new Rect(toggleTracks[i].toggleEvents[j].box.x+toggleTracks[i].toggleEvents[j].box.width, 0,1,toggleTracks[i].toggleEvents[j].box.y),Texture2D.whiteTexture);
                        }
                        else if (isResizingLeft)
                        {
                            GUI.DrawTexture(new Rect(toggleTracks[i].toggleEvents[j].box.x, 0,1,toggleTracks[i].toggleEvents[j].box.y),Texture2D.whiteTexture);
                        }
                        else if (isResizingRight)
                        {
                            GUI.DrawTexture(new Rect(toggleTracks[i].toggleEvents[j].box.x+toggleTracks[i].toggleEvents[j].box.width, 0,1,toggleTracks[i].toggleEvents[j].box.y),Texture2D.whiteTexture);
                        }
                    }
                }
            }



            PopAddEvent(popUpEventType, toggleSwitchIndex, e);
            PopDeleteEvent(toggleSwitchIndex, selectedClipEventIndex);
          
            EditorGUILayout.EndVertical();

            #endregion Tracks

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();

            #endregion ScrollView2
        }
        else if(currentAnimationClip != null)
        {
            if(GUILayout.Button("Create Events Config",GUILayout.Width(200)))
            {
                clipEventsConfig.animationClips.Add(currentAnimationClip);
                TracksList tracksListSo=ScriptableObject.CreateInstance<TracksList>();
                tracksListSo.animationClip=currentAnimationClip;
                string path = "Assets/Editor/ClipTracksList/"+ System.Guid.NewGuid().ToString()+".asset";
                AssetDatabase.CreateAsset(tracksListSo,path);
                clipEventsConfig.tracksLists.Add(tracksListSo);
                EditorUtility.SetDirty(clipEventsConfig);
                AssetDatabase.SaveAssets();
                RefreshTrackDisplay();
            }
        }
        
        if (clipEventsConfig.animationClips.Contains(currentAnimationClip)&&currentTracksList.deltaPositionsLoaded&&EditorApplication.isPlaying)
        {
            GUI.DrawTexture(new Rect(currentTimePositionX,0,1,Screen.height),Texture2D.whiteTexture);
        }
        
        EditorGUILayout.EndScrollView();
        #endregion Scrollview1
      
        EditorGUILayout.EndVertical();
        
     

         /*GUI.VerticalScrollbar(new Rect(985,42,20,760),
            scrollbarValue,                     // 当前值
            20,                        // 滑块大小
            0,                                  // 最小值
            1000                     // 最大值
            // 设置滑条高度
        );*/
         
        
    }

    private void RefreshTrackDisplay()
    {
        
       currentTracksList=clipEventsConfig.tracksLists.FirstOrDefault(x=>x.animationClip==currentAnimationClip);
       if (currentTracksList!= null)
       {    
           toggleTracks = currentTracksList.tracksList;
       }
       
    }

    private void PopDeleteEvent(int trackNumber,int clipEventNumber)
    {
        if (isDelPopVisible)
        {
            GUILayout.BeginArea(deletePopUpRect);
            if(GUILayout.Button("Delete"))
            {

                string path = AssetDatabase.GetAssetPath(toggleTracks[trackNumber].toggleEvents[clipEventNumber]);
                string path2=AssetDatabase.GetAssetPath(toggleTracks[trackNumber].toggleEvents[clipEventNumber].clipEvent);
                AssetDatabase.DeleteAsset(path2);
                toggleTracks[trackNumber].toggleEvents.Remove(toggleTracks[trackNumber].toggleEvents[clipEventNumber]);
                EditorUtility.SetDirty(currentTracksList);
                AssetDatabase.DeleteAsset(path);
                isDelPopVisible = false;
                deletePopUpRect = Rect.zero;
                Repaint();
             //   GUI.FocusControl(null);
            }
            
            GUILayout.EndArea();
        }
    }
    private void PopAddEvent(ClipEventType clipEventType,int trackNum,Event e)
    {  
        if (isPopVisible)
        {
            GUILayout.BeginArea(popUpRect,GUI.skin.box);

           
            if (clipEventType == ClipEventType.SetIK)
            {
                foreach (var animEnum in eventTypeList.SetIKList)
                {
                    if (animEnum == SetIKType.SetRightIKOff)
                    { 
                        if (GUILayout.Button("SetRightIKOff"))
                        {   SetIKEvent clipEvent=ScriptableObject.CreateInstance<SetIKEvent>();
                            clipEvent.setIKType = SetIKType.SetRightIKOff;
                            clipEvent.clipEventType = ClipEventType.SetIK;
                            SetAndAddClipEventToAsset(clipEvent);
                        }
                    }
                    if(animEnum == SetIKType.SetLeftIKOff)
                    {
                        if (GUILayout.Button("SetLeftIKOff"))
                        {  
                            SetIKEvent clipEvent=ScriptableObject.CreateInstance<SetIKEvent>();
                            clipEvent.setIKType = SetIKType.SetLeftIKOff;
                            clipEvent.clipEventType = ClipEventType.SetIK;
                            SetAndAddClipEventToAsset(clipEvent);
                        }
                    }
                }
            }
            else if (clipEventType == ClipEventType.SetHeavySp)
            {
                foreach (var clipEnum in eventTypeList.SetHeavySpList)
                {
                    if (clipEnum == SetHeavySpType.SetHeavySp)
                    {
                        if (GUILayout.Button("SetHeavySp"))
                        {   
                           
                            SetHeavySpEvent clipEvent=ScriptableObject.CreateInstance<SetHeavySpEvent>();
                            Debug.Log(clipEvent.GetTypeName());
                            clipEvent.setHeavySpType = SetHeavySpType.SetHeavySp;
                            clipEvent.clipEventType = ClipEventType.SetHeavySp;
                            SetAndAddClipEventToAsset(clipEvent);
                        }
                    }
                    else if (clipEnum == SetHeavySpType.OpenRightDamageCollider)
                    {
                        if (GUILayout.Button("OpenRightDamageCollider"))
                        {   
                           
                            SetHeavySpEvent clipEvent=ScriptableObject.CreateInstance<SetHeavySpEvent>();
                            Debug.Log(clipEvent.GetTypeName());
                            clipEvent.setHeavySpType = SetHeavySpType.OpenRightDamageCollider;
                            clipEvent.clipEventType = ClipEventType.SetHeavySp;
                            SetAndAddClipEventToAsset(clipEvent);
                        }
                    }
                }
            }
            
 
            GUILayout.EndArea();
        }
    }

    private void SetAndAddClipEventToAsset(AnimationEventsData clipEvent)
    {    ToggleEvent mySO = ScriptableObject.CreateInstance<ToggleEvent>();
         clipEvent.animationClip = currentAnimationClip;
         clipEvent.StartFrame=(int)((popUpRect.x-timeStartPoint)/10);
         clipEvent.EndFrame=(int)((popUpRect.x-timeStartPoint)/10+10);
         clipEvent.stopCoroutineIfAnimExit = true;
         // 设置 ScriptableObject 的字段（如果需要）
         mySO.clipEvent = clipEvent;
         mySO.box = new Rect(  clipEvent.StartFrame*10 +timeStartPoint, toggleTracks[toggleSwitchIndex].rect.y,
             (clipEvent.EndFrame-clipEvent.StartFrame)*10,
             toggleTracks[toggleSwitchIndex].rect.height);
         
         toggleTracks[toggleSwitchIndex].toggleEvents.Add(mySO);
         EditorUtility.SetDirty( toggleTracks[toggleSwitchIndex]); 
         // 设置保存路径，.asset 文件将会被保存到 "Assets/MyAssets" 文件夹下
         string path1 = "Assets/AnimationClipEvents/"+currentAnimationClip.name+"_"+System.Guid.NewGuid().ToString()+".asset";
         string path2 = "Assets/Editor/ClipEventsOnTrack/"+currentAnimationClip.name+"_"+System.Guid.NewGuid().ToString()+".asset";
         // 确保目标路径的文件夹存在
         //  System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

         // 使用 AssetDatabase 创建 .asset 文件并保存
         AssetDatabase.CreateAsset(clipEvent, path1);
         AssetDatabase.CreateAsset(mySO, path2);
         // 保存所有的更改
         AssetDatabase.SaveAssets();

         // 刷新编辑器，以便显示更新后的资源
         AssetDatabase.Refresh();
         
         Repaint();
         isPopVisible = false;
         popUpRect = Rect.zero;
    }
    private void HandleBoxDragging(Event e,ref Rect curRect)
    {
        if (e.mousePosition.x - curRect.xMin < edgeRange && e.mousePosition.x - curRect.xMin >=0&&e.type == EventType.MouseDown&&e.button==0&&curRect.Contains(e.mousePosition))
        {
            isDelPopVisible = false;
            isPopVisible = false;
            isResizingLeft = true;
            e.Use();
        }
        else if (curRect.xMax-e.mousePosition.x < edgeRange && curRect.xMax-e.mousePosition.x  >= 0 &&
                 e.type == EventType.MouseDown&&e.button==0&&curRect.Contains(e.mousePosition))
        {   isDelPopVisible = false;
            isPopVisible = false;
            isResizingRight = true;
            e.Use();
        }
        else if (e.type == EventType.MouseDown &&e.button==0&& curRect.Contains(e.mousePosition))
        {
            if (isResizingLeft == false && isResizingRight == false)
            {   isDelPopVisible = false;
                isPopVisible = false;
                isDragging = true;
                e.Use();
            }
            // GUI.FocusControl(null); // 取消对控件的焦点
          
        }
        
        if (e.type == EventType.MouseUp)
        {  
            isDragging = false;
            isResizingLeft = false;
            isResizingRight = false;
            Repaint();
      //      GUI.FocusControl(null);
        }
        
        if (isDragging && e.type == EventType.MouseDrag&&e.button==0)
        {   
            if (curRect.xMin == timeStartPoint)
            {
                if (e.delta.x > 0)
                {
                    oneMoveStep += e.delta.x;
                    if (oneMoveStep >= 10)
                    {
                        oneMoveStep = 10;
                        curRect.position+= new Vector2(oneMoveStep,0);
                        oneMoveStep = 0;
                        e.Use(); 
                    }
                }
            }
            else if(curRect.xMin>=timeStartPoint+10)
            {
                oneMoveStep += e.delta.x;
                if (Mathf.Abs(oneMoveStep )>= 10)
                {
                    oneMoveStep = oneMoveStep > 10 ? 10 : -10;
                    curRect.position+= new Vector2(oneMoveStep,0);
                    oneMoveStep = 0;
                    e.Use(); 
                }
            }
         
        }
        else if (isResizingLeft && e.type == EventType.MouseDrag&&e.button==0)
        { 
            if (curRect.xMin+10 < curRect.xMax&&curRect.xMin>=timeStartPoint+10)
            {   oneMoveStep += e.delta.x;
                if (Mathf.Abs(oneMoveStep) >= 10)
                {   oneMoveStep = oneMoveStep >= 10 ? 10 : -10;
                    curRect.xMin += oneMoveStep;
                    oneMoveStep = 0;
                    e.Use();
                }
                
            }
            else if(curRect.xMin>=timeStartPoint+10)
            {   
                if (e.delta.x < 0)
                {   oneMoveStep += e.delta.x;
                    if (oneMoveStep <= -10)
                    {
                        oneMoveStep = -10;
                        curRect.xMin +=oneMoveStep;
                        oneMoveStep = 0;
                        e.Use();
                    }
                }
            }
            else if (curRect.xMin == timeStartPoint)
            {
                if (e.delta.x > 0)
                {
                    oneMoveStep += e.delta.x;
                    if (oneMoveStep >= 10)
                    {
                        oneMoveStep = 10;
                        curRect.xMin +=oneMoveStep;
                        oneMoveStep = 0;
                        e.Use();
                    }
                }
            }
        }
        else if(isResizingRight && e.type == EventType.MouseDrag&&e.button==0)
        {  
            if (curRect.xMax > curRect.xMin+10)
            {   oneMoveStep += e.delta.x;
                if (Mathf.Abs(oneMoveStep) >= 10)
                {   oneMoveStep = oneMoveStep >= 10 ? 10 : -10;
                    curRect.xMax += oneMoveStep;
                    oneMoveStep = 0;
                    e.Use();
                }
            }
            else
            {
                if (e.delta.x > 0)
                {   oneMoveStep += e.delta.x;
                    if (oneMoveStep >= 10)
                    {
                        oneMoveStep = 10;
                        curRect.xMax += oneMoveStep;
                        oneMoveStep = 0;
                        e.Use();
                    }
                }
            }
        }
    }

    private void SaveFrameChange(ToggleEvent toggleEvent)
    {
        toggleEvent.clipEvent.StartFrame = (int)(toggleEvent.box.xMin - timeStartPoint) / 10;
        toggleEvent.clipEvent.EndFrame = (int)(toggleEvent.box.xMax -timeStartPoint) / 10;
    }

    public void OnDisable()
    {
        
        for (int i = 0; i < toggleTracks.Count; i++)
        {

            for (int j = toggleTracks[i].toggleEvents.Count - 1; j >= 0; j--)
            {
                toggleTracks[i].toggleEvents[j].isSelected = false;
            }
            //   DrawScaleLines(3,1000,tmp,10,100);
        }

    }
}


public class SingleExecutionManager
{
    private readonly HashSet<int> executedOperations = new HashSet<int>();

    public void ExecuteOnce(int key, Action action)
    {
        if (!executedOperations.Contains(key))
        {
            action.Invoke(); // 执行传入的操作
            executedOperations.Add(key); // 记录已执行的操作
        }
    }
    
    
}


