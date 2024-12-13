using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AnimationModeTest : EditorWindow
{
    protected GameObject go;
    protected AnimationClip animationClip;
    protected float time = 0.0f;
    protected bool lockSelection = false;
    private AnimatorOverrideController runtimeController;
    private AnimatorController animatorController; // 动态创建的 AnimatorController
    private Animator animator;
    protected GameObject Model;
    [MenuItem("Examples/AnimationMode demo", false, 2000)]
    public static void DoWindow()
    {
        var window = GetWindowWithRect<AnimationModeTest>(new Rect(0, 0, 300, 300));
        window.Show();
    }

    public void OnSelectionChange()
    {
        if (!lockSelection)
        {
            go = Selection.activeGameObject;
            Repaint();
        }
    }

    private void OnEnable()
    {
        animator = null;
        AnimationMode.StopAnimationMode();
    }
    public void OnGUI()
    {
        if (Selection.activeGameObject == null)
        {
            
            EditorGUILayout.HelpBox("Please select a GameObject", MessageType.Info);
            
            return;
        }
        else if (Selection.activeGameObject != null&&Selection.activeGameObject.GetComponent<Animator>() != null)
        {    
            go=Selection.activeGameObject;
        }
        else
        {
            EditorGUILayout.HelpBox("Please select  Animtor", MessageType.Info);
        }
        
        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        GUILayout.Toggle(AnimationMode.InAnimationMode(), "Animate");
        if (EditorGUI.EndChangeCheck())
            ToggleAnimationMode();

        GUILayout.FlexibleSpace();
        lockSelection = GUILayout.Toggle(lockSelection, "Lock");
        GUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        animationClip = EditorGUILayout.ObjectField(animationClip, typeof(AnimationClip), false) as AnimationClip;
        if (animationClip != null)
        {
            float startTime = 0.0f;
            float stopTime = animationClip.length;
            time = EditorGUILayout.Slider(time, startTime, stopTime);
        }
        else if (AnimationMode.InAnimationMode())
        {   Debug.Log("Animation Mode");
            AnimationMode.StopAnimationMode();
        }
        EditorGUILayout.EndVertical();
    }

    void Update()
    {
        if (go == null || animationClip == null)
            return;

        animator = go.GetComponent<Animator>();
        if (animator == null)
            animator = go.AddComponent<Animator>();

        if (runtimeController == null)
        {
            CreateRuntimeController(animationClip);
        }

        animator.runtimeAnimatorController = runtimeController;

        // 播放动画并采样当前时间点
        animator.Play("Default", 0, time / animationClip.length);
        animator.Update(0);

        SceneView.RepaintAll();
    }

    void CreateRuntimeController(AnimationClip clip)
    {
        // 确保之前的资源已释放
        if (runtimeController != null)
            DestroyImmediate(runtimeController);
        if (animatorController != null)
            DestroyImmediate(animatorController);

        // 创建 AnimatorController
        animatorController = AnimatorController.CreateAnimatorControllerAtPath("Assets/TempAnimatorController.controller");
        var stateMachine = animatorController.layers[0].stateMachine;

        // 添加一个状态
        var state = stateMachine.AddState("Default");
        state.motion = clip;

        // 创建 AnimatorOverrideController
        runtimeController = new AnimatorOverrideController();
        runtimeController.runtimeAnimatorController = animatorController;
    }

    void ToggleAnimationMode()
    {
        if (AnimationMode.InAnimationMode())
        {
            Debug.Log("Exiting Animation Mode");
            AnimationMode.StopAnimationMode();
            Debug.Log("Animation Mode after Stop: " + AnimationMode.InAnimationMode());
        }
        else
        {
            Debug.Log("Entering Animation Mode");
            AnimationMode.StartAnimationMode();
            Debug.Log("Animation Mode after Start: " + AnimationMode.InAnimationMode());
        }
    }

    void OnDisable()
    {   AnimationMode.StopAnimationMode();
        Debug.Log("close");
        // 确保释放动态创建的资源
        if (runtimeController != null)
        {
            DestroyImmediate(runtimeController);
            animator.runtimeAnimatorController = null;
            runtimeController = null;
        }

        if (animatorController != null)
        {
        //  DestroyImmediate(animatorController);
            animatorController = null;
        }
    }
}
