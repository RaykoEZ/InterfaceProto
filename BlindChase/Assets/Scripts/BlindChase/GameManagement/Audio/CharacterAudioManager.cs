using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BlindChase.GameManagement
{
    public enum CharacterAudioType 
    { 
        Advance,
        TakeDamage
    }

    public class CharacterAudioManager : AudioManager
    {
        CharacterAudioCollection m_audioCollection;

        public virtual void Init(string characterId) 
        {
            Addressables.LoadAssetAsync<CharacterAudioCollection>($"{characterId}_Audio").Completed += OnAudioLoaded;
        }

        void OnAudioLoaded(AsyncOperationHandle<CharacterAudioCollection> obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded && obj.Result != null)
            {
                m_audioCollection = obj.Result;
            }
            else
            {
                Debug.LogError("Failed loading Character Animation Asset");
            }
        }

        public void PlayAudio(CharacterAudioType audioType) 
        {
            switch (audioType)
            {
                case CharacterAudioType.Advance:
                    m_audioSource.clip = m_audioCollection.OAdvance;
                    break;
                case CharacterAudioType.TakeDamage:
                    m_audioSource.clip = m_audioCollection.OnDamaged;
                    break;
                default:
                    break;
            }

            m_audioSource?.Play();
        }

    }

}


