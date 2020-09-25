using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using BlindChase;

namespace BlindChase.Animation
{
    public class AnimatorSetupHelper : MonoBehaviour
    {
        [SerializeField] Animator m_setupTarget = default;
        [SerializeField] SpriteRenderer m_spriteRenderer = default;

        protected static readonly string AnimatorName = "AnimOverride";

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

            m_setupTarget.runtimeAnimatorController = animator;
            m_spriteRenderer.enabled = true;
        }

        protected virtual void OnAnimAssetLoaded(AsyncOperationHandle<AnimatorOverrideController> obj)
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

    }

}


