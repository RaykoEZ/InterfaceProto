using UnityEngine;
using BlindChase;


namespace BlindChase 
{
    public class TileTrigger : MonoBehaviour
    {

        public event OnTileTriggered OnPonterSelected = default;
        public event OnTileTriggered OnPonterHover = default;
        public event OnTileTriggered OnPonterExit = default;

        // Start is called before the first frame update
        public void OnSelect()
        {
            OnPonterSelected?.Invoke();
        }

        public void OnHover()
        {
            OnPonterHover?.Invoke();
        }

        public void OnExit()
        {
            OnPonterExit?.Invoke();
        }
    }

}

