using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SG
{


    [CreateAssetMenu(menuName = "Item/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPefab;
        public bool isUnarmed;
        public int weaponID;
        [Header("Idle Animations")] 
        public string Right_Hand_Idle;
        public string Left_Hand_Idle;
        public string Two_Hand_Idle;
        public string Right_Hand_Move;
        public string Right_Hand_Run;
        public string Two_Hand_Move;
        public string Two_Hand_Run;
        
        [Header("Right Hand Attack Animations")]
        public string RH_Light_Attack_01;
        public string RH_Light_Attack_02;
        public string RH_Light_Attack_03;
        public string RH_LightDash;
        public string RH_HeavySubStart_Attack;
        public string RH_HeavyStart_Attack_01;
        public string RH_HeavyEnd_Attack_01;
        public string RH_HeavyStart_Attack_02;
        public string RH_HeavyEnd_Attack_02;
        public string RH_LightStep;

        [Header("Left Hand Attack Animations")]
        public string LH_Light_Attack_01;
        public string LH_Light_Attack_02;
        
        public string Change;
        [Header("Two Hand Attack Animations")]
        public string TH_Light_Attack_01;
        public string TH_Light_Attack_02;
        public string TH_Light_Attack_03;
        public string TH_Light_Dash;
        public string TH_HeavySubStart_Attack;
        public string TH_HeavyStart_Attack_01;
        public string TH_HeavyEnd_Attack_01;
        public string TH_HeavyStart_Attack_02;
        public string TH_HeavyEnd_Attack_02;
        public string TH_LightStep;
        
        
        
        [Header("Stamina Costs")]
        public int baseStamina;

        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;
        
    }
}