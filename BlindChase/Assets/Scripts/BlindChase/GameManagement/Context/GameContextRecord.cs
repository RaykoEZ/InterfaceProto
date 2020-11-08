using System.Collections.Generic;

namespace BlindChase.GameManagement
{
    // A container of game context to prevent unwanted context mutation
    public struct GameContextRecord
    {
        WorldContext m_world;
        CharacterContext m_character;

        public WorldContext WorldRecord { 
            get { return m_world; } }
        public CharacterContext CharacterRecord
        {
            get { return m_character; }
        }

        public GameContextRecord(WorldContext w, CharacterContext c) 
        {
            m_world = new WorldContext(w);
            m_character = new CharacterContext(c);
        }

        public GameContextRecord(GameContextRecord c)
        {
            m_world = new WorldContext(c.m_world);
            m_character = new CharacterContext(c.m_character);
        }
    }
}