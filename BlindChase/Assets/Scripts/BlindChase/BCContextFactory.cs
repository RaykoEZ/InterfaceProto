using UnityEngine;
using System.Collections;

namespace BlindChase 
{
    public interface IBCContextFactory
    {
        void OnContextUpdated();
        void Shutdown();

    }
}

