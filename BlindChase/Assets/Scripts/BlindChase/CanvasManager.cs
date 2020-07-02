using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.Utility
{
    // Overriding canvas sort orders, 
    // maybe we will have canvases generated during a game session in the future.
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] Canvas m_gameBoardCanvas = default;
        [SerializeField] Canvas m_backgroundCanvas = default;
        [SerializeField] List<Canvas> m_overlayCanvas = default;

        public Canvas GameBoardCanvas
        {
            get
            {
                return m_gameBoardCanvas;
            }
        }

        public Canvas BackgroundCanvas
        {
            get
            {
                return m_backgroundCanvas;
            }
        }

        public List<Canvas> OverlayCanvas 
        { get 
            {
                return m_overlayCanvas;             
            } 
        }

        private void Start()
        {
            // First two canvases to render BG with gameplay objects on top of BG
            BackgroundCanvas.sortingOrder = 0;
            GameBoardCanvas.sortingOrder = 1;

            // All overlay canvases' sort order are overriden here
            for (int i = 0; i < OverlayCanvas.Count; ++i)
            {
                OverlayCanvas[i].sortingOrder = i + 2;
            }
        }



    }
}


