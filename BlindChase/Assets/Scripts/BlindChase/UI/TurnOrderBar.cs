using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlindChase;

namespace BlindChase
{
    public class CharacterTurnRacer 
    {
        public float Speed { get; set; }
        public float Progress { get; set; }
    }

    public class TurnOrderBar : UIBehaviour
    {
        [SerializeField] float m_dt = default;

        Dictionary<TileId, CharacterTurnRacer> m_raceProgresses = new Dictionary<TileId, CharacterTurnRacer>();
        public delegate void OnGoalReached(TileId racerId);
        protected event OnGoalReached OnCharacterGoalReached_Internal = default;
        public event OnGoalReached OnCharacterGoalReached = default;

        bool m_raceFinished = false;

        void OnEnable()
        {
            OnCharacterGoalReached_Internal += OnGoalReach;
        }

        void OnDisable()
        {
            OnCharacterGoalReached_Internal -= OnGoalReach;
        }

        void OnGoalReach(TileId id) 
        {
            Pause();
            // Reset progress when goal reached.
            m_raceProgresses[id].Progress = 0f;
            OnCharacterGoalReached?.Invoke(id);
        }

        public void AddParticipant(TileId id, CharacterTurnRacer racer) 
        {
            if (!m_raceProgresses.ContainsKey(id)) 
            {
                m_raceProgresses.Add(id, racer);
            }
        }

        public void RemoveParticipant(TileId id)
        {
            if (m_raceProgresses.ContainsKey(id))
            {
                m_raceProgresses.Remove(id);
            }
        }

        public void SetParticipantState(TileId id, CharacterTurnRacer racer)
        {
            if (m_raceProgresses.ContainsKey(id))
            {
                m_raceProgresses[id] = racer;
            }
        }

        public void Pause() 
        {
            m_raceFinished = true;
            StopCoroutine(UpdateProgress());
        }

        public void StartRace()
        {
            m_raceFinished = false;
            StartCoroutine(UpdateProgress());
        }

        TileId OnRaceFinish(List<TileId> finishedRacers)
        {
            if (finishedRacers.Count == 1) 
            {
                return finishedRacers[0];
            }

            TileId finishing = finishedRacers[0];
            float maxSpeed = 0f;

            for (int i = 0; i < finishedRacers.Count; ++i)
            {
                float spped = m_raceProgresses[finishedRacers[i]].Speed;
                if (spped > maxSpeed)
                {
                    maxSpeed = spped;
                    finishing = finishedRacers[i];
                }
            }
            return finishing;
        }


        IEnumerator UpdateProgress() 
        {
            List<TileId> finishedRacers = new List<TileId>();

            while (!m_raceFinished) 
            {
                finishedRacers.Clear();

                foreach (TileId id in m_raceProgresses.Keys)
                {
                    CharacterTurnRacer value = m_raceProgresses[id];

                    if (value.Progress >= 1.0f)
                    {
                        finishedRacers.Add(id);
                    }
                    else
                    {
                        float randFactor = Random.Range(0.9f, 1.1f);
                        value.Progress += value.Speed * 0.1f * m_dt * randFactor;
                    }
                    
                }

                if (finishedRacers.Count > 0) 
                {
                    TileId finishingRacerId = OnRaceFinish(finishedRacers);
                    OnCharacterGoalReached_Internal.Invoke(finishingRacerId);
                }

                yield return null;

            }

        }
    }
}


