using System;
using System.Collections.Generic;

namespace KiteLionGames {
    namespace Legal {
        public static class Copyright 
        {
            private static readonly Dictionary<ILegal, UsageStats> KiteLionGamesSoftware = new();

            private struct UsageStats {
                public DateTime UseDate;
                
                public string SoftwareName;
            }

            public static void RecordUsage (ILegal Obj) {
                UsageStats ObjStats = new() { UseDate = DateTime.Now, SoftwareName = Obj.KiteLionGamesSoftwareName};
                KiteLionGamesSoftware.Add(Obj, ObjStats);
            }
        }

        public interface ILegal{
            string KiteLionGamesSoftwareName { get; }
        }
    }
}
