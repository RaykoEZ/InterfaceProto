using System;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace BlindChase.GameManagement
{
    [CreateAssetMenu(fileName = "CharacterDatabase", menuName = "BlindChase/Create collection of character data", order = 1)]
    public class CharacterDatabase : ScriptableObject
    {
        static Dictionary<string, CharacterData> m_characterDataCollection = default;
        static Dictionary<string, Sprite> m_characterIcons = default;
        static Dictionary<CharacterClassType, Sprite> m_classIcons = default;

        const string c_characterDataRoot = "Characters/Characters";
        static Newtonsoft.Json.Converters.StringEnumConverter m_enumConverter =
           new Newtonsoft.Json.Converters.StringEnumConverter();

        void OnEnable()
        {
            m_enumConverter.AllowIntegerValues = true;

            TextAsset raw = Resources.Load<TextAsset>(c_characterDataRoot);
            m_characterDataCollection = JsonConvert.DeserializeObject<Dictionary<string, CharacterData>>(raw.text, 
                m_enumConverter);
        }

        public CharacterData GetCharacterData(string id) 
        { 
            if(string.IsNullOrWhiteSpace(id) || !m_characterDataCollection.ContainsKey(id)) 
            {
                return new CharacterData();
            }
            return m_characterDataCollection[id];
        }

        public Sprite GetCharacterIcon(string id) 
        {
            return null;      
        }
    }

}


