
namespace BlindChase.GameManagement
{
    // A container of game context to prevent unwanted context mutation
    public class GameContextRecord
    {
        WorldContext m_world;
        CharacterContext m_character;

        public WorldContext WorldRecord { 
            get { return new WorldContext(m_world); }
            set { m_world = new WorldContext(value); } }
        public CharacterContext CharacterRecord { 
            get { return new CharacterContext(m_character); }
            set { m_character = new CharacterContext(value); } }

        public GameContextRecord(WorldContext w, CharacterContext c) 
        {
            m_world = new WorldContext(w);
            m_character = new CharacterContext(c);
        }
    }
}