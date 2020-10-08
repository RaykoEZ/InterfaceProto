using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase.Ui
{
    [RequireComponent(typeof(Animator))]
    public class UIBehaviour : MonoBehaviour
    {
        [SerializeField] Animator m_animator = default;

        public virtual void OnShow() 
        {
            m_animator?.SetBool("IsActive", true);
        }

        public virtual void OnHide()
        {
            m_animator?.SetBool("IsActive", false);
        }
    }

}


