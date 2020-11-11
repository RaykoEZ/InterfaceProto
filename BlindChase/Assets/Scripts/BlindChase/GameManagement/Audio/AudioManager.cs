using UnityEngine;

namespace BlindChase.GameManagement
{
    public class AudioManager : MonoBehaviour 
    {
        [SerializeField] protected AudioSource m_audioSource = default;


        public void PlayAudio(AudioClip audio)
        {
            m_audioSource?.PlayOneShot(audio);
        }
    }

}


