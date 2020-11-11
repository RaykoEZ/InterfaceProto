using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using BlindChase.GameManagement;

namespace BlindChase.Animation
{
    public class AnimatorHelper : MonoBehaviour
    {
        [SerializeField] Animator m_animator = default;
        [SerializeField] SpriteRenderer m_spriteRenderer = default;
        [SerializeField] MotionHandler m_motionHandler = default;

        protected static readonly string AnimatorName = "AnimOverride";
        protected static readonly Dictionary<CommandTypes, string> m_animationStateNames = new Dictionary<CommandTypes, string>
        {
            {CommandTypes.NONE, "Idle" },
            {CommandTypes.ADVANCE, "OnAdvance" }
        };

        public void Init(string characterId)
        {
            m_spriteRenderer.enabled = false;
            Addressables.LoadAssetAsync<AnimatorOverrideController>($"{characterId}_{AnimatorName}").Completed += OnAnimAssetLoaded;
        }

        void SetAnimationClip(AnimatorOverrideController animator) 
        {
            if (animator == null) 
            {
                Debug.LogError("Null ref in animator setup for character.");
                return;
            }

            m_animator.runtimeAnimatorController = animator;
            m_spriteRenderer.enabled = true;
        }

        void OnAnimAssetLoaded(AsyncOperationHandle<AnimatorOverrideController> obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded && obj.Result != null)
            {
                SetAnimationClip(obj.Result);
            }
            else
            {
                Debug.LogError("Failed loading Character Animation Asset");
            }
        }

        public void TriggerAnimation(CommandTypes commandType, string triggerState, Action onFinished = null, MotionDetail motion = null) 
        {
            StartCoroutine(SetAnimationState(commandType, triggerState, onFinished, motion));
        }

        public void TriggerAnimation(string stateName, string triggerState, Action OnFinished = null, MotionDetail motion = null)
        {
            StartCoroutine(SetAnimationState(stateName, triggerState, OnFinished, motion));
        }

        IEnumerator SetAnimationState(CommandTypes commandType, string triggerName, Action onFinish = null, MotionDetail motion = null)
        {
            if (motion != null) 
            {
                m_motionHandler.StartMotion(m_animator, m_animationStateNames[commandType], motion.Destination);
            }

            m_animator.SetTrigger(triggerName);
            yield return new WaitUntil
                (
                () =>
                {
                    return m_animator.GetCurrentAnimatorStateInfo(0).IsName(m_animationStateNames[commandType]) &&
                            m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f;
                }
            );
            onFinish?.Invoke();
        }

        IEnumerator SetAnimationState(string stateName, string triggerName, Action onFinish = null, MotionDetail motion = null)
        {
            if (motion != null)
            {
                m_motionHandler.StartMotion(m_animator, stateName, motion.Destination);
            }

            m_animator.SetTrigger(triggerName);
            yield return new WaitUntil
                (
                () =>
                {
                    return m_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                            m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f;
                }
            );

            onFinish?.Invoke();
        }
    }

}


