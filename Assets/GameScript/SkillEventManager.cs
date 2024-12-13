using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SG
{
    public class SkillEventManager : MonoBehaviour
    {
        public ActorController actorController;
        public WeaponSlotManager weaponSlotManager;
        public IKtest iktest;
        public SetIKEvent setIKEvent;
        public AnimationEventsData[] animationEvents;
        public Dictionary<AnimationClip,List<AnimationEventsData>> AnimationEventsDict = new Dictionary<AnimationClip, List<AnimationEventsData>>();

        public Animator animator;
        public AnimatorStateInfo stateInfo;
        public AnimatorClipInfo[] currentClipInfo;
        public AnimatorClipInfo[] nextClipInfo;
        
        public Dictionary<AnimationEventsData,UnityAction<IEnumerator>> StopCoroutineDict = new Dictionary<AnimationEventsData,UnityAction<IEnumerator>>();
        public List<AnimationEventsData> coroutineRemoveList=new List<AnimationEventsData>();
        
       
        /*private Dictionary<AnimEventType,Dictionary<AnimEvent,Func<AnimationEventsData,IEnumerator>>> clipEventFuncDict = new Dictionary<AnimEventType,Dictionary<AnimEvent,Func<AnimationEventsData,IEnumerator>>>();
        
        
        private void InitClipEventFuncDict()
        {
            clipEventFuncDict.Add(AnimEventType.SetIK,new Dictionary<AnimEvent,Func<AnimationEventsData,IEnumerator>>());

            foreach (var v in clipEventFuncDict)
            {
                if (v.Key == AnimEventType.SetIK)
                {
                    v.Value.Add(AnimEvent.SetRightIK,SetRightIKOFF);
                }
            }
        }*/
            
        private void Awake()
        {
            animationEvents = ABManager.Instance.LoadAllResources("clipevent").Cast<AnimationEventsData>().ToArray();
           
            InitAnimationEventsIEnumerator(animationEvents);
            actorController=GetComponentInChildren<ActorController>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            nextClipInfo = animator.GetNextAnimatorClipInfo(0);
            
            StopAnimationEvents();
        }

        private void StopCoroutineAddListener(AnimationEventsData clipEvent)
        {
            if (!StopCoroutineDict.ContainsKey(clipEvent))
            {
                StopCoroutineDict.Add(clipEvent, (x) =>
                {
                    clipEvent.isCoroutineRunning = false;
                    StopCoroutine(x);
                    if (coroutineRemoveList.Contains(clipEvent) == false)
                    {
                        coroutineRemoveList.Add(clipEvent);
                    }
                    
                });
            }
            
        }
        
        public void StopAnimationEvents()
        {

            foreach (var v in StopCoroutineDict)
            {   
                if (currentClipInfo.Any((x) => x.clip.name == v.Key.animationClip.name) == false&&
                    nextClipInfo.Any((x) => x.clip.name == v.Key.animationClip.name)==false&&
                    coroutineRemoveList.Contains(v.Key)==false)
                {                   

                    v.Value.Invoke(v.Key.EventCoroutine);
                }
            }

            foreach (var v in coroutineRemoveList)
            {
                if (StopCoroutineDict.ContainsKey(v))
                {
                    StopCoroutineDict.Remove(v);
                }
            }
            coroutineRemoveList.Clear();
        }
        
        public void ExecuteCoroutine(AnimationEventsData clipEvent)
        {

            if (clipEvent.isCoroutineRunning == false)
            {
                if (clipEvent.stopCoroutineIfAnimExit)
                {
                    if (StopCoroutineDict.ContainsKey(clipEvent) == false)
                    {
                        StopCoroutineAddListener(clipEvent);
                    }
                }

                if (clipEvent is SetIKEvent tmpSetIKEvent)
                {
                    tmpSetIKEvent.EventCoroutine =tmpSetIKEvent.SetIKFunc(tmpSetIKEvent);
                }
                else if(clipEvent is SetHeavySpEvent tmpSetHeavySpEvent)
                {
                    tmpSetHeavySpEvent.EventCoroutine = tmpSetHeavySpEvent.SetHeavySpFunc(tmpSetHeavySpEvent);
                }
         
                StartCoroutine(clipEvent.EventCoroutine);
                
            }
            else
            {
                if (StopCoroutineDict.ContainsKey(clipEvent) == false)
                {
                    StopCoroutineDict[clipEvent].Invoke(clipEvent.EventCoroutine);
                }
            }
            
        }
        
        
        
        public void ExecuteAnimationEvents(AnimationClip animationClip)
        {
            foreach (var pr in AnimationEventsDict)
            {
                if (pr.Key.name == animationClip.name) //应该不能用名字匹配
                {   
                    foreach (var clipEvent in pr.Value)
                    {   
                        ExecuteCoroutine(clipEvent);
                    }
                }
               
            }
        }

        public void InitAnimationEventsIEnumerator(AnimationEventsData[] clipEvents)
        {
            foreach (var clipEvent in clipEvents)
            {

                if (clipEvent.clipEventType == ClipEventType.SetIK)
                {
                    if (clipEvent is SetIKEvent tmpSetIKEvent)
                    {
                        if (tmpSetIKEvent.setIKType == SetIKType.SetLeftIKOff)
                        {
                            tmpSetIKEvent.SetIKFunc = SetLeftIKOFF;
                            
                        }
                        else if (tmpSetIKEvent.setIKType == SetIKType.SetRightIKOff)
                        {
                            tmpSetIKEvent.SetIKFunc = SetRightIKOFF;
                        }
                    }
                }
                
                else if (clipEvent.clipEventType == ClipEventType.SetHeavySp)
                {
                    if (clipEvent is SetHeavySpEvent tmpSetHeavySpEvent)
                    {
                        if (tmpSetHeavySpEvent.setHeavySpType==SetHeavySpType.SetHeavySp)
                        {
                            tmpSetHeavySpEvent.SetHeavySpFunc = SetHeavySp;
                        }
                        else if (tmpSetHeavySpEvent.setHeavySpType == SetHeavySpType.OpenRightDamageCollider)
                        {
                            tmpSetHeavySpEvent.SetHeavySpFunc = OpenRightDamageCollider;
                        }
                    }
                }
                
                if (!AnimationEventsDict.ContainsKey(clipEvent.animationClip))//
                {   
                    AnimationEventsDict.Add(clipEvent.animationClip, new List<AnimationEventsData>());
                }
                AnimationEventsDict[clipEvent.animationClip].Add(clipEvent);
                
                /*
                if (clipEvent.clipEventCellType==ClipEventCellType.SetRightIK)
                {   clipEvent.AnimFunc=SetRightIKOFF;
                  //  animationEvent.EventCoroutine = SetRightIKOFF(animationEvent);
                    if (!AnimationEventsDict.ContainsKey(clipEvent.animationClip))//
                    {   
                        AnimationEventsDict.Add(clipEvent.animationClip, new List<AnimationEventsData>());
                    }
                    AnimationEventsDict[clipEvent.animationClip].Add(clipEvent);
                }
                else if (clipEvent.clipEventCellType == ClipEventCellType.SetLeftIK)
                {
                    clipEvent.AnimFunc=SetLeftIKOFF;
                    if (!AnimationEventsDict.ContainsKey(clipEvent.animationClip))//
                    {   
                        AnimationEventsDict.Add(clipEvent.animationClip, new List<AnimationEventsData>());
                    }
                    AnimationEventsDict[clipEvent.animationClip].Add(clipEvent);
                }
                else if (clipEvent.clipEventCellType == ClipEventCellType.SetHeavySp)
                {
                    clipEvent.AnimFunc=SetHeavySp;
                    if (!AnimationEventsDict.ContainsKey(clipEvent.animationClip))//
                    {   
                        AnimationEventsDict.Add(clipEvent.animationClip, new List<AnimationEventsData>());
                    }
                    AnimationEventsDict[clipEvent.animationClip].Add(clipEvent);
                }*/
            }

        }

    
        
        
        public IEnumerator SetRightIKOFF(SetIKEvent animationEvent)
        {   animationEvent.isCoroutineRunning = true;
            float frameRate =animationEvent.animationClip.frameRate;
            float startTime = 1 / frameRate * animationEvent.StartFrame;
            float endTime = 1 / frameRate * animationEvent.EndFrame;
            if (animationEvent.StartFrame == 0)
            {
                iktest.SetRightFootIKOff();
                yield return new  WaitForSeconds(endTime);
                animationEvent.isCoroutineRunning = false;
                // iktest.SetRightFootIKOn();
            }
            else
            {
                yield return new  WaitForSeconds(startTime);
                Debug.Log("SetRightFootIKOff "+Time.time);
                //iktest.SetRightFootIKOff();
                yield return new  WaitForSeconds(endTime-startTime);
                Debug.Log("SetRightFootIKOn "+Time.time);
                // iktest.SetRightFootIKOn();
               
            }
            animationEvent.isCoroutineRunning = false;
            //  yield return null;
        }
        public IEnumerator SetLeftIKOFF(AnimationEventsData animationEvent)
        {   animationEvent.isCoroutineRunning = true;
            float frameRate =animationEvent.animationClip.frameRate;
            float startTime = 1 / frameRate * animationEvent.StartFrame;
            float endTime = 1 / frameRate * animationEvent.EndFrame;
            if (animationEvent.StartFrame == 0)
            {
                //iktest.SetRightFootIKOff();
                yield return new  WaitForSeconds(endTime);
                animationEvent.isCoroutineRunning = false;
                // iktest.SetRightFootIKOn();
            }
            else
            {
                yield return new  WaitForSeconds(startTime);
                Debug.Log("SetLeftFootIKOff "+Time.time);
                //iktest.SetRightFootIKOff();
                yield return new  WaitForSeconds(endTime-startTime);
                Debug.Log("SetLeftFootIKOn "+Time.time);
                // iktest.SetRightFootIKOn();
               
            }
            animationEvent.isCoroutineRunning = false;
            //  yield return null;
        }
        
        
        public IEnumerator SetHeavySp(AnimationEventsData animationEvent)
        {   animationEvent.isCoroutineRunning = true;
            float frameRate =animationEvent.animationClip.frameRate;
            float startTime = 1 / frameRate * animationEvent.StartFrame;
            float endTime = 1 / frameRate * animationEvent.EndFrame;
            if (animationEvent.StartFrame == 0)
            {
                //iktest.SetRightFootIKOff();
                actorController.SetHeavyAttackSpTrue();
                Debug.Log("SetHeavySpTrue"+Time.time);
                yield return new  WaitForSeconds(endTime);
                actorController.SetHeavyAttackSpFalse();
                Debug.Log("SetHeavySpFalse"+Time.time);
                animationEvent.isCoroutineRunning = false;
                // iktest.SetRightFootIKOn();
            }
            else
            {   
                yield return new  WaitForSeconds(startTime);
                actorController.SetHeavyAttackSpTrue();
                Debug.Log("SetHeavySpTrue"+Time.time);
                //iktest.SetRightFootIKOff();
                yield return new  WaitForSeconds(endTime-startTime);
                actorController.SetHeavyAttackSpFalse();
                Debug.Log("SetHeavySpFalse"+Time.time);
                // iktest.SetRightFootIKOn();
               
            }
            animationEvent.isCoroutineRunning = false;
            //  yield return null;
        }

        public IEnumerator OpenRightDamageCollider(AnimationEventsData animationEvent)
        {   animationEvent.isCoroutineRunning = true;
            float frameRate =animationEvent.animationClip.frameRate;
            float startTime = 1 / frameRate * animationEvent.StartFrame;
            float endTime = 1 / frameRate * animationEvent.EndFrame;
            if (animationEvent.StartFrame == 0)
            {
                weaponSlotManager.OpenRightHandDamageCollider();
                Debug.Log("OpenRightHandDamageCollider"+Time.time);
                yield return new  WaitForSeconds(endTime);
                weaponSlotManager.CloseRightHandDamageCollider();
                Debug.Log("CloseRightHandDamageCollider"+Time.time);
                animationEvent.isCoroutineRunning = false;
           
            }
            else
            {   
                yield return new  WaitForSeconds(startTime);
                weaponSlotManager.OpenRightHandDamageCollider();
                Debug.Log("OpenRightHandDamageCollider"+Time.time);
               
                yield return new  WaitForSeconds(endTime-startTime);
                weaponSlotManager.CloseRightHandDamageCollider();
                Debug.Log("CloseRightHandDamageCollider"+Time.time);

            }
            animationEvent.isCoroutineRunning = false;
          
        }
    }
    
    
}