using System;
using System.Collections;
using UnityEngine;

namespace BlindChase.Animation
{
    public class MotionDetail 
    {
        public Vector3 Destination { get; private set; }

        public MotionDetail(Vector3 destination) 
        {
            Destination = destination;
        }
    }

    public class MotionHandler : MonoBehaviour
    {
        // During animations that move the object, this is used for an animation clip to override 
        // and tell how far we are from the target position.
        // This is a NORMALIZED value
        [SerializeField] float m_motionProgress = 0.0f;

        void ResetMotion()
        {
            m_motionProgress = 0.0f;
        }

        public void StartMotion( Animator anim, string stateName, Vector3 destination) 
        {
            StartCoroutine(OnMotion(anim, stateName, destination));
        }

        IEnumerator OnMotion(Animator anim, string stateName, Vector3 destination) 
        {
            Func<bool> endCond =
                () =>
                {
                    return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                            anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f;
                }; 

            while (!endCond.Invoke()) 
            {
                Vector3 newPos = Vector3.Lerp(transform.position, destination, m_motionProgress);
                newPos.z = transform.position.z;
                transform.position = newPos;
                yield return null;
            }

            Vector3 end = Vector3.Lerp(transform.position, destination, 1.0f);
            end.z = transform.position.z;
            transform.position = end;
            // Reset Progress
            ResetMotion();
        }
    }

}


