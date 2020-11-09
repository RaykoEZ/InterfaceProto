using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BlindChase.GameManagement
{
    [CreateAssetMenu(fileName = "CharacterAudioCollection", menuName = "BlindChase/Audio/Create a collection of character audio clips", order = 1)]
    public class CharacterAudioCollection : ScriptableObject
    {
        //[SerializeField] AudioClip m_onIdle = default;
        public AudioClip OAdvance = default;
        public AudioClip OnDamaged = default;
        //public AudioClip m_onDefeat = default;
        //public AudioClip m_onWin = default;
        //public AudioClip m_onLose = default;

    }

}


