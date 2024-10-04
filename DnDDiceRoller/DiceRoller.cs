using System;
using System.Collections.Generic;

namespace DnDDiceRoller
{
    public class DiceRoller
    {
        // Random number generator
        private static readonly Random random = new Random();

        // Method to roll a single die
        private int RollDie(int sides)
        {
            return random.Next(1, sides + 1);
        }//do not touch yet!, needs update to display on current users indiviual rolls with multiple dice. 

        //
        public (List<int> IndividualRolls, int Total) RollMultipleDice(Dictionary<int, int> diceTypes, int modifier = 0)
        {
            List<int> results = new List<int>();
            int total = 0;

            foreach (var diceType in diceTypes)
            {
                int numberOfDice = diceType.Value;
                int sides = diceType.Key;

                for (int i = 0; i < numberOfDice; i++)
                {
                    int rollResult = RollDie(sides);
                    results.Add(rollResult);
                    total += rollResult;
                }
            }

            // Add the modifier to the total
            total += modifier;

            return (results, total);
        }
    }
}