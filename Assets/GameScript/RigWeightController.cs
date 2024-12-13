using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Linq;
namespace SG
{


    public class RigWeightController : MonoBehaviour
    {   public TwoBoneIKConstraint RightHandConstraint;
        public TwoBoneIKConstraint LeftHandConstraint;
        public GameObject RightHandTarget;
        public GameObject LeftHandTarget;
        public PlayerInventory playerInventory;
        public ActorController actorController;
        public InteractBoolList interactBoolList;
        public WeaponSlotManager weaponSlotManager;
        // Start is called before the first frame update
        public float weight;

        private float dampVec1;
        private float dampVec2;
        private void Start()
        {
            weaponSlotManager = GetComponent<WeaponSlotManager>();
            playerInventory=GetComponentInParent<PlayerInventory>();
            interactBoolList=GetComponentInParent<InteractBoolList>();
            actorController=GetComponent<ActorController>();
            weight = 0f;
        }
        // Update is called once per frame
        void Update()
        {
            if (interactBoolList.InAttack.Any(func => func()))
            {
                weight=Mathf.SmoothDamp(weight,0f,ref dampVec1,Time.deltaTime*10f);
            }
            else
            {
                weight=Mathf.SmoothDamp(weight,1f,ref dampVec2,Time.deltaTime*30f);
            }
            
            if (playerInventory.curRightWeapon.weaponID == 6250)
            {
                if (actorController.isTwoHand)
                {
                    Transform bothRightHandRigTargetTransform = weaponSlotManager.rightHandSlot.currentWeaponModel.transform.Find("BothRightHandRigTargetTransform");
                    Transform bothLeftHandRigTargetTransform = weaponSlotManager.rightHandSlot.currentWeaponModel.transform.Find("BothLeftHandRigTargetTransform");
                    RightHandTarget.transform.position = bothRightHandRigTargetTransform.position;
                    RightHandTarget.transform.rotation = bothRightHandRigTargetTransform.rotation;
                    LeftHandConstraint.transform.position = bothLeftHandRigTargetTransform.position;
                    LeftHandConstraint.transform.rotation = bothLeftHandRigTargetTransform.rotation;
                    RightHandConstraint.weight=weight;
                    LeftHandConstraint.weight =weight;
                }
                else
                {
                    Transform rightHandRigTargetTransform = weaponSlotManager.rightHandSlot.currentWeaponModel.transform.Find("RightHandRigTargetTransform");
                    RightHandTarget.transform.position = rightHandRigTargetTransform.position;
                    RightHandTarget.transform.rotation = rightHandRigTargetTransform.rotation;
                    RightHandConstraint.weight = weight;
                    LeftHandConstraint.weight = Mathf.Lerp(LeftHandConstraint.weight,0f,Time.deltaTime);
                }
              
            }
            else
            {
                RightHandConstraint.weight=Mathf.Lerp(RightHandConstraint.weight,0f,Time.deltaTime*20f);
                LeftHandConstraint.weight = Mathf.Lerp(LeftHandConstraint.weight,0f,Time.deltaTime*20f);
            }
        }
    }
}