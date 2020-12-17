using System;
using System.Security.Policy;

namespace BadgeFarmer
{
    public class BadgeOLD
    {
        private const int MaxLevel = 5;
        public int AvailableLevels => (Level >= MaxLevel && !IsCompleted) ? int.MaxValue : MaxLevel - Level;
        public string Name { get; private set; }

        public bool IsCompleted { get; private set; }

        public int Level { get; private set; }

        public Uri PageUri { get; private set; }
        
        public BadgeOLD(string name, bool isCompleted, int level, Uri pageUri)
        {
            Name = name;
            IsCompleted = isCompleted;
            Level = level;
            PageUri = pageUri;
        }
    }
}