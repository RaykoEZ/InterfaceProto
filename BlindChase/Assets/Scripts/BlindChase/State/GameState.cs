using System;
using UnityEngine;

namespace BlindChase.State
{

    public interface IGameState
    {
        // Type returned = the next game state.
        void StateOps(IGameState incomingState);
        Type TransitionTo();
    }

    public class RoundStart : IGameState 
    {
        public void StateOps(IGameState incomingState) 
        { 
            
        }
        public Type TransitionTo() { return typeof(PlayerTurn); }
    }

    // If player won on turn end, => Game End
    public class RoundEnd : IGameState
    {
        public void StateOps(IGameState incomingState)
        {
        }
        public Type TransitionTo() { return typeof(RoundStart); }
    }

    // Need to listen to win detector, if won/loss, => GaemEnd
    public class PlayerTurn : IGameState
    {
        public void StateOps(IGameState incomingState)
        {
        }
        public Type TransitionTo() { return typeof(RoundEnd); }
    }

    public class GameStart : IGameState
    {
        public void StateOps(IGameState incomingState)
        {
        }
        public Type TransitionTo() { return typeof(RoundStart); }
    }


    public class GameEnd : IGameState
    {
        public void StateOps(IGameState incomingState)
        {
        }
        public Type TransitionTo() { return null; }
    }

    // T : Previous game state
    public class Interrupted : IGameState
    {

        IGameState m_previous;
        public void StateOps(IGameState incomingState)
        {
            m_previous = incomingState;
        }
        public Type TransitionTo() { return m_previous.GetType(); }
    }

}