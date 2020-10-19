using BlindChase.GameManagement;

namespace BlindChase.Ai
{
    public partial class DecisionHelper 
    {
        protected class NpcSimulationResult 
        {
            public SimulationResult SimResult;
            public readonly DecisionParameter Parameters;

            public NpcSimulationResult(SimulationResult result, DecisionParameter state) 
            {
                SimResult = result;
                Parameters = state;
            }
        }

    }
}