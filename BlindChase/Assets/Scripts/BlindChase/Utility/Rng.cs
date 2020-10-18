using System;
using System.Collections.Generic;

namespace BlindChase.Utility
{
    public static class RngHelper
    {
        static Random m_seedGen = new Random();
        static readonly int m_choiceSeed = m_seedGen.Next(int.MaxValue);
        static readonly Random m_choiceGen = new Random(m_choiceSeed);

        // Given the list of choices & how many to choose, 
        // return list of unique choices from the list of choices.
        public static List<T> ChooseUnique<T>(List<T> choices, int choiceLimit) 
        {
            // If I can choose more than there is in the choices, choose all.
            if(choices.Count <= choiceLimit) 
            {
                return choices;
            }

            List<T> result = new List<T>(choiceLimit);
            for(int i = 0; i < choiceLimit; ++i) 
            {
                int chosenIndex = RandomInt(m_choiceGen, 0, choices.Count);
                result.Add(choices[chosenIndex]);
                // Remove choice when chosen
                choices.RemoveAt(chosenIndex);
            }
            return result;
        }


        static int RandomInt(Random gen, int min, int max) 
        {
            return gen.Next(min, max);
        }
    }

}


