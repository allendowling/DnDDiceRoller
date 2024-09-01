using System;
using System.Collections.Generic;

namespace DnDDiceRoller
{
    public class DiceRoller
    {
        private static readonly Random random = new Random();//really cool built in method for random logic, find a random number between 1 and what dice side is selected

        // Method to roll a single die
        private int RollDie(int sides)
        {
            return random.Next(1, sides + 1);
        }//do not touch yet!, needs update to display on current users indiviual rolls with multiple dice. 

        // Method to roll multiple dice of different types and return individual results with a modifier
        public List<int> RollMultipleDice(Dictionary<int, int> diceTypes, int modifier = 0)
        {
            List<int> results = new List<int>();

            foreach (var diceType in diceTypes)
            {
                int numberOfDice = diceType.Value;
                int sides = diceType.Key;

                for (int i = 0; i < numberOfDice; i++)
                {
                    int rollResult = RollDie(sides) + modifier;  // Apply the modifier to each roll
                    results.Add(rollResult);
                }
            }

            return results;
        }
    }
}