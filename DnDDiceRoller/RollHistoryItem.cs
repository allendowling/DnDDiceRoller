using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDDiceRoller
{
    public class RollHistoryItem
    {
        public string User { get; set; }
        public int Total { get; set; }
        public List<int> IndividualRolls { get; set; }
        public string RollDetails { get; set; }
        public string Timestamp { get; set; }
        public  Dictionary<int, int> DiceUsed { get; set; }

        public string DiceDetails
        {
            get
            {
                return string.Join(", ", DiceUsed.Select(d => $"{d.Value}d{d.Key}"));
            }
        }
    }
}
