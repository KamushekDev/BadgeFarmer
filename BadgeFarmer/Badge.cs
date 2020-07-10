using System;
using System.Security.Policy;

namespace BadgeFarmer
{
    public class Badge
    {
        public string Name { get; private set; }

        public bool IsCompleted { get; private set; }

        public int Level { get; private set; }

        public Uri PageUri { get; private set; }
        
        public Badge(string name, bool isCompleted, int level, Uri pageUri)
        {
            Name = name;
            IsCompleted = isCompleted;
            Level = level;
            PageUri = pageUri;
        }
    }
}