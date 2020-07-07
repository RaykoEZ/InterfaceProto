using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase 
{
    public class TileBehaviour : MonoBehaviour
    {
        protected BCEventHandler eventHandler; 

        public virtual void Init(BCEventHandler e) 
        {
            eventHandler = e;

        }
    }
}


