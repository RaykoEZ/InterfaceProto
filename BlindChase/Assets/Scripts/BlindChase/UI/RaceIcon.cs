using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlindChase;

namespace BlindChase.Ui
{
    public class RaceIcon : MonoBehaviour
    {
        [SerializeField] Image m_characterIcon = default;

        public void Init(Sprite sprite)
        {
            m_characterIcon.sprite = sprite;
        }
    }

}


