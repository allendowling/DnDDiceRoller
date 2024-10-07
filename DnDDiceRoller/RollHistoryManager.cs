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
        // Limit the history to 50 items
        private const int MaxHistoryCount = 50; // Limit the history to 50 items
        public ObservableCollection<RollHistoryItem> RollHistory { get; private set; }// Create a new ObservableCollection to store the roll history
        // Create a new RollHistoryManager constructor
        public RollHistoryManager()
        // Initialize the RollHistory collection
        {
            RollHistory = new ObservableCollection<RollHistoryItem>();
        }
        // Create a method to add a new roll to the history
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
        // Create a method to get the roll history
        public IEnumerable<RollHistoryItem> GetRollHistory()
        // Return the RollHistory collection
        {
            return RollHistory;
        }
    }
}