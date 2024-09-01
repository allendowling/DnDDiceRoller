using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDDiceRoller
{
    public class RollHistoryManager
    {
        private const int MaxHistoryCount = 50; // Limit the history to 50 items
        public ObservableCollection<RollHistoryItem> RollHistory { get; private set; }

        public RollHistoryManager()
        {
            RollHistory = new ObservableCollection<RollHistoryItem>();
        }

        public void AddRollHistoryItem(RollHistoryItem item)
        {
            // Insert the new roll at the beginning of the list
            RollHistory.Insert(0, item);

            // If the list exceeds the max history count, remove the oldest item
            if (RollHistory.Count > MaxHistoryCount)
            {
                RollHistory.RemoveAt(RollHistory.Count - 1);
            }
        }

        public IEnumerable<RollHistoryItem> GetRollHistory()
        {
            return RollHistory;
        }
    }
}