using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{


    public class AnimatorManager : MonoBehaviour
    {   
        public Animator animator;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlayAnimation(string animName)
        {
            animator.CrossFadeInFixedTime(animName, 0.2f);
        }
    }
}