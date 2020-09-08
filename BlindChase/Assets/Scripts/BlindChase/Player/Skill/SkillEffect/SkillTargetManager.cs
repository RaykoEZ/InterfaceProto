using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlindChase;
using BlindChase.Events;

namespace BlindChase
{

    public class SkillTargetManager : MonoBehaviour
    {
        [SerializeField] CharacterHUD m_HUD = default;

        public event OnSkillTargetConfirmed OnTargetConfirmed = default;
        int m_targetLimit = 0;
        HashSet<Vector3> m_targets = new HashSet<Vector3>();

        public void Init()
        {
            m_HUD.OnRangeCancel += TargetsCancelled;
            m_HUD.OnRangeConfirm += TargetsConfirmed;

        }

        public void Shutdown() 
        {
            m_HUD.OnRangeCancel -= TargetsCancelled;
            m_HUD.OnRangeConfirm -= TargetsConfirmed;

            OnTargetConfirmed = null;
        }

        public void SetTargetInfo(int limit) 
        {
            m_targetLimit = limit;
        }

        public void ToggleTarget(Vector3 target) 
        {
            if (m_targets.Contains(target)) 
            {
                m_targets.Remove(target);
                return;
            }

            if(m_targets.Count < m_targetLimit) 
            {
                m_targets.Add(target);
            }

            if (IsTargetLimitSatisfied()) 
            {
                m_HUD.SkillTargetSatisfied();
            }
        }

        bool IsTargetLimitSatisfied() 
        {
            return m_targets.Count <= m_targetLimit && m_targets.Count > 0;
        }

        public void TargetsConfirmed() 
        {
            if (m_targets.Count > 0) 
            {
                OnTargetConfirmed?.Invoke(new HashSet<Vector3>(m_targets));
                m_targets.Clear();
                m_targetLimit = 0;
            }
            else 
            {
                Debug.LogError("Please select your targets.");
            }
        }

        void TargetsCancelled()
        {
            m_targets.Clear();
        }
    }

}


