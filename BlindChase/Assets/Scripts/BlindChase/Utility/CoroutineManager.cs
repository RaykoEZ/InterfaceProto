using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase
{

    public class CoroutineManager : MonoBehaviour
    {
        protected delegate void OnCoroutineInterrupt(IEnumerator runThis);
        protected Stack<IEnumerator> m_coroutines = new Stack<IEnumerator>();
        protected IEnumerator m_currentCoroutine = default;
        protected OnCoroutineInterrupt m_onCoroutineInterrupted = default;

        bool m_coroutineInProgress = false;

        void Start()
        {
            Init();
        }

        void OnDestroy()
        {
            Shutdown();
        }

        protected virtual void Init()
        {
            m_onCoroutineInterrupted += OnCoroutineInterrupted;
        }

        protected virtual void Shutdown()
        {
            m_onCoroutineInterrupted -= OnCoroutineInterrupted;
        }

        public void ScheduleCoroutine(IEnumerator coroutine, bool interruptNow = false)
        {
            if (interruptNow && m_coroutineInProgress)
            {
                m_onCoroutineInterrupted?.Invoke(coroutine);
            }
            else
            {
                m_coroutines.Push(coroutine);
            }
        }

        public void StartScheduledCoroutines()
        {
            StartCoroutine(StartCurrentCoroutine());
        }

        public void ClearCoroutines()
        {
            StopCurrentCoroutine();
            m_coroutines.Clear();
        }

        public void StopCurrentCoroutine() 
        {
            StopCoroutine(StartCurrentCoroutine());
            StopCoroutine(m_currentCoroutine);
        }

        void OnCoroutineInterrupted(IEnumerator runThis) 
        {
            StopCurrentCoroutine();
            StartInterruptCoroutine(runThis);
        }

        IEnumerator StartCurrentCoroutine() 
        {
            yield return !m_coroutineInProgress;
            while (m_coroutines.Count > 0) 
            {
                m_coroutineInProgress = true;
                m_currentCoroutine = m_coroutines.Pop();
                yield return StartCoroutine(m_currentCoroutine);
                m_coroutineInProgress = false;
            }
        }

        IEnumerator StartInterruptCoroutine(IEnumerator coroutine)
        {
            m_coroutineInProgress = true;
            m_currentCoroutine = coroutine;
            yield return StartCoroutine(coroutine);
            m_coroutineInProgress = false;
            StartCurrentCoroutine();
        }

    }

}


