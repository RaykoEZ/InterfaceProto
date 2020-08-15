using UnityEngine;
using System.Collections;

namespace BlindChase 
{
    public interface IBCContext 
    { 
    
    }

    public interface IBCContextFactory<T> where T : IBCContext
    {
        void Reset(T newContext);

        void Shutdown();

    }
}

