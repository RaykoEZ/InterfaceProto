using UnityEngine;
using BlindChase;


namespace BlindChase 
{
    public class TileTrigger : MonoBehaviour
    {

        [SerializeField] TileBehaviour m_behaviour = default;
        // Start is called before the first frame update
        void OnMouseDown()
        {
            m_behaviour.OnPlayerSelect();
        }
    }

}

