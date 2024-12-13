using System;
using System.Collections;
using System.Collections.Generic;



using UnityEngine;
using UnityEngine.Animations.Rigging;


public class ControlAnimationPlay : MonoBehaviour
{
    public Animator animator;
    public AnimatorOverrideController overrideController;
    public AnimationClip currentClip;
    public float progressTime;
    private float receiveTime;
    public GameObject model;
    public Transform rootTransform;
    public Rigidbody rigidBody;
    
    
    public List<Vector3> deltaPositions;
    public Vector3 lastPosition;
    
    public int totalFrames;
    public int inputFrame;
    public int currentFrame;
    public int lastFrame;
    
   

    private float relaytime;
    
    private float accumulatedTime;

    public bool playFlag = false;

    public bool animationControl = false;
    
    public bool loadAnimation = false;
    
    public bool loadAnimationComplete = false;
    
    public bool alreadyExist = false;
    private void Awake()
    { 
        overrideController.runtimeAnimatorController=animator.runtimeAnimatorController;
        animator.runtimeAnimatorController=overrideController;
        
    }

    private void Start()
    {
           
           totalFrames=(int)Mathf.Round(currentClip.frameRate*currentClip.length);
           accumulatedTime = 0;
           playFlag = false;
           animationControl = false;
           Time.fixedDeltaTime=1/currentClip.frameRate;
           lastFrame=currentFrame;
           lastPosition=rootTransform.position;
    }
    
    private void Update()
    {
        if (playFlag)
        {
            playFlag=false;
            animator.Play("Default",0);
        }
     
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1&&loadAnimation==true&&animator.GetCurrentAnimatorStateInfo(0).IsName("Default"))
        {   Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime );
            loadAnimation = false;
            loadAnimationComplete = true;
            animationControl = true;
        }
   
        if (animationControl)
        {
            animator.speed = 0;
            animator.Play("Default",0, (float)currentFrame/(float)totalFrames);
        }
        Time.fixedDeltaTime=1/currentClip.frameRate;
    }

    
    private void OnAnimatorMove()
    {   
        
        if (loadAnimation&&alreadyExist==false)
        {   
            accumulatedTime+=Time.fixedDeltaTime;
            if (accumulatedTime >= 1 / currentClip.frameRate)
            {
                deltaPositions.Add(rootTransform.position-lastPosition);
                lastPosition=rootTransform.position;
                accumulatedTime=0;
            }
        }

        currentFrame=Mathf.Clamp(currentFrame,0,deltaPositions.Count>0?deltaPositions.Count-1:0);
        lastFrame=Mathf.Clamp(lastFrame,0,deltaPositions.Count>0?deltaPositions.Count-1:0);
        if (animationControl == false)
        {
            rigidBody.position += animator.deltaPosition;
            
        }
        else
        {
            if (currentFrame != lastFrame)
            {
                if (currentFrame > lastFrame)
                {
                    for (int i = lastFrame; i <= currentFrame; i++)
                    {
                        rigidBody.position += deltaPositions[i];
                    }
                }
                else
                {
                    for (int i = lastFrame; i >= currentFrame; i--)
                    {
                        rigidBody.position -= deltaPositions[i];
                    }
                }
                lastFrame = currentFrame;
            }
        }
    //    Debug.Log(animator.deltaPosition.ToString("F5")+" "+rootTransform.position.ToString("F5")+" "+lastPosition.ToString("F5")
    //    +" "+animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    private void FixedUpdate()
    {
        
    }
    private void OnDisable()
    {
        
    }
}
